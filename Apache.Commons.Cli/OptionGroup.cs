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
    using System.Linq;
    using System.Text;


    /**
     * A group of mutually exclusive options.
     */
    [Serializable]
    public class OptionGroup {

        /** hold the options */
        private readonly Dictionary<string, Option> optionMap = new Dictionary<string, Option>();

        /** the name of the selected option */
        private string selected;

        /** specified whether this group is required */
        private bool required;


        /**
         * Add the specified <code>Option</code> to this group.
         *
         * @param option the option to add to this group
         * @return this option group with the option added
         */
        public OptionGroup AddOption(Option option) {
            // key   - option name
            // value - the option
            optionMap[option.GetKey()] = option;

            return this;
        }


        /**
         * @return the names of the options in this group as a
         * <code>Collection</code>
         */
        public ICollection<string> GetNames() {
            // the key set is the collection of names
            return optionMap.Keys;
        }


        /**
         * @return the options in this group as a <code>Collection</code>
         */
        public ICollection<Option> GetOptions() {
            // the values are the collection of options
            return optionMap.Values;
        }


        /**
         * Set the selected option of this group to <code>name</code>.
         *
         * @param option the option that is selected
         * @throws AlreadySelectedException if an option from this group has
         * already been selected.
         */
        public void SetSelected(Option option) {
            
            if (option == null) {
                // reset the option previously selected
                selected = null;
                return;
            }
        
            // if no option has already been selected or the 
            // same option is being reselected then set the
            // selected member variable
            if (selected == null || selected.Equals(option.GetKey())) {
                selected = option.GetKey();
            }
            else {
                throw new AlreadySelectedException(this, option);
            }
        }


        /**
         * @return the selected option name
         */
        public string GetSelected() {
            return selected;
        }


        /**
         * @param required specifies if this group is required
         */
        public void SetRequired(bool required) {
            this.required = required;
        }


        /**
         * Returns whether this option group is required.
         *
         * @return whether this option group is required
         */
        public bool IsRequired() {
            return required;
        }


        /**
         * Returns the stringified version of this OptionGroup.
         *
         * @return the stringified representation of this group
         */
        public override string ToString() {
            StringBuilder buff = new StringBuilder();

            List<Option> option = GetOptions().ToList();
            int lastIndex = option.Count - 1;

            buff.Append("[");
            
            for (int index = 0; index <= lastIndex; index++) {
                option[index].GetOpt();

                if (option[index].GetOpt() != null) {
                    buff.Append("-");
                    buff.Append(option[index].GetOpt());
                }
                else {
                    buff.Append("--");
                    buff.Append(option[index].GetLongOpt());
                }

                if (option[index].GetDescription() != null) {
                    buff.Append(" ");
                    buff.Append(option[index].GetDescription());
                }

                if (index < lastIndex ) {
                    buff.Append(", ");
                }
            }

            buff.Append("]");

            return buff.ToString();
        }
    }
}