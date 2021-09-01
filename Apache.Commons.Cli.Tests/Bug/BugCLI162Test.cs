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

    using System;
    using System.Data;
    using System.IO;
    using Xunit;

    public class BugCLI162Test {

        /** Constant for the line separator.*/
        private static readonly string CR = Environment.NewLine;

        private HelpFormatter formatter;
        private StringWriter sw;

        
        public BugCLI162Test() {
            formatter = new HelpFormatter();
            sw = new StringWriter();
        }

        
        [Fact]
        public void InfiniteLoopTest() {
            Options options = new Options();
            options.AddOption("h", "help", false, "This is a looooong description");
            
            // used to hang & crash
            formatter.PrintHelp(sw, 20, "app", null, options, HelpFormatter.DEFAULT_LEFT_PAD, HelpFormatter.DEFAULT_DESC_PAD, null);

            string expected = "usage: app" + CR +
                    " -h,--help   This is" + CR +
                    "             a" + CR +
                    "             looooon" + CR +
                    "             g" + CR +
                    "             descrip" + CR +
                    "             tion" + CR;
            
            Assert.Equal(expected, sw.ToString());
        }


        [Fact]
        public void PrintHelpLongLinesTest() {
            
            // Constants used for options
            string OPT = "-";
            string OPT_COLUMN_NAMES = "l";
            string OPT_CONNECTION = "c";
            string OPT_DESCRIPTION = "e";
            string OPT_DRIVER = "d";
            string OPT_DRIVER_INFO = "n";
            string OPT_FILE_BINDING = "b";
            string OPT_FILE_JDBC = "j";
            string OPT_FILE_SFMD = "f";
            string OPT_HELP = "h";
            string OPT_HELP_ = "help";
            string OPT_INTERACTIVE = "i";
            string OPT_JDBC_TO_SFMD = "2";
            string OPT_JDBC_TO_SFMD_L = "jdbc2sfmd";
            string OPT_METADATA = "m";
            string OPT_PARAM_MODES_INT = "o";
            string OPT_PARAM_MODES_NAME = "O";
            string OPT_PARAM_NAMES = "a";
            string OPT_PARAM_TYPES_INT = "y";
            string OPT_PARAM_TYPES_NAME = "Y";
            string OPT_PASSWORD = "p";
            string OPT_PASSWORD_L = "password";
            string OPT_SQL = "s";
            string OPT_SQL_L = "sql";
            string OPT_SQL_SPLIT_DEFAULT = "###";
            string OPT_SQL_SPLIT_L = "splitSql";
            string OPT_STACK_TRACE = "t";
            string OPT_TIMING = "g";
            string OPT_TRIM_L = "trim";
            string OPT_USER = "u";
            string OPT_WRITE_TO_FILE = "w";
            string _PMODE_IN = "IN";
            string _PMODE_INOUT = "INOUT";
            string _PMODE_OUT = "OUT";
            string _PMODE_UNK = "Unknown";
            string PMODES = _PMODE_IN + ", " + _PMODE_INOUT + ", " + _PMODE_OUT + ", " + _PMODE_UNK;

            // Options build
            Options commandLineOptions;
            commandLineOptions = new Options();
            commandLineOptions.AddOption(OPT_HELP, OPT_HELP_, false, "Prints help and quits");
            commandLineOptions.AddOption(OPT_DRIVER, "driver", true, "JDBC driver class name");
            commandLineOptions.AddOption(OPT_DRIVER_INFO, "info", false, "Prints driver information and properties. If "
                + OPT
                + OPT_CONNECTION
                + " is not specified, all drivers on the classpath are displayed.");
            commandLineOptions.AddOption(OPT_CONNECTION, "url", true, "Connection URL");
            commandLineOptions.AddOption(OPT_USER, "user", true, "A database user name");
            commandLineOptions.AddOption(
                            OPT_PASSWORD,
                            OPT_PASSWORD_L,
                            true,
                            "The database password for the user specified with the "
                                + OPT
                                + OPT_USER
                                + " option. You can obfuscate the password with org.mortbay.jetty.security.Password, see http://docs.codehaus.org/display/JETTY/Securing+Passwords");
            commandLineOptions.AddOption(OPT_SQL, OPT_SQL_L, true, "Runs SQL or {call stored_procedure(?, ?)} or {?=call function(?, ?)}");
            commandLineOptions.AddOption(OPT_FILE_SFMD, "sfmd", true, "Writes a SFMD file for the given SQL");
            commandLineOptions.AddOption(OPT_FILE_BINDING, "jdbc", true, "Writes a JDBC binding node file for the given SQL");
            commandLineOptions.AddOption(OPT_FILE_JDBC, "node", true, "Writes a JDBC node file for the given SQL (internal debugging)");
            commandLineOptions.AddOption(OPT_WRITE_TO_FILE, "outfile", true, "Writes the SQL output to the given file");
            commandLineOptions.AddOption(OPT_DESCRIPTION, "description", true,
                    "SFMD description. A default description is used if omited. Example: " + OPT + OPT_DESCRIPTION + " \"Runs such and such\"");
            commandLineOptions.AddOption(OPT_INTERACTIVE, "interactive", false,
                    "Runs in interactive mode, reading and writing from the console, 'go' or '/' sends a statement");
            commandLineOptions.AddOption(OPT_TIMING, "printTiming", false, "Prints timing information");
            commandLineOptions.AddOption(OPT_METADATA, "printMetaData", false, "Prints metadata information");
            commandLineOptions.AddOption(OPT_STACK_TRACE, "printStack", false, "Prints stack traces on errors");
            
            Option option = new Option(OPT_COLUMN_NAMES, "columnNames", true, "Column XML names; default names column labels. Example: "
                + OPT
                + OPT_COLUMN_NAMES
                + " \"cname1 cname2\"");
            commandLineOptions.AddOption(option);
            
            option = new Option(OPT_PARAM_NAMES, "paramNames", true, "Parameter XML names; default names are param1, param2, etc. Example: "
                + OPT
                + OPT_PARAM_NAMES
                + " \"pname1 pname2\"");
            commandLineOptions.AddOption(option);
            
            //
            OptionGroup pOutTypesOptionGroup = new OptionGroup();
            string pOutTypesOptionGroupDoc = OPT + OPT_PARAM_TYPES_INT + " and " + OPT + OPT_PARAM_TYPES_NAME + " are mutually exclusive.";
            string typesClassName = typeof(DbType).FullName;
            
            option = new Option(OPT_PARAM_TYPES_INT, "paramTypes", true, "Parameter types from "
                + typesClassName
                + ". "
                + pOutTypesOptionGroupDoc
                + " Example: "
                + OPT
                + OPT_PARAM_TYPES_INT
                + " \"-10 12\"");
            
            commandLineOptions.AddOption(option);
            
            option = new Option(OPT_PARAM_TYPES_NAME, "paramTypeNames", true, "Parameter "
                + typesClassName
                + " names. "
                + pOutTypesOptionGroupDoc
                + " Example: "
                + OPT
                + OPT_PARAM_TYPES_NAME
                + " \"CURSOR VARCHAR\"");
            
            commandLineOptions.AddOption(option);
            commandLineOptions.AddOptionGroup(pOutTypesOptionGroup);
            
            //
            OptionGroup modesOptionGroup = new OptionGroup();
            string modesOptionGroupDoc = OPT + OPT_PARAM_MODES_INT + " and " + OPT + OPT_PARAM_MODES_NAME + " are mutually exclusive.";
            option = new Option(OPT_PARAM_MODES_INT, "paramModes", true, "Parameters modes ("
                + (int)ParameterDirection.Input
                + "=IN, "
                + (int)ParameterDirection.InputOutput
                + "=INOUT, "
                + (int)ParameterDirection.Output
                + "=OUT, "
                + (int)ParameterDirection.ReturnValue
                + "=RetVal "
                + "). "
                + modesOptionGroupDoc
                + " Example for 2 parameters, OUT and IN: "
                + OPT
                + OPT_PARAM_MODES_INT
                + " \""
                + (int)ParameterDirection.Output
                + " "
                + (int)ParameterDirection.Input
                + "\"");

            modesOptionGroup.AddOption(option);
            
            option = new Option(OPT_PARAM_MODES_NAME, "paramModeNames", true, "Parameters mode names ("
                + PMODES
                + "). "
                + modesOptionGroupDoc
                + " Example for 2 parameters, OUT and IN: "
                + OPT
                + OPT_PARAM_MODES_NAME
                + " \""
                + _PMODE_OUT
                + " "
                + _PMODE_IN
                + "\"");
            
            modesOptionGroup.AddOption(option);
            commandLineOptions.AddOptionGroup(modesOptionGroup);
            
            option = new Option(null, OPT_TRIM_L, true,
                    "Trims leading and trailing spaces from all column values. Column XML names can be optionally specified to set which columns to trim.");
            
            option.SetOptionalArg(true);
            commandLineOptions.AddOption(option);
            
            option = new Option(OPT_JDBC_TO_SFMD, OPT_JDBC_TO_SFMD_L, true,
                    "Converts the JDBC file in the first argument to an SMFD file specified in the second argument.");
            
            option.SetArgs(2);
            commandLineOptions.AddOption(option);

            formatter.PrintHelp(sw, HelpFormatter.DEFAULT_WIDTH, this.GetType().FullName, null, commandLineOptions, HelpFormatter.DEFAULT_LEFT_PAD, HelpFormatter.DEFAULT_DESC_PAD, null);
            
            string expected = "usage: Apache.Commons.Cli.Bug.BugCLI162Test" + CR +
                    " -2,--jdbc2sfmd <arg>        Converts the JDBC file in the first argument" + CR +
                    "                             to an SMFD file specified in the second" + CR +
                    "                             argument." + CR +
                    " -a,--paramNames <arg>       Parameter XML names; default names are" + CR +
                    "                             param1, param2, etc. Example: -a \"pname1" + CR +
                    "                             pname2\"" + CR +
                    " -b,--jdbc <arg>             Writes a JDBC binding node file for the given" + CR +
                    "                             SQL" + CR +
                    " -c,--url <arg>              Connection URL" + CR +
                    " -d,--driver <arg>           JDBC driver class name" + CR +
                    " -e,--description <arg>      SFMD description. A default description is" + CR +
                    "                             used if omited. Example: -e \"Runs such and" + CR +
                    "                             such\"" + CR +
                    " -f,--sfmd <arg>             Writes a SFMD file for the given SQL" + CR +
                    " -g,--printTiming            Prints timing information" + CR +
                    " -h,--help                   Prints help and quits" + CR +
                    " -i,--interactive            Runs in interactive mode, reading and writing" + CR +
                    "                             from the console, 'go' or '/' sends a" + CR +
                    "                             statement" + CR +
                    " -j,--node <arg>             Writes a JDBC node file for the given SQL" + CR +
                    "                             (internal debugging)" + CR +
                    " -l,--columnNames <arg>      Column XML names; default names column" + CR +
                    "                             labels. Example: -l \"cname1 cname2\"" + CR +
                    " -m,--printMetaData          Prints metadata information" + CR +
                    " -n,--info                   Prints driver information and properties. If" + CR +
                    "                             -c is not specified, all drivers on the" + CR +
                    "                             classpath are displayed." + CR +
                    " -o,--paramModes <arg>       Parameters modes (1=IN, 3=INOUT, 2=OUT," + CR +
                    "                             6=RetVal ). -o and -O are mutually exclusive." + CR +
                    "                             Example for 2 parameters, OUT and IN: -o \"2" + CR +
                    "                             1\"" + CR +
                    " -O,--paramModeNames <arg>   Parameters mode names (IN, INOUT, OUT," + CR +
                    "                             Unknown). -o and -O are mutually exclusive." + CR +
                    "                             Example for 2 parameters, OUT and IN: -O \"OUT" + CR +
                    "                             IN\"" + CR +
                    " -p,--password <arg>         The database password for the user specified" + CR +
                    "                             with the -u option. You can obfuscate the" + CR +
                    "                             password with" + CR +
                    "                             org.mortbay.jetty.security.Password, see" + CR +
                    "                             http://docs.codehaus.org/display/JETTY/Securi" + CR +
                    "                             ng+Passwords" + CR +
                    " -s,--sql <arg>              Runs SQL or {call stored_procedure(?, ?)} or" + CR +
                    "                             {?=call function(?, ?)}" + CR +
                    " -t,--printStack             Prints stack traces on errors" + CR +
                    "    --trim <arg>             Trims leading and trailing spaces from all" + CR +
                    "                             column values. Column XML names can be" + CR +
                    "                             optionally specified to set which columns to" + CR +
                    "                             trim." + CR +
                    " -u,--user <arg>             A database user name" + CR +
                    " -w,--outfile <arg>          Writes the SQL output to the given file" + CR +
                    " -y,--paramTypes <arg>       Parameter types from System.Data.DbType. -y" + CR +
                    "                             and -Y are mutually exclusive. Example: -y" + CR +
                    "                             \"-10 12\"" + CR +
                    " -Y,--paramTypeNames <arg>   Parameter System.Data.DbType names. -y and -Y" + CR +
                    "                             are mutually exclusive. Example: -Y \"CURSOR" + CR +
                    "                             VARCHAR\"" + CR;
            Assert.Equal(expected, sw.ToString());
        }


        [Fact]
        public void LongLineChunkingTest() {
            Options options = new Options();
            options.AddOption("x", "extralongarg", false,
                                         "This description has ReallyLongValuesThatAreLongerThanTheWidthOfTheColumns " +
                                         "and also other ReallyLongValuesThatAreHugerAndBiggerThanTheWidthOfTheColumnsBob, " +
                                         "yes. ");

            formatter.PrintHelp(sw, 35, this.GetType().FullName, "Header", options, 0, 5, "Footer");
            
            string expected = "usage:" + CR +
                              "       Apache.Commons.Cli.Bug.BugCL" + CR +
                              "       I162Test" + CR +
                              "Header" + CR +
                              "-x,--extralongarg     This" + CR +
                              "                      description" + CR +
                              "                      has" + CR +
                              "                      ReallyLongVal" + CR +
                              "                      uesThatAreLon" + CR +
                              "                      gerThanTheWid" + CR +
                              "                      thOfTheColumn" + CR +
                              "                      s and also" + CR +
                              "                      other" + CR +
                              "                      ReallyLongVal" + CR +
                              "                      uesThatAreHug" + CR +
                              "                      erAndBiggerTh" + CR +
                              "                      anTheWidthOfT" + CR +
                              "                      heColumnsBob," + CR +
                              "                      yes." + CR +
                              "Footer" + CR;
            Assert.Equal(expected, sw.ToString());
        }

        
        [Fact]
        public void LongLineChunkingIndentIgnoredTest() {
            Options options = new Options();
            options.AddOption("x", "extralongarg", false, "This description is Long." );

            formatter.PrintHelp(sw, 22, this.GetType().FullName, "Header", options, 0, 5, "Footer");
            
            string expected = "usage:" + CR +
                              "       Apache.Commons." + CR +
                              "       Cli.Bug.BugCLI1" + CR +
                              "       62Test" + CR +
                              "Header" + CR +
                              "-x,--extralongarg" + CR +
                              " This description is" + CR +
                              " Long." + CR +
                              "Footer" + CR;
            
            Assert.Equal(expected, sw.ToString());
        }
    }
}
