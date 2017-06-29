using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    [TestFixture]
    public class TestCaseWrapperTests
    {
        /**********************************************************************/
        #region Test Procedures

        private static TestCase MakeTestCase()
        {
            var testCase = new TestCase();

            testCase.Id = Guid.NewGuid();
            testCase.DisplayName = "DisplayName";
            testCase.FullyQualifiedName = "Namespace.Class.DisplayName";
            testCase.Source = "Source";
            testCase.CodeFilePath = "CodeFilePath";
            testCase.LineNumber = 1;
            testCase.ExecutorUri = new Uri("uri://executor");

            return testCase;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_TestCaseIsNull_ThrowsException()
        {
            var testCase = (TestCase)null;

            var result = Assert.Throws<ArgumentNullException>(() => new ReadOnlyTestCase(testCase));

            Assert.AreEqual("testCase", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsIdToTestCaseId()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.Id;

            Assert.AreEqual(testCase.Id, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDisplayNameToTestCaseDisplayName()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.DisplayName;

            Assert.AreEqual(testCase.DisplayName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsFullyQualifiedNameToTestCaseFullyQualifiedName()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.DisplayName;

            Assert.AreEqual(testCase.DisplayName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceToTestCaseSource()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.Source;

            Assert.AreEqual(testCase.Source, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsCodeFilePathToTestCaseCodeFilePath()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.CodeFilePath;

            Assert.AreEqual(testCase.CodeFilePath, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsLineNumberToTestCaseLineNumber()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.LineNumber;

            Assert.AreEqual(testCase.LineNumber, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsExecutorUriToTestCaseExecutorUri()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.ExecutorUri;

            Assert.AreEqual(testCase.ExecutorUri, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsTraitsToTestCaseTraits()
        {
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestCase(testCase);

            var result = uut.Traits;

            Assert.AreSame(testCase.Traits, result);
        }

        #endregion Constructor Tests
    }
}
