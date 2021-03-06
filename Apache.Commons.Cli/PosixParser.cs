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
     * The class PosixParser provides an implementation of the
     * {@link Parser#flatten(Options,String[],boolean) flatten} method.
     *
     * @deprecated since 1.3, use the {@link DefaultParser} instead
     */
    [Obsolete]
    public class PosixParser : Parser {

        /** holder for flattened tokens */
        private readonly List<string> tokens = new List<string>();

        /** specifies if bursting should continue */
        private bool eatTheRest;

        /** holder for the current option */
        private Option currentOption;

        /** the command line Options */
        private Options options;


        /**
         * Resets the members to their original state i.e. remove
         * all of <code>tokens</code> entries and set <code>eatTheRest</code>
         * to false.
         */
        private void Init() {
            eatTheRest = false;
            tokens.Clear();
        }


        /**
         * <p>An implementation of {@link Parser}'s abstract
         * {@link Parser#flatten(Options,String[],boolean) flatten} method.</p>
         *
         * <p>The following are the rules used by this flatten method.</p>
         * <ol>
         *  <li>if <code>stopAtNonOption</code> is <b>true</b> then do not
         *  burst anymore of <code>arguments</code> entries, just add each
         *  successive entry without further processing.  Otherwise, ignore
         *  <code>stopAtNonOption</code>.</li>
         *  <li>if the current <code>arguments</code> entry is "<b>--</b>"
         *  just add the entry to the list of processed tokens</li>
         *  <li>if the current <code>arguments</code> entry is "<b>-</b>"
         *  just add the entry to the list of processed tokens</li>
         *  <li>if the current <code>arguments</code> entry is two characters
         *  in length and the first character is "<b>-</b>" then check if this
         *  is a valid {@link Option} id.  If it is a valid id, then add the
         *  entry to the list of processed tokens and set the current {@link Option}
         *  member.  If it is not a valid id and <code>stopAtNonOption</code>
         *  is true, then the remaining entries are copied to the list of
         *  processed tokens.  Otherwise, the current entry is ignored.</li>
         *  <li>if the current <code>arguments</code> entry is more than two
         *  characters in length and the first character is "<b>-</b>" then
         *  we need to burst the entry to determine its constituents.  For more
         *  information on the bursting algorithm see
         *  {@link PosixParser#burstToken(String, boolean) burstToken}.</li>
         *  <li>if the current <code>arguments</code> entry is not handled
         *  by any of the previous rules, then the entry is added to the list
         *  of processed tokens.</li>
         * </ol>
         *
         * @param options The command line {@link Options}
         * @param arguments The command line arguments to be parsed
         * @param stopAtNonOption Specifies whether to stop flattening
         * when an non option is found.
         * @return The flattened <code>arguments</code> String array.
         */
        protected override string[] Flatten(Options options, string[] arguments, bool stopAtNonOption) {
            Init();
            this.options = options;

            // an iterator for the command line tokens
            int argc = arguments.Length;

            // process each command line token
            for (int i = 0; i < argc; i++) {

                // get the next command line token
                string token = arguments[i];

                // single or double hyphen
                if ("-".Equals(token) || "--".Equals(token)) {
                    tokens.Add(token);
                }
            
                // handle long option --foo or --foo=bar
                else if (token.StartsWith("--")) {
                    int pos = token.IndexOf('=');
                    string opt = pos == -1 ? token : token.Substring(0, pos); // --foo
                
                    IList<string> matchingOpts = options.GetMatchingOptions(opt);

                    if (matchingOpts.Count == 0) {
                        ProcessNonOptionToken(token, stopAtNonOption);
                    }
                    else if (matchingOpts.Count > 1) {
                        throw new AmbiguousOptionException(opt, matchingOpts);
                    }
                    else {
                        currentOption = options.GetOption(matchingOpts[0]);
                    
                        tokens.Add("--" + currentOption.GetLongOpt());
                    
                        if (pos != -1) {
                            tokens.Add(token.Substring(pos + 1));
                        }
                    }
                }

                else if (token.StartsWith("-")) {
                    if (token.Length == 2 || options.HasOption(token)) {
                        ProcessOptionToken(token, stopAtNonOption);
                    }
                    else if (options.GetMatchingOptions(token).Count != 0) {
                        IList<string> matchingOpts = options.GetMatchingOptions(token);
                    
                        if (matchingOpts.Count > 1) {
                            throw new AmbiguousOptionException(token, matchingOpts);
                        }
                    
                        Option opt = options.GetOption(matchingOpts[0]);
                        ProcessOptionToken("-" + opt.GetLongOpt(), stopAtNonOption);
                    }
                
                    // requires bursting
                    else {
                        BurstToken(token, stopAtNonOption);
                    }
                }
                else {
                    ProcessNonOptionToken(token, stopAtNonOption);
                }

                Gobble(arguments, ref i);
            }

            return tokens.ToArray();
        }


        /**
         * Adds the remaining tokens to the processed tokens list.
         *
         * @param iter An iterator over the remaining tokens
         */
        private void Gobble(string[] args, ref int index) {
            if (eatTheRest) {
                int argc = args.Length;

                while (++index < argc) {
                    tokens.Add(args[index]);
                }
            }
        }


        /**
         * Add the special token "<b>--</b>" and the current <code>value</code>
         * to the processed tokens list. Then add all the remaining
         * <code>argument</code> values to the processed tokens list.
         *
         * @param value The current token
         */
        private void ProcessNonOptionToken(string value, bool stopAtNonOption) {
            if (stopAtNonOption && (currentOption == null || !currentOption.HasArg())) {
                eatTheRest = true;
                tokens.Add("--");
            }

            tokens.Add(value);
        }


        /**
         * <p>If an {@link Option} exists for <code>token</code> then
         * add the token to the processed list.</p>
         *
         * <p>If an {@link Option} does not exist and <code>stopAtNonOption</code>
         * is set then add the remaining tokens to the processed tokens list
         * directly.</p>
         *
         * @param token The current option token
         * @param stopAtNonOption Specifies whether flattening should halt
         * at the first non option.
         */
        private void ProcessOptionToken(string token, bool stopAtNonOption) {
            if (stopAtNonOption && !options.HasOption(token)) {
                eatTheRest = true;
            }

            if (options.HasOption(token)) {
                currentOption = options.GetOption(token);
            }

            tokens.Add(token);
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
         * @param stopAtNonOption Specifies whether to stop processing
         * at the first non-Option encountered.
         */
        protected void BurstToken(string token, bool stopAtNonOption) {
            for (int i = 1; i < token.Length; i++) {
                string ch = Convert.ToString(token[i]);

                if (options.HasOption(ch)) {
                    tokens.Add("-" + ch);
                    currentOption = options.GetOption(ch);

                    if (currentOption.HasArg() && token.Length != i + 1) {
                        tokens.Add(token.Substring(i + 1));
                        break;
                    }
                }
                else if (stopAtNonOption) {
                    ProcessNonOptionToken(token.Substring(i), true);
                    break;
                }
                else {
                    tokens.Add(token);
                    break;
                }
            }
        }
    }
}
