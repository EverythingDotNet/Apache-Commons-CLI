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

namespace Apache.Commons.Cli {

    using System;

    using Xunit;

    public class OptionTest {
        
        [Serializable]
        private class TestOption : Option {

            public TestOption(string opt, bool hasArg, string description)
                : base(opt, hasArg, description) { 

            }

            
            public override bool AddValue(string value) {
                AddValueForProcessing(value);
                return true;
            }
        }


        [Fact]
        public void ClearTest() {
            TestOption option = new TestOption("x", true, "");
            Assert.Equal(0, option.GetValuesList().Count);
            
            option.AddValue("a");
            Assert.Equal(1, option.GetValuesList().Count);
            
            option.ClearValues();
            Assert.Equal(0, option.GetValuesList().Count);
        }


        // See https://issues.apache.org/jira/browse/CLI-21
        [Fact]
        public void CloneTest() {
            TestOption a = new TestOption("a", true, "");
            TestOption b = (TestOption) a.Clone();
            
            Assert.Equal(a, b);
            Assert.NotSame(a, b);
            
            a.SetDescription("a");
            Assert.Equal("", b.GetDescription());

            b.SetArgs(2);
            b.AddValue("b1");
            b.AddValue("b2");

            Assert.Equal(1, a.GetArgs());
            Assert.Equal(0, a.GetValuesList().Count);
            Assert.Equal(2, b.GetValues().Length);
        }


        [Fact]
        public void HashCodeTest() {
            Assert.NotEqual(Option.Builder("test").Build().GetHashCode(), Option.Builder("test2").Build().GetHashCode()) ;
            Assert.NotEqual(Option.Builder("test").Build().GetHashCode(), Option.Builder().LongOpt("test").Build().GetHashCode()) ;
            Assert.NotEqual(Option.Builder("test").Build().GetHashCode(), Option.Builder("test").LongOpt("long test").Build().GetHashCode()) ;
        }


        [Serializable]
        private class DefaultOption : Option {

            private readonly string defaultValue;

            public DefaultOption(string opt, string description, string defaultValue) 
                : base(opt, true, description) { 
                
                this.defaultValue = defaultValue;
            }

            
            public override string GetValue() {
                return base.GetValue() != null ? base.GetValue() : defaultValue;
            }
        }


        [Fact]
        public void SubclassTest() {
            Option option = new DefaultOption("f", "file", "myfile.txt");
            Option clone = (Option) option.Clone();
            
            Assert.Equal("myfile.txt", clone.GetValue());
            Assert.Equal(typeof(DefaultOption), clone.GetType());
        }


        [Fact]
        public void HasArgNameTest() {
            Option option = new Option("f", null);

            option.SetArgName(null);
            Assert.False(option.HasArgName());

            option.SetArgName("");
            Assert.False(option.HasArgName());

            option.SetArgName("file");
            Assert.True(option.HasArgName());
        }


        [Fact]
        public void HasArgsTest() {
            Option option = new Option("f", null);

            option.SetArgs(0);
            Assert.False(option.HasArgs());

            option.SetArgs(1);
            Assert.False(option.HasArgs());

            option.SetArgs(10);
            Assert.True(option.HasArgs());

            option.SetArgs(Option.UNLIMITED_VALUES);
            Assert.True(option.HasArgs());

            option.SetArgs(Option.UNINITIALIZED);
            Assert.False(option.HasArgs());
        }


        [Fact]
        public void GetValueTest() {
            Option option = new Option("f", null);
            option.SetArgs(Option.UNLIMITED_VALUES);

            Assert.Equal("default", option.GetValue("default"));
            Assert.Null(option.GetValue(0));

            option.AddValueForProcessing("foo");

            Assert.Equal("foo", option.GetValue());
            Assert.Equal("foo", option.GetValue(0));
            Assert.Equal("foo", option.GetValue("default"));
        }


        [Fact]
        public void BuilderMethodsTest() {
            char defaultSeparator = (char) 0;

            CheckOption(Option.Builder("a").Desc("desc").Build(), 
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").LongOpt("aaa").Build(),
                "a", "desc", "aaa", Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").HasArg(true).Build(),
                "a", "desc", null, 1, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").HasArg(false).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").HasArg(true).Build(),
                "a", "desc", null, 1, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").NumberOfArgs(3).Build(),
                "a", "desc", null, 3, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").Required(true).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, true, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").Required(false).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));

            CheckOption(Option.Builder("a").Desc("desc").ArgName("arg1").Build(),
                "a", "desc", null, Option.UNINITIALIZED, "arg1", false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").OptionalArg(false).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").OptionalArg(true).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, true, defaultSeparator, typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").ValueSeparator(':').Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, ':', typeof(string));
            CheckOption(Option.Builder("a").Desc("desc").OptionType(typeof(Int32)).Build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(Int32));
        }


        [Fact]
        public void BuilderInsufficientParams1Test() {
            Assert.Throws<ArgumentException>(() => Option.Builder().Desc("desc").Build());
        }

        [Fact]
        public void BuilderInsufficientParams2Test() {
            Assert.Throws<ArgumentException>(() => Option.Builder(null).Desc("desc").Build());
        }


        private static void CheckOption(Option option, string opt, string description, string longOpt, int numArgs, string argName,  bool required, bool optionalArg, char valueSeparator, Type cls) {
            Assert.Equal(opt, option.GetOpt());
            Assert.Equal(description, option.GetDescription());
            Assert.Equal(longOpt, option.GetLongOpt());
            Assert.Equal(numArgs, option.GetArgs());
            Assert.Equal(argName, option.GetArgName());
            Assert.Equal(required, option.IsRequired());

            Assert.Equal(optionalArg, option.HasOptionalArg());
            Assert.Equal(valueSeparator, option.GetValueSeparator());
            Assert.Equal(cls, option.GetOptionType());
        }
    }
}
