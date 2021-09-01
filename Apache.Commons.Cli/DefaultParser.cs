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
     * Default parser.
     *
     * @since 1.3
     */
    public class DefaultParser : ICommandLineParser {

        /** The command-line instance. */
        protected CommandLine cmd;

        /** The current options. */
        protected Options options;

        /**
         * Flag indicating how unrecognized tokens are handled. <code>true</code> to stop
         * the parsing and add the remaining tokens to the args list.
         * <code>false</code> to throw an exception.
         */
        protected bool stopAtNonOption;

        /** The token currently processed. */
        protected string currentToken;

        /** The last option parsed. */
        protected Option currentOption;

        /** Flag indicating if tokens should no longer be analyzed and simply added as arguments of the command line. */
        protected bool skipParsing;

        /** The required options and groups expected to be found when parsing the command line. */
        protected List<object> expectedOpts;

        /** Flag indicating if partial matching of long options is supported. */
        private readonly bool allowPartialMatching;


        /**
         * Creates a new DefaultParser instance with partial matching enabled.
         *
         * By "partial matching" we mean that given the following code:
         * <pre>
         *     {@code
         *          final Options options = new Options();
         *      options.addOption(new Option("d", "debug", false, "Turn on debug."));
         *      options.addOption(new Option("e", "extract", false, "Turn on extract."));
         *      options.addOption(new Option("o", "option", true, "Turn on option with argument."));
         *      }
         * </pre>
         * with "partial matching" turned on, <code>-de</code> only matches the
         * <code>"debug"</code> option. However, with "partial matching" disabled,
         * <code>-de</code> would enable both <code>debug</code> as well as
         * <code>extract</code> options.
         */
        public DefaultParser() {
            this.allowPartialMatching = true;
        }


        /**
         * Create a new DefaultParser instance with the specified partial matching policy.
         *
         * By "partial matching" we mean that given the following code:
         * <pre>
         *     {@code
         *          final Options options = new Options();
         *      options.addOption(new Option("d", "debug", false, "Turn on debug."));
         *      options.addOption(new Option("e", "extract", false, "Turn on extract."));
         *      options.addOption(new Option("o", "option", true, "Turn on option with argument."));
         *      }
         * </pre>
         * with "partial matching" turned on, <code>-de</code> only matches the
         * <code>"debug"</code> option. However, with "partial matching" disabled,
         * <code>-de</code> would enable both <code>debug</code> as well as
         * <code>extract</code> options.
         *
         * @param allowPartialMatching if partial matching of long options shall be enabled
         */
        public DefaultParser(bool allowPartialMatching) {
            this.allowPartialMatching = allowPartialMatching;
        }


        public CommandLine Parse(Options options, string[] arguments) {
            return Parse(options, arguments, null);
        }


        /**
         * Parse the arguments according to the specified options and properties.
         *
         * @param options    the specified Options
         * @param arguments  the command line arguments
         * @param properties command line option name-value pairs
         * @return the list of atomic option and value tokens
         *
         * @throws ParseException if there are any problems encountered
         * while parsing the command line tokens.
         */
        public CommandLine Parse(Options options, string[] arguments, Dictionary<string, string> properties) {
            return Parse(options, arguments, properties, false);
        }

    
        public CommandLine Parse(Options options, string[] arguments, bool stopAtNonOption) {
            return Parse(options, arguments, null, stopAtNonOption);
        }


        /**
         * Parse the arguments according to the specified options and properties.
         *
         * @param options         the specified Options
         * @param arguments       the command line arguments
         * @param properties      command line option name-value pairs
         * @param stopAtNonOption if <code>true</code> an unrecognized argument stops
         *     the parsing and the remaining arguments are added to the
         *     {@link CommandLine}s args list. If <code>false</code> an unrecognized
         *     argument triggers a ParseException.
         *
         * @return the list of atomic option and value tokens
         * @throws ParseException if there are any problems encountered
         * while parsing the command line tokens.
         */
        public CommandLine Parse(Options options, string[] arguments, Dictionary<string, string> properties, bool stopAtNonOption) {
            this.options = options;
            this.stopAtNonOption = stopAtNonOption;
            skipParsing = false;
            currentOption = null;
            expectedOpts = new List<object>(options.GetRequiredOptions());

            // clear the data from the groups
            foreach (OptionGroup group in options.GetOptionGroups()) {
                group.SetSelected(null);
            }

            cmd = new CommandLine();

            if (arguments != null) {
                foreach (string argument in arguments) {
                    HandleToken(argument);
                }
            }

            // check the arguments of the last option
            CheckRequiredArgs();

            // add the default options
            HandleProperties(properties);

            CheckRequiredOptions();

            return cmd;
        }


        /**
         * Sets the values of Options using the values in <code>properties</code>.
         *
         * @param properties The value properties to be processed.
         */
        private void HandleProperties(Dictionary<string, string> properties) {
            if (properties == null) {
                return;
            }

            for (IEnumerator<string> e = properties.Keys.GetEnumerator(); e.MoveNext();) {
                string option = e.Current.ToString();

                Option opt = options.GetOption(option);
            
                if (opt == null) {
                    throw new UnrecognizedOptionException("Default option wasn't defined", option);
                }

                // if the option is part of a group, check if another option of the group has been selected
                OptionGroup group = options.GetOptionGroup(opt);
                bool selected = group != null && group.GetSelected() != null;

                if (!cmd.HasOption(option) && !selected) {
                    // get the value from the properties
                    string value = properties[option];

                    if (opt.HasArg()) {
                        if (opt.GetValues() == null || opt.GetValues().Length == 0) {
                            opt.AddValueForProcessing(value);
                        }
                    }
                    else if (!("yes".Equals(value, StringComparison.OrdinalIgnoreCase)
                        || "true".Equals(value, StringComparison.OrdinalIgnoreCase)
                        || "1".Equals(value, StringComparison.OrdinalIgnoreCase))) {
                        
                        // if the value is not yes, true or 1 then don't add the option to the CommandLine
                        continue;
                    }

                    HandleOption(opt);
                    currentOption = null;
                }
            }
        }


        /**
         * Throws a {@link MissingOptionException} if all of the required options
         * are not present.
         *
         * @throws MissingOptionException if any of the required Options
         * are not present.
         */
        private void CheckRequiredOptions() {
            // if there are required options that have not been processed
            if (expectedOpts.Count != 0) {
                throw new MissingOptionException(expectedOpts);
            }
        }


        /**
         * Throw a {@link MissingArgumentException} if the current option
         * didn't receive the number of arguments expected.
         */
        private void CheckRequiredArgs() {
            if (currentOption != null && currentOption.RequiresArg()) {
                throw new MissingArgumentException(currentOption);
            }
        }


        /**
         * Handle any command line token.
         *
         * @param token the command line token to handle
         * @throws ParseException
         */
        private void HandleToken(string token) {
            currentToken = token;

            if (skipParsing) {
                cmd.AddArg(token);
            }
            else if ("--".Equals(token)) {
                skipParsing = true;
            }
            else if (currentOption != null && currentOption.AcceptsArg() && IsArgument(token)) {
                currentOption.AddValueForProcessing(Util.StripLeadingAndTrailingQuotes(token));
            }
            else if (token.StartsWith("--")) {
                HandleLongOption(token);
            }
            else if (token.StartsWith("-") && !"-".Equals(token)) {
                HandleShortAndLongOption(token);
            }
            else {
                HandleUnknownToken(token);
            }

            if (currentOption != null && !currentOption.AcceptsArg()) {
                currentOption = null;
            }
        }


        /**
         * Returns true is the token is a valid argument.
         *
         * @param token
         */
        private bool IsArgument(string token) {
            return !IsOption(token) || IsNegativeNumber(token);
        }


        /**
         * Check if the token is a negative number.
         *
         * @param token
         */
        private bool IsNegativeNumber(string token) {
            try {
                Double.Parse(token);
                return true;
            }
            catch (FormatException) {
                return false;
            }
        }


        /**
         * Tells if the token looks like an option.
         *
         * @param token
         */
        private bool IsOption(string token) {
            return IsLongOption(token) || IsShortOption(token);
        }


        /**
         * Tells if the token looks like a short option.
         *
         * @param token
         */
        private bool IsShortOption(string token) {
            // short options (-S, -SV, -S=V, -SV1=V2, -S1S2)
            //            return token.StartsWith("-") && token.Length >= 2 && options.HasShortOption(token.Substring(1, 1));

            if (!token.StartsWith("-") || token.Length == 1) {
                return false;
            }

            // remove leading "-" and "=value"
            int pos = token.IndexOf("=");
            string optName = pos == -1 ? token.Substring(1) : token.Substring(1, pos);
            
            if (options.HasShortOption(optName)) {
                return true;
            }

            // check for several concatenated short options
            return optName.Length != 0 && options.HasShortOption(Convert.ToString(optName[0]));
        }


        /**
         * Tells if the token looks like a long option.
         *
         * @param token
         */
        private bool IsLongOption(string token) {
            if (!token.StartsWith("-") || token.Length == 1) {
                return false;
            }

            int pos = token.IndexOf("=");
            string t = pos == -1 ? token : token.Substring(0, pos);

            if (options.GetMatchingOptions(t).Count != 0) {
                // long or partial long options (--L, -L, --L=V, -L=V, --l, --l=V)
                return true;
            }
            else if (GetLongPrefix(token) != null && !token.StartsWith("--")) {
                // -LV
                return true;
            }

            return false;
        }


        /**
         * Handles an unknown token. If the token starts with a dash an
         * UnrecognizedOptionException is thrown. Otherwise the token is added
         * to the arguments of the command line. If the stopAtNonOption flag
         * is set, this stops the parsing and the remaining tokens are added
         * as-is in the arguments of the command line.
         *
         * @param token the command line token to handle
         */
        private void HandleUnknownToken(string token) {
            if (token.StartsWith("-") && token.Length > 1 && !stopAtNonOption) {
                throw new UnrecognizedOptionException("Unrecognized option: " + token, token);
            }

            cmd.AddArg(token);
        
            if (stopAtNonOption) {
                skipParsing = true;
            }
        }


        /**
         * Handles the following tokens:
         *
         * --L
         * --L=V
         * --L V
         * --l
         *
         * @param token the command line token to handle
         */
        private void HandleLongOption(string token) {
            if (token.IndexOf('=') == -1) {
                HandleLongOptionWithoutEqual(token);
            }
            else {
                HandleLongOptionWithEqual(token);
            }
        }


        /**
         * Handles the following tokens:
         *
         * --L
         * -L
         * --l
         * -l
         *
         * @param token the command line token to handle
         */
        private void HandleLongOptionWithoutEqual(string token) {
            IList<string> matchingOpts = options.GetMatchingOptions(token);
        
            if (matchingOpts.Count == 0) {
                HandleUnknownToken(currentToken);
            }
            else if (matchingOpts.Count > 1 && !options.HasLongOption(token)) {
                throw new AmbiguousOptionException(token, matchingOpts);
            }
            else {
                string key = options.HasLongOption(token) ? token : matchingOpts[0];
                HandleOption(options.GetOption(key));
            }
        }


        /**
         * Handles the following tokens:
         *
         * --L=V
         * -L=V
         * --l=V
         * -l=V
         *
         * @param token the command line token to handle
         */
        private void HandleLongOptionWithEqual(string token) {
            int pos = token.IndexOf('=');
            string value = token.Substring(pos + 1);
            string opt = token.Substring(0, pos);

            IList<string> matchingOpts = options.GetMatchingOptions(opt);
        
            if (matchingOpts.Count == 0) {
                HandleUnknownToken(currentToken);
            }
            else if (matchingOpts.Count > 1 && !options.HasLongOption(opt)) {
                throw new AmbiguousOptionException(opt, matchingOpts);
            }
            else {
                string key = options.HasLongOption(opt) ? opt : matchingOpts[0];
                Option option = options.GetOption(key);

                if (option.AcceptsArg()) {
                    HandleOption(option);
                    currentOption.AddValueForProcessing(value);
                    currentOption = null;
                }
                else {
                    HandleUnknownToken(currentToken);
                }
            }
        }


        /**
         * Handles the following tokens:
         *
         * -S
         * -SV
         * -S V
         * -S=V
         * -S1S2
         * -S1S2 V
         * -SV1=V2
         *
         * -L
         * -LV
         * -L V
         * -L=V
         * -l
         *
         * @param token the command line token to handle
         */
        private void HandleShortAndLongOption(string token) {
            string t = Util.StripLeadingHyphens(token);
            int pos = t.IndexOf('=');

            if (t.Length == 1) {
                // -S
                if (options.HasShortOption(t)) {
                    HandleOption(options.GetOption(t));
                }
                else {
                    HandleUnknownToken(token);
                }
            }
            else if (pos == -1) {
                // no equal sign found (-xxx)
                if (options.HasShortOption(t)) {
                    HandleOption(options.GetOption(t));
                }
                else if (GetMatchingLongOptions(t).Count != 0) {
                    // -L or -l
                    HandleLongOptionWithoutEqual(token);
                }
                else {
                    // look for a long prefix (-Xmx512m)
                    string opt = GetLongPrefix(t);

                    if (opt != null && options.GetOption(opt).AcceptsArg()) {
                        HandleOption(options.GetOption(opt));
                        
                        currentOption.AddValueForProcessing(t.Substring(opt.Length));
                        currentOption = null;
                    }
                    else if (IsJavaProperty(t)) {
                        // -SV1 (-Dflag)
                        HandleOption(options.GetOption(t.Substring(0, 1)));
                    
                        currentOption.AddValueForProcessing(t.Substring(1));
                        currentOption = null;
                    }
                    else {
                        // -S1S2S3 or -S1S2V
                        handleConcatenatedOptions(token);
                    }
                }
            }
            else {
                // equal sign found (-xxx=yyy)
                string opt = t.Substring(0, pos);
                string value = t.Substring(pos + 1);

                if (opt.Length == 1) {
                    // -S=V
                    Option option = options.GetOption(opt);
                
                    if (option != null && option.AcceptsArg()) {
                        HandleOption(option);
                    
                        currentOption.AddValueForProcessing(value);
                        currentOption = null;
                    }
                    else {
                        HandleUnknownToken(token);
                    }
                }
                else if (IsJavaProperty(opt)) {
                    // -SV1=V2 (-Dkey=value)
                    HandleOption(options.GetOption(opt.Substring(0, 1)));
                    
                    currentOption.AddValueForProcessing(opt.Substring(1));
                    currentOption.AddValueForProcessing(value);
                    currentOption = null;
                }
                else {
                    // -L=V or -l=V
                    HandleLongOptionWithEqual(token);
                }
            }
        }


        /**
         * Search for a prefix that is the long name of an option (-Xmx512m)
         *
         * @param token
         */
        private string GetLongPrefix(string token) {
            string t = Util.StripLeadingHyphens(token);

            int i;
            string opt = null;
        
            for (i = t.Length - 2; i > 1; i--) {
                string prefix = t.Substring(0, i);
            
                if (options.HasLongOption(prefix)) {
                    opt = prefix;
                    break;
                }
            }
        
            return opt;
        }


        /**
         * Check if the specified token is a Java-like property (-Dkey=value).
         */
        private bool IsJavaProperty(string token) {
            string opt = token.Substring(0, 1);
            Option option = options.GetOption(opt);

            return option != null && (option.GetArgs() >= 2 || option.GetArgs() == Option.UNLIMITED_VALUES);
        }

    
        private void HandleOption(Option option) {
            // check the previous option before handling the next one
            CheckRequiredArgs();

            option = (Option) option.Clone();

            UpdateRequiredOptions(option);

            cmd.AddOption(option);

            if (option.HasArg()) {
                currentOption = option;
            }
            else {
                currentOption = null;
            }
        }


        /**
         * Removes the option or its group from the list of expected elements.
         *
         * @param option
         */
        private void UpdateRequiredOptions(Option option) {
            if (option.IsRequired()) {
                expectedOpts.Remove(option.GetKey());
            }

            // if the option is in an OptionGroup make that option the selected option of the group
            if (options.GetOptionGroup(option) != null) {
                OptionGroup group = options.GetOptionGroup(option);

                if (group.IsRequired()) {
                    expectedOpts.Remove(group);
                }

                group.SetSelected(option);
            }
        }


        /**
         * Returns a list of matching option strings for the given token, depending
         * on the selected partial matching policy.
         *
         * @param token the token (may contain leading dashes)
         * @return the list of matching option strings or an empty list if no matching option could be found
         */
        private IList<string> GetMatchingLongOptions(string token) {
            if (allowPartialMatching) {
                return options.GetMatchingOptions(token);
            }

            List<string> matches = new List<string>(1);
            
            if (options.HasLongOption(token)) {
                Option option = options.GetOption(token);
                matches.Add(option.GetLongOpt());
            }

            return matches;
        }


        /**
         * Breaks <code>token</code> into its constituent parts
         * using the following algorithm.
         *
         * <ul>
         *  <li>ignore the first character ("<b>-</b>")</li>
         *  <li>for each remaining character check if an {@link Option}
         *  exists with that id.</li>
         *  <li>if an {@link Option} does exist then add that character
         *  prepended with "<b>-</b>" to the list of processed tokens.</li>
         *  <li>if the {@link Option} can have an argument value and there
         *  are remaining characters in the token then add the remaining
         *  characters as a token to the list of processed tokens.</li>
         *  <li>if an {@link Option} does <b>NOT</b> exist <b>AND</b>
         *  <code>stopAtNonOption</code> <b>IS</b> set then add the special token
         *  "<b>--</b>" followed by the remaining characters and also
         *  the remaining tokens directly to the processed tokens list.</li>
         *  <li>if an {@link Option} does <b>NOT</b> exist <b>AND</b>
         *  <code>stopAtNonOption</code> <b>IS NOT</b> set then add that
         *  character prepended with "<b>-</b>".</li>
         * </ul>
         *
         * @param token The current token to be <b>burst</b>
         * at the first non-Option encountered.
         * @throws ParseException if there are any problems encountered
         *                        while parsing the command line token.
         */
        protected void handleConcatenatedOptions(string token)  {
        
            for (int i = 1; i < token.Length; i++) {
                string ch = Convert.ToString(token[i]);

                if (!options.HasOption(ch)) {
                    HandleUnknownToken(stopAtNonOption && i > 1 ? token.Substring(i) : token);
                    break;
                }
    
                HandleOption(options.GetOption(ch));

                if (currentOption != null && token.Length != i + 1) {
                    // add the trail as an argument of the option
                    currentOption.AddValueForProcessing(token.Substring(i + 1));
                    break;
                }
            }
        }
    }
}
