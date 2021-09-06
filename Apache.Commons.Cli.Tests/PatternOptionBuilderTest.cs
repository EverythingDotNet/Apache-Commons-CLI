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
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using Xunit;

    /**
     * Test case for the PatternOptionBuilder class.
     */
    public class PatternOptionBuilderTest {

        [Fact]
        public void SimplePatternTest() {
            Options options = PatternOptionBuilder.ParsePattern("a:b@cde>f+n%t/m*z#");
            string[] args = new string[] {
                "-c", 
                "-a", "foo", 
                "-b", "System.Collections.ArrayList", 
                "-e", "build.xml", 
                "-f", "System.Globalization.Calendar", 
                "-n", "4.5", 
                "-t", "https://commons.apache.org", 
                "-z", "Thu Jun 06 17:48:57 EDT 2002", 
                "-m", "test*"
            };

            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, args);

            Assert.Equal("foo", line.GetOptionValue("a"));
            Assert.Equal("foo", line.GetOptionObject("a"));
            Assert.Equal(new ArrayList(), line.GetOptionObject("b"));
            Assert.True(line.HasOption("c"), "boolean true flag c");
            Assert.False(line.HasOption("d"), "boolean false flag d");
            Assert.Equal(typeof(FileInfo), line.GetOptionObject('e').GetType());
            Assert.Equal(new FileInfo("build.xml").ToString(), line.GetOptionObject("e").ToString());
            Assert.Equal(typeof(Calendar), line.GetOptionObject("f"));
            Assert.Equal((double)4.5, line.GetOptionObject("n"));
            Assert.Equal(new Uri("https://commons.apache.org"), line.GetOptionObject("t"));

            // tests the char methods of CommandLine that delegate to the String methods
            Assert.Equal("foo", line.GetOptionValue('a'));
            Assert.Equal("foo", line.GetOptionObject('a'));
            Assert.Equal(new ArrayList(), line.GetOptionObject('b'));
            Assert.True(line.HasOption('c'), "boolean true flag c");
            Assert.False(line.HasOption('d'), "boolean false flag d");
            Assert.Equal(typeof(FileInfo), line.GetOptionObject('e').GetType());
            Assert.Equal(new FileInfo("build.xml").ToString(), line.GetOptionObject('e').ToString());
            Assert.Equal(typeof(Calendar), line.GetOptionObject('f'));
            Assert.Equal((double)4.5, line.GetOptionObject('n'));
            Assert.Equal(new Uri("https://commons.apache.org"), line.GetOptionObject('t'));

            // FILES NOT SUPPORTED YET
            Assert.Throws<NotImplementedException>(() => Assert.Equal(new FileInfo[0], line.GetOptionObject('m')));

            // DATES NOT SUPPORTED YET
            Assert.Throws<NotImplementedException>(() => Assert.Equal(new DateTime(1023400137276L), line.GetOptionObject('z')));
        }


        [Fact]
        public void EmptyPatternTest() {
            Options options = PatternOptionBuilder.ParsePattern("");
            Assert.True(options.GetOptions().Count == 0);
        }

        [Fact]
        public void UntypedPatternTest()
        {
            Options options = PatternOptionBuilder.ParsePattern("abc");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-abc" });

            Assert.True(line.HasOption('a'));
            Assert.Null(line.GetOptionObject('a'));
            Assert.True(line.HasOption('b'));
            Assert.Null(line.GetOptionObject('b'));
            Assert.True(line.HasOption('c'));
            Assert.Null(line.GetOptionObject('c'));
        }


        [Fact]
        public void ValueTypePatternTest()
        {
            Options options = PatternOptionBuilder.ParsePattern("n%d%x%");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-n", "1", "-d", "2.1", "-x", "3,5" });

            Assert.Equal(typeof(long), line.GetOptionObject("n").GetType());
            Assert.Equal((long)1, line.GetOptionObject("n"));

            Assert.Equal(typeof(double), line.GetOptionObject("d").GetType());
            Assert.Equal((double)2.1, line.GetOptionObject("d"));

            Assert.Null(line.GetOptionObject("x"));
        }


        [Fact]
        public void TypePatternTest() {
            Options options = PatternOptionBuilder.ParsePattern("c+d+e+");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-c", "System.Globalization.Calendar", "-d", "System.DateTime", "-e", "System.ClassNotExisting" });

            Assert.Equal(typeof(Calendar), line.GetOptionObject("c"));
            Assert.Equal(typeof(DateTime), line.GetOptionObject("d"));
            Assert.Null(line.GetOptionObject("e"));
        }


        [Fact]
        public void ObjectPatternTest() {
            Options options = PatternOptionBuilder.ParsePattern("o@i@n@");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-o", "System.String", "-i", "System.Globalization.Calendar", "-n", "System.DateTime" });

            Assert.Equal("", line.GetOptionObject("o"));
            Assert.Null(line.GetOptionObject("i"));
            Assert.Null(line.GetOptionObject("n"));
        }


        [Fact]
        public void UriPatternTest()
        {
            Options options = PatternOptionBuilder.ParsePattern("u/v/");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-u", "https://commons.apache.org", "-v", "foo://commons.apache.org" });

            Assert.Equal(new Uri("https://commons.apache.org"), line.GetOptionObject("u"));
            Assert.Null(line.GetOptionObject("v"));
        }


        [Fact]
        public void ExistingFilePatternTest() {
            Options options = PatternOptionBuilder.ParsePattern("g<");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-g", @"..\..\..\Resources\existing-readable.file" });

            object parsedReadableFileStream = line.GetOptionObject("g");

            Assert.NotNull(parsedReadableFileStream);
            Assert.True(parsedReadableFileStream is FileStream, "option g not FileStream");
        }


        [Fact]
        public void ExistingFilePatternFileNotExistTest() {
            Options options = PatternOptionBuilder.ParsePattern("f<");
            ICommandLineParser parser = new PosixParser();
            CommandLine line = parser.Parse(options, new string[] { "-f", "non-existing.file" });

            Assert.Null(line.GetOptionObject("f"));
        }


        [Fact]
        public void RequiredOptionTest() {
            Options options = PatternOptionBuilder.ParsePattern("!n%m%");
            ICommandLineParser parser = new PosixParser();

            MissingOptionException e = Assert.Throws<MissingOptionException>(() => parser.Parse(options, new string[] { "" }));

            Assert.Equal(1, e.MissingOptions.Count);
            Assert.True(e.MissingOptions.Contains("n"));
        }
    }
}
