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
    
    using Xunit;

    public class OptionGroupTest {

        private Options _options = null;
        private readonly Parser parser = new PosixParser();


        public OptionGroupTest() {
            Option file = new Option("f", "file", false, "file to process");
            Option dir = new Option("d", "directory", false, "directory to process");

            OptionGroup group = new OptionGroup();
            group.AddOption(file);
            group.AddOption(dir);

            _options = new Options().AddOptionGroup(group);

            Option section = new Option("s", "section", false, "section to process");
            Option chapter = new Option("c", "chapter", false, "chapter to process");

            OptionGroup group2 = new OptionGroup();
            group2.AddOption(section);
            group2.AddOption(chapter);

            _options.AddOptionGroup(group2);

            Option importOpt = new Option(null, "import", false, "section to process");
            Option exportOpt = new Option(null, "export", false, "chapter to process");

            OptionGroup group3 = new OptionGroup();
            group3.AddOption(importOpt);
            group3.AddOption(exportOpt);

            _options.AddOptionGroup(group3);
            _options.AddOption("r", "revision", false, "revision number");
        }


        [Fact]
        public void SingleOptionFromGroupTest() {
            string[] args = new string[] { "-f" };

            CommandLine cl = parser.Parse(_options, args);

            Assert.True(!cl.HasOption("r"), "Confirm -r is NOT set");
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True( cl.GetArgList().Count == 0, "Confirm no extra args");
        }


        [Fact]
        public void SingleOptionTest() {
            string[] args = new string[] { "-r" };

            CommandLine cl = parser.Parse(_options, args);

            Assert.True(cl.HasOption("r"), "Confirm -r is set");
            Assert.True(!cl.HasOption("f"), "Confirm -f is NOT set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 0, "Confirm no extra args");
        }


        [Fact]
        public void TwoValidOptionsTest() {
            string[] args = new string[] { "-r", "-f" };

            CommandLine cl = parser.Parse( _options, args);

            Assert.True(cl.HasOption("r"), "Confirm -r is set");
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 0, "Confirm no extra args");
        }


        [Fact]
        public void SingleLongOptionTest() {
            string[] args = new string[] { "--file" };

            CommandLine cl = parser.Parse( _options, args);

            Assert.True(!cl.HasOption("r"), "Confirm -r is NOT set");
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 0, "Confirm no extra args");
        }

        
        [Fact]
        public void TwoValidLongOptionsTest() {
            string[] args = new string[] { "--revision", "--file" };

            CommandLine cl = parser.Parse( _options, args);

            Assert.True(cl.HasOption("r"), "Confirm -r is set");
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 0, "Confirm no extra args");
        }


        [Fact]
        public void NoOptionsExtraArgsTest() {
            string[] args = new string[] { "arg1", "arg2" };

            CommandLine cl = parser.Parse( _options, args);

            Assert.True(!cl.HasOption("r"), "Confirm -r is NOT set");
            Assert.True(!cl.HasOption("f"), "Confirm -f is NOT set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(!cl.HasOption("s"), "Confirm -s is NOT set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 2, "Confirm TWO extra args");
        }

        
        [Fact]
        public void TwoOptionsFromGroupTest() {
            string[] args = new string[] { "-f", "-d" };

            AlreadySelectedException e = Assert.Throws<AlreadySelectedException>(() => parser.Parse(_options, args));

            Assert.NotNull(e.OptionGroup);
            Assert.Equal("f", e.OptionGroup.GetSelected());
            Assert.Equal("d", e.Option.GetOpt());
        }
    

        [Fact]
        public void TwoLongOptionsFromGroupTest() {
            string[] args = new string[] { "--file", "--directory" };

            AlreadySelectedException e = Assert.Throws<AlreadySelectedException>(() => parser.Parse(_options, args));

            Assert.NotNull(e.OptionGroup);
            Assert.Equal("f", e.OptionGroup.GetSelected());
            Assert.Equal("d", e.Option.GetOpt());
        }


        [Fact]
        public void TwoOptionsFromDifferentGroupTest() {
            string[] args = new string[] { "-f", "-s" };

            CommandLine cl = parser.Parse( _options, args);

            Assert.True(!cl.HasOption("r"), "Confirm -r is NOT set");
            Assert.True(cl.HasOption("f"), "Confirm -f is set");
            Assert.True(!cl.HasOption("d"), "Confirm -d is NOT set");
            Assert.True(cl.HasOption("s"),"Confirm -s is set");
            Assert.True(!cl.HasOption("c"), "Confirm -c is NOT set");
            Assert.True(cl.GetArgList().Count == 0, "Confirm NO extra args");
        }


        [Fact]
        public void TwoOptionsFromGroupWithPropertiesTest() {
            string[] args = new string[] { "-f" };

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("d", "true");

            CommandLine cl = parser.Parse( _options, args, properties);

            Assert.True(cl.HasOption("f"));
            Assert.True(!cl.HasOption("d"));
        }


        [Fact]
        public void ValidLongOnlyOptionsTest() {
            CommandLine cl1 = parser.Parse(_options, new string[] { "--export" });
            Assert.True(cl1.HasOption("export"), "Confirm --export is set");

            CommandLine cl2 = parser.Parse(_options, new string[] { "--import" });
            Assert.True(cl2.HasOption("import"), "Confirm --import is set");
        }

        
        
        [Fact]
        public void ToStringTest() {
            OptionGroup group1 = new OptionGroup();
            group1.AddOption(new Option(null, "foo", false, "Foo"));
            group1.AddOption(new Option(null, "bar", false, "Bar"));

            if (!"[--bar Bar, --foo Foo]".Equals(group1.ToString())) {
                Assert.Equal("[--foo Foo, --bar Bar]", group1.ToString());
            }

            OptionGroup group2 = new OptionGroup();
            group2.AddOption(new Option("f", "foo", false, "Foo"));
            group2.AddOption(new Option("b", "bar", false, "Bar"));

            if (!"[-b Bar, -f Foo]".Equals(group2.ToString())) {
                Assert.Equal("[-f Foo, -b Bar]", group2.ToString());
            }
        }


        [Fact]
        public void GetNamesTest() {
            OptionGroup group = new OptionGroup();
            group.AddOption(OptionBuilder.Create('a'));
            group.AddOption(OptionBuilder.Create('b'));

            Assert.NotNull(group.GetNames());
            Assert.Equal(2, group.GetNames().Count);
            Assert.True(group.GetNames().Contains("a"));
            Assert.True(group.GetNames().Contains("b"));
        }
    }
}
