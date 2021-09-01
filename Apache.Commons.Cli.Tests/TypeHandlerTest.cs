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

    using Xunit;

    public class TypeHandlerTest {

        public class Instantiable { 

        }


        public static class NotInstantiable {

            static NotInstantiable() { }
        }


        [Fact]
        public void CreateValueStringTest() {
            Assert.Equal("String", TypeHandler.CreateValue("String", PatternOptionBuilder.STRING_VALUE));
        }


        [Fact]
        public void CreateValueObject_UnknownTypeTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("unknown", PatternOptionBuilder.OBJECT_VALUE));
        }


        [Fact]
        public void CreateValueObject_NotInstantiableTypeTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue(typeof(NotInstantiable).Name, PatternOptionBuilder.OBJECT_VALUE));
        }


        [Fact]
        public void CreateValueObject_InstantiableTypeTest() {
            object result = TypeHandler.CreateValue(typeof(Instantiable).AssemblyQualifiedName, PatternOptionBuilder.OBJECT_VALUE);
            Assert.True(result is Instantiable);
        }

        
        [Fact]
        public void CreateValueNumber_NoNumberTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("not a number", PatternOptionBuilder.VALUETYPE_VALUE));
        }

        
        [Fact]
        public void CreateValueNumber_DoubleTest() {
            Assert.Equal(1.5d, TypeHandler.CreateValue("1.5", PatternOptionBuilder.VALUETYPE_VALUE));
        }

        
        [Fact]
        public void CreateValueNumber_LongTest() {
            Assert.Equal((long)15, TypeHandler.CreateValue("15", PatternOptionBuilder.VALUETYPE_VALUE));
        }

        
        [Fact]
        public void CreateValueDateTimeTest() {
            Assert.Throws<NotImplementedException>(() => TypeHandler.CreateValue("what ever", PatternOptionBuilder.DATETIME_VALUE));
        }

        
        [Fact]
        public void CreateValueType_NotFoundTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("what ever", PatternOptionBuilder.TYPE_VALUE));
        }

        
        [Fact]
        public void CreateValueTypeTest() {
            object type = TypeHandler.CreateValue(typeof(Instantiable).AssemblyQualifiedName, PatternOptionBuilder.TYPE_VALUE);
            Assert.Equal(typeof(Instantiable), type);
        }


        [Fact]
        public void CreateValueFileInfoTest() {
            FileInfo result = TypeHandler.CreateValue("some-file.txt", PatternOptionBuilder.FILEINFO_VALUE) as FileInfo;
            Assert.Equal("some-file.txt", result.Name);
        }

        
        [Fact]
        public void CreateValueExistingFileTest() {
            FileStream result = TypeHandler.CreateValue(@"..\..\..\Resources\existing-readable.file", PatternOptionBuilder.FILESTREAM_VALUE) as FileStream;
            Assert.NotNull(result);
        }


        [Fact]
        public void CreateValueExistingFile_NonExistingFileTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("non-existing.file", PatternOptionBuilder.FILESTREAM_VALUE));
        }


        [Fact]
        public void CreateValueFilesTest() {
            Assert.Throws<NotImplementedException>(() => TypeHandler.CreateValue("some.files", PatternOptionBuilder.FILEINFO_ARRAY_VALUE));
        }


        [Fact]
        public void CreateValueUriTest() {
            string urlString = "https://commons.apache.org/";
            Uri result = TypeHandler.CreateValue(urlString, PatternOptionBuilder.URI_VALUE) as Uri;

            Assert.Equal(urlString, result.ToString());
        }


        [Fact]
        public void CreateValueURL_MalformedTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("malformed-url", PatternOptionBuilder.URI_VALUE));
        }

        
        [Fact]
        public void CreateValueInteger_FailureTest() {
            Assert.Throws<ParseException>(() => TypeHandler.CreateValue("just-a-string", typeof(ValueType)));
        }
    }
}
