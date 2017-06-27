using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestCase : ITestObject
    {
        Guid Id { get; }

        string DisplayName { get; }

        string Class { get; }

        string Namespace { get; }

        string SourceAssemblyPath { get; }

        string SourceFilePath { get; }

        int SourceFileLine { get; }

        Uri ExecutorUri { get; }
    }

    public class TestCase : TestObject, ITestCase
    {
        /**********************************************************************/
        #region Constructors

        internal TestCase(VsTestPlatform.ObjectModel.TestCase vsTestCase) : base(vsTestCase)
        {
            Id = vsTestCase.Id;
            DisplayName = vsTestCase.DisplayName;
            SourceAssemblyPath = vsTestCase.Source;
            SourceFilePath = vsTestCase.CodeFilePath;
            SourceFileLine = vsTestCase.LineNumber;
            ExecutorUri = vsTestCase.ExecutorUri;

            var methodNameIndex = vsTestCase.FullyQualifiedName.LastIndexOf(vsTestCase.DisplayName);
            var namespaceAndClass = vsTestCase.FullyQualifiedName.Substring(0, methodNameIndex + 1);
            var classSeparatorIndex = namespaceAndClass.LastIndexOf('.');

            Class = namespaceAndClass.Substring(classSeparatorIndex + 1);
            Namespace = namespaceAndClass.Substring(0, classSeparatorIndex + 1);
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCase

        public Guid Id { get; }

        public string DisplayName { get; }

        public string Class { get; }

        public string Namespace { get; }

        public string SourceAssemblyPath { get; }

        public string SourceFilePath { get; }

        public int SourceFileLine { get; }

        public Uri ExecutorUri { get; }

        #endregion ITestCase
    }
}
