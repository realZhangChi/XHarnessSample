using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XHarnessSample.Platforms.Android.tests
{
    public class Simple_Tests
    {
        [Fact]
        public void Should_Success()
        {
            Assert.Equal(1, 1);
        }

        [Fact]
        public void Should_Fail()
        {
            Assert.Equal(0, 1);
        }
    }
}
