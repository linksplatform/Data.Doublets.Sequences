using Xunit;

namespace Platform.Data.Doublets.Sequences.Tests
{
    /// <summary>
    /// <para>
    /// Represents the links extensions tests.
    /// </para>
    /// <para></para>
    /// </summary>
    public class ILinksExtensionsTests
    {
        /// <summary>
        /// <para>
        /// Tests that format test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void FormatTest()
        {
            using (var scope = new TempLinksTestScope())
            {
                var links = scope.Links;
                var link = links.Create();
                var linkString = links.Format(link);
                Assert.Equal("(1: 1 1)", linkString);
            }
        }
    }
}
