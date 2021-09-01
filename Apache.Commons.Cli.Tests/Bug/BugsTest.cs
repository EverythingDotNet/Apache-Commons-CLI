#region Apache License
/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

namespace Apache.Commons.Cli.Bug {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    
    using Xunit;

    public class BugsTest {
        
        [Fact]
        public void Test11457() {
            Options options = new Options();
            options.AddOption(OptionBuilder.WithLongOpt("verbose").Create());
            
            string[] args = new string[]{"--verbose"};

            ICommandLineParser parser = new PosixParser();
            CommandLine cmd = parser.Parse(options, args);
            
            Assert.True(cmd.HasOption("verbose"));
        }


        [Fact]
        public void Test11458() {
            Options options = new Options();
            options.AddOption( OptionBuilder.WithValueSeparator( '=' ).HasArgs().Create( 'D' ) );
            options.AddOption( OptionBuilder.WithValueSeparator( ':' ).HasArgs().Create( 'p' ) );
            
            string[] args = new string[] { "-DJAVA_HOME=/opt/java" , "-pfile1:file2:file3" };

            ICommandLineParser parser = new PosixParser();
            CommandLine cmd = parser.Parse(options, args);

            string[] values = cmd.GetOptionValues('D');

            Assert.Equal("JAVA_HOME", values[0]);
            Assert.Equal("/opt/java", values[1]);

            values = cmd.GetOptionValues('p');

            Assert.Equal("file1", values[0]);
            Assert.Equal("file2", values[1]);
            Assert.Equal("file3", values[2]);

            
            foreach(Option opt in cmd.GetOptions()) {
                
                switch (opt.GetId()) {
                    case 'D':
                        Assert.Equal("JAVA_HOME", opt.GetValue(0));
                        Assert.Equal("/opt/java", opt.GetValue(1));
                        break;

                    case 'p':
                        Assert.Equal("file1", opt.GetValue(0));
                        Assert.Equal("file2", opt.GetValue(1));
                        Assert.Equal("file3", opt.GetValue(2));
                        break;

                    default:
                        Assert.True(false, "-D option not found");
                        break;
                }
            }
        }

        
        [Fact]
        public void Test11680() {
            Options options = new Options();
            options.AddOption("f", true, "foobar");
            options.AddOption("m", true, "missing");
            
            string[] args = new string[]{"-f", "foo"};

            ICommandLineParser parser = new PosixParser();
            CommandLine cmd = parser.Parse(options, args);

            cmd.GetOptionValue("f", "default f");
            cmd.GetOptionValue("m", "default m");
        }


        [Fact]
        public void Test11456() {
            // Posix
            Options options = new Options();
            options.AddOption( OptionBuilder.HasOptionalArg().Create( 'a' ) );
            options.AddOption( OptionBuilder.HasArg().Create( 'b' ) );
            
            string[] args = new string[] { "-a", "-bvalue" };

            ICommandLineParser parser = new PosixParser();
            CommandLine cmd = parser.Parse( options, args );

            Assert.Equal( "value" , cmd.GetOptionValue( 'b' ));

            // GNU
            options = new Options();
            options.AddOption( OptionBuilder.HasOptionalArg().Create( 'a' ) );
            options.AddOption( OptionBuilder.HasArg().Create( 'b' ) );
            args = new string[] { "-a", "-b", "value" };

            parser = new GnuParser();

            cmd = parser.Parse( options, args );
            Assert.Equal( "value" , cmd.GetOptionValue( 'b' ));
        }

        
        [Fact]
        public void Test12210() {
            // create the main options object which will handle the first parameter
            Options mainOptions = new Options();

            // There can be 2 main exclusive options:  -exec|-rep
            // Therefore, place them in an option group
            
            string[] argv = new string[] { "-exec", "-exec_opt1", "-exec_opt2" };
            OptionGroup grp = new OptionGroup();

            grp.AddOption(new Option("exec",false,"description for this option"));
            grp.AddOption(new Option("rep",false,"description for this option"));

            mainOptions.AddOptionGroup(grp);

            // for the exec option, there are 2 options...
            Options execOptions = new Options();
            execOptions.AddOption("exec_opt1", false, " desc");
            execOptions.AddOption("exec_opt2", false, " desc");

            // similarly, for rep there are 2 options...
            Options repOptions = new Options();
            repOptions.AddOption("repopto", false, "desc");
            repOptions.AddOption("repoptt", false, "desc");

            // create the parser
            GnuParser parser = new GnuParser();

            // finally, parse the arguments:

            // first parse the main options to see what the user has specified
            // We set stopAtNonOption to true so it does not touch the remaining
            // options
            CommandLine cmd = parser.Parse(mainOptions,argv,true);
            
            // get the remaining options...
            argv = cmd.GetArgs();

            if(cmd.HasOption("exec")) {
                cmd = parser.Parse(execOptions,argv,false);
                
                // process the exec_op1 and exec_opt2...
                Assert.True( cmd.HasOption("exec_opt1") );
                Assert.True( cmd.HasOption("exec_opt2") );
            }
            else if(cmd.HasOption("rep")) {
                cmd = parser.Parse(repOptions,argv,false);
                // process the rep_op1 and rep_opt2...
            }
            else {
                Assert.True(false,  "exec option not found" );
            }
        }


