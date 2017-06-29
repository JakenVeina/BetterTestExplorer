using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.Common;
using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.CommonTests
{
    [TestFixture]
    public class TestCasesEventArgsTests
    {
        /**********************************************************************/
        #region Test Procedures

        private ITestCase MakeFakeTestCase()
        {
            var testCase = Substitute.For<ITestCase>();
            testCase.Id.Returns(Guid.NewGuid());

            return testCase;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor() Tests

        [Test]
        public void Constructor_Default_Always_SetsTestResultsByTestCaseIdToEmpty()
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
            var testCases = (IEnumerable<ITestCase>)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCasesEventArgs(testCases));

            Assert.AreEqual("testCases", result.ParamName);
        }

        [TestCase(3)]
        public void Constructor_TestCases_Otherwise_TestCasesByIdValuesIsEquivalentToTestCases(int testCaseCount)
        {
            var testResults = Enumerable.Range(1, testCaseCount)
                                        .Select(x => MakeFakeTestCase())
                                        .ToArray();

            var uut = new TestCasesEventArgs(testResults);

            var result = uut.TestCasesById.Values;

            CollectionAssert.AreEquivalent(testResults, result);
        }

        [TestCase(3)]
        public void Constructor_TestCases_Otherwise_TestCasesByIdEachKeyIsEqualToValueId(int testCaseCount)
        {
            var testResults = Enumerable.Range(1, testCaseCount)
                                        .Select(x => MakeFakeTestCase())
                                        .ToArray();

            var uut = new TestCasesEventArgs(testResults);

            var result = uut.TestCasesById;

            foreach (var testCasePair in result)
                Assert.AreEqual(testCasePair.Key, testCasePair.Value.Id);
        }

        #endregion Constructor(testCases) Tests
    }
}
