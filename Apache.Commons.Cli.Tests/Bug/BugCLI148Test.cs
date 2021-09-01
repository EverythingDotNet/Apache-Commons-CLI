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

namespace Apache.Commons.Cli.Bug {

    using Xunit;

    /**
     * https://issues.apache.org/jira/browse/CLI-148
     */
    public class BugCLI148Test {

        private Options options;

        
        public BugCLI148Test() {
            options = new Options();
            options.AddOption(OptionBuilder.HasArg().Create('t'));
            options.AddOption(OptionBuilder.HasArg().Create('s'));
        }


        [Fact]
        public void Workaround1Test() {
            string[] args = new string[] { "-t-something" }; 
            
            ICommandLineParser parser = new PosixParser();
            CommandLine commandLine = parser.Parse(options, args);

            Assert.Equal("-something", commandLine.GetOptionValue('t'));
        }

        
        [Fact]
        public void Workaround2Test() {
            string[] args = new string[] { "-t", "\"-something\"" }; 
            
            ICommandLineParser parser = new PosixParser();
            CommandLine commandLine = parser.Parse(options, args);
            
            Assert.Equal("-something", commandLine.GetOptionValue('t'));
        }
    }
}
