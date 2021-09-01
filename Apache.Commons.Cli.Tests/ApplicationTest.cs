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
    using System.IO;
    using Xunit;

    /**
     * This is a collection of tests that test real world applications command lines.
     *
     * <p>
     * The following applications are tested:
     * <ul>
     *   <li>ls</li>
     *   <li>Ant</li>
     *   <li>Groovy</li>
     *   <li>man</li>
     * </ul>
     * </p>
     */
    public class ApplicationTest {

        [Fact]
        public void LsTest() {
            Options options = new Options();
            options.AddOption( "a", "all", false, "do not hide entries starting with ." );
            options.AddOption( "A", "almost-all", false, "do not list implied . and .." );
            options.AddOption( "b", "escape", false, "print octal escapes for nongraphic characters" );
            options.AddOption( OptionBuilder.WithLongOpt( "block-size" )
                                            .WithDescription( "use SIZE-byte blocks" )
                                            .HasArg()
                                            .WithArgName("SIZE")
                                            .Create() );
            options.AddOption( "B", "ignore-backups", false, "do not list implied entried ending with ~");
            options.AddOption( "c", false, "with -lt: sort by, and show, ctime (time of last modification of file status information) with -l:show ctime and sort by name otherwise: sort by ctime" );
            options.AddOption( "C", false, "list entries by columns" );

            string[] args = new string[]{ "--block-size=10" };

            // create the command line parser
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse( options, args );
            
            Assert.True( line.HasOption( "block-size" ) );
            Assert.Equal( "10" , line.GetOptionValue( "block-size" ));
        }


        /**
         * Ant test
         */
        [Fact]
        public void AntTest() {
            Options options = new Options();
            options.AddOption( "help", false, "print this message" );
            options.AddOption( "projecthelp", false, "print project help information" );
            options.AddOption( "version", false, "print the version information and exit" );
            options.AddOption( "quiet", false, "be extra quiet" );
            options.AddOption( "verbose", false, "be extra verbose" );
            options.AddOption( "debug", false, "print debug information" );
            options.AddOption( "logfile", true, "use given file for log" );
            options.AddOption( "logger", true, "the class which is to perform the logging" );
            options.AddOption( "listener", true, "add an instance of a class as a project listener" );
            options.AddOption( "buildfile", true, "use given buildfile" );
            options.AddOption( OptionBuilder.WithDescription( "use value for given property" )
                                            .HasArgs()
                                            .WithValueSeparator()
                                            .Create( 'D' ) );
                               //, null, true, , false, true );
            options.AddOption( "find", true, "search for buildfile towards the root of the filesystem and use it" );

            string[] args = new string[] { 
                "-buildfile", "mybuild.xml",
                "-Dproperty=value", "-Dproperty1=value1",
                "-projecthelp" };

            // use the GNU parser
            ICommandLineParser parser = new GnuParser();
            CommandLine line = parser.Parse( options, args );

            // check multiple values
            string[] opts = line.GetOptionValues( "D" );
            
            Assert.Equal( "property", opts[0] );
            Assert.Equal( "value", opts[1] );
            Assert.Equal( "property1", opts[2] );
            Assert.Equal( "value1", opts[3] );

            // check single value
            Assert.Equal( "mybuild.xml" , line.GetOptionValue( "buildfile"));

            // check option
            Assert.True( line.HasOption( "projecthelp") );
        }


        [Fact]
        public void GroovyTest() {
            Options options = new Options();

            options.AddOption(
                OptionBuilder.WithLongOpt("define").
                    WithDescription("define a system property").
                    HasArg(true).
                    WithArgName("name=value").
                    Create('D'));
            options.AddOption(
                OptionBuilder.HasArg(false)
                .WithDescription("usage information")
                .WithLongOpt("help")
                .Create('h'));
            options.AddOption(
                OptionBuilder.HasArg(false)
                .WithDescription("debug mode will print out full stack traces")
                .WithLongOpt("debug")
                .Create('d'));
            options.AddOption(
                OptionBuilder.HasArg(false)
                .WithDescription("display the Groovy and JVM versions")
                .WithLongOpt("version")
                .Create('v'));
            options.AddOption(
                OptionBuilder.WithArgName("charset")
                .HasArg()
                .WithDescription("specify the encoding of the files")
                .WithLongOpt("encoding")
                .Create('c'));
            options.AddOption(
                OptionBuilder.WithArgName("script")
                .HasArg()
                .WithDescription("specify a command line script")
                .Create('e'));
            options.AddOption(
                OptionBuilder.WithArgName("extension")
                .HasOptionalArg()
                .WithDescription("modify files in place; create backup if extension is given (e.g. \'.bak\')")
                .Create('i'));
            options.AddOption(
                OptionBuilder.HasArg(false)
                .WithDescription("process files line by line using implicit 'line' variable")
                .Create('n'));
            options.AddOption(
                OptionBuilder.HasArg(false)
                .WithDescription("process files line by line and print result (see also -n)")
                .Create('p'));
            options.AddOption(
                OptionBuilder.WithArgName("port")
                .HasOptionalArg()
                .WithDescription("listen on a port and process inbound lines")
                .Create('l'));
            options.AddOption(
                OptionBuilder.WithArgName("splitPattern")
                .HasOptionalArg()
                .WithDescription("split lines using splitPattern (default '\\s') using implicit 'split' variable")
                .WithLongOpt("autosplit")
                .Create('a'));

            Parser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-e", "println 'hello'" }, true);

            Assert.True(line.HasOption('e'));
            Assert.Equal("println 'hello'", line.GetOptionValue('e'));
        }


        /**
         * author Slawek Zachcial
         */
        [Fact]
        public void ManTest() {
            string cmdLine =
                    "man [-c|-f|-k|-w|-tZT device] [-adlhu7V] [-Mpath] [-Ppager] [-Slist] " +
                            "[-msystem] [-pstring] [-Llocale] [-eextension] [section] page ...";
            
            Options options = new Options().
                    AddOption("a", "all", false, "find all matching manual pages.").
                    AddOption("d", "debug", false, "emit debugging messages.").
                    AddOption("e", "extension", false, "limit search to extension type 'extension'.").
                    AddOption("f", "whatis", false, "equivalent to whatis.").
                    AddOption("k", "apropos", false, "equivalent to apropos.").
                    AddOption("w", "location", false, "print physical location of man page(s).").
                    AddOption("l", "local-file", false, "interpret 'page' argument(s) as local filename(s)").
                    AddOption("u", "update", false, "force a cache consistency check.").
                    //FIXME - should generate -r,--prompt string
                    AddOption("r", "prompt", true, "provide 'less' pager with prompt.").
                    AddOption("c", "catman", false, "used by catman to reformat out of date cat pages.").
                    AddOption("7", "ascii", false, "display ASCII translation or certain latin1 chars.").
                    AddOption("t", "troff", false, "use troff format pages.").
                    //FIXME - should generate -T,--troff-device device
                    AddOption("T", "troff-device", true, "use groff with selected device.").
                    AddOption("Z", "ditroff", false, "use groff with selected device.").
                    AddOption("D", "default", false, "reset all options to their default values.").
                    //FIXME - should generate -M,--manpath path
                    AddOption("M", "manpath", true, "set search path for manual pages to 'path'.").
                    //FIXME - should generate -P,--pager pager
                    AddOption("P", "pager", true, "use program 'pager' to display output.").
                    //FIXME - should generate -S,--sections list
                    AddOption("S", "sections", true, "use colon separated section list.").
                    //FIXME - should generate -m,--systems system
                    AddOption("m", "systems", true, "search for man pages from other unix system(s).").
                    //FIXME - should generate -L,--locale locale
                    AddOption("L", "locale", true, "define the locale for this particular man search.").
                    //FIXME - should generate -p,--preprocessor string
                    AddOption("p", "preprocessor", true, "string indicates which preprocessor to run.\n" +
                             " e - [n]eqn  p - pic     t - tbl\n" +
                             " g - grap    r - refer   v - vgrind").
                    AddOption("V", "version", false, "show version.").
                    AddOption("h", "help", false, "show this usage message.");

            HelpFormatter hf = new HelpFormatter();
            
            string EOL = Environment.NewLine;
            StringWriter sw = new StringWriter();
            hf.PrintHelp(sw, 60, cmdLine, null, options, HelpFormatter.DEFAULT_LEFT_PAD, HelpFormatter.DEFAULT_DESC_PAD, null, false);
            
            Assert.Equal("usage: man [-c|-f|-k|-w|-tZT device] [-adlhu7V] [-Mpath]" + EOL +
                            "           [-Ppager] [-Slist] [-msystem] [-pstring]" + EOL +
                            "           [-Llocale] [-eextension] [section] page ..." + EOL +
                            " -7,--ascii                display ASCII translation or" + EOL +
                            "                           certain latin1 chars." + EOL +
                            " -a,--all                  find all matching manual pages." + EOL +
                            " -c,--catman               used by catman to reformat out of" + EOL +
                            "                           date cat pages." + EOL +
                            " -d,--debug                emit debugging messages." + EOL +
                            " -D,--default              reset all options to their" + EOL +
                            "                           default values." + EOL +
                            " -e,--extension            limit search to extension type" + EOL +
                            "                           'extension'." + EOL +
                            " -f,--whatis               equivalent to whatis." + EOL +
                            " -h,--help                 show this usage message." + EOL +
                            " -k,--apropos              equivalent to apropos." + EOL +
                            " -l,--local-file           interpret 'page' argument(s) as" + EOL +
                            "                           local filename(s)" + EOL +
                            " -L,--locale <arg>         define the locale for this" + EOL +
                            "                           particular man search." + EOL +
                            " -M,--manpath <arg>        set search path for manual pages" + EOL +
                            "                           to 'path'." + EOL +
                            " -m,--systems <arg>        search for man pages from other" + EOL +
                            "                           unix system(s)." + EOL +
                            " -P,--pager <arg>          use program 'pager' to display" + EOL +
                            "                           output." + EOL +
                            " -p,--preprocessor <arg>   string indicates which" + EOL +
                            "                           preprocessor to run." + EOL +
                            "                           e - [n]eqn  p - pic     t - tbl" + EOL +
                            "                           g - grap    r - refer   v -" + EOL +
                            "                           vgrind" + EOL +
                            " -r,--prompt <arg>         provide 'less' pager with prompt." + EOL +
                            " -S,--sections <arg>       use colon separated section list." + EOL +
                            " -t,--troff                use troff format pages." + EOL +
                            " -T,--troff-device <arg>   use groff with selected device." + EOL +
                            " -u,--update               force a cache consistency check." + EOL +
                            " -V,--version              show version." + EOL +
                            " -w,--location             print physical location of man" + EOL +
                            "                           page(s)." + EOL +
                            " -Z,--ditroff              use groff with selected device." + EOL,
                   
                    sw.ToString());
        }


        /**
         * Real world test with long and short options.
         */
        [Fact]
        public void NltTest() {
            Option help = new Option("h", "help", false, "print this message");
            Option version = new Option("v", "version", false, "print version information");
            Option newRun = new Option("n", "new", false, "Create NLT cache entries only for new items");
            Option trackerRun = new Option("t", "tracker", false, "Create NLT cache entries only for tracker items");

            Option timeLimit = OptionBuilder.WithLongOpt("limit").HasArg()
                                            .WithValueSeparator()
                                            .WithDescription("Set time limit for execution, in minutes")
                                            .Create("l");

            Option age = OptionBuilder.WithLongOpt("age").HasArg()
                                      .WithValueSeparator()
                                      .WithDescription("Age (in days) of cache item before being recomputed")
                                      .Create("a");

            Option server = OptionBuilder.WithLongOpt("server").HasArg()
                                         .WithValueSeparator()
                                         .WithDescription("The NLT server address")
                                         .Create("s");

            Option numResults = OptionBuilder.WithLongOpt("results").HasArg()
                                             .WithValueSeparator()
                                             .WithDescription("Number of results per item")
                                             .Create("r");

            Option configFile = OptionBuilder.WithLongOpt("file").HasArg()
                                             .WithValueSeparator()
                                             .WithDescription("Use the specified configuration file")
                                             .Create();

            Options options = new Options();
            
            options.AddOption(help);
            options.AddOption(version);
            options.AddOption(newRun);
            options.AddOption(trackerRun);
            options.AddOption(timeLimit);
            options.AddOption(age);
            options.AddOption(server);
            options.AddOption(numResults);
            options.AddOption(configFile);

            string[] args = new string[] {
                    "-v",
                    "-l", "10",
                    "-age", "5",
                    "-file", "filename"
                };

            // create the command line parser
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, args);
            
            Assert.True(line.HasOption("v"));
            Assert.Equal("10", line.GetOptionValue("l"));
            Assert.Equal("10", line.GetOptionValue("limit"));
            Assert.Equal("5", line.GetOptionValue("a"));
            Assert.Equal("5", line.GetOptionValue("age"));
            Assert.Equal("filename", line.GetOptionValue("file"));
        }
    }
}
