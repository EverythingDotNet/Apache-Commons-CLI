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

    public class UtilTest {
        
        [Fact]
        public void StripLeadingHyphensTest() {
            Assert.Equal("f", Util.StripLeadingHyphens("-f"));
            Assert.Equal("foo", Util.StripLeadingHyphens("--foo"));
            Assert.Equal("-foo", Util.StripLeadingHyphens("---foo"));
            Assert.Null(Util.StripLeadingHyphens(null));
        }

        
        [Fact]
        public void StripLeadingAndTrailingQuotesTest() {
            Assert.Equal("foo", Util.StripLeadingAndTrailingQuotes("\"foo\""));
            Assert.Equal("foo \"bar\"", Util.StripLeadingAndTrailingQuotes("foo \"bar\""));
            Assert.Equal("\"foo\" bar", Util.StripLeadingAndTrailingQuotes("\"foo\" bar"));
            Assert.Equal("\"foo\" and \"bar\"", Util.StripLeadingAndTrailingQuotes("\"foo\" and \"bar\""));
            Assert.Equal("\"", Util.StripLeadingAndTrailingQuotes("\""));
        }
    }
}
