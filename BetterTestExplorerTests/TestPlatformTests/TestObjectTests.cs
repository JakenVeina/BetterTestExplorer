using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    [TestFixture]
    public class TestObjectTests
    {
        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_VsTestObjectIsNull_ThrowsException()
        {
            var vsTestObject = (VsTestPlatform.TestCase)null;

            var result = Assert.Throws<TargetInvocationException>(() => Substitute.ForPartsOf<TestObject>(vsTestObject)).InnerException;

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual("vsTestObject", ((ArgumentNullException)result).ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsTraitsToVsTestObjectTraits()
        {
            var vsTestObject = new VsTestPlatform.TestCase();

            var uut = Substitute.ForPartsOf<TestObject>(vsTestObject);

            var result = uut.Traits;

            Assert.AreSame(vsTestObject.Traits, result);
        }

        #endregion Constructor Tests
    }
}
