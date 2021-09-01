#region Apache License
/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

namespace Apache.Commons.Cli.Bug {

    using Xunit;

    /**
     * Test for CLI-265.
     * <p>
     * The issue is that a short option with an optional value will use whatever comes next as value.
     */
    public class BugCLI265Test {

        private DefaultParser parser;
        private Options options;

        public BugCLI265Test() {
            parser = new DefaultParser();

            Option optionT1 = Option.Builder("t1").HasArg().NumberOfArgs(1).OptionalArg(true).ArgName("t1_path").Build();
            Option optionA = Option.Builder("a").HasArg(false).Build();
            Option optionB = Option.Builder("b").HasArg(false).Build();
            Option optionLast = Option.Builder("last").HasArg(false).Build();

            options = new Options().AddOption(optionT1).AddOption(optionA).AddOption(optionB).AddOption(optionLast);
        }


        [Fact]
        public void ShouldParseShortOptionWithValue() {
            string[] shortOptionWithValue = new string[]{"-t1", "path/to/my/db"};

            CommandLine commandLine = parser.Parse(options, shortOptionWithValue);

            Assert.Equal("path/to/my/db", commandLine.GetOptionValue("t1"));
            Assert.False(commandLine.HasOption("last"));
        }

        
        [Fact]
        public void ShouldParseShortOptionWithoutValue() {
            string[] twoShortOptions = new string[] { "-t1", "-last" };

            CommandLine commandLine = parser.Parse(options, twoShortOptions);

            Assert.True(commandLine.HasOption("t1"));
            Assert.NotEqual("-last", commandLine.GetOptionValue("t1"));
            Assert.True(commandLine.HasOption("last"));
        }


        [Fact]
        public void ShouldParseConcatenatedShortOptions() {
            string[] concatenatedShortOptions = new string[] { "-t1", "-ab" };

            CommandLine commandLine = parser.Parse(options, concatenatedShortOptions);

            Assert.True(commandLine.HasOption("t1"));
            Assert.Null(commandLine.GetOptionValue("t1"));
            Assert.True(commandLine.HasOption("a"));
            Assert.True(commandLine.HasOption("b"));
            Assert.False(commandLine.HasOption("last"));
        }
    }
}
