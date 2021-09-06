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

    /// <summary>
    /// Thrown when a required option has not been provided.
    /// </summary>
    [Serializable]
    public class MissingOptionException : ParseException {

        /// <summary>
        /// The list of missing options and groups.
        /// </summary>
        public IList<object> MissingOptions { get; init; }


        /// <summary>
        /// Construct a new <code>MissingSelectedException</code> with the specified detail message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingOptionException(string message)
            : base(message) {

        }


        /// <summary>
        /// Constructs a new <code>MissingSelectedException</code> with the specified list of missing options.
        /// </summary>
        /// <param name="missingOptions">The list of missing options and groups.</param>
        /// <since>1.2</since>
        public MissingOptionException(ICollection<object> missingOptions)
            : this(CreateMessage(missingOptions)) {

            this.MissingOptions = new List<object>(missingOptions);
        }


        /// <summary>
        /// Build the exception message from the specified list of options.
        /// </summary>
        /// <param name="missingOptions">The list of missing options and groups.</param>
        /// <returns>The exception message string.</returns>
        /// <since>1.2</since>
        private static string CreateMessage(ICollection<object> missingOptions) {
            StringBuilder buf = new StringBuilder("Missing required option");

            buf.Append(missingOptions.Count == 1 ? "" : "s");
            buf.Append(": ");
            buf.Append(String.Join(", ", missingOptions));

            return buf.ToString();
        }
    }
}
