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

    using Xunit;

    public class ValueTest {

        private CommandLine _cl = null;
        private readonly Options opts = new Options();


        public ValueTest() {
            opts.AddOption("a", false, "toggle -a");
            opts.AddOption("b", true, "set -b");
            opts.AddOption("c", "c", false, "toggle -c");
            opts.AddOption("d", "d", true, "set -d");

            opts.AddOption(OptionBuilder.HasOptionalArg().Create('e'));
            opts.AddOption(OptionBuilder.HasOptionalArg().WithLongOpt("fish").Create());
            opts.AddOption(OptionBuilder.HasOptionalArgs().WithLongOpt("gravy").Create());
            opts.AddOption(OptionBuilder.HasOptionalArgs(2).WithLongOpt("hide").Create());
            opts.AddOption(OptionBuilder.HasOptionalArgs(2).Create('i'));
            opts.AddOption(OptionBuilder.HasOptionalArgs().Create('j'));

            string[] args = new string[] { "-a",
                "-b", "foo",
                "--c",
                "--d", "bar"
            };

            Parser parser = new PosixParser();
            _cl = parser.Parse(opts, args);
        }


        [Fact]
        public void ShortNoArgumentTest() {
            Assert.True(_cl.HasOption("a"));
            Assert.Null(_cl.GetOptionValue("a"));
        }


        [Fact]
        public void ShortNoArgumentWithOptionTest() {
            Assert.True(_cl.HasOption(opts.GetOption("a")));
            Assert.Null(_cl.GetOptionValue(opts.GetOption("a")));
        }


        [Fact]
        public void ShortWithArgument() {
            Assert.True(_cl.HasOption("b"));
            Assert.NotNull(_cl.GetOptionValue("b"));
            Assert.Equal("foo", _cl.GetOptionValue("b"));
        }


        [Fact]
        public void ShortWithArgumentWithOptionTest() {
            Assert.True(_cl.HasOption(opts.GetOption("b")));
            Assert.NotNull(_cl.GetOptionValue(opts.GetOption("b")));
            Assert.Equal("foo", _cl.GetOptionValue(opts.GetOption("b")));
        }


        [Fact]
        public void LongNoArgumentTest() {
            Assert.True(_cl.HasOption("c"));
            Assert.Null(_cl.GetOptionValue("c"));
        }


        [Fact]
        public void LongNoArgumentWithOptionTest() {
            Assert.True(_cl.HasOption(opts.GetOption("c")));
            Assert.Null(_cl.GetOptionValue(opts.GetOption("c")));
        }


        [Fact]
        public void LongWithArgumentTest() {
            Assert.True(_cl.HasOption("d"));
            Assert.NotNull(_cl.GetOptionValue("d"));
            Assert.Equal("bar", _cl.GetOptionValue("d"));
        }


        [Fact]
        public void LongWithArgumentWithOptionTest() {
            Assert.True(_cl.HasOption(opts.GetOption("d")));
            Assert.NotNull(_cl.GetOptionValue(opts.GetOption("d")));
            Assert.Equal("bar", _cl.GetOptionValue(opts.GetOption("d")));
        }

        
        [Fact]
        public void ShortOptionalArgumentNoValueTest() {
            string[] args = new string[] { "-e" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);
            
            Assert.True(cmd.HasOption("e") );
            Assert.Null(cmd.GetOptionValue("e") );
        }


        [Fact]
        public void ShortOptionalArgumentNoValueWithOptionTest() {
            string[] args = new string[] { "-e" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("e")));
            Assert.Null(cmd.GetOptionValue(opts.GetOption("e")));
        }

        
        [Fact]
        public void ShortOptionalArgumentValueTest() {
            string[] args = new string[] { "-e", "everything" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("e"));
            Assert.Equal("everything", cmd.GetOptionValue("e"));
        }

        
        [Fact]
        public void ShortOptionalArgumentValueWithOptionTest() {
            string[] args = new string[] { "-e", "everything" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("e")));
            Assert.Equal("everything", cmd.GetOptionValue(opts.GetOption("e")));
        }

        
        [Fact]
        public void LongOptionalNoValueTest() {
            string[] args = new string[] { "--fish" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("fish"));
            Assert.Null(cmd.GetOptionValue("fish"));
        }


        [Fact]
        public void LongOptionalNoValueWithOptionTest() {
            string[] args = new string[] { "--fish" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("fish")));
            Assert.Null(cmd.GetOptionValue(opts.GetOption("fish")));
        }

        
        [Fact]
        public void LongOptionalArgumentValueTest() {
            string[] args = new string[] { "--fish", "face" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("fish"));
            Assert.Equal("face", cmd.GetOptionValue("fish"));
        }

        
        [Fact]
        public void LongOptionalArgumentValueWithOptionTest() {
            string[] args = new string[] { "--fish", "face" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("fish")));
            Assert.Equal("face", cmd.GetOptionValue(opts.GetOption("fish")));
        }

        
        [Fact]
        public void ShortOptionalArgumentValuesTest() {
            string[] args = new string[] { "-j", "ink", "idea" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("j"));
            Assert.Equal("ink", cmd.GetOptionValue("j"));
            Assert.Equal("ink", cmd.GetOptionValues("j")[0]);
            Assert.Equal("idea", cmd.GetOptionValues("j")[1]);
            Assert.Empty(cmd.GetArgs());
        }


        [Fact]
        public void ShortOptionalArgumentValuesWithOptionTest() {
            string[] args = new string[] { "-j", "ink", "idea" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("j")));
            Assert.Equal("ink", cmd.GetOptionValue(opts.GetOption("j")));
            Assert.Equal("ink", cmd.GetOptionValues(opts.GetOption("j"))[0]);
            Assert.Equal("idea", cmd.GetOptionValues(opts.GetOption("j"))[1]);
            Assert.Empty(cmd.GetArgs());
        }

        
        [Fact]
        public void LongOptionalArgumentValuesTest() {
            string[] args = new string[] { "--gravy", "gold", "garden" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("gravy"));
            Assert.Equal("gold", cmd.GetOptionValue("gravy"));
            Assert.Equal("gold", cmd.GetOptionValues("gravy")[0]);
            Assert.Equal("garden", cmd.GetOptionValues("gravy")[1]);
            Assert.Empty(cmd.GetArgs());
        }

        
        [Fact]
        public void LongOptionalArgumentValuesWithOptionTest() {
            string[] args = new string[] { "--gravy", "gold", "garden" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("gravy")));
            Assert.Equal("gold", cmd.GetOptionValue(opts.GetOption("gravy")));
            Assert.Equal("gold", cmd.GetOptionValues(opts.GetOption("gravy"))[0]);
            Assert.Equal("garden", cmd.GetOptionValues(opts.GetOption("gravy"))[1]);
            Assert.Empty(cmd.GetArgs());
        }

        
        [Fact]
        public void ShortOptionalNArgumentValues() {
            string[] args = new string[] { "-i", "ink", "idea", "isotope", "ice" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("i"));
            Assert.Equal("ink", cmd.GetOptionValue("i"));
            Assert.Equal("ink", cmd.GetOptionValues("i")[0]);
            Assert.Equal("idea", cmd.GetOptionValues("i")[1]);
            Assert.Equal(2, cmd.GetArgs().Length);
            Assert.Equal("isotope", cmd.GetArgs()[0]);
            Assert.Equal("ice", cmd.GetArgs()[1]);
        }

        
        [Fact]
        public void ShortOptionalNArgumentValuesWithOptionTest() {
            string[] args = new string[] { "-i", "ink", "idea", "isotope", "ice" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("i"));
            Assert.Equal("ink", cmd.GetOptionValue(opts.GetOption("i")));
            Assert.Equal("ink", cmd.GetOptionValues(opts.GetOption("i"))[0]);
            Assert.Equal("idea", cmd.GetOptionValues(opts.GetOption("i"))[1]);
            Assert.Equal(2, cmd.GetArgs().Length);
            Assert.Equal("isotope", cmd.GetArgs()[0]);
            Assert.Equal("ice", cmd.GetArgs()[1]);
        }

        
        [Fact]
        public void LongOptionalNArgumentValuesTest() {
            string[] args = new string[] {
                "--hide", "house", "hair", "head"
            };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption("hide"));
            Assert.Equal("house", cmd.GetOptionValue("hide"));
            Assert.Equal("house", cmd.GetOptionValues("hide")[0]);
            Assert.Equal("hair", cmd.GetOptionValues("hide")[1]);
            Assert.Single(cmd.GetArgs());
            Assert.Equal("head", cmd.GetArgs()[0]);
        }

        
        [Fact]
        public void LongOptionalNArgumentValuesWithOptionTest() {
            string[] args = new string[] {
                "--hide", "house", "hair", "head"
            };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.Parse(opts, args);

            Assert.True(cmd.HasOption(opts.GetOption("hide")));
            Assert.Equal("house", cmd.GetOptionValue(opts.GetOption("hide")));
            Assert.Equal("house", cmd.GetOptionValues(opts.GetOption("hide"))[0]);
            Assert.Equal("hair", cmd.GetOptionValues(opts.GetOption("hide"))[1]);
            Assert.Single(cmd.GetArgs());
            Assert.Equal("head", cmd.GetArgs()[0]);
        }
    }
}
