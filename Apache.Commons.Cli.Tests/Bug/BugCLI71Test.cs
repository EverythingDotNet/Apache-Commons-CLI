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

    public class BugCLI71Test {
        
        private Options options;
        private ICommandLineParser parser;

        
        public BugCLI71Test() {
            options = new Options();

            Option algorithm = new Option("a" , "algo", true, "the algorithm which it to perform executing");
            algorithm.SetArgName("algorithm name");
            options.AddOption(algorithm);

            Option key = new Option("k" , "key", true, "the key the setted algorithm uses to process");
            algorithm.SetArgName("value");
            options.AddOption(key);

            parser = new PosixParser();
        }

        
        [Fact]
        public void BasicTest() {
            string[] args = new string[] { "-a", "Caesar", "-k", "A" };
            CommandLine line = parser.Parse( options, args);
            
            Assert.Equal( "Caesar", line.GetOptionValue("a") );
            Assert.Equal( "A", line.GetOptionValue("k") );
        }

        
        [Fact]
        public void MistakenArgumentTest() {
            string[] args = new string[] { "-a", "Caesar", "-k", "A" };
            CommandLine line = parser.Parse( options, args);
            
            args = new string[] { "-a", "Caesar", "-k", "a" };
            line = parser.Parse(options, args);
            
            Assert.Equal( "Caesar", line.GetOptionValue("a") );
            Assert.Equal( "a", line.GetOptionValue("k") );
        }

        
        [Fact]
        public void LackOfErrorTest() {
            string[] args = new string[] { "-k", "-a",  "Caesar" };

            MissingArgumentException e = Assert.Throws<MissingArgumentException>(() => parser.Parse(options, args));
            
            Assert.Equal("k", e.MissingArgumentOption.GetOpt());
        }

        
        [Fact]
        public void GetsDefaultIfOptionalTest() {
            string[] args = new string[] { "-k", "-a", "Caesar" };
            options.GetOption("k").SetOptionalArg(true);
            
            CommandLine line = parser.Parse( options, args);

            Assert.Equal( "Caesar", line.GetOptionValue("a") );
            Assert.Equal( "a", line.GetOptionValue('k', "a") );
        }
    }
}
