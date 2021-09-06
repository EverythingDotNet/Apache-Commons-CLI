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
    /// Thrown when more than one option in an option group has been provided.
    /// </summary>
    [Serializable]
    public class AlreadySelectedException : ParseException {

        /// <summary>
        /// The option group selected.
        /// </summary>
        public OptionGroup OptionGroup { get; init; }

        /// <summary>
        /// The option that triggered the exception.
        /// </summary>
        public Option Option { get; init; }


        /// <summary>
        /// Construct a new <code>AlreadySelectedException</code> with the specified detail message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AlreadySelectedException(string message)
            : base(message) {
        
        }


        /// <summary>
        /// Construct a new <code>AlreadySelectedException</code> for the specified option group.
        /// </summary>
        /// <param name="group">The option group already selected.</param>
        /// <param name="option">The option that triggered the exception.</param>
        /// <since>1.2</since>
        public AlreadySelectedException(OptionGroup group, Option option)
            : this("The option '" + option.GetKey() + "' was specified but an option from this group "
                    + "has already been selected: '" + group.GetSelected() + "'") {

            this.OptionGroup = group;
            this.Option = option;
        }
    }
}