using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.Common;

namespace BetterTestExplorerTests.CommonTests
{
    [TestFixture]
    public class TestCasesEventArgsTests
    {
        /**********************************************************************/
        #region Constructor() Tests

        [Test]
        public void Constructor_Default_Always_SetsTestCasesByIdToEmpty()
        {
            var uut = new TestCasesEventArgs();

            var result = uut.TestCasesById;

            CollectionAssert.IsEmpty(result);
        }

        #endregion Constructor() Tests

        /**********************************************************************/
        #region Constructor(testCases) Tests

        [Test]
        public void Constructor_TestCases_TestCasesIsNull_ThrowsException()
        {
            var testCases = (IEnumerable<TestCase>)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCasesEventArgs(testCases));

            Assert.AreEqual("testCases", result.ParamName);
        }

        [TestCase(3)]
        public void Constructor_TestCases_Otherwise_TestCasesByIdValuesIsEquivalentToTestCases(int testCaseCount)
        {
            var testCases = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();

            var uut = new TestCasesEventArgs(testCases);

            var result = uut.TestCasesById.Values;

            CollectionAssert.AreEquivalent(testCases, result);
        }

        [TestCase(3)]
        public void Constructor_TestCases_Otherwise_TestCasesByIdEachKeyIsEqualToValueId(int testCaseCount)
        {
            var testCases = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() } ).ToArray();

            var uut = new TestCasesEventArgs(testCases);

            var result = uut.TestCasesById;

            foreach (var testCasePair in result)
                Assert.AreEqual(testCasePair.Key, testCasePair.Value.Id);
        }

        #endregion Constructor(testCases) Tests
    }
}
