using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    [TestFixture]
    public class TestResultsEventArgsTests
    {
        /**********************************************************************/
        #region Test Procedures

        private ITestCase MakeFakeTestCase()
        {
            var testCase = Substitute.For<ITestCase>();
            testCase.Id.Returns(Guid.NewGuid());

            return testCase;
        }

        private ITestResult MakeFakeTestResult(ITestCase testCase)
        {
            var testResult = Substitute.For<ITestResult>();
            testResult.TestCase.Returns(testCase);

            return testResult;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor() Tests

        [Test]
        public void Constructor_Default_Always_SetsTestResultsByTestCaseIdToEmpty()
        {
            var uut = new TestResultsEventArgs();

            var result = uut.TestResultsByTestCaseId;

            CollectionAssert.IsEmpty(result);
        }

        #endregion Constructor() Tests

        /**********************************************************************/
        #region Constructor(testResults) Tests

        [Test]
        public void Constructor_TestResults_TestResultsIsNull_ThrowsException()
        {
            var testResults = (IEnumerable<ITestResult>)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultsEventArgs(testResults));

            Assert.AreEqual("testResults", result.ParamName);
        }

        [TestCase(3)]
        public void Constructor_TestResults_Otherwise_TestResultsByTestCaseIdValuesIsEquivalentToTestResults(int testCaseCount)
        {
            var testResults = Enumerable.Range(1, testCaseCount)
                                        .Select(x => MakeFakeTestCase())
                                        .Select(x => MakeFakeTestResult(x))
                                        .ToArray();

            var uut = new TestResultsEventArgs(testResults);

            var result = uut.TestResultsByTestCaseId.Values;

            CollectionAssert.AreEquivalent(testResults, result);
        }

        [TestCase(3)]
        public void Constructor_TestResults_Otherwise_TestResultsByTestCaseIdEachKeyIsEqualToValueTestCaseId(int testCaseCount)
        {
            var testResults = Enumerable.Range(1, testCaseCount)
                                        .Select(x => MakeFakeTestCase())
                                        .Select(x => MakeFakeTestResult(x))
                                        .ToArray();

            var uut = new TestResultsEventArgs(testResults);

            var result = uut.TestResultsByTestCaseId;

            foreach (var testResultPair in result)
                Assert.AreEqual(testResultPair.Key, testResultPair.Value.TestCase.Id);
        }

        #endregion Constructor(testResults) Tests
    }
}
