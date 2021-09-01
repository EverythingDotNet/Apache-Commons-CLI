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

    public class BasicParserTest : ParserTestCase {

        public BasicParserTest()
            : base() {
                parser = new BasicParser();
        }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void DoubleDash2Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void LongWithoutEqualSingleDashTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void AmbiguousLongWithoutEqualSingleDashTest() { }


        [Fact(Skip = "not supported by the BasicParser (CLI-184)")]
        public override void NegativeOptionTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void PropertiesOption1Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void PropertiesOption2Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void ShortWithEqualTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void ShortWithoutEqualTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void LongWithEqualDoubleDashTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void LongWithEqualSingleDashTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void UnambiguousPartialLongOption1Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void UnambiguousPartialLongOption2Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void UnambiguousPartialLongOption3Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void UnambiguousPartialLongOption4Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void AmbiguousPartialLongOption1Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void AmbiguousPartialLongOption2Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void AmbiguousPartialLongOption3Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void AmbiguousPartialLongOption4Test() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void PartialLongOptionSingleDashTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void BurstingTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void UnrecognizedOptionWithBurstingTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void MissingArgWithBurstingTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void StopBurstingTest() { }


        [Fact(Skip = "not supported by the BasicParser")]
        public override void StopBursting2Test() { }
    }
}
