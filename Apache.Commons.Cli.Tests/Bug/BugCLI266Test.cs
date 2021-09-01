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
    
    using System.Collections.Generic;

    using Xunit;

    public class BugCLI266Test {  

        private readonly List<string> insertedOrder  =   new List<string>() { "h", "d", "f", "x", "s", "p", "t", "w", "o" };
        private readonly List<string> sortOrder      =   new List<string>() { "d", "f", "h", "o", "p", "s", "t", "w", "x" };


        [Fact]
        public void OptionComparatorDefaultOrderTest() {
            HelpFormatter formatter = new HelpFormatter();
            List<Option> options = new List<Option>(GetOptions().GetOptions());
            
            options.Sort(formatter.GetOptionComparator());
            
            int i = 0;
            
            foreach(Option o in options) {
                Assert.Equal(o.GetOpt(), sortOrder[i]);
                i++;
            }
        }

        
        [Fact]
        public void OptionComparatorInsertedOrderTest() {
            ICollection<Option> options = GetOptions().GetOptions();
            int i = 0;
            
            foreach(Option o in options) {
                Assert.Equal(o.GetOpt(), insertedOrder[i]);
                i++;
            }
        }


        private Options GetOptions() {
            Options options = new Options();
            Option help = Option.Builder("h")
                    .LongOpt("help")
                    .Desc("Prints this help message")
                    .Build();

            options.AddOption(help);

            BuildOptionsGroup(options);

            Option t = Option.Builder("t")
                    .Required()
                    .HasArg()
                    .ArgName("file")
                    .Build();
            
            Option w = Option.Builder("w")
                    .Required()
                    .HasArg()
                    .ArgName("word")
                    .Build();
            
            Option o = Option.Builder("o")
                    .HasArg()
                    .ArgName("directory")
                    .Build();
            
            options.AddOption(t);
            options.AddOption(w);
            options.AddOption(o);
            
            return options;
        }


        private void BuildOptionsGroup(Options options) {
            OptionGroup firstGroup = new OptionGroup();
            OptionGroup secondGroup = new OptionGroup();
            
            firstGroup.SetRequired(true);
            secondGroup.SetRequired(true);

            firstGroup.AddOption(Option.Builder("d")
                    .LongOpt("db")
                    .HasArg()
                    .ArgName("table-name")
                    .Build());
            
            firstGroup.AddOption(Option.Builder("f")
                    .LongOpt("flat-file")
                    .HasArg()
                    .ArgName("input.csv")
                    .Build());
            
            options.AddOptionGroup(firstGroup);

            secondGroup.AddOption(Option.Builder("x")
                    .HasArg()
                    .ArgName("arg1")
                    .Build());
            
            secondGroup.AddOption(Option.Builder("s")
                    .Build());
            
            secondGroup.AddOption(Option.Builder("p")
                    .HasArg()
                    .ArgName("arg1")
                    .Build());
            
            options.AddOptionGroup(secondGroup);
        }
    }
}
