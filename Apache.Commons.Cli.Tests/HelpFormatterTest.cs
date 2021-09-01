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

    using Xunit;

    /**
     * Test case for the HelpFormatter class.
     */
    public class HelpFormatterTest {
        
        private static readonly string EOL = Environment.NewLine;


        [Fact]
        public void FindWrapPosTest() {
            HelpFormatter hf = new HelpFormatter();

            string text = "This is a test.";
            
            // text width should be max 8; the wrap position is 7
            Assert.Equal(7, hf.FindWrapPos(text, 8, 0));

            // starting from 8 must give -1 - the wrap pos is after end
            Assert.Equal(-1, hf.FindWrapPos(text, 8, 8));

            // words longer than the width are cut
            text = "aaaa aa";
            Assert.Equal(3, hf.FindWrapPos(text, 3, 0));

            // last word length is equal to the width
            text = "aaaaaa aaaaaa";
            Assert.Equal(6, hf.FindWrapPos(text, 6, 0));
            Assert.Equal(-1, hf.FindWrapPos(text, 6, 7));

            text = "aaaaaa\n aaaaaa";
            Assert.Equal(7, hf.FindWrapPos(text, 6, 0));

            text = "aaaaaa\t aaaaaa";
            Assert.Equal(7, hf.FindWrapPos(text, 6, 0));
        }

        
        [Fact]
        public void RenderWrappedTextWordCutTest() {
            int width = 7;
            int padding = 0;
            string text = "Thisisatest.";
            string expected = "Thisisa" + EOL +
                              "test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, text);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void RenderWrappedTextSingleLineTest() {
            // single line text
            int width = 12;
            int padding = 0;
            string text = "This is a test.";
            string expected = "This is a" + EOL +
                              "test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, text);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void RenderWrappedTextSingleLinePaddedTest() {
            // single line padded text
            int width = 12;
            int padding = 4;
            string text = "This is a test.";
            string expected = "This is a" + EOL +
                              "    test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, text);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void RenderWrappedTextSingleLinePadded2Test() {
            // single line padded text 2
            int width = 53;
            int padding = 24;
            string text = "  -p,--period <PERIOD>  PERIOD is time duration of form " +
                          "DATE[-DATE] where DATE has form YYYY[MM[DD]]";
            string expected = "  -p,--period <PERIOD>  PERIOD is time duration of" + EOL +
                              "                        form DATE[-DATE] where DATE" + EOL +
                              "                        has form YYYY[MM[DD]]";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, text);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void RenderWrappedTextMultiLineTest() {
            // multi line text
            int width = 16;
            int padding = 0;
            string expected = "aaaa aaaa aaaa" + EOL +
                          "aaaaaa" + EOL +
                          "aaaaa";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, expected);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void RenderWrappedTextMultiLinePaddedTest() {
            // multi-line padded text
            int width = 16;
            int padding = 4;
            string text = "aaaa aaaa aaaa" + EOL +
                          "aaaaaa" + EOL +
                          "aaaaa";
            string expected = "aaaa aaaa aaaa" + EOL +
                              "    aaaaaa" + EOL +
                              "    aaaaa";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().RenderWrappedText(sb, width, padding, text);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void PrintOptionsTest() {
            StringBuilder sb = new StringBuilder();
            HelpFormatter hf = new HelpFormatter();
            
            int leftPad = 1;
            int descPad = 3;
            string lpad = hf.CreatePadding(leftPad);
            string dpad = hf.CreatePadding(descPad);
            Options options;
            string expected;

            options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
            expected = lpad + "-a" + dpad + "aaaa aaaa aaaa aaaa aaaa";
            hf.RenderOptions(sb, 60, options, leftPad, descPad);
            
            Assert.Equal(expected, sb.ToString());

            int nextLineTabStop = leftPad + descPad + "-a".Length;
            expected = lpad + "-a" + dpad + "aaaa aaaa aaaa" + EOL +
                       hf.CreatePadding(nextLineTabStop) + "aaaa aaaa";
            sb.Length = 0;
            hf.RenderOptions(sb, nextLineTabStop + 17, options, leftPad, descPad);
            
            Assert.Equal(expected, sb.ToString());


            options = new Options().AddOption("a", "aaa", false, "dddd dddd dddd dddd");
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd dddd dddd";
            sb.Length = 0;
            hf.RenderOptions(sb, 60, options, leftPad, descPad);
            
            Assert.Equal(expected, sb.ToString());

            nextLineTabStop = leftPad + descPad + "-a,--aaa".Length;
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + EOL +
                       hf.CreatePadding(nextLineTabStop) + "dddd dddd";
            sb.Length = 0;
            hf.RenderOptions(sb, 25, options, leftPad, descPad);
            
            Assert.Equal(expected, sb.ToString());

            options = new Options().
                    AddOption("a", "aaa", false, "dddd dddd dddd dddd").
                    AddOption("b", false, "feeee eeee eeee eeee");
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + EOL +
                       hf.CreatePadding(nextLineTabStop) + "dddd dddd" + EOL +
                       lpad + "-b      " + dpad + "feeee eeee" + EOL +
                       hf.CreatePadding(nextLineTabStop) + "eeee eeee";
            sb.Length = 0;
            hf.RenderOptions(sb, 25, options, leftPad, descPad);
            
            Assert.Equal(expected, sb.ToString());
        }


        [Fact]
        public void PrintHelpWithEmptySyntaxTest() {
            HelpFormatter formatter = new HelpFormatter();

            Assert.Throws<ArgumentException>(() => formatter.PrintHelp(null, new Options()));
            Assert.Throws<ArgumentException>(() => formatter.PrintHelp("", new Options()));
        }


        [Fact]
        public void AutomaticUsageTest() {
            HelpFormatter hf = new HelpFormatter();
            Options options;
            string expected = "usage: app [-a]";

            MemoryStream outStream = new MemoryStream();
            StreamWriter pw = new StreamWriter(outStream);

            options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
            hf.PrintUsage(pw, 60, "app", options);
            pw.Flush();

            Assert.Equal(expected, Encoding.Default.GetString(outStream.ToArray()).Trim());

            outStream.Position = 0;
            outStream.SetLength(0);

            expected = "usage: app [-a] [-b]";
            options = new Options()
                .AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa")
                .AddOption("b", false, "bbb");
            
            hf.PrintUsage(pw, 60, "app", options);
            pw.Flush();
            
            Assert.Equal(expected, Encoding.Default.GetString(outStream.ToArray()).Trim());
            
            outStream.Position = 0;
            outStream.SetLength(0);

        }


        // This test ensures the options are properly sorted
        // See https://issues.apache.org/jira/browse/CLI-131
        [Fact]
        public void PrintUsageTest() {
            Option optionA = new Option("a", "first");
            Option optionB = new Option("b", "second");
            Option optionC = new Option("c", "third");
            Options opts = new Options();
            
            opts.AddOption(optionA);
            opts.AddOption(optionB);
            opts.AddOption(optionC);
            
            HelpFormatter helpFormatter = new HelpFormatter();
            MemoryStream bytesOut = new MemoryStream();
            StreamWriter printWriter = new StreamWriter(bytesOut);
            helpFormatter.PrintUsage(printWriter, 80, "app", opts);
            printWriter.Close();
            
            Assert.Equal("usage: app [-a] [-b] [-c]" + EOL, Encoding.Default.GetString(bytesOut.ToArray()));
        }


        internal sealed class SortedComparer : IComparer<Option> {
            
            public int Compare(Option opt1, Option opt2) {
                // reverses the functionality of the default comparator
                return opt2.GetKey().CompareTo(opt1.GetKey());
            }
        }


        // uses the test for CLI-131 to implement CLI-155
        [Fact]
        public void PrintSortedUsageTest() {
            Options opts = new Options();
            opts.AddOption(new Option("a", "first"));
            opts.AddOption(new Option("b", "second"));
            opts.AddOption(new Option("c", "third"));

            HelpFormatter helpFormatter = new HelpFormatter();

            helpFormatter.SetOptionComparator(new SortedComparer());

            StringWriter outWriter = new StringWriter();
            helpFormatter.PrintUsage(outWriter, 80, "app", opts);

            Assert.Equal("usage: app [-c] [-b] [-a]" + EOL, outWriter.ToString());
        }


        [Fact]
        public void PrintSortedUsageWithNullComparatorTest() {
            Options opts = new Options();
            opts.AddOption(new Option("c", "first"));
            opts.AddOption(new Option("b", "second"));
            opts.AddOption(new Option("a", "third"));

            HelpFormatter helpFormatter = new HelpFormatter();
            helpFormatter.SetOptionComparator(null);

            StringWriter outWriter = new StringWriter();
            helpFormatter.PrintUsage(outWriter, 80, "app", opts);

            Assert.Equal("usage: app [-c] [-b] [-a]" + EOL, outWriter.ToString());
        }


        [Fact]
        public void PrintOptionGroupUsageTest() {
            OptionGroup group = new OptionGroup();
            group.AddOption(Option.Builder("a").Build());
            group.AddOption(Option.Builder("b").Build());
            group.AddOption(Option.Builder("c").Build());

            Options options = new Options();
            options.AddOptionGroup(group);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.PrintUsage(outWriter, 80, "app", options);

            Assert.Equal("usage: app [-a | -b | -c]" + EOL, outWriter.ToString());
        }


        [Fact]
        public void PrintRequiredOptionGroupUsageTest() {
            OptionGroup group = new OptionGroup();
            group.AddOption(Option.Builder("a").Build());
            group.AddOption(Option.Builder("b").Build());
            group.AddOption(Option.Builder("c").Build());
            group.SetRequired(true);

            Options options = new Options();
            options.AddOptionGroup(group);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.PrintUsage(outWriter, 80, "app", options);

            Assert.Equal("usage: app -a | -b | -c" + EOL, outWriter.ToString());
        }


        [Fact]
        public void PrintOptionWithEmptyArgNameUsageTest() {
            Option option = new Option("f", true, null);
            option.SetArgName("");
            option.SetRequired(true);

            Options options = new Options();
            options.AddOption(option);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.PrintUsage(outWriter, 80, "app", options);

            Assert.Equal("usage: app -f" + EOL, outWriter.ToString());
        }


        [Fact]
        public void DefaultArgNameTest() {
            Option option = Option.Builder("f").HasArg().Required(true).Build();

            Options options = new Options();
            options.AddOption(option);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.SetArgName("argument");
            formatter.PrintUsage(outWriter, 80, "app", options);

            Assert.Equal("usage: app -f <argument>" + EOL, outWriter.ToString());
        }


        [Fact]
        public void RTrimTest() {
            HelpFormatter formatter = new HelpFormatter();

            Assert.Null(formatter.RTrim(null));
            Assert.Equal("", formatter.RTrim(""));
            Assert.Equal("  foo", formatter.RTrim("  foo  "));
        }


        [Fact]
        public void AccessorsTest() {
            HelpFormatter formatter = new HelpFormatter();

            formatter.SetArgName("argname");
            Assert.Equal("argname", formatter.GetArgName());

            formatter.SetDescPadding(3);
            Assert.Equal(3, formatter.GetDescPadding());

            formatter.SetLeftPadding(7);
            Assert.Equal(7, formatter.GetLeftPadding());

            formatter.SetLongOptPrefix("~~");
            Assert.Equal("~~", formatter.GetLongOptPrefix());

            formatter.SetNewLine("\n");
            Assert.Equal("\n", formatter.GetNewLine());

            formatter.SetOptPrefix("~");
            Assert.Equal("~", formatter.GetOptPrefix());

            formatter.SetSyntaxPrefix("-> ");
            Assert.Equal("-> ", formatter.GetSyntaxPrefix());

            formatter.SetWidth(80);
            Assert.Equal(80, formatter.GetWidth());
        }


        [Fact]
        public void HeaderStartingWithLineSeparatorTest() {
            // related to Bugzilla #21215
            Options options = new Options();
            HelpFormatter formatter = new HelpFormatter();
            string header = EOL + "Header";
            string footer = "Footer";
            StringWriter outWriter = new StringWriter();
            
            formatter.PrintHelp(outWriter, 80, "foobar", header, options, 2, 2, footer, true);
            
            Assert.Equal(
                    "usage: foobar" + EOL +
                    "" + EOL +
                    "Header" + EOL +
                    "" + EOL +
                    "Footer" + EOL
                    , outWriter.ToString());
        }


        [Fact]
        public void IndentedHeaderAndFooterTest() {
            // related to CLI-207
            Options options = new Options();
            HelpFormatter formatter = new HelpFormatter();
            string header = "  Header1\n  Header2";
            string footer = "  Footer1\n  Footer2";
            StringWriter outWriter = new StringWriter();
            
            formatter.PrintHelp(outWriter, 80, "foobar", header, options, 2, 2, footer, true);

            Assert.Equal(
                    "usage: foobar" + EOL +
                    "  Header1" + EOL +
                    "  Header2" + EOL +
                    "" + EOL +
                    "  Footer1" + EOL +
                    "  Footer2" + EOL
                    , outWriter.ToString());
        }


        [Fact]
        public void OptionWithoutShortFormatTest() {
            // related to Bugzilla #19383 (CLI-67)
            Options options = new Options();
            options.AddOption(new Option("a", "aaa", false, "aaaaaaa"));
            options.AddOption(new Option(null, "bbb", false, "bbbbbbb"));
            options.AddOption(new Option("c", null, false, "ccccccc"));

            HelpFormatter formatter = new HelpFormatter();
            StringWriter outWriter = new StringWriter();
            
            formatter.PrintHelp(outWriter, 80, "foobar", "", options, 2, 2, "", true);
            
            Assert.Equal(
                    "usage: foobar [-a] [--bbb] [-c]" + EOL +
                    "  -a,--aaa  aaaaaaa" + EOL +
                    "     --bbb  bbbbbbb" + EOL +
                    "  -c        ccccccc" + EOL
                    , outWriter.ToString());
        }


        [Fact]
        public void OptionWithoutShortFormat2Test() {
            // related to Bugzilla #27635 (CLI-26)
            Option help = new Option("h", "help", false, "print this message");
            Option version = new Option("v", "version", false, "print version information");
            Option newRun = new Option("n", "new", false, "Create NLT cache entries only for new items");
            Option trackerRun = new Option("t", "tracker", false, "Create NLT cache entries only for tracker items");

            Option timeLimit = Option.Builder("l")
                                     .LongOpt("limit")
                                     .HasArg()
                                     .ValueSeparator()
                                     .Desc("Set time limit for execution, in mintues")
                                     .Build();

            Option age = Option.Builder("a").LongOpt("age")
                                            .HasArg()
                                            .ValueSeparator()
                                            .Desc("Age (in days) of cache item before being recomputed")
                                            .Build();

            Option server = Option.Builder("s").LongOpt("server")
                                               .HasArg()
                                               .ValueSeparator()
                                               .Desc("The NLT server address")
                                               .Build();

            Option numResults = Option.Builder("r").LongOpt("results")
                                                   .HasArg()
                                                   .ValueSeparator()
                                                   .Desc("Number of results per item")
                                                   .Build();

            Option configFile = Option.Builder().LongOpt("config")
                                                .HasArg()
                                                .ValueSeparator()
                                                .Desc("Use the specified configuration file")
                                                .Build();

            Options mOptions = new Options();
            mOptions.AddOption(help);
            mOptions.AddOption(version);
            mOptions.AddOption(newRun);
            mOptions.AddOption(trackerRun);
            mOptions.AddOption(timeLimit);
            mOptions.AddOption(age);
            mOptions.AddOption(server);
            mOptions.AddOption(numResults);
            mOptions.AddOption(configFile);

            HelpFormatter formatter = new HelpFormatter();
            string EOL = Environment.NewLine;
            StringWriter outWriter = new StringWriter();
            
            formatter.PrintHelp(outWriter, 80,"commandline","header",mOptions,2,2,"footer",true);
            Assert.Equal(
                    "usage: commandline [-a <arg>] [--config <arg>] [-h] [-l <arg>] [-n] [-r <arg>]" + EOL +
                    "       [-s <arg>] [-t] [-v]" + EOL +
                    "header"+EOL+
                    "  -a,--age <arg>      Age (in days) of cache item before being recomputed"+EOL+
                    "     --config <arg>   Use the specified configuration file"+EOL+
                    "  -h,--help           print this message"+EOL+
                    "  -l,--limit <arg>    Set time limit for execution, in mintues"+EOL+
                    "  -n,--new            Create NLT cache entries only for new items"+EOL+
                    "  -r,--results <arg>  Number of results per item"+EOL+
                    "  -s,--server <arg>   The NLT server address"+EOL+
                    "  -t,--tracker        Create NLT cache entries only for tracker items"+EOL+
                    "  -v,--version        print version information"+EOL+
                    "footer"+EOL
                    , outWriter.ToString());
        }


        [Fact]
        public void HelpWithLongOptSeparatorTest() {
            Options options = new Options();
            options.AddOption( "f", true, "the file" );
            options.AddOption(Option.Builder("s").LongOpt("size").Desc("the size").HasArg().ArgName("SIZE").Build());
            options.AddOption(Option.Builder().LongOpt("age").Desc("the age").HasArg().Build());

            HelpFormatter formatter = new HelpFormatter();
            Assert.Equal(HelpFormatter.DEFAULT_LONG_OPT_SEPARATOR, formatter.GetLongOptSeparator());
            
            formatter.SetLongOptSeparator("=");
            Assert.Equal("=", formatter.GetLongOptSeparator());

            StringWriter outWriter = new StringWriter();

            formatter.PrintHelp(outWriter, 80, "create", "header", options, 2, 2, "footer");

            Assert.Equal(
                    "usage: create" + EOL +
                    "header" + EOL +
                    "     --age=<arg>    the age" + EOL +
                    "  -f <arg>          the file" + EOL +
                    "  -s,--size=<SIZE>  the size" + EOL +
                    "footer" + EOL,
                    outWriter.ToString());
        }


        [Fact]
        public void UsageWithLongOptSeparatorTest() {
            Options options = new Options();
            options.AddOption( "f", true, "the file" );
            options.AddOption(Option.Builder("s").LongOpt("size").Desc("the size").HasArg().ArgName("SIZE").Build());
            options.AddOption(Option.Builder().LongOpt("age").Desc("the age").HasArg().Build());

            HelpFormatter formatter = new HelpFormatter();
            formatter.SetLongOptSeparator("=");

            StringWriter outWriter = new StringWriter();

            formatter.PrintUsage(outWriter, 80, "create", options);

            Assert.Equal("usage: create [--age=<arg>] [-f <arg>] [-s <SIZE>]", outWriter.ToString().Trim());
        }
    }
}
