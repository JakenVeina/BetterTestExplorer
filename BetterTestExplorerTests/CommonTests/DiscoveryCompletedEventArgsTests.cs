using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.Common;

namespace BetterTestExplorerTests.CommonTests
{
    [TestFixture]
    public class DiscoveryCompletedEventArgsTests
    {
        /**********************************************************************/
        #region Constructor() Tests

        [Test]
        public void Constructor_Default_Always_SetsSourceAssemblyPathsToEmpty()
        {
            var uut = new DiscoveryCompletedEventArgs();

            var result = uut.SourceAssemblyPaths;

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Constructor_Default_Always_SetsWasDiscoveryAbortedToFalse()
        {
            var uut = new DiscoveryCompletedEventArgs();

            var result = uut.WasDiscoveryAborted;
        }

        #endregion Constructor() Tests

        /**********************************************************************/
        #region Constructor(sourceAssemblyPaths, wasDiscoveryAborted) Tests

        [Test]
        public void Constructor_SourceAssemblyPathsWAsDiscoveryAborted_SourceAssemblyPathsIsNull_ThrowsException()
        {
            var sourceAssemblyPaths = (IEnumerable<string>)null;
            var wasDiscoveryAborted = true;

            var result = Assert.Throws<ArgumentNullException>(() => new DiscoveryCompletedEventArgs(sourceAssemblyPaths, wasDiscoveryAborted));

            Assert.AreEqual("sourceAssemblyPaths", result.ParamName);
        }

        [TestCase(3)]
        public void Constructor_SourceAssemblyPathsWasDiscoveryAborted_Otherwise_SourceAssemblyPathsIsEquivalentToGiven(int sourceAssemblyPathCount)
        {
            var sourceAssemblyPaths = Enumerable.Range(1, sourceAssemblyPathCount).Select(x => x.ToString());
            var wasDiscoveryAborted = true;

            var uut = new DiscoveryCompletedEventArgs(sourceAssemblyPaths, wasDiscoveryAborted);

            var result = uut.SourceAssemblyPaths;

            CollectionAssert.AreEquivalent(sourceAssemblyPaths, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_SourceAssemblyPathsWasDiscoveryAborted_Otherwise_SetsWasDiscoveryAbortedToGiven(bool wasDiscoveryAborted)
        {
            var sourceAssemblyPaths = Enumerable.Empty<string>();

            var uut = new DiscoveryCompletedEventArgs(sourceAssemblyPaths, wasDiscoveryAborted);

            var result = uut.WasDiscoveryAborted;

            Assert.AreEqual(wasDiscoveryAborted, result);
        }

        #endregion Constructor(sourceAssemblyPaths, wasDiscoveryAborted) Tests
    }
}
