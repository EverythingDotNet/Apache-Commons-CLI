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
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class OptionsTest {
        
        [Fact]
        public void SimpleTest() {
            Options opts = new Options();

            opts.AddOption("a", false, "toggle -a");
            opts.AddOption("b", true, "toggle -b");

            Assert.True(opts.HasOption("a"));
            Assert.True(opts.HasOption("b"));
        }

        
        [Fact]
        public void DuplicateSimpleTest() {
            Options opts = new Options();
            opts.AddOption("a", false, "toggle -a");
            opts.AddOption("a", true, "toggle -a*");

            Assert.Equal("toggle -a*", opts.GetOption("a").GetDescription());
        }

        
        [Fact]
        public void LongTest() {
            Options opts = new Options();

            opts.AddOption("a", "--a", false, "toggle -a");
            opts.AddOption("b", "--b", true, "set -b");

            Assert.True(opts.HasOption("a"));
            Assert.True(opts.HasOption("b"));
        }


        [Fact]
        public void DuplicateLongTest() {
            Options opts = new Options();
            opts.AddOption("a", "--a", false, "toggle -a");
            opts.AddOption("a", "--a", false, "toggle -a*");
            
            Assert.Equal("toggle -a*", opts.GetOption("a").GetDescription());
        }


        [Fact]
        public void HelpOptionsTest() {
            Option longOnly1 = OptionBuilder.WithLongOpt("long-only1").Create();
            Option longOnly2 = OptionBuilder.WithLongOpt("long-only2").Create();
            Option shortOnly1 = OptionBuilder.Create("1");
            Option shortOnly2 = OptionBuilder.Create("2");
            Option bothA = OptionBuilder.WithLongOpt("bothA").Create("a");
            Option bothB = OptionBuilder.WithLongOpt("bothB").Create("b");

            Options options = new Options();
            options.AddOption(longOnly1);
            options.AddOption(longOnly2);
            options.AddOption(shortOnly1);
            options.AddOption(shortOnly2);
            options.AddOption(bothA);
            options.AddOption(bothB);

            ICollection<Option> allOptions = new List<Option>();
            allOptions.Add(longOnly1);
            allOptions.Add(longOnly2);
            allOptions.Add(shortOnly1);
            allOptions.Add(shortOnly2);
            allOptions.Add(bothA);
            allOptions.Add(bothB);

            ICollection<Option> helpOptions = options.HelpOptions();

            Assert.True(helpOptions.Intersect(allOptions).Count() == helpOptions.Count(), "Everything in all should be in help");
            Assert.True(allOptions.Intersect(helpOptions).Count() == allOptions.Count(), "Everything in help should be in all");
        }

        
        [Fact]
        public void MissingOptionExceptionTest() {
            Options options = new Options();
            options.AddOption(OptionBuilder.IsRequired().Create("f"));
            
            MissingOptionException e = Assert.Throws<MissingOptionException>(() => new PosixParser().Parse(options, new string[0]));

            Assert.Equal("Missing required option: f", e.Message);
        }


        [Fact]
        public void MissingOptionsExceptionTest() {
            Options options = new Options();
            options.AddOption(OptionBuilder.IsRequired().Create("f"));
            options.AddOption(OptionBuilder.IsRequired().Create("x"));
            
            
            MissingOptionException e = Assert.Throws<MissingOptionException>(() => new PosixParser().Parse(options, new string[0]));

            Assert.Equal("Missing required options: f, x", e.Message);
        }


        [Fact]
        public void ToStringTest() {
            Options options = new Options();
            options.AddOption("f", "foo", true, "Foo");
            options.AddOption("b", "bar", false, "Bar");

            string s = options.ToString();
            
            Assert.NotNull(s);
            Assert.True(s.ToLower().Contains("foo"), "foo option missing");
            Assert.True(s.ToLower().Contains("bar"), "bar option missing");
        }

        
        [Fact]
        public void GetOptionsGroupsTest() {
            Options options = new Options();

            OptionGroup group1 = new OptionGroup();
            group1.AddOption(OptionBuilder.Create('a'));
            group1.AddOption(OptionBuilder.Create('b'));

            OptionGroup group2 = new OptionGroup();
            group2.AddOption(OptionBuilder.Create('x'));
            group2.AddOption(OptionBuilder.Create('y'));

            options.AddOptionGroup(group1);
            options.AddOptionGroup(group2);

            Assert.NotNull(options.GetOptionGroups());
            Assert.Equal(2, options.GetOptionGroups().Count);
        }


        [Fact]
        public void GetMatchingOptsTest() {
            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("version").Create());
            options.AddOption(OptionBuilder.WithLongOpt("verbose").Create());

            Assert.True(options.GetMatchingOptions("foo").Count == 0);
            Assert.Equal(1, options.GetMatchingOptions("version").Count);
            Assert.Equal(2, options.GetMatchingOptions("ver").Count);
        }
    }
}
