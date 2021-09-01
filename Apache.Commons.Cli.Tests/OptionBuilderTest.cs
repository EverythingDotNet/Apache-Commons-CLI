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

    using Xunit;

    public class OptionBuilderTest {

        [Fact]
        public void CompleteOptionTest( ) {
            Option simple = OptionBuilder.WithLongOpt( "simple option")
                                         .HasArg( )
                                         .IsRequired( )
                                         .HasArgs( )
                                         .WithOptionType( typeof(float) )
                                         .WithDescription( "this is a simple option" )
                                         .Create( 's' );

            Assert.Equal( "s", simple.GetOpt() );
            Assert.Equal( "simple option", simple.GetLongOpt() );
            Assert.Equal( "this is a simple option", simple.GetDescription() );
            Assert.Equal(typeof(float), simple.GetOptionType());
            Assert.True( simple.HasArg() );
            Assert.True( simple.IsRequired() );
            Assert.True( simple.HasArgs() );
        }


        [Fact]
        public void TwoCompleteOptionsTest( ) {
            Option simple = OptionBuilder.WithLongOpt( "simple option")
                                         .HasArg( )
                                         .IsRequired( )
                                         .HasArgs( )
                                         .WithOptionType( typeof(float) )
                                         .WithDescription( "this is a simple option" )
                                         .Create( 's' );

            Assert.Equal( "s", simple.GetOpt() );
            Assert.Equal( "simple option", simple.GetLongOpt() );
            Assert.Equal( "this is a simple option", simple.GetDescription() );
            Assert.Equal(typeof(float), simple.GetOptionType());
            Assert.True( simple.HasArg() );
            Assert.True( simple.IsRequired() );
            Assert.True( simple.HasArgs() );

            simple = OptionBuilder.WithLongOpt( "dimple option")
                                  .HasArg( )
                                  .WithDescription( "this is a dimple option" )
                                  .Create( 'd' );

            Assert.Equal( "d", simple.GetOpt() );
            Assert.Equal( "dimple option", simple.GetLongOpt() );
            Assert.Equal( "this is a dimple option", simple.GetDescription() );
            Assert.Equal(typeof(string), simple.GetOptionType());
            Assert.True( simple.HasArg() );
            Assert.True( !simple.IsRequired() );
            Assert.True( !simple.HasArgs() );
        }


        [Fact]
        public void BaseOptionCharOptTest() {
            Option baseOption = OptionBuilder.WithDescription( "option description")
                                       .Create( 'o' );

            Assert.Equal( "o", baseOption.GetOpt() );
            Assert.Equal( "option description", baseOption.GetDescription() );
            Assert.True( !baseOption.HasArg() );
        }


        [Fact]
        public void BaseOptionStringOptTest() {
            Option baseOption = OptionBuilder.WithDescription( "option description")
                                       .Create( "o" );

            Assert.Equal( "o", baseOption.GetOpt() );
            Assert.Equal( "option description", baseOption.GetDescription() );
            Assert.True( !baseOption.HasArg() );
        }

        
        [Fact]
        public void SpecialOptCharsTest() {
            // '?'
            Option opt1 = OptionBuilder.WithDescription("help options").Create('?');
            Assert.Equal("?", opt1.GetOpt());

            // '@'
            Option opt2 = OptionBuilder.WithDescription("read from stdin").Create('@');
            Assert.Equal("@", opt2.GetOpt());

            // ' '
            Assert.Throws<ArgumentException>(() => OptionBuilder.Create(' '));
        }


        [Fact]
        public void OptionArgNumbersTest() {
            Option opt = OptionBuilder.WithDescription( "option description" )
                                      .HasArgs( 2 )
                                      .Create( 'o' );
            
            Assert.Equal( 2, opt.GetArgs() );
        }


        [Fact]
        public void IllegalOptionsTest() {
            // bad single character option
            Assert.Throws<ArgumentException>(() => OptionBuilder.WithDescription("option description").Create('"'));

            // bad character in option string
            Assert.Throws<ArgumentException>(() => OptionBuilder.Create( "opt`" ));

            // valid option
            OptionBuilder.Create( "opt" );
        }

        
        [Fact]
        public void CreateIncompleteOptionTest() {

            Assert.Throws<ArgumentException>(() => OptionBuilder.HasArg().Create());

            // implicitly reset the builder
            OptionBuilder.Create( "opt" );
        }


        [Fact]
        public void BuilderIsResettedAlwaysTest() {
            Assert.Throws<ArgumentException>(() => OptionBuilder.WithDescription("XUnit").Create('"'));

            Assert.Null(OptionBuilder.Create('x').GetDescription());

            Assert.Throws<ArgumentException>(() => OptionBuilder.WithDescription("XUnit").Create());
            Assert.Null(OptionBuilder.Create('x').GetDescription());
        }
    }
}
