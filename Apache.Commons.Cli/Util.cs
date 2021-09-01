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

    /**
     * Contains useful helper methods for classes within this package.
     */
    internal static class Util {

        /**
         * Remove the hyphens from the beginning of <code>str</code> and
         * return the new String.
         *
         * @param str The string from which the hyphens should be removed.
         *
         * @return the new String.
         */
        internal static string StripLeadingHyphens(string str) {
            if (str == null) {
                return null;
            }

            if (str.StartsWith("--")) {
                return str.Substring(2, str.Length - 2);
            }
            else if (str.StartsWith("-")) {
                return str.Substring(1, str.Length - 1);
            }

            return str;
        }


        /**
         * Remove the leading and trailing quotes from <code>str</code>.
         * E.g. if str is '"one two"', then 'one two' is returned.
         *
         * @param str The string from which the leading and trailing quotes
         * should be removed.
         *
         * @return The string without the leading and trailing quotes.
         */
        internal static string StripLeadingAndTrailingQuotes(string str) {
            int length = str.Length;

            if (length > 1 && str.StartsWith("\"") && str.EndsWith("\"") && str.Substring(1, length - 2).IndexOf('"') == -1) {
                str = str.Substring(1, length - 2);
            }

            return str;
        }
    }
}
