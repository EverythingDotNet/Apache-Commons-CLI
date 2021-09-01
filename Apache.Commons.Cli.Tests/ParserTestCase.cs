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
    using Xunit;

    /**
     * Abstract test case testing common parser features.
     */
    public abstract class ParserTestCase {

        protected ICommandLineParser parser;

        protected Options options;


        public ParserTestCase() {
            options = new Options()
                .AddOption("a", "enable-a", false, "turn [a] on or off")
                .AddOption("b", "bfile", true, "set the value of [b]")
                .AddOption("c", "copt", false, "turn [c] on or off");
        }


        [Fact]
        public virtual void SimpleShortTest() {
            string[] args = new string[] {
                "-a",
                "-b", "toast",
                "foo", "bar"
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 2, "Confirm size of extra args");
        }


        [Fact]
        public virtual void SimpleLongTest() {
            string[] args = new string[] {
                "--enable-a",
                "--bfile", "toast", "foo", "bar"
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetOptionValue("bfile").Equals("toast"), "Confirm arg of --bfile");
            Assert.True(cl.GetArgList().Count == 2, "Confirm size of extra args");
        }


        [Fact]
        public virtual void MultipleTest() {
            string[] args = new string[] { 
                "-c", "foobar", 
                "-b", "toast" 
            };

            CommandLine cl = parser.Parse(options, args, true);
    
            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(cl.GetArgList().Count == 3, "Confirm  3 extra args: " + cl.GetArgList().Count);

            cl = parser.Parse(options, cl.GetArgs());

            Assert.True(!cl.HasOption("c"), "Confirm -c is not set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 1, "Confirm  1 extra arg: " + cl.GetArgList().Count);
            Assert.True(cl.GetArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.GetArgList()[0]);
        }


        [Fact]
        public virtual void MultipleWithLongTest() {
            string[] args = new string[] { 
                "--copt", "foobar",
                "--bfile", "toast" 
            };

            CommandLine cl = parser.Parse(options, args, true);

            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(cl.GetArgList().Count == 3, "Confirm  3 extra args: " + cl.GetArgList().Count);

            cl = parser.Parse(options, cl.GetArgs());

            Assert.True(!cl.HasOption("c"), "Confirm -c is not set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 1, "Confirm  1 extra arg: " + cl.GetArgList().Count);
            Assert.True(cl.GetArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.GetArgList()[0]);
        }


        [Fact]
        public virtual void UnrecognizedOptionTest() {
            string[] args = new string[] { 
                "-a", 
                "-d", 
                "-b", "toast", "foo", "bar" 
            };

            UnrecognizedOptionException e = Assert.Throws<UnrecognizedOptionException>(() => parser.Parse(options, args));
    
            Assert.Equal("-d", e.GetOption());
        }


        [Fact]
        public virtual void MissingArgTest() {
            string[] args = new string[] { "-b" };

            MissingArgumentException e = Assert.Throws<MissingArgumentException>(() => parser.Parse(options, args));
    
            Assert.Equal("b", e.GetOption().GetOpt());
        }


        [Fact]
        public virtual void DoubleDash1Test() {
            string[] args = new string[] { 
                "--copt",
                "--",
                "-b", "toast" };

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(!cl.HasOption("b"), "Confirm -b is not set");
            Assert.True(cl.GetArgList().Count == 2, "Confirm 2 extra args: " + cl.GetArgList().Count);
        }

        
        [Fact]
        public virtual void DoubleDash2Test() {
            Options options = new Options();
            options.AddOption(OptionBuilder.HasArg().Create('n'));
            options.AddOption(OptionBuilder.Create('m'));

            MissingArgumentException e = Assert.Throws<MissingArgumentException>(() => parser.Parse(options, new string[] { "-n", "--", "-m" }));

            Assert.NotNull(e.GetOption());
            Assert.Equal("n", e.GetOption().GetOpt());
        }


        [Fact]
        public virtual void SingleDashTest() {
            string[] args = new string[] { 
                "--copt",
                "-b", 
                "-",
                "-a",
                "-" 
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("-"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 1, "Confirm 1 extra arg: " + cl.GetArgList().Count);
            Assert.True(cl.GetArgList()[0].Equals("-"), "Confirm value of extra arg: " + cl.GetArgList()[0]);
        }

        
        [Fact]
        public virtual void StopAtUnexpectedArgTest() {
            string[] args = new string[] { 
                "-c", "foober",
                "-b", "toast" 
            };

            CommandLine cl = parser.Parse(options, args, true);

            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(cl.GetArgList().Count == 3, "Confirm  3 extra args: " + cl.GetArgList().Count);
        }


        [Fact]
        public virtual void StopAtExpectedArgTest() {
            string[] args = new string[] { 
                "-b", "foo" 
            };

            CommandLine cl = parser.Parse(options, args, true);

            Assert.True(cl.HasOption('b'), "Confirm -b is set");
            Assert.Equal("foo", cl.GetOptionValue('b'));
            Assert.True(cl.GetArgList().Count == 0, "Confirm no extra args: " + cl.GetArgList().Count);
        }


        [Fact]
        public virtual void StopAtNonOptionShortTest() {
            string[] args = new string[] {
                "-z",
                "-a",
                "-btoast"
            };

            CommandLine cl = parser.Parse(options, args, true);

            Assert.False(cl.HasOption("a"), "Confirm -a is not set");
            Assert.True(cl.GetArgList().Count == 3, "Confirm  3 extra args: " + cl.GetArgList().Count);
        }

        
        [Fact]
        public virtual void StopAtNonOptionLongTest() {
            string[] args = new string[] {
                "--zop==1",
                "-abtoast",
                "--b=bar"
            };

            CommandLine cl = parser.Parse(options, args, true);

            Assert.False(cl.HasOption("a"), "Confirm -a is not set");
            Assert.False(cl.HasOption("b"), "Confirm -b is not set");
            Assert.True(cl.GetArgList().Count == 3, "Confirm  3 extra args: " + cl.GetArgList().Count);
        }

        
        [Fact]
        public virtual void NegativeArgumentTest() {
            string[] args = new string[] { 
                "-b", "-1" 
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("-1", cl.GetOptionValue("b"));
        }

        
        [Fact]
        public virtual void NegativeOptionTest() {
            string[] args = new string[] { 
                "-b", "-1" 
            };

            options.AddOption("1", false, null);

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("-1", cl.GetOptionValue("b"));
        }


        [Fact]
        public virtual void ArgumentStartingWithHyphenTest()  {
            string[] args = new string[] { 
                "-b", 
                "-foo" 
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("-foo", cl.GetOptionValue("b"));
        }

        
        [Fact]
        public virtual void ShortWithEqualTest() {
            string[] args = new string[] { "-f=bar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }


        [Fact]
        public virtual void ShortWithoutEqualTest() {
            string[] args = new string[] { "-fbar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }


        [Fact]
        public virtual void LongWithEqualDoubleDashTest() {
            string[] args = new string[] { "--foo=bar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }


        [Fact]
        public virtual void LongWithEqualSingleDashTest() {
            string[] args = new string[] { "-foo=bar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }


        [Fact]
        public virtual void LongWithoutEqualSingleDashTest() {
            string[] args = new string[] { "-foobar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args);

            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }


        [Fact]
        public virtual void AmbiguousLongWithoutEqualSingleDashTest() {
            string[] args = new string[] { 
                "-b", 
                "-foobar" 
            };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasOptionalArg().Create('f'));
            options.AddOption(OptionBuilder.WithLongOpt("bar").HasOptionalArg().Create('b'));

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("b"));
            Assert.True(cl.HasOption("f"));
            Assert.Equal("bar", cl.GetOptionValue("foo"));
        }

        
        [Fact]
        public virtual void LongWithoutEqualDoubleDashTest() {
            string[] args = new string[] { "--foobar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").HasArg().Create('f'));

            CommandLine cl = parser.Parse(options, args, true);

            Assert.False(cl.HasOption("foo")); // foo isn't expected to be recognized with a double dash
        }


        [Fact]
        public virtual void LongWithUnexpectedArgument1Test() {
            string[] args = new string[] { "--foo=bar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").Create('f'));

            UnrecognizedOptionException e = Assert.Throws<UnrecognizedOptionException>(() => parser.Parse(options, args));

            Assert.Equal("--foo=bar", e.GetOption());
        }


        [Fact]
        public virtual void LongWithUnexpectedArgument2Test() {
            string[] args = new string[] { "-foobar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").Create('f'));

            UnrecognizedOptionException e = Assert.Throws<UnrecognizedOptionException>(() =>parser.Parse(options, args));
        
            Assert.Equal("-foobar", e.GetOption());
        }


        [Fact]
        public virtual void ShortWithUnexpectedArgumentTest() {
            string[] args = new string[] { "-f=bar" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("foo").Create('f'));

            UnrecognizedOptionException e = Assert.Throws<UnrecognizedOptionException>(() => parser.Parse(options, args));
    
            Assert.Equal("-f=bar", e.GetOption());
        }


        [Fact]
        public virtual void PropertiesOption1Test() {
            string[] args = new string[] { 
                "-Jsource=1.5", 
                "-J", "target", "1.5", 
                "foo" 
            };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithValueSeparator().HasArgs(2).Create('J'));

            CommandLine cl = parser.Parse(options, args);

            List<string> values = cl.GetOptionValues("J").ToList();

            Assert.NotNull(values);
            Assert.Equal(4, values.Count);
            Assert.Equal("source", values[0]);
            Assert.Equal("1.5", values[1]);
            Assert.Equal("target", values[2]);
            Assert.Equal("1.5", values[3]);

            List<string> argsleft = cl.GetArgList();
            Assert.Equal(1, argsleft.Count);
            Assert.Equal("foo", argsleft[0]);
        }

        
        [Fact]
        public virtual void PropertiesOption2Test() {
            string[] args = new string[] { 
                "-Dparam1", 
                "-Dparam2=value2", 
                "-D" 
            };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithValueSeparator().HasOptionalArgs(2).Create('D'));

            CommandLine cl = parser.Parse(options, args);

            Dictionary<string, string> props = cl.GetOptionProperties("D");

            Assert.NotNull(props);
            Assert.Equal(2, props.Count);
            Assert.Equal("true", props["param1"]);
            Assert.Equal("value2", props["param2"]);

            List<string> argsleft = cl.GetArgList();

            Assert.Equal(0, argsleft.Count);
        }


        [Fact]
        public virtual void UnambiguousPartialLongOption1Test() {
            string[] args = new string[] { "--ver" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("help").Create());

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("version"), "Confirm --version is set");
        }


        [Fact]
        public virtual void UnambiguousPartialLongOption2Test() {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("help").Create());

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("version"), "Confirm --version is set");
        }


        [Fact]
        public virtual void UnambiguousPartialLongOption3Test() {
            string[] args = new string[] { "--ver=1" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("verbose").HasOptionalArg().Create());
            options.AddOption(OptionBuilder.WithLongOpt("help").Create());

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("verbose"), "Confirm --verbose is set");
            Assert.Equal("1", cl.GetOptionValue("verbose"));
        }


        [Fact]
        public virtual void UnambiguousPartialLongOption4Test() {
            string[] args = new string[] { "-ver=1" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("verbose").HasOptionalArg().Create());
            options.AddOption(OptionBuilder.WithLongOpt("help").Create());

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("verbose"), "Confirm --verbose is set");
            Assert.Equal("1", cl.GetOptionValue("verbose"));
        }


        [Fact]
        public virtual void AmbiguousPartialLongOption1Test() {
            string[] args = new string[] { "--ver" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("verbose").Create());

            AmbiguousOptionException e = Assert.Throws<AmbiguousOptionException>(() => parser.Parse(options, args));

            Assert.Equal("--ver", e.GetOption());
            Assert.NotNull(e.GetMatchingOptions());
            Assert.Equal(2, e.GetMatchingOptions().Count);
        }


        [Fact]
        public virtual void AmbiguousPartialLongOption2Test() {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("verbose").Create());

            AmbiguousOptionException e = Assert.Throws<AmbiguousOptionException>(() => parser.Parse(options, args));

            Assert.Equal("-ver", e.GetOption());
            Assert.NotNull(e.GetMatchingOptions());
            Assert.Equal(2, e.GetMatchingOptions().Count);
        }


        [Fact]
        public virtual void AmbiguousPartialLongOption3Test()  {
            string[] args = new string[] { "--ver=1" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("verbose").HasOptionalArg().Create());

            AmbiguousOptionException e = Assert.Throws<AmbiguousOptionException>(() => parser.Parse(options, args));

            Assert.Equal("--ver", e.GetOption());
            Assert.NotNull(e.GetMatchingOptions());
            Assert.Equal(2, e.GetMatchingOptions().Count);
        }

        [Fact]
        public virtual void AmbiguousPartialLongOption4Test() {
            string[] args = new string[] { "-ver=1" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("verbose").HasOptionalArg().Create());

            AmbiguousOptionException e = Assert.Throws<AmbiguousOptionException>(() => parser.Parse(options, args));

            Assert.Equal("-ver", e.GetOption());
            Assert.NotNull(e.GetMatchingOptions());
            Assert.Equal(2, e.GetMatchingOptions().Count);
        }


        [Fact]
        public virtual void PartialLongOptionSingleDashTest()  {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.HasArg().Create('v'));

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("version"), "Confirm --version is set");
            Assert.True(!cl.HasOption("v"), "Confirm -v is not set");
        }


        [Fact]
        public virtual void WithRequiredOptionTest()  {
            string[] args = new string[] { 
                "-b", "file" 
            };

            Options options = new Options();
            options.AddOption("a", "enable-a", false, null);
            options.AddOption(OptionBuilder.WithLongOpt("bfile").HasArg().IsRequired().Create('b'));

            CommandLine cl = parser.Parse(options, args);

            Assert.True(!cl.HasOption("a"), "Confirm -a is NOT set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("file"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 0, "Confirm NO of extra args");
        }


        [Fact]
        public virtual void OptionAndRequiredOptionTest()  {
            string[] args = new string[] { 
                "-a", 
                "-b", "file" 
            };

            Options options = new Options();
            options.AddOption("a", "enable-a", false, null);
            options.AddOption(OptionBuilder.WithLongOpt("bfile").HasArg().IsRequired().Create('b'));

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("file"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 0, "Confirm NO of extra args");
        }


        [Fact]
        public virtual void MissingRequiredOptionTest() {
            string[] args = new string[] { "-a" };

            Options options = new Options();
            options.AddOption("a", "enable-a", false, null);
            options.AddOption(OptionBuilder.WithLongOpt("bfile").HasArg().IsRequired().Create('b'));

            MissingOptionException e = Assert.Throws<MissingOptionException>(() => parser.Parse(options, args));

            Assert.Equal("Missing required option: b", e.Message);
            Assert.True(e.GetMissingOptions().Contains("b"));
        }


        [Fact]
        public virtual void MissingRequiredOptionsTest() {
            string[] args = new string[] { "-a" };

            Options options = new Options();
            options.AddOption("a", "enable-a", false, null);
            options.AddOption(OptionBuilder.WithLongOpt("bfile").HasArg().IsRequired().Create('b'));
            options.AddOption(OptionBuilder.WithLongOpt("cfile").HasArg().IsRequired().Create('c'));


            MissingOptionException e = Assert.Throws<MissingOptionException>(() => parser.Parse(options, args));

            Assert.Equal("Missing required options: b, c", e.Message);
            Assert.True(e.GetMissingOptions().Contains("b"));
            Assert.True(e.GetMissingOptions().Contains("c"));
        }


        [Fact]
        public virtual void MissingRequiredGroupTest() {
            OptionGroup group = new OptionGroup();
            group.AddOption(OptionBuilder.Create("a"));
            group.AddOption(OptionBuilder.Create("b"));
            group.SetRequired(true);

            Options options = new Options();
            options.AddOptionGroup(group);
            options.AddOption(OptionBuilder.IsRequired().Create("c"));



            MissingOptionException e = Assert.Throws<MissingOptionException>(() => parser.Parse(options, new string[] { "-c" }));

            Assert.Equal(1, e.GetMissingOptions().Count);
            Assert.IsType<OptionGroup>(e.GetMissingOptions()[0]);
        }


        [Fact]
        public virtual void OptionGroupTest()  {
            OptionGroup group = new OptionGroup();
            group.AddOption(OptionBuilder.Create("a"));
            group.AddOption(OptionBuilder.Create("b"));

            Options options = new Options();
            options.AddOptionGroup(group);

            parser.Parse(options, new string[] { "-b" });

            Assert.Equal("b", group.GetSelected());
        }


        [Fact]
        public virtual void OptionGroupLongTest()  {
            OptionGroup group = new OptionGroup();
            group.AddOption(OptionBuilder.WithLongOpt("foo").Create());
            group.AddOption(OptionBuilder.WithLongOpt("bar").Create());

            Options options = new Options();
            options.AddOptionGroup(group);

            CommandLine cl = parser.Parse(options, new string[] { "--bar" });

            Assert.True(cl.HasOption("bar"));
            Assert.Equal("bar", group.GetSelected());
        }


        [Fact]
        public virtual void ReuseOptionsTwiceTest() {
            Options opts = new Options();
            opts.AddOption(OptionBuilder.IsRequired().Create('v'));

            // first parsing
            parser.Parse(opts, new string[] { "-v" });

            // second parsing, with the same Options instance and an invalid command line
            MissingOptionException e = Assert.Throws<MissingOptionException>(() => parser.Parse(opts, new string[0]));
        }

        [Fact]
        public virtual void BurstingTest()  {
            string[] args = new string[] { 
                "-acbtoast", "foo", "bar" 
            };

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 2, "Confirm size of extra args");
        }


        [Fact]
        public virtual void UnrecognizedOptionWithBurstingTest()  {
            string[] args = new string[] { 
                "-adbtoast", "foo", "bar" 
            };

            UnrecognizedOptionException e = Assert.Throws<UnrecognizedOptionException>(() => parser.Parse(options, args));

            Assert.Equal("-adbtoast", e.GetOption());
        }


        [Fact]
        public virtual void MissingArgWithBurstingTest() {
            string[] args = new string[] { "-acb" };

            MissingArgumentException e = Assert.Throws<MissingArgumentException>(() => parser.Parse(options, args));

            Assert.Equal("b", e.GetOption().GetOpt());
        }


        [Fact]
        public virtual void StopBurstingTest() {
            string[] args = new string[] { "-azc" };

            CommandLine cl = parser.Parse(options, args, true);
            Assert.True(cl.HasOption("a"), "Confirm -a is set");
            Assert.False(cl.HasOption("c"), "Confirm -c is not set");

            Assert.True(cl.GetArgList().Count == 1, "Confirm  1 extra arg: " + cl.GetArgList().Count);
            Assert.Contains("zc", cl.GetArgList());
        }


        [Fact]
        public virtual void StopBursting2Test() {
            string[] args = new string[] { 
                "-c", "foobar", 
                "-btoast" 
            };

            CommandLine cl = parser.Parse(options, args, true);
            Assert.True(cl.HasOption("c"), "Confirm -c is set");
            Assert.True(cl.GetArgList().Count == 2, "Confirm  2 extra args: " + cl.GetArgList().Count);

            cl = parser.Parse(options, cl.GetArgs());

            Assert.True(!cl.HasOption("c"), "Confirm -c is not set");
            Assert.True(cl.HasOption("b"), "Confirm -b is set");
            Assert.True(cl.GetOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.True(cl.GetArgList().Count == 1, "Confirm  1 extra arg: " + cl.GetArgList().Count);
            Assert.True(cl.GetArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.GetArgList()[0]);
        }


        [Fact]
        public virtual void UnlimitedArgsTest()  {
            string[] args = new string[] { 
                "-e", "one", "two", 
                "-f", "alpha" 
            };

            Options options = new Options();
            options.AddOption(OptionBuilder.HasArgs().Create("e"));
            options.AddOption(OptionBuilder.HasArgs().Create("f"));

            CommandLine cl = parser.Parse(options, args);

            Assert.True(cl.HasOption("e"), "Confirm -e is set");
            Assert.Equal(2, cl.GetOptionValues("e").Length);
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.Equal(1, cl.GetOptionValues("f").Length);
        }


        private CommandLine Parse(ICommandLineParser parser, Options opts, string[] args, Dictionary<string, string> properties) {
            if (parser is Parser) {
                return ((Parser)parser).Parse(opts, args, properties);
            }
            
            if (parser is DefaultParser) {
                return ((DefaultParser)parser).Parse(opts, args, properties);
            }
            
            throw new NotSupportedException("Default options not supported by this parser");
        }


        [Fact]
        public virtual void PropertyOptionSingularValueTest() {
            Options opts = new Options();
            opts.AddOption(OptionBuilder.HasOptionalArgs(2).WithLongOpt("hide").Create());

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("hide", "seek");

            CommandLine cmd = Parse(parser, opts, null, properties);
        
            Assert.True(cmd.HasOption("hide"));
            Assert.Equal("seek", cmd.GetOptionValue("hide"));
            Assert.True(!cmd.HasOption("fake"));
        }


        [Fact]
        public virtual void PropertyOptionFlagsTest() {
            Options opts = new Options();
            opts.AddOption("a", false, "toggle -a");
            opts.AddOption("c", "c", false, "toggle -c");
            opts.AddOption(OptionBuilder.HasOptionalArg().Create('e'));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("a", "true");
            properties.Add("c", "yes");
            properties.Add("e", "1");

            CommandLine cmd = Parse(parser, opts, null, properties);

            Assert.True(cmd.HasOption("a"));
            Assert.True(cmd.HasOption("c"));
            Assert.True(cmd.HasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "false");
            properties.Add("c", "no");
            properties.Add("e", "0");

            cmd = Parse(parser, opts, null, properties);

            Assert.True(!cmd.HasOption("a"));
            Assert.True(!cmd.HasOption("c"));
            Assert.True(cmd.HasOption("e")); // this option accepts an argument


            properties = new Dictionary<string, string>();
            properties.Add("a", "TRUE");
            properties.Add("c", "nO");
            properties.Add("e", "TrUe");

            cmd = Parse(parser, opts, null, properties);

            Assert.True(cmd.HasOption("a"));
            Assert.True(!cmd.HasOption("c"));
            Assert.True(cmd.HasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "just a string");
            properties.Add("e", "");

            cmd = Parse(parser, opts, null, properties);

            Assert.True(!cmd.HasOption("a"));
            Assert.True(!cmd.HasOption("c"));
            Assert.True(cmd.HasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "0");
            properties.Add("c", "1");

            cmd = Parse(parser, opts, null, properties);

            Assert.True(!cmd.HasOption("a"));
            Assert.True(cmd.HasOption("c"));
        }


        [Fact]
        public virtual void PropertyOptionMultipleValuesTest() {
            Options opts = new Options();
            opts.AddOption(OptionBuilder.HasArgs().WithValueSeparator(',').Create('k'));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("k", "one,two");

            string[] values = new string[] { "one", "two" };

            CommandLine cmd = Parse(parser, opts, null, properties);

            Assert.True(cmd.HasOption("k"));
            Assert.Equal(values, cmd.GetOptionValues('k'));
        }


        [Fact]
        public virtual void PropertyOverrideValuesTest()  {
            string[] args = new string[] {
                "-j", "found",
                "-i", "ink"
            };

            Options opts = new Options();
            opts.AddOption(OptionBuilder.HasOptionalArgs(2).Create('i'));
            opts.AddOption(OptionBuilder.HasOptionalArgs().Create('j'));


            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("j", "seek");

            CommandLine cmd = Parse(parser, opts, args, properties);

            Assert.True(cmd.HasOption("j"));
            Assert.Equal("found", cmd.GetOptionValue("j"));
            Assert.True(cmd.HasOption("i"));
            Assert.Equal("ink", cmd.GetOptionValue("i"));
            Assert.True(!cmd.HasOption("fake"));
        }


        [Fact]
        public virtual void PropertyOptionRequiredTest() {
            Options opts = new Options();
            opts.AddOption(OptionBuilder.IsRequired().Create("f"));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("f", "true");

            CommandLine cmd = Parse(parser, opts, null, properties);

            Assert.True(cmd.HasOption("f"));
        }


        [Fact]
        public virtual void PropertyOptionUnexpectedTest()  {
            Options opts = new Options();

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("f", "true");

            Assert.Throws<UnrecognizedOptionException>(() => Parse(parser, opts, null, properties));
        }


        [Fact]
        public virtual void PropertyOptionGroupTest() {
            Options opts = new Options();

            OptionGroup group1 = new OptionGroup();
            group1.AddOption(new Option("a", null));
            group1.AddOption(new Option("b", null));
            opts.AddOptionGroup(group1);

            OptionGroup group2 = new OptionGroup();
            group2.AddOption(new Option("x", null));
            group2.AddOption(new Option("y", null));
            opts.AddOptionGroup(group2);

            string[] args = new string[] { "-a" };

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("b", "true");
            properties.Add("x", "true");

            CommandLine cmd = Parse(parser, opts, args, properties);

            Assert.True(cmd.HasOption("a"));
            Assert.False(cmd.HasOption("b"));
            Assert.True(cmd.HasOption("x"));
            Assert.False(cmd.HasOption("y"));
        }
    }
}
