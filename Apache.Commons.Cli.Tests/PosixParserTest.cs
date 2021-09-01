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

    using Xunit;

    /**
     * Test case for the PosixParser.
     */
    public class PosixParserTest : ParserTestCase {
        
        public PosixParserTest()
            : base() { 
            
            parser = new PosixParser();
        }

        [Fact(Skip ="not supported by the PosixParser")]
        public override void DoubleDash2Test() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void LongWithoutEqualSingleDashTest() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void AmbiguousLongWithoutEqualSingleDashTest() { }


        [Fact(Skip ="not supported by the PosixParser (CLI-184)")]
        public override void NegativeOptionTest() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void LongWithUnexpectedArgument1Test() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void LongWithEqualSingleDashTest() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void ShortWithEqualTest() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void UnambiguousPartialLongOption4Test() { }


        [Fact(Skip ="not supported by the PosixParser")]
        public override void AmbiguousPartialLongOption4Test() { }
    }
}
