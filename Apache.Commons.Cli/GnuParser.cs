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
     * The class GnuParser provides an implementation of the
     * {@link Parser#flatten(Options, String[], boolean) flatten} method.
     *
     * @deprecated since 1.3, use the {@link DefaultParser} instead
     */
    [Obsolete]
    public class GnuParser : Parser {

        /**
         * This flatten method does so using the following rules:
         * <ol>
         *   <li>If an {@link Option} exists for the first character of
         *   the <code>arguments</code> entry <b>AND</b> an {@link Option}
         *   does not exist for the whole <code>argument</code> then
         *   add the first character as an option to the processed tokens
         *   list e.g. "-D" and add the rest of the entry to the also.</li>
         *   <li>Otherwise just add the token to the processed tokens list.</li>
         * </ol>
         *
         * @param options         The Options to parse the arguments by.
         * @param arguments       The arguments that have to be flattened.
         * @param stopAtNonOption specifies whether to stop flattening when
         *                        a non option has been encountered
         * @return a String array of the flattened arguments
         */
        protected override string[] Flatten(Options options, string[] arguments, bool stopAtNonOption) {
            List<string> tokens = new List<string>();

            bool eatTheRest = false;

            for (int i = 0; i < arguments.Length; i++) {
                string arg = arguments[i];

                if ("--".Equals(arg)) {
                    eatTheRest = true;
                    tokens.Add("--");
                }
                else if ("-".Equals(arg)) {
                    tokens.Add("-");
                }
                else if (arg.StartsWith("-")) {
                    string opt = Util.StripLeadingHyphens(arg);

                    if (options.HasOption(opt)) {
                        tokens.Add(arg);
                    }
                    else {
                        if (opt.IndexOf('=') != -1 && options.HasOption(opt.Substring(0, opt.IndexOf('=')))) {
                            // the format is --foo=value or -foo=value
                            tokens.Add(arg.Substring(0, arg.IndexOf('='))); // --foo
                            tokens.Add(arg.Substring(arg.IndexOf('=') + 1)); // value
                        }
                        else if (options.HasOption(arg.Substring(0, 2))) {
                            // the format is a special properties option (-Dproperty=value)
                            tokens.Add(arg.Substring(0, 2)); // -D
                            tokens.Add(arg.Substring(2)); // property=value
                        }
                        else {
                            eatTheRest = stopAtNonOption;
                            tokens.Add(arg);
                        }
                    }
                }
                else {
                    tokens.Add(arg);
                }

                if (eatTheRest) {
                    for (i++; i < arguments.Length; i++) {
                        tokens.Add(arguments[i]);
                    }
                }
            }

            return tokens.ToArray();
        }
    }
}