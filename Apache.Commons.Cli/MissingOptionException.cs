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
     * Thrown when a required option has not been provided.
     */
    [Serializable]
    public class MissingOptionException : ParseException {

        /** The list of missing options and groups */
        private IList<object> missingOptions;


        /**
         * Construct a new <code>MissingSelectedException</code>
         * with the specified detail message.
         *
         * @param message the detail message
         */
        public MissingOptionException(string message)
            : base(message) {

        }


        /**
         * Constructs a new <code>MissingSelectedException</code> with the
         * specified list of missing options.
         *
         * @param missingOptions the list of missing options and groups
         * @since 1.2
         */
        public MissingOptionException(ICollection<object> missingOptions)
            : this(CreateMessage(missingOptions)) {

            this.missingOptions = new List<object>(missingOptions);
        }


        /**
         * Returns the list of options or option groups missing in the command line parsed.
         *
         * @return the missing options, consisting of String instances for simple
         *         options, and OptionGroup instances for required option groups.
         * @since 1.2
         */
        public IList<object> GetMissingOptions() {
            return missingOptions;
        }


        /**
         * Build the exception message from the specified list of options.
         *
         * @param missingOptions the list of missing options and groups
         * @since 1.2
         */
        private static string CreateMessage(ICollection<object> missingOptions) {
            StringBuilder buf = new StringBuilder("Missing required option");

            buf.Append(missingOptions.Count == 1 ? "" : "s");
            buf.Append(": ");
            buf.Append(String.Join(", ", missingOptions));

            return buf.ToString();
        }
    }
}