        [Fact]
        public void Test13425() {
            Options options = new Options();
            
            Option oldpass = OptionBuilder.WithLongOpt( "old-password" )
                .WithDescription( "Use this option to specify the old password" )
                .HasArg()
                .Create( 'o' );

            Option newpass = OptionBuilder.WithLongOpt( "new-password" )
                .WithDescription( "Use this option to specify the new password" )
                .HasArg()
                .Create( 'n' );

            string[] args = {
                "-o",
                "-n",
                "newpassword"
            };

            options.AddOption( oldpass );
            options.AddOption( newpass );

            Parser parser = new PosixParser();

            Assert.Throws<MissingArgumentException>(() => parser.Parse(options, args));
        }

        
        [Fact]
        public void Test13666() {
            Options options = new Options();
            
            Option dir = OptionBuilder.WithDescription( "dir" ).HasArg().Create( 'd' );
            options.AddOption( dir );

            TextWriter oldSystemOut = Console.Out;
            
            try {
                MemoryStream bytes = new MemoryStream();
                TextWriter print = new StreamWriter(bytes);

                // capture this platform's eol symbol
                print.WriteLine();
                print.Flush();
                
                string eol = Encoding.Default.GetString(bytes.ToArray());

                bytes.Position = 0;
                bytes.SetLength(0);

                Console.SetOut(new StreamWriter(bytes));

                HelpFormatter formatter = new HelpFormatter();
                formatter.PrintHelp( "dir", options );

                Assert.Equal("usage: dir" + eol + " -d <arg>   dir" + eol, Encoding.Default.GetString(bytes.ToArray()));
            }
            finally {
                Console.SetOut(oldSystemOut);
            }
        }


        [Fact]
        public void Test13935() {
            OptionGroup directions = new OptionGroup();

            Option left = new Option( "l", "left", false, "go left" );
            Option right = new Option( "r", "right", false, "go right" );
            Option straight = new Option( "s", "straight", false, "go straight" );
            Option forward = new Option( "f", "forward", false, "go forward" );
            
            forward.SetRequired( true );

            directions.AddOption( left );
            directions.AddOption( right );
            directions.SetRequired( true );

            Options opts = new Options();
            opts.AddOptionGroup( directions );
            opts.AddOption( straight );

            ICommandLineParser parser = new PosixParser();

            string[] args = new string[] {  };

            Assert.Throws<MissingOptionException>(() => parser.Parse(opts, args));

            args = new string[] { "-s" };
            
            Assert.Throws<MissingOptionException>(() => parser.Parse(opts, args));

            args = new string[] { "-s", "-l" };
            CommandLine line = parser.Parse(opts, args);
            
            Assert.NotNull(line);

            opts.AddOption( forward );
            args = new string[] { "-s", "-l", "-f" };
            line = parser.Parse(opts, args);
            
            Assert.NotNull(line);
        }


        [Fact]
        public void Test14786() {
            Option o = OptionBuilder.IsRequired().WithDescription("test").Create("test");
            
            Options opts = new Options();
            opts.AddOption(o);
            opts.AddOption(o);

            ICommandLineParser parser = new GnuParser();

            string[] args = new string[] { "-test" };

            CommandLine line = parser.Parse( opts, args );
            
            Assert.True( line.HasOption( "test" ) );
        }


        [Fact]
        public void Test15046() {
            ICommandLineParser parser = new PosixParser();
            string[] CLI_ARGS = new string[] {"-z", "c"};

            Options options = new Options();
            options.AddOption(new Option("z", "timezone", true, "affected option"));

            parser.Parse(options, CLI_ARGS);

            //now add conflicting option
            options.AddOption("c", "conflict", true, "conflict option");
            CommandLine line = parser.Parse(options, CLI_ARGS);
            
            Assert.Equal( "c" , line.GetOptionValue('z'));
            Assert.True( !line.HasOption("c") );
        }


        [Fact]
        public void Test15648() {           
            string[] args = new string[] { "-m", "\"Two Words\"" };
            
            Option m = OptionBuilder.HasArgs().Create("m");
            
            Options options = new Options();
            options.AddOption( m );

            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse( options, args );
            
            Assert.Equal( "Two Words", line.GetOptionValue( "m" ) );
        }

        
        [Fact]
        public void Test31148() {
            Option multiArgOption = new Option("o","option with multiple args");
            multiArgOption.SetArgs(1);

            Options options = new Options();
            options.AddOption(multiArgOption);

            Parser parser = new PosixParser();
            string[] args = new string[]{};
            
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("o","ovalue");
            
            CommandLine cl = parser.Parse(options,args,props);

            Assert.True(cl.HasOption('o'));
            Assert.Equal("ovalue",cl.GetOptionValue('o'));
        }
    }
}
