using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MailPreview.Tests
{
    public class MailFolderTests
    {
        private readonly ITestOutputHelper _output;

        public MailFolderTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void CanGetMailFromFolder()
        {
            var testDir = new DirectoryInfo(@"D:\Mail\samples");
            var folder = new MailFolder(testDir);
            var items = folder.GetMailItems();
            Assert.Equal(items.Count, testDir.GetFiles("*.eml").Length);
            Assert.True(items.Any());
        }
    }
}
