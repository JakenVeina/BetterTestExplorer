using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    [TestFixture]
    public class TestCaseTests
    {
        /**********************************************************************/
        #region Test Procedures

        private static VsTestPlatform.TestCase MakeVsTestCase()
        {
            var vsTestCase = new VsTestPlatform.TestCase();

            vsTestCase.Id = Guid.NewGuid();
            vsTestCase.DisplayName = "DisplayName";
            vsTestCase.FullyQualifiedName = "Namespace.Class.DisplayName";
            vsTestCase.Source = "Source";
            vsTestCase.CodeFilePath = "CodeFilePath";
            vsTestCase.LineNumber = 1;
            vsTestCase.ExecutorUri = new Uri("uri://executor");

            return vsTestCase;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_VsTestCaseIsNull_ThrowsException()
        {
            var vsTestCase = (VsTestPlatform.TestCase)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCase(vsTestCase));

            Assert.AreEqual("vsTestCase", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsIdToVsTestCaseId()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.Id;

            Assert.AreEqual(vsTestCase.Id, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDisplayNameToVsTestCaseDisplayName()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.DisplayName;

            Assert.AreEqual(vsTestCase.DisplayName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceAssemblyPathToVsTestCaseSource()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.SourceAssemblyPath;

            Assert.AreEqual(vsTestCase.Source, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceFilePathToVsTestCaseCodeFilePath()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.SourceFilePath;

            Assert.AreEqual(vsTestCase.CodeFilePath, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceFileLineToVsTestCaseLineNumber()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.SourceFileLine;

            Assert.AreEqual(vsTestCase.LineNumber, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsExecutorUriToVsTestCaseExecutorUri()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.ExecutorUri;

            Assert.AreEqual(vsTestCase.ExecutorUri, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsTraitsToVsTestCaseTraits()
        {
            var vsTestCase = MakeVsTestCase();

            var uut = new TestCase(vsTestCase);

            var result = uut.Traits;

            Assert.AreSame(vsTestCase.Traits, result);
        }

        [TestCase("Namespace.Class.Method", "Method", "Class")]
        [TestCase("Namespace1.Namespace2.Class.Method", "Method",  "Class")]
        [TestCase("Namespace.Class.Method(\"A.B\")", "Method(\"A.B\")", "Class")]
        public void Constructor_Otherwise_SetsClassFromVsTestCaseFullyQualifiedNameAsExpected(string fullyQualifiedName, string displayName, string className)
        {
            var vsTestCase = MakeVsTestCase();
            vsTestCase.FullyQualifiedName = fullyQualifiedName;
            vsTestCase.DisplayName = displayName;

            var uut = new TestCase(vsTestCase);

            var result = uut.ClassName;

            Assert.AreEqual(className, result);
        }

        [TestCase("Namespace.Class.Method", "Method", "Namespace")]
        [TestCase("Namespace1.Namespace2.Class.Method", "Method", "Namespace1.Namespace2")]
        [TestCase("Namespace.Class.Method(\"A.B\")", "Method(\"A.B\")", "Namespace")]
        public void Constructor_Otherwise_SetsNamespaceFromVsTestCaseFullyQualifiedNameAsExpected(string fullyQualifiedName, string displayName, string namespaceName)
        {
            var vsTestCase = MakeVsTestCase();
            vsTestCase.FullyQualifiedName = fullyQualifiedName;
            vsTestCase.DisplayName = displayName;

            var uut = new TestCase(vsTestCase);

            var result = uut.NamespaceName;

            Assert.AreEqual(namespaceName, result);
        }

        #endregion Constructor Tests
    }
}
