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

    public class ValuesTest {
        private CommandLine cmd;

        
        public ValuesTest() {
            Options options = new Options();

            options.AddOption("a", false, "toggle -a");
            options.AddOption("b", true, "set -b");
            options.AddOption("c", "c", false, "toggle -c");
            options.AddOption("d", "d", true, "set -d");

            options.AddOption(OptionBuilder.WithLongOpt("e").HasArgs().WithDescription("set -e ").Create('e'));
            options.AddOption("f", "f", false, "jk");
            options.AddOption(OptionBuilder.WithLongOpt("g").HasArgs(2).WithDescription("set -g").Create('g'));
            options.AddOption(OptionBuilder.WithLongOpt("h").HasArg().WithDescription("set -h").Create('h'));
            options.AddOption(OptionBuilder.WithLongOpt("i").WithDescription("set -i").Create('i'));
            options.AddOption(OptionBuilder.WithLongOpt("j").HasArgs().WithDescription("set -j").WithValueSeparator('=').Create('j'));
            options.AddOption(OptionBuilder.WithLongOpt("k").HasArgs().WithDescription("set -k").WithValueSeparator('=').Create('k'));
            options.AddOption(OptionBuilder.WithLongOpt("m").HasArgs().WithDescription("set -m").WithValueSeparator().Create('m'));

            string[] args = new string[] { 
                "-a",
                "-b", "foo",
                "--c",
                "--d", "bar",
                "-e", "one", "two",
                "-f", "arg1", "arg2",
                "-g", "val1", "val2" , "arg3",
                "-h", "val1", "-i",
                "-h", "val2",
                "-jkey=value",
                "-j", "key=value",
                "-kkey1=value1",
                "-kkey2=value2",
                "-mkey=value"
            };

            ICommandLineParser parser = new PosixParser();

            cmd = parser.Parse(options,args);
        }

        
        [Fact]
        public void ShortArgumentsTest() {
            Assert.True(cmd.HasOption("a"), "Option a is not set");
            Assert.True(cmd.HasOption("c"), "Option c is not set");

            Assert.Null(cmd.GetOptionValues("a"));
            Assert.Null(cmd.GetOptionValues("c"));
        }

        
        [Fact]
        public void ShortArgumentsWithValueTest() {
            Assert.True(cmd.HasOption("b"), "Option b is not set");
            Assert.True(cmd.GetOptionValue("b").Equals("foo"));
            Assert.Single(cmd.GetOptionValues("b"));

            Assert.True(cmd.HasOption("d"), "Option d is not set");
            Assert.True(cmd.GetOptionValue("d").Equals("bar"));
            Assert.Single(cmd.GetOptionValues("d"));
        }


        [Fact]
        public void MultipleArgumentValuesTest() {
            Assert.True(cmd.HasOption("e"), "Option e is not set");
            Assert.Equal(new string[] { "one", "two" }, cmd.GetOptionValues("e"));
        }


        [Fact]
        public void TwoArgumentValuesTest() {
            Assert.True(cmd.HasOption("g"), "Option g is not set");
            Assert.Equal(new string[] { "val1", "val2" }, cmd.GetOptionValues("g"));
        }

        
        [Fact]
        public void ComplexValuesTest() {
            Assert.True(cmd.HasOption("i"), "Option i is not set");
            Assert.True(cmd.HasOption("h"), "Option h is not set");
            Assert.Equal(new string[] { "val1", "val2" }, cmd.GetOptionValues("h"));
        }


        [Fact]
        public void ExtraArgumentsTest() {
            Assert.Equal(new string[] { "arg1", "arg2", "arg3" }, cmd.GetArgs());
        }

        
        [Fact]
        public void CharSeparatorTest() {
            // tests the char methods of CommandLine that delegate to the String methods
            Assert.True(cmd.HasOption("j"), "Option j is not set");
            Assert.True(cmd.HasOption('j'), "Option j is not set");
            Assert.Equal(new string[] { "key", "value", "key", "value" }, cmd.GetOptionValues("j"));
            Assert.Equal(new string[] { "key", "value", "key", "value" }, cmd.GetOptionValues('j'));

            Assert.True(cmd.HasOption("k"), "Option k is not set");
            Assert.True(cmd.HasOption('k'), "Option k is not set");
            Assert.Equal(new string[] { "key1", "value1", "key2", "value2" }, cmd.GetOptionValues("k"));
            Assert.Equal(new string[] { "key1", "value1", "key2", "value2" }, cmd.GetOptionValues('k'));

            Assert.True(cmd.HasOption("m"), "Option m is not set");
            Assert.True(cmd.HasOption('m'), "Option m is not set");
            Assert.Equal(new string[] { "key", "value" }, cmd.GetOptionValues("m"));
            Assert.Equal(new string[] { "key", "value" }, cmd.GetOptionValues('m'));
        }


        /**
         * jkeyes - commented out this test as the new architecture
         * breaks this type of functionality.  I have left the test
         * here in case I get a brainwave on how to resolve this.
         */
        /*
        public void testGetValue()
        {
            // the 'm' option
            assertTrue( _option.getValues().length == 2 );
            assertEquals( _option.getValue(), "key" );
            assertEquals( _option.getValue( 0 ), "key" );
            assertEquals( _option.getValue( 1 ), "value" );

            try {
                assertEquals( _option.getValue( 2 ), "key" );
                fail( "IndexOutOfBounds not caught" );
            }
            catch( IndexOutOfBoundsException exp ) {

            }

            try {
                assertEquals( _option.getValue( -1 ), "key" );
                fail( "IndexOutOfBounds not caught" );
            }
            catch( IndexOutOfBoundsException exp ) {

            }
        }
        */
    }
}
