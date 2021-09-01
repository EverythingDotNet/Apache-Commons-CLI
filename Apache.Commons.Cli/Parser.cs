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

    /**
     * <code>Parser</code> creates {@link CommandLine}s.
     *
     * @deprecated since 1.3, the two-pass parsing with the flatten method is not enough flexible to handle complex cases
     */
    [Obsolete]
    public abstract class Parser : ICommandLineParser {

        /** commandline instance */
        protected CommandLine cmd;

        /** current Options */
        private Options options;

        /** list of required options strings */
        private IList<object> requiredOptions;


        protected void SetOptions(Options options) {
            this.options = options;
            this.requiredOptions = new List<object>(options.GetRequiredOptions());        
        }

    
        protected Options GetOptions() {
            return options;
        }

        protected IList<object> GetRequiredOptions() {
            return requiredOptions;
        }


        /**
         * Subclasses must implement this method to reduce
         * the <code>arguments</code> that have been passed to the parse method.
         *
         * @param opts The Options to parse the arguments by.
         * @param arguments The arguments that have to be flattened.
         * @param stopAtNonOption specifies whether to stop
         * flattening when a non option has been encountered
         * @return a String array of the flattened arguments
         * @throws ParseException if there are any problems encountered
         *                        while parsing the command line tokens.
         */
        protected abstract string[] Flatten(Options opts, string[] arguments, bool stopAtNonOption);


        /**
         * Parses the specified <code>arguments</code> based
         * on the specified {@link Options}.
         *
         * @param options the <code>Options</code>
         * @param arguments the <code>arguments</code>
         * @return the <code>CommandLine</code>
         * @throws ParseException if there are any problems encountered
         *                        while parsing the command line tokens.
         */
        public CommandLine Parse(Options options, string[] arguments) {
            return Parse(options, arguments, null, false);
        }


        /**
         * Parse the arguments according to the specified options and properties.
         *
         * @param options    the specified Options
         * @param arguments  the command line arguments
         * @param properties command line option name-value pairs
         * @return the list of atomic option and value tokens
         * @throws ParseException if there are any problems encountered
         *                        while parsing the command line tokens.
         *
         * @since 1.1
         */
        public CommandLine Parse(Options options, string[] arguments, Dictionary<string, string> properties) {
            return Parse(options, arguments, properties, false);
        }


        /**
         * Parses the specified <code>arguments</code>
         * based on the specified {@link Options}.
         *
         * @param options         the <code>Options</code>
         * @param arguments       the <code>arguments</code>
         * @param stopAtNonOption if <code>true</code> an unrecognized argument stops
         *     the parsing and the remaining arguments are added to the
         *     {@link CommandLine}s args list. If <code>false</code> an unrecognized
         *     argument triggers a ParseException.
         * @return the <code>CommandLine</code>
         * @throws ParseException if an error occurs when parsing the arguments.
         */
        public CommandLine Parse(Options options, string[] arguments, bool stopAtNonOption) {
            return Parse(options, arguments, null, stopAtNonOption);
        }


        /**
         * Parse the arguments according to the specified options and
         * properties.
         *
         * @param options the specified Options
         * @param arguments the command line arguments
         * @param properties command line option name-value pairs
         * @param stopAtNonOption if <code>true</code> an unrecognized argument stops
         *     the parsing and the remaining arguments are added to the
         *     {@link CommandLine}s args list. If <code>false</code> an unrecognized
         *     argument triggers a ParseException.
         *
         * @return the list of atomic option and value tokens
         *
         * @throws ParseException if there are any problems encountered
         * while parsing the command line tokens.
         *
         * @since 1.1
         */
        public CommandLine Parse(Options options, string[] arguments, Dictionary<string, string> properties, bool stopAtNonOption) {
        
            // clear out the data in options in case it's been used before (CLI-71)
            foreach (Option opt in options.HelpOptions()) {
                opt.ClearValues();
            }
        
            // clear the data from the groups
            foreach (OptionGroup group in options.GetOptionGroups()) {
                group.SetSelected(null);
            }        

            // initialise members
            SetOptions(options);

            cmd = new CommandLine();

            bool eatTheRest = false;

            if (arguments == null) {
                arguments = new string[0];
            }

            string[] tokenList = Flatten(options, arguments, stopAtNonOption);
            int tokenCount = tokenList.Length;

            // process each flattened token
            for (int i = 0; i < tokenCount; i++) {
                string t = tokenList[i];

                // the value is the double-dash
                if ("--".Equals(t)) {
                    eatTheRest = true;
                }

                // the value is a single dash
                else if ("-".Equals(t)) {
                    if (stopAtNonOption) {
                        eatTheRest = true;
                    }
                    else {
                        cmd.AddArg(t);
                    }
                }

                // the value is an option
                else if (t.StartsWith("-")) {
                    if (stopAtNonOption && !GetOptions().HasOption(t)) {
                        eatTheRest = true;
                        cmd.AddArg(t);
                    }
                    else {
                        ProcessOption(options, t, tokenList, ref i);
                    }
                }

                // the value is an argument
                else {
                    cmd.AddArg(t);

                    if (stopAtNonOption) {
                        eatTheRest = true;
                    }
                }

                // eat the remaining tokens
                if (eatTheRest) {
                    while (++i < tokenCount) {
                        string str = tokenList[i];

                        // ensure only one double-dash is added
                        if (!"--".Equals(str)) {
                            cmd.AddArg(str);
                        }
                    }
                }
            }

            ProcessProperties(properties);
            CheckRequiredOptions();

            return cmd;
        }


        /**
         * Sets the values of Options using the values in <code>properties</code>.
         *
         * @param properties The value properties to be processed.
         * @throws ParseException if there are any problems encountered
         *                        while processing the properties.
         */
        protected void ProcessProperties(IDictionary<string, string> properties) {
            if (properties == null) {
                return;
            }

            foreach (string option in properties.Keys) {
                Option opt = options.GetOption(option);
            
                if (opt == null) {
                    throw new UnrecognizedOptionException("Default option wasn't defined", option);
                }
            
                // if the option is part of a group, check if another option of the group has been selected
                OptionGroup group = options.GetOptionGroup(opt);
                bool selected = group != null && group.GetSelected() != null;
            
                if (!cmd.HasOption(option) && !selected) {
                    // get the value from the properties instance
                    string value = properties[option];

                    if (opt.HasArg()) {
                        if (opt.GetValues() == null || opt.GetValues().Length == 0) {
                            try {
                                opt.AddValueForProcessing(value);
                            }
                            catch (ApplicationException) {
                                // if we cannot add the value don't worry about it
                            }
                        }
                    }
                    else if (!("yes".Equals(value, StringComparison.OrdinalIgnoreCase) 
                        || "true".Equals(value, StringComparison.OrdinalIgnoreCase)
                        || "1".Equals(value, StringComparison.OrdinalIgnoreCase))) {

                        // if the value is not yes, true or 1 then don't add the
                        // option to the CommandLine
                        continue;
                    }

                    cmd.AddOption(opt);
                    UpdateRequiredOptions(opt);
                }
            }
        }


        /**
         * Throws a {@link MissingOptionException} if all of the required options
         * are not present.
         *
         * @throws MissingOptionException if any of the required Options are not present.
         */
        protected void CheckRequiredOptions() {
            // if there are required options that have not been processed
            if (GetRequiredOptions().Count != 0) {
                throw new MissingOptionException(GetRequiredOptions());
            }
        }


        /**
         * Process the argument values for the specified Option
         * <code>opt</code> using the values retrieved from the
         * specified iterator <code>iter</code>.
         *
         * @param opt The current Option
         * @param iter The iterator over the flattened command line Options.
         *
         * @throws ParseException if an argument value is required
         * and it is has not been found.
         */
        public void ProcessArgs(Options options, Option option, string[] tokens, ref int index) {

            // loop until an option is found
            while (++index < tokens.Length) {
                string str = tokens[index];
            
                // found an Option, not an argument
                if (options.HasOption(str) && str.StartsWith("-")) {
                    index--;
                    break;
                }

                // found a value
                try {
                    option.AddValueForProcessing(Util.StripLeadingAndTrailingQuotes(str));
                }
                catch (ApplicationException) {
                    index--;
                    break;
                }
            }

            if (option.GetValues() == null && !option.HasOptionalArg()) {
                throw new MissingArgumentException(option);
            }
        }


        /**
         * Process the Option specified by <code>arg</code> using the values
         * retrieved from the specified iterator <code>iter</code>.
         *
         * @param arg The String value representing an Option
         * @param iter The iterator over the flattened command line arguments.
         *
         * @throws ParseException if <code>arg</code> does not represent an Option
         */
        protected void ProcessOption(Options options, string arg, string[] tokens, ref int index) {
            bool hasOption = options.HasOption(arg);

            // if there is no option throw an UnrecognisedOptionException
            if (!hasOption) {
                throw new UnrecognizedOptionException("Unrecognized option: " + arg, arg);
            }

            // get the option represented by arg
            Option opt = (Option) GetOptions().GetOption(arg).Clone();
        
            // update the required options and groups
            UpdateRequiredOptions(opt);
        
            // if the option takes an argument value
            if (opt.HasArg()) {
                ProcessArgs(options, opt, tokens, ref index);
            }
        
            // set the option on the command line
            cmd.AddOption(opt);
        }


        /**
         * Removes the option or its group from the list of expected elements.
         *
         * @param opt
         */
        private void UpdateRequiredOptions(Option opt) {

            // if the option is a required option remove the option from
            // the requiredOptions list
            if (opt.IsRequired()) {
                GetRequiredOptions().Remove(opt.GetKey());
            }

            // if the option is in an OptionGroup make that option the selected
            // option of the group
            if (GetOptions().GetOptionGroup(opt) != null) {
                OptionGroup group = GetOptions().GetOptionGroup(opt);

                if (group.IsRequired()) {
                    GetRequiredOptions().Remove(group);
                }

                group.SetSelected(opt);
            }
        }
    }
}