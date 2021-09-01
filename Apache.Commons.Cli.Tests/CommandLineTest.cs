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

    using System;
    using System.Collections.Generic;
    
    using Xunit;

    public class CommandLineTest {

        [Fact]
        public void GetOptionPropertiesTest() {
            string[] args = new string[] { 
                "-Dparam1=value1", 
                "-Dparam2=value2", 
                "-Dparam3", 
                "-Dparam4=value4", 
                "-D", 
                "--property", "foo=bar" 
            };

            Options options = new Options();
            options.AddOption(OptionBuilder.WithValueSeparator().HasOptionalArgs(2).Create('D'));
            options.AddOption(OptionBuilder.WithValueSeparator().HasArgs(2).WithLongOpt("property").Create());

            Parser parser = new GnuParser();
            CommandLine cl = parser.Parse(options, args);

            Dictionary<string, string> props = cl.GetOptionProperties("D");
    
            Assert.NotNull(props);
            Assert.Equal(4, props.Count);
            Assert.Equal("value1", props["param1"]);
            Assert.Equal("value2", props["param2"]);
            Assert.Equal("true", props["param3"]);
            Assert.Equal("value4", props["param4"]);

            Assert.Equal("bar", cl.GetOptionProperties("property")["foo"]);
        }

        
        [Fact]
        public void GetOptionPropertiesWithOptionTest() {
            string[] args = new string[] { 
                "-Dparam1=value1", 
                "-Dparam2=value2", 
                "-Dparam3", 
                "-Dparam4=value4", 
                "-D", 
                "--property", "foo=bar" 
            };

            Options options = new Options();
            Option option_D = OptionBuilder.WithValueSeparator().HasOptionalArgs(2).Create('D');
            Option option_property = OptionBuilder.WithValueSeparator().HasArgs(2).WithLongOpt("property").Create();
            
            options.AddOption(option_D);
            options.AddOption(option_property);

            Parser parser = new GnuParser();
            CommandLine cl = parser.Parse(options, args);

            Dictionary<string, string> props = cl.GetOptionProperties(option_D);
            
            Assert.NotNull(props);
            Assert.Equal(4, props.Count);
            Assert.Equal("value1", props["param1"]);
            Assert.Equal("value2", props["param2"]);
            Assert.Equal("true", props["param3"]);
            Assert.Equal("value4", props["param4"]);

            Assert.Equal("bar", cl.GetOptionProperties(option_property)["foo"]);
        }

        
        [Fact]
        public void GetOptionsTest() {
            CommandLine cmd = new CommandLine();
            Assert.NotNull(cmd.GetOptions());
            Assert.Empty(cmd.GetOptions());

            cmd.AddOption(new Option("a", null));
            cmd.AddOption(new Option("b", null));
            cmd.AddOption(new Option("c", null));

            Assert.Equal(3, cmd.GetOptions().Length);
        }

        
        [Fact]
        public void GetParsedOptionValueTest() {
            Options options = new Options();
            options.AddOption(OptionBuilder.HasArg().WithOptionType(typeof(ValueType)).Create("i"));
            options.AddOption(OptionBuilder.HasArg().Create("f"));

            ICommandLineParser parser = new DefaultParser();
            CommandLine cmd = parser.Parse(options, new string[] { "-i", "123", "-f", "foo" });

            Assert.Equal(123, Convert.ToInt32(cmd.GetParsedOptionValue("i")));
            Assert.Equal("foo", cmd.GetParsedOptionValue("f"));
        }

        
        [Fact]
        public void GetParsedOptionValueWithCharTest() {
            Options options = new Options();
            options.AddOption(Option.Builder("i").HasArg().OptionType(typeof(ValueType)).Build());
            options.AddOption(Option.Builder("f").HasArg().Build());

            ICommandLineParser parser = new DefaultParser();
            CommandLine cmd = parser.Parse(options, new string[] { "-i", "123", "-f", "foo" });

            Assert.Equal(123, Convert.ToInt32(cmd.GetParsedOptionValue('i').ToString()));
            Assert.Equal("foo", cmd.GetParsedOptionValue('f'));
        }


        [Fact]
        public void GetParsedOptionValueWithOptionTest() {
            Options options = new Options();
            Option opt_i = Option.Builder("i").HasArg().OptionType(typeof(ValueType)).Build();
            Option opt_f = Option.Builder("f").HasArg().Build();
            
            options.AddOption(opt_i);
            options.AddOption(opt_f);

            ICommandLineParser parser = new DefaultParser();
            CommandLine cmd = parser.Parse(options, new string[] { "-i", "123", "-f", "foo" });

            Assert.Equal(123, Convert.ToInt32(cmd.GetParsedOptionValue(opt_i)));
            Assert.Equal("foo", cmd.GetParsedOptionValue(opt_f));
        }

        
        [Fact]
        public void NullhOptionTest() {
            Options options = new Options();
            Option opt_i = Option.Builder("i").HasArg().OptionType(typeof(ValueType)).Build();
            Option opt_f = Option.Builder("f").HasArg().Build();
            
            options.AddOption(opt_i);
            options.AddOption(opt_f);
            
            ICommandLineParser parser = new DefaultParser();
            CommandLine cmd = parser.Parse(options, new String[] { "-i", "123", "-f", "foo" });
            
            Assert.Null(cmd.GetOptionValue((Option)null));
            Assert.Null(cmd.GetParsedOptionValue((Option)null));
        }

        [Fact]
        public void BuilderTest() {
            CommandLine.Builder builder = new CommandLine.Builder();
            builder.AddArg( "foo" ).AddArg( "bar" );
            builder.AddOption( Option.Builder( "T" ).Build() );
            
            CommandLine cmd = builder.Build();

            Assert.Equal( "foo", cmd.GetArgs()[0] );
            Assert.Equal( "bar", cmd.GetArgList()[1]);
            Assert.Equal( "T", cmd.GetOptions()[0].GetOpt());
        }
    }
}
