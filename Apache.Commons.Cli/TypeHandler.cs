#region Apache License
/*
  Licensed to the Apache Software Foundation (ASF) under one or more
  contributor license agreements.  See the NOTICE file distributed with
  this work for additional information regarding copyright ownership.
  The ASF licenses this file to You under the Apache License, Version 2.0
  (the "License"); you may not use this file except in compliance with
  the License.  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 */
#endregion

namespace Apache.Commons.Cli {

    using System;
    using System.IO;
    using System.Reflection;

    /**
     * This is a temporary implementation. TypeHandler will handle the
     * pluggableness of OptionTypes and it will direct all of these types
     * of conversion functionalities to ConvertUtils component in Commons
     * already. BeanUtils I think.
     */
    public class TypeHandler {

        /**
         * Returns the <code>Object</code> of type <code>obj</code>
         * with the value of <code>str</code>.
         *
         * @param str the command line value
         * @param obj the type of argument
         * @return The instance of <code>obj</code> initialized with
         * the value of <code>str</code>.
         * @throws ParseException if the value creation for the given object type failed
         */
        public static object CreateValue(string str, object obj) {
            return CreateValue(str, obj as Type);
        }


        /**
         * Returns the <code>Object</code> of type <code>clazz</code>
         * with the value of <code>str</code>.
         *
         * @param str the command line value
         * @param clazz the class representing the type of argument
         * @param <T> type of argument
         * @return The instance of <code>clazz</code> initialized with
         * the value of <code>str</code>.
         * @throws ParseException if the value creation for the given class failed
         */
        public static object CreateValue(string str, Type clazz) {
            if (PatternOptionBuilder.STRING_VALUE == clazz) {
                return str;
            }
            else if (PatternOptionBuilder.OBJECT_VALUE == clazz) {
                return CreateObject(str);
            }
            else if (PatternOptionBuilder.VALUETYPE_VALUE == clazz) {
                return CreateValueType(str);
            }
            else if (PatternOptionBuilder.DATETIME_VALUE == clazz) {
                return CreateDateTime(str);
            }
            else if (PatternOptionBuilder.TYPE_VALUE == clazz) {
                return CreateType(str);
            }
            else if (PatternOptionBuilder.FILEINFO_VALUE == clazz) {
                return CreateFileInfo(str);
            }
            else if (PatternOptionBuilder.FILESTREAM_VALUE == clazz) {
                return OpenFileStream(str);
            }
            else if (PatternOptionBuilder.FILEINFO_ARRAY_VALUE == clazz) {
                return CreateFiles(str);
            }
            else if (PatternOptionBuilder.URI_VALUE == clazz) {
                return CreateUri(str);
            }
            else {
                throw new ParseException($"Unable to parse {str} for type {clazz.Name}");
            }
        }


        /**
          * Create an Object from the classname and empty constructor.
          *
          * @param classname the argument value
          * @return the initialized object
          * @throws ParseException if the class could not be found or the object could not be created
          */
        public static object CreateObject(string classname) {
            Type cl;

            try {
                cl = Type.GetType(classname);
                
                if (cl == typeof(string)) {
                    return String.Empty;
                }
            }
            catch (SystemException) {
                throw new ParseException("Unable to find the class: " + classname);
            }

            try {
                
                ConstructorInfo constructor = cl.GetConstructor(Type.EmptyTypes);
                
                if (constructor != null) {
                    return constructor.Invoke(new object[0]);
                }
                //else {
                //    return null;
                //}

                throw new ParseException($"Unable to create an instance of: {classname}");
            }
            catch (Exception e) {
                throw new ParseException(e.GetType().Name + "; Unable to create an instance of: " + classname);
            }
        }


        /**
         * Create a number from a String. If a . is present, it creates a
         * Double, otherwise a Long.
         *
         * @param str the value
         * @return the number represented by <code>str</code>
         * @throws ParseException if <code>str</code> is not a number
         */
        public static ValueType CreateValueType(string str) {
            try {
                if (str.IndexOf('.') != -1) {
                    return Double.Parse(str);
                }

//                try {
//                    return Int32.Parse(str);
//                }
//                catch {
                    return Int64.Parse(str);
//                }
            }
            catch (FormatException e) {
                throw new ParseException(e.Message);
            }
        }


        /**
         * Returns the class whose name is <code>classname</code>.
         *
         * @param classname the class name
         * @return The class if it is found
         * @throws ParseException if the class could not be found
         */
        public static Type CreateType(string classname) {
            Type ret = null;

            try {
                ret = Type.GetType(classname);
            }
            catch (SystemException) {
                throw new ParseException("Unable to find the class: " + classname);
            }

            if (ret == null) {
                throw new ParseException($"Unable to find the class: {classname}");
            }

            return ret;
        }


        /**
         * Returns the date represented by <code>str</code>.
         * <p>
         * This method is not yet implemented and always throws an
         * {@link UnsupportedOperationException}.
         *
         * @param str the date string
         * @return The date if <code>str</code> is a valid date string,
         * otherwise return null.
         * @throws UnsupportedOperationException always
         */
        public static DateTime CreateDateTime(string str) {
            throw new NotImplementedException("Not yet implemented");
        }


        /**
         * Returns the URL represented by <code>str</code>.
         *
         * @param str the URL string
         * @return The URL in <code>str</code> is well-formed
         * @throws ParseException if the URL in <code>str</code> is not well-formed
         */
        public static Uri CreateUri(string str) {
            Uri uriResult;

            if (!Uri.IsWellFormedUriString(str, UriKind.Absolute)) {
                throw new ParseException($"Unable to parse the URL: {str}");
            }

            try {
                bool result = Uri.TryCreate(str, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (result)
                    return uriResult;

                return null;
            }
            catch (UriFormatException) {
                throw new ParseException($"Unable to parse the URL: {str}");
            }
        }


        /**
         * Returns the File represented by <code>str</code>.
         *
         * @param str the File location
         * @return The file represented by <code>str</code>.
         */
        public static FileInfo CreateFileInfo(string str) {
            return new FileInfo(str);
        }


        /**
         * Returns the opened FileInputStream represented by <code>str</code>.
         *
         * @param str the file location
         * @return The file input stream represented by <code>str</code>.
         * @throws ParseException if the file is not exist or not readable
         */
        public static FileStream OpenFileStream(string str) {
            try {
                return new FileStream(str, FileMode.Open);
            }
            catch (FileNotFoundException) {
                throw new ParseException("Unable to find file: " + str);
            }
        }

        /**
         * Returns the File[] represented by <code>str</code>.
         * <p>
         * This method is not yet implemented and always throws an
         * {@link UnsupportedOperationException}.
         *
         * @param str the paths to the files
         * @return The File[] represented by <code>str</code>.
         * @throws UnsupportedOperationException always
         */
        public static FileInfo[] CreateFiles(string str) {
            // to implement/port:
            //        return FileW.findFiles(str);
            throw new NotImplementedException("Not yet implemented");
        }
    }
}
