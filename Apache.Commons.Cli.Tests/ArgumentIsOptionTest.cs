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

    public class ArgumentIsOptionTest {
        
        private Options options = null;
        private ICommandLineParser parser = null;

        
        public ArgumentIsOptionTest() {
            options = new Options().AddOption("p", false, "Option p").AddOption("attr", true, "Option accepts argument");

            parser = new PosixParser();
        }


        [Fact]
        public void OptionAndOptionWithArgumentTest() {
            string[] args = new string[]{
                    "-p",
                    "-attr",
                    "p"
            };

            CommandLine cl = parser.Parse(options, args);
            
            Assert.True(cl.HasOption("p"), "Confirm -p is set");
            Assert.True(cl.HasOption("attr"), "Confirm -attr is set");
            Assert.True(cl.GetOptionValue("attr").Equals("p"), "Confirm arg of -attr");
            Assert.True(cl.GetArgs().Length == 0, "Confirm all arguments recognized");
        }


        [Fact]
        public void OptionWithArgumentTest() {
            string[] args = new string[]{
                    "-attr",
                    "p"
            };

            CommandLine cl = parser.Parse(options, args);
            
            Assert.False(cl.HasOption("p"), "Confirm -p is set");
            Assert.True(cl.HasOption("attr"), "Confirm -attr is set");
            Assert.True(cl.GetOptionValue("attr").Equals("p"), "Confirm arg of -attr");
            Assert.True(cl.GetArgs().Length == 0, "Confirm all arguments recognized");
        }

        
        [Fact]
        public void OptionTest() {
            string[] args = new string[]{
                    "-p"
            };

            CommandLine cl = parser.Parse(options, args);
            
            Assert.True(cl.HasOption("p"), "Confirm -p is set");
            Assert.False(cl.HasOption("attr"), "Confirm -attr is not set");
            Assert.True(cl.GetArgs().Length == 0, "Confirm all arguments recognized");
        }
    }
}
