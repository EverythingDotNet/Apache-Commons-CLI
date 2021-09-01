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
    using System.Collections.Generic;
    using System.Text;

    /**
     * Exception thrown when an option can't be identified from a partial name.
     *
     * @since 1.3
     */
    [Serializable]
    public class AmbiguousOptionException : UnrecognizedOptionException {


        /** The list of options matching the partial name specified */
        private readonly ICollection<string> matchingOptions;


        /**
         * Constructs a new AmbiguousOptionException.
         *
         * @param option          the partial option name
         * @param matchingOptions the options matching the name
         */
        public AmbiguousOptionException(string option, IList<string> matchingOptions)
            : base(CreateMessage(option, matchingOptions), option) {
        
            this.matchingOptions = matchingOptions;
        }


        /**
         * Returns the options matching the partial name.
         * @return a collection of options matching the name
         */
        public ICollection<string> GetMatchingOptions() {
            return matchingOptions;
        }


        /**
         * Build the exception message from the specified list of options.
         *
         * @param option
         * @param matchingOptions
         * @return
         */
        private static string CreateMessage(string option, IList<string> matchingOptions) {
            StringBuilder buf = new StringBuilder("Ambiguous option: '");
            
            buf.Append(option);
            buf.Append("'  (could be: '");
            buf.Append(String.Join("', '", matchingOptions));
            buf.Append("')");

            return buf.ToString();
        }
    }
}