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
    using System.IO;
    using System.Text;

    /**
     * A formatter of help messages for command line options.
     *
     * <p>Example:</p>
     *
     * <pre>
     * Options options = new Options();
     * options.addOption(OptionBuilder.withLongOpt("file")
     *                                .withDescription("The file to be processed")
     *                                .hasArg()
     *                                .withArgName("FILE")
     *                                .isRequired()
     *                                .create('f'));
     * options.addOption(OptionBuilder.withLongOpt("version")
     *                                .withDescription("Print the version of the application")
     *                                .create('v'));
     * options.addOption(OptionBuilder.withLongOpt("help").create('h'));
     *
     * String header = "Do something useful with an input file\n\n";
     * String footer = "\nPlease report issues at http://example.com/issues";
     *
     * HelpFormatter formatter = new HelpFormatter();
     * formatter.printHelp("myapp", header, options, footer, true);
     * </pre>
     *
     * This produces the following output:
     *
     * <pre>
     * usage: myapp -f &lt;FILE&gt; [-h] [-v]
     * Do something useful with an input file
     *
     *  -f,--file &lt;FILE&gt;   The file to be processed
     *  -h,--help
     *  -v,--version       Print the version of the application
     *
     * Please report issues at http://example.com/issues
     * </pre>
     */
    public class HelpFormatter {

        /** default number of characters per line */
        public static readonly int DEFAULT_WIDTH = 74;

        /** default padding to the left of each line */
        public static readonly int DEFAULT_LEFT_PAD = 1;

        /** number of space characters to be prefixed to each description line */
        public static readonly int DEFAULT_DESC_PAD = 3;

        /** the string to display at the beginning of the usage statement */
        public static readonly string DEFAULT_SYNTAX_PREFIX = "usage: ";

        /** default prefix for shortOpts */
        public static readonly string DEFAULT_OPT_PREFIX = "-";

        /** default prefix for long Option */
        public static readonly string DEFAULT_LONG_OPT_PREFIX = "--";

        /**
         * default separator displayed between a long Option and its value
         *
         * @since 1.3
         **/
        public static readonly string DEFAULT_LONG_OPT_SEPARATOR = " ";

        /** default name for an argument */
        public static readonly string DEFAULT_ARG_NAME = "arg";


        // -------------------------------------------------------------- Attributes

        /**
         * number of characters per line
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setWidth methods instead.
         */
        [Obsolete]
        public int defaultWidth = DEFAULT_WIDTH;

        /**
         * amount of padding to the left of each line
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setLeftPadding methods instead.
         */
        [Obsolete]
        public int defaultLeftPad = DEFAULT_LEFT_PAD;

        /**
         * the number of characters of padding to be prefixed
         * to each description line
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setDescPadding methods instead.
         */
        [Obsolete]
        public int defaultDescPad = DEFAULT_DESC_PAD;

        /**
         * the string to display at the beginning of the usage statement
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setSyntaxPrefix methods instead.
         */
        [Obsolete]
        public string defaultSyntaxPrefix = DEFAULT_SYNTAX_PREFIX;

        /**
         * the new line string
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setNewLine methods instead.
         */
        [Obsolete]
        public string defaultNewLine = Environment.NewLine;

        /**
         * the shortOpt prefix
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setOptPrefix methods instead.
         */
        [Obsolete]
        public string defaultOptPrefix = DEFAULT_OPT_PREFIX;

        /**
         * the long Opt prefix
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setLongOptPrefix methods instead.
         */
        [Obsolete]
        public string defaultLongOptPrefix = DEFAULT_LONG_OPT_PREFIX;

        /**
         * the name of the argument
         *
         * @deprecated Scope will be made private for next major version
         * - use get/setArgName methods instead.
         */
        [Obsolete]
        public string defaultArgName = DEFAULT_ARG_NAME;

        /**
         * Comparator used to sort the options when they output in help text
         *
         * Defaults to case-insensitive alphabetical sorting by option key
         */
        protected IComparer<Option> optionComparator = new OptionComparator();

        /** The separator displayed between the long option and its value. */
        private string longOptSeparator = DEFAULT_LONG_OPT_SEPARATOR;


        /**
         * Sets the 'width'.
         *
         * @param width the new value of 'width'
         */
        public void SetWidth(int width) {
            this.defaultWidth = width;
        }


        /**
         * Returns the 'width'.
         *
         * @return the 'width'
         */
        public int GetWidth() {
            return defaultWidth;
        }


        /**
         * Sets the 'leftPadding'.
         *
         * @param padding the new value of 'leftPadding'
         */
        public void SetLeftPadding(int padding) {
            this.defaultLeftPad = padding;
        }


        /**
         * Returns the 'leftPadding'.
         *
         * @return the 'leftPadding'
         */
        public int GetLeftPadding() {
            return defaultLeftPad;
        }


        /**
         * Sets the 'descPadding'.
         *
         * @param padding the new value of 'descPadding'
         */
        public void SetDescPadding(int padding) {
            this.defaultDescPad = padding;
        }


        /**
         * Returns the 'descPadding'.
         *
         * @return the 'descPadding'
         */
        public int GetDescPadding() {
            return defaultDescPad;
        }


        /**
         * Sets the 'syntaxPrefix'.
         *
         * @param prefix the new value of 'syntaxPrefix'
         */
        public void SetSyntaxPrefix(string prefix) {
            this.defaultSyntaxPrefix = prefix;
        }


        /**
         * Returns the 'syntaxPrefix'.
         *
         * @return the 'syntaxPrefix'
         */
        public string GetSyntaxPrefix() {
            return defaultSyntaxPrefix;
        }


        /**
         * Sets the 'newLine'.
         *
         * @param newline the new value of 'newLine'
         */
        public void SetNewLine(string newline) {
            this.defaultNewLine = newline;
        }


        /**
         * Returns the 'newLine'.
         *
         * @return the 'newLine'
         */
        public string GetNewLine() {
            return defaultNewLine;
        }


        /**
         * Sets the 'optPrefix'.
         *
         * @param prefix the new value of 'optPrefix'
         */
        public void SetOptPrefix(string prefix) {
            this.defaultOptPrefix = prefix;
        }


        /**
         * Returns the 'optPrefix'.
         *
         * @return the 'optPrefix'
         */
        public string GetOptPrefix() {
            return defaultOptPrefix;
        }


        /**
         * Sets the 'longOptPrefix'.
         *
         * @param prefix the new value of 'longOptPrefix'
         */
        public void SetLongOptPrefix(string prefix) {
            this.defaultLongOptPrefix = prefix;
        }


        /**
         * Returns the 'longOptPrefix'.
         *
         * @return the 'longOptPrefix'
         */
        public string GetLongOptPrefix() {
            return defaultLongOptPrefix;
        }


        /**
         * Set the separator displayed between a long option and its value.
         * Ensure that the separator specified is supported by the parser used,
         * typically ' ' or '='.
         *
         * @param longOptSeparator the separator, typically ' ' or '='.
         * @since 1.3
         */
        public void SetLongOptSeparator(string longOptSeparator) {
            this.longOptSeparator = longOptSeparator;
        }


        /**
         * Returns the separator displayed between a long option and its value.
         *
         * @return the separator
         * @since 1.3
         */
        public string GetLongOptSeparator() {
            return longOptSeparator;
        }


        /**
         * Sets the 'argName'.
         *
         * @param name the new value of 'argName'
         */
        public void SetArgName(string name) {
            this.defaultArgName = name;
        }


        /**
         * Returns the 'argName'.
         *
         * @return the 'argName'
         */
        public string GetArgName() {
            return defaultArgName;
        }


        /**
         * Comparator used to sort the options when they output in help text.
         * Defaults to case-insensitive alphabetical sorting by option key.
         *
         * @return the {@link Comparator} currently in use to sort the options
         * @since 1.2
         */
        public IComparer<Option> GetOptionComparator() {
            return optionComparator;
        }


        /**
         * Set the comparator used to sort the options when they output in help text.
         * Passing in a null comparator will keep the options in the order they were declared.
         *
         * @param comparator the {@link Comparator} to use for sorting the options
         * @since 1.2
         */
        public void SetOptionComparator(IComparer<Option> comparator) {
            this.optionComparator = comparator;
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param cmdLineSyntax the syntax for this application
         * @param options the Options instance
         */
        public void PrintHelp(string cmdLineSyntax, Options options) {
            PrintHelp(GetWidth(), cmdLineSyntax, null, options, null, false);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param cmdLineSyntax the syntax for this application
         * @param options the Options instance
         * @param autoUsage whether to print an automatically generated
         * usage statement
         */
        public void PrintHelp(string cmdLineSyntax, Options options, bool autoUsage) {
            PrintHelp(GetWidth(), cmdLineSyntax, null, options, null, autoUsage);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param footer the banner to display at the end of the help
         */
        public void PrintHelp(string cmdLineSyntax, string header, Options options, string footer) {
            PrintHelp(cmdLineSyntax, header, options, footer, false);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param footer the banner to display at the end of the help
         * @param autoUsage whether to print an automatically generated
         * usage statement
         */
        public void PrintHelp(string cmdLineSyntax, string header, Options options, string footer, bool autoUsage) {
            PrintHelp(GetWidth(), cmdLineSyntax, header, options, footer, autoUsage);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param width the number of characters to be displayed on each line
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param footer the banner to display at the end of the help
         */
        public void PrintHelp(int width, string cmdLineSyntax, string header, Options options, string footer) {
            PrintHelp(width, cmdLineSyntax, header, options, footer, false);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.  This method prints help information to
         * System.out.
         *
         * @param width the number of characters to be displayed on each line
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param footer the banner to display at the end of the help
         * @param autoUsage whether to print an automatically generated
         * usage statement
         */
        public void PrintHelp(int width, string cmdLineSyntax, string header, Options options, string footer, bool autoUsage) {
            TextWriter pw = Console.Out;
            PrintHelp(pw, width, cmdLineSyntax, header, options, GetLeftPadding(), GetDescPadding(), footer, autoUsage);
            pw.Flush();
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.
         *
         * @param pw the writer to which the help will be written
         * @param width the number of characters to be displayed on each line
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param leftPad the number of characters of padding to be prefixed
         * to each line
         * @param descPad the number of characters of padding to be prefixed
         * to each description line
         * @param footer the banner to display at the end of the help
         *
         * @throws IllegalStateException if there is no room to print a line
         */
        public void PrintHelp(TextWriter pw, int width, string cmdLineSyntax, string header, Options options, int leftPad, int descPad, string footer) {
            PrintHelp(pw, width, cmdLineSyntax, header, options, leftPad, descPad, footer, false);
        }


        /**
         * Print the help for <code>options</code> with the specified
         * command line syntax.
         *
         * @param pw the writer to which the help will be written
         * @param width the number of characters to be displayed on each line
         * @param cmdLineSyntax the syntax for this application
         * @param header the banner to display at the beginning of the help
         * @param options the Options instance
         * @param leftPad the number of characters of padding to be prefixed
         * to each line
         * @param descPad the number of characters of padding to be prefixed
         * to each description line
         * @param footer the banner to display at the end of the help
         * @param autoUsage whether to print an automatically generated
         * usage statement
         *
         * @throws IllegalStateException if there is no room to print a line
         */
        public void PrintHelp(TextWriter pw, int width, string cmdLineSyntax, string header, Options options, int leftPad, int descPad, string footer, bool autoUsage) {
            if (cmdLineSyntax == null || cmdLineSyntax.Length == 0) {
                throw new ArgumentException("cmdLineSyntax not provided");
            }

            if (autoUsage) {
                PrintUsage(pw, width, cmdLineSyntax, options);
            }
            else {
                PrintUsage(pw, width, cmdLineSyntax);
            }

            if (header != null && header.Trim().Length > 0) {
                PrintWrapped(pw, width, header);
            }

            PrintOptions(pw, width, options, leftPad, descPad);

            if (footer != null && footer.Trim().Length > 0) {
                PrintWrapped(pw, width, footer);
            }
        }


        /**
         * Prints the usage statement for the specified application.
         *
         * @param pw The PrintWriter to print the usage statement
         * @param width The number of characters to display per line
         * @param app The application name
         * @param options The command line Options
         */
        public void PrintUsage(TextWriter pw, int width, string app, Options options) {
            // initialise the string buffer
            StringBuilder buff = new StringBuilder(GetSyntaxPrefix()).Append(app).Append(" ");

            // create a list for processed option groups
            List<OptionGroup> processedGroups = new List<OptionGroup>();

            List<Option> optList = new List<Option>(options.GetOptions());
            
            if (GetOptionComparator() != null) {
                optList.Sort(GetOptionComparator());
            }

            // iterate over the options
            for (int i = 0; i < optList.Count; i++) {

                // get the next Option
                Option option = optList[i];

                // check if the option is part of an OptionGroup
                OptionGroup group = options.GetOptionGroup(option);

                // if the option is part of a group 
                if (group != null) {

                    // and if the group has not already been processed
                    if (!processedGroups.Contains(group)) {

                        // add the group to the processed list
                        processedGroups.Add(group);

                        // add the usage clause
                        AppendOptionGroup(buff, group);
                    }

                    // otherwise the option was displayed in the group
                    // previously so ignore it.
                }

                // if the Option is not part of an OptionGroup
                else {
                    AppendOption(buff, option, option.IsRequired());
                }

                if (i < optList.Count - 1) {
                    buff.Append(" ");
                }
            }

            // call printWrapped
            PrintWrapped(pw, width, buff.ToString().IndexOf(' ') + 1, buff.ToString());
        }


        /**
         * Appends the usage clause for an OptionGroup to a StringBuffer.
         * The clause is wrapped in square brackets if the group is required.
         * The display of the options is handled by appendOption
         * @param buff the StringBuffer to append to
         * @param group the group to append
         * @see #appendOption(StringBuffer,Option,boolean)
         */
        private void AppendOptionGroup(StringBuilder buff, OptionGroup group) {
            if (!group.IsRequired()) {
                buff.Append("[");
            }

            List<Option> optList = new List<Option>(group.GetOptions());

            if (GetOptionComparator() != null) {
                optList.Sort(GetOptionComparator());
            }

            // for each option in the OptionGroup
            for (int i = 0; i < optList.Count; i++) {

                // whether the option is required or not is handled at group level
                AppendOption(buff, optList[i], true);

                if (i < optList.Count - 1) {
                    buff.Append(" | ");
                }
            }

            if (!group.IsRequired()) {
                buff.Append("]");
            }
        }


        /**
         * Appends the usage clause for an Option to a StringBuffer.
         *
         * @param buff the StringBuffer to append to
         * @param option the Option to append
         * @param required whether the Option is required or not
         */
        private void AppendOption(StringBuilder buff, Option option, bool required) {
            if (!required) {
                buff.Append("[");
            }

            if (option.GetOpt() != null) {
                buff.Append("-").Append(option.GetOpt());
            }
            else {
                buff.Append("--").Append(option.GetLongOpt());
            }

            // if the Option has a value and a non blank argname
            if (option.HasArg() && (option.GetArgName() == null || option.GetArgName().Length != 0)) {
                buff.Append(option.GetOpt() == null ? longOptSeparator : " ");
                buff.Append("<").Append(option.GetArgName() != null ? option.GetArgName() : GetArgName()).Append(">");
            }

            // if the Option is not a required option
            if (!required) {
                buff.Append("]");
            }
        }


        /**
         * Print the cmdLineSyntax to the specified writer, using the
         * specified width.
         *
         * @param pw The printWriter to write the help to
         * @param width The number of characters per line for the usage statement.
         * @param cmdLineSyntax The usage statement.
         */
        public void PrintUsage(TextWriter pw, int width, string cmdLineSyntax) {
            int argPos = cmdLineSyntax.IndexOf(' ') + 1;

            PrintWrapped(pw, width, GetSyntaxPrefix().Length + argPos, GetSyntaxPrefix() + cmdLineSyntax);
        }


        /**
         * Print the help for the specified Options to the specified writer,
         * using the specified width, left padding and description padding.
         *
         * @param pw The printWriter to write the help to
         * @param width The number of characters to display per line
         * @param options The command line Options
         * @param leftPad the number of characters of padding to be prefixed
         * to each line
         * @param descPad the number of characters of padding to be prefixed
         * to each description line
         */
        public void PrintOptions(TextWriter pw, int width, Options options, int leftPad, int descPad) {
            StringBuilder sb = new StringBuilder();

            RenderOptions(sb, width, options, leftPad, descPad);
            pw.WriteLine(sb.ToString());
        }


        /**
         * Print the specified text to the specified PrintWriter.
         *
         * @param pw The printWriter to write the help to
         * @param width The number of characters to display per line
         * @param text The text to be written to the PrintWriter
         */
        public void PrintWrapped(TextWriter pw, int width, string text) {
            PrintWrapped(pw, width, 0, text);
        }


        /**
         * Print the specified text to the specified PrintWriter.
         *
         * @param pw The printWriter to write the help to
         * @param width The number of characters to display per line
         * @param nextLineTabStop The position on the next line for the first tab.
         * @param text The text to be written to the PrintWriter
         */
        public void PrintWrapped(TextWriter pw, int width, int nextLineTabStop, string text) {
            StringBuilder sb = new StringBuilder(text.Length);

            RenderWrappedTextBlock(sb, width, nextLineTabStop, text);
            pw.WriteLine(sb.ToString());
        }


        // --------------------------------------------------------------- Protected

        /**
         * Render the specified Options and return the rendered Options
         * in a StringBuffer.
         *
         * @param sb The StringBuffer to place the rendered Options into.
         * @param width The number of characters to display per line
         * @param options The command line Options
         * @param leftPad the number of characters of padding to be prefixed
         * to each line
         * @param descPad the number of characters of padding to be prefixed
         * to each description line
         *
         * @return the StringBuffer with the rendered Options contents.
         */
        public StringBuilder RenderOptions(StringBuilder sb, int width, Options options, int leftPad, int descPad) {
            string lpad = CreatePadding(leftPad);
            string dpad = CreatePadding(descPad);

            // first create list containing only <lpad>-a,--aaa where 
            // -a is opt and --aaa is long opt; in parallel look for 
            // the longest opt string this list will be then used to 
            // sort options ascending
            int max = 0;

            List<StringBuilder> prefixList = new List<StringBuilder>();
            List<Option> optList = options.HelpOptions();

            if (GetOptionComparator() != null) {
                optList.Sort(GetOptionComparator());
            }

            foreach (Option option in optList) {
                StringBuilder optBuf = new StringBuilder();

                if (option.GetOpt() == null) {
                    optBuf.Append(lpad).Append("   ").Append(GetLongOptPrefix()).Append(option.GetLongOpt());
                }
                else {
                    optBuf.Append(lpad).Append(GetOptPrefix()).Append(option.GetOpt());

                    if (option.HasLongOpt()) {
                        optBuf.Append(',').Append(GetLongOptPrefix()).Append(option.GetLongOpt());
                    }
                }

                if (option.HasArg()) {
                    string argName = option.GetArgName();
                    
                    if (argName != null && argName.Length == 0) {
                        // if the option has a blank argname
                        optBuf.Append(' ');
                    }
                    else {
                        optBuf.Append(option.HasLongOpt() ? longOptSeparator : " ");
                        optBuf.Append("<").Append(argName != null ? option.GetArgName() : GetArgName()).Append(">");
                    }
                }

                prefixList.Add(optBuf);
                max = optBuf.Length > max ? optBuf.Length : max;
            }

            int x = 0;

            for (int i = 0; i < optList.Count; i++) {
                Option option = optList[i];
                StringBuilder optBuf = new StringBuilder(prefixList[x++].ToString());

                if (optBuf.Length < max) {
                    optBuf.Append(CreatePadding(max - optBuf.Length));
                }

                optBuf.Append(dpad);

                int nextLineTabStop = max + descPad;

                if (option.GetDescription() != null) {
                    optBuf.Append(option.GetDescription());
                }

                RenderWrappedText(sb, width, nextLineTabStop, optBuf.ToString());

                if (i < optList.Count - 1) {
                    sb.Append(GetNewLine());
                }
            }

            return sb;
        }


        /**
         * Render the specified text and return the rendered Options
         * in a StringBuffer.
         *
         * @param sb The StringBuffer to place the rendered text into.
         * @param width The number of characters to display per line
         * @param nextLineTabStop The position on the next line for the first tab.
         * @param text The text to be rendered.
         *
         * @return the StringBuffer with the rendered Options contents.
         */
        public StringBuilder RenderWrappedText(StringBuilder sb, int width, int nextLineTabStop, string text) {
            int pos = FindWrapPos(text, width, 0);

            if (pos == -1) {
                sb.Append(RTrim(text));
                return sb;
            }

            sb.Append(RTrim(text.Substring(0, pos))).Append(GetNewLine());

            if (nextLineTabStop >= width) {
                // stops infinite loop happening
                nextLineTabStop = 1;
            }

            // all following lines must be padded with nextLineTabStop space characters
            string padding = CreatePadding(nextLineTabStop);

            while (true) {
                text = padding + text.Substring(pos).Trim();
                pos = FindWrapPos(text, width, 0);

                if (pos == -1) {
                    sb.Append(text);
                    return sb;
                }

                if (text.Length > width && pos == nextLineTabStop - 1) {
                    pos = width;
                }

                sb.Append(RTrim(text.Substring(0, pos))).Append(GetNewLine());
            }
        }


        /**
         * Render the specified text width a maximum width. This method differs
         * from renderWrappedText by not removing leading spaces after a new line.
         *
         * @param sb The StringBuffer to place the rendered text into.
         * @param width The number of characters to display per line
         * @param nextLineTabStop The position on the next line for the first tab.
         * @param text The text to be rendered.
         */
        private StringBuilder RenderWrappedTextBlock(StringBuilder sb, int width, int nextLineTabStop, string text) {
            try {
                TextReader inReader = new StringReader(text);
                string line;
                bool firstLine = true;
                
                while ((line = inReader.ReadLine()) != null) {
                    if (!firstLine) {
                        sb.Append(GetNewLine());
                    }
                    else {
                        firstLine = false;
                    }

                    RenderWrappedText(sb, width, nextLineTabStop, line);
                }
            }
            catch (IOException) {
                // cannot happen
            }

            return sb;
        }


        /**
         * Finds the next text wrap position after <code>startPos</code> for the
         * text in <code>text</code> with the column width <code>width</code>.
         * The wrap point is the last position before startPos+width having a
         * whitespace character (space, \n, \r). If there is no whitespace character
         * before startPos+width, it will return startPos+width.
         *
         * @param text The text being searched for the wrap position
         * @param width width of the wrapped text
         * @param startPos position from which to start the lookup whitespace
         * character
         * @return position on which the text must be wrapped or -1 if the wrap
         * position is at the end of the text
         */
        internal protected int FindWrapPos(string text, int width, int startPos) {
            // the line ends before the max wrap pos or a new line char found
            int pos = text.IndexOf('\n', startPos);
            
            if (pos != -1 && pos <= width) {
                return pos + 1;
            }

            pos = text.IndexOf('\t', startPos);
            
            if (pos != -1 && pos <= width) {
                return pos + 1;
            }

            if (startPos + width >= text.Length) {
                return -1;
            }

            // look for the last whitespace character before startPos+width
            for (pos = startPos + width; pos >= startPos; --pos) {
                char c = text[pos];

                if (c == ' ' || c == '\n' || c == '\r') {
                    break;
                }
            }

            // if we found it - just return
            if (pos > startPos) {
                return pos;
            }

            // if we didn't find one, simply chop at startPos+width
            pos = startPos + width;

            return pos == text.Length ? -1 : pos;
        }


        /**
         * Return a String of padding of length <code>len</code>.
         *
         * @param len The length of the String of padding to create.
         *
         * @return The String of padding
         */
        internal protected string CreatePadding(int len) {
            return new string(' ', len);
        }


        /**
         * Remove the trailing whitespace from the specified String.
         *
         * @param s The String to remove the trailing padding from.
         *
         * @return The String of without the trailing padding
         */
        internal protected string RTrim(string s) {
            if (s == null || s.Length == 0) {
                return s;
            }

            int pos = s.Length;

            while (pos > 0 && char.IsWhiteSpace(s[pos - 1])) {
                --pos;
            }

            return s.Substring(0, pos);
        }


        // ------------------------------------------------------ Package protected
        // ---------------------------------------------------------------- Private
        // ---------------------------------------------------------- Inner classes
        /**
         * This class implements the <code>Comparator</code> interface
         * for comparing Options.
         */
        [Serializable]
        private class OptionComparator : IComparer<Option> {

            /**
             * Compares its two arguments for order. Returns a negative
             * integer, zero, or a positive integer as the first argument
             * is less than, equal to, or greater than the second.
             *
             * @param opt1 The first Option to be compared.
             * @param opt2 The second Option to be compared.
             * @return a negative integer, zero, or a positive integer as
             *         the first argument is less than, equal to, or greater than the
             *         second.
             */
            int IComparer<Option>.Compare(Option opt1, Option opt2) {

                if (!String.IsNullOrEmpty(opt1.GetOpt()) && !String.IsNullOrEmpty(opt2.GetOpt())) {
                    if (String.Compare(opt1.GetOpt(), opt2.GetOpt(), StringComparison.OrdinalIgnoreCase) != 0) {
                        return String.Compare(opt1.GetOpt(), opt2.GetOpt(), StringComparison.OrdinalIgnoreCase);
                    }
                    else if (String.IsNullOrEmpty(opt1.GetLongOpt()) && String.IsNullOrEmpty(opt2.GetLongOpt())) {
                        return -String.Compare(opt1.GetOpt(), opt2.GetOpt(), StringComparison.Ordinal);
                    }
                    else {
                        return CompareStringIgnoreCase((String.IsNullOrEmpty(opt1.GetLongOpt()) ? opt1.GetOpt() : opt1.GetLongOpt()),
                            (String.IsNullOrEmpty(opt2.GetLongOpt()) ? opt2.GetOpt() : opt2.GetLongOpt()));
                    }
                }
                else if (String.IsNullOrEmpty(opt1.GetOpt()) && String.IsNullOrEmpty(opt2.GetOpt())) {
                    return CompareStringIgnoreCase((String.IsNullOrEmpty(opt1.GetLongOpt()) ? String.Empty : opt1.GetLongOpt()),
                        (String.IsNullOrEmpty(opt2.GetLongOpt()) ? String.Empty : opt2.GetLongOpt()));
                }
                else if (String.IsNullOrEmpty(opt1.GetOpt())) {
                    if (String.Compare((String.IsNullOrEmpty(opt1.GetLongOpt()) ? String.Empty : opt1.GetLongOpt()).Substring(0, 1), opt2.GetOpt(), StringComparison.OrdinalIgnoreCase) != 0) {
                        return String.Compare((String.IsNullOrEmpty(opt1.GetLongOpt()) ? String.Empty : opt1.GetLongOpt()).Substring(0, 1), opt2.GetOpt(), StringComparison.OrdinalIgnoreCase);
                    }

                    return CompareStringIgnoreCase((String.IsNullOrEmpty(opt1.GetLongOpt()) ? String.Empty : opt1.GetLongOpt()),
                        (String.IsNullOrEmpty(opt2.GetLongOpt()) ? opt2.GetOpt() : opt2.GetLongOpt()));
                }
                else {
                    if (String.Compare(opt1.GetOpt(), (String.IsNullOrEmpty(opt2.GetLongOpt()) ? String.Empty : opt2.GetLongOpt()).Substring(0, 1), StringComparison.OrdinalIgnoreCase) != 0) {
                        return String.Compare(opt1.GetOpt(), (String.IsNullOrEmpty(opt2.GetLongOpt()) ? String.Empty : opt2.GetLongOpt()).Substring(0, 1), StringComparison.OrdinalIgnoreCase);
                    }

                    return CompareStringIgnoreCase(
                        (String.IsNullOrEmpty(opt1.GetLongOpt()) ? opt1.GetOpt() : opt1.GetLongOpt()),
                        (String.IsNullOrEmpty(opt2.GetLongOpt()) ? String.Empty : opt2.GetLongOpt()));
                }
            }


            int CompareStringIgnoreCase(string str1, string str2) {
                int i = 0;

                while (i < str1.Length && i < str2.Length) {
                    if ((Char.IsLetter(str1[i]) && Char.IsLetter(str2[i])) &&
                        ((Char.IsUpper(str1[i]) && Char.IsLower(str2[i])) ||
                        (Char.IsLower(str1[i]) && Char.IsUpper(str2[i])))) {
                        return (str2[i] - str1[i]);
                    }
                    else if (String.Compare(str1.Substring(i, 1), str2.Substring(i,1), StringComparison.OrdinalIgnoreCase) != 0) {
                        return String.Compare(str1.Substring(i, 1), str2.Substring(i, 1), StringComparison.OrdinalIgnoreCase);
                    }

                    i++;
                }

                if (i < str1.Length) {
                    return 1;
                }

                if (i < str2.Length) {
                    return -1;
                }

                return 0;
            }
        }
    }
}
