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

    /// <summary>
    /// Exception thrown during parsing signalling an unrecognized option was seen.
    /// </summary>
    [Serializable]
    public class UnrecognizedOptionException : ParseException {

        /// <summary>
        /// The unrecognized option.
        /// </summary>
        public string UnrecognizedOption { get; init; }


        /// <summary>
        /// Construct a new <code>UnrecognizedArgumentException</code> with the specified detail message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnrecognizedOptionException(string message)
            : base(message) {

        }


        /// <summary>
        /// Construct a new <code>UnrecognizedArgumentException</code> with the specified option and detail message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="option">The unrecognized option.</param>
        /// <since>1.2</since>
        public UnrecognizedOptionException(string message, string option)
            : this(message) {
            
            this.UnrecognizedOption = option;
        }
    }
}