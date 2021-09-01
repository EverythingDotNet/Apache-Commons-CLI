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

    public class DisablePartialMatchingTest {
        
        [Fact]
        public void DisabledPartialMatchingTest() {
            ICommandLineParser parser = new DefaultParser(false);

            Options options = new Options();

            options.AddOption(new Option("d", "debug", false, "Turn on debug."));
            options.AddOption(new Option("e", "extract", false, "Turn on extract."));
            options.AddOption(new Option("o", "option", true, "Turn on option with argument."));

            CommandLine line = parser.Parse(options, new string[] { "-de", "--option=foobar" } );

            Assert.True(line.HasOption("debug"), "There should be an option debug in any case...");
            Assert.True(line.HasOption("extract"), "There should be an extract option because partial matching is off");
            Assert.True(line.HasOption("option"), "There should be an option option with a argument value");
        }

        [Fact]
        public void RegularPartialMatchingTest() {
            ICommandLineParser parser = new DefaultParser();

            Options options = new Options();

            options.AddOption(new Option("d", "debug", false, "Turn on debug."));
            options.AddOption(new Option("e", "extract", false, "Turn on extract."));
            options.AddOption(new Option("o", "option", true, "Turn on option with argument."));

            CommandLine line = parser.Parse(options, new string[] { "-de", "--option=foobar" });

            Assert.True(line.HasOption("debug"), "There should be an option debug in any case...");
            Assert.False(line.HasOption("extract"), "There should not be an extract option because partial matching only selects debug");
            Assert.True(line.HasOption("option"), "There should be an option option with a argument value");
        }
    }
}
