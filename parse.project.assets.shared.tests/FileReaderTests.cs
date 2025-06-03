using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using parse.project.assets.shared.Read;
using System.Collections.Generic;
using System.IO;
using System.Text;
using parse.project.assets.shared.Parse;

namespace parse.project.assets.shared.Tests
{
    [TestFixture]
    public class FileReaderTests
    {
        private FileReader _fileReader;
        private Mock<TextReader> _textReaderMock;
        private const string SampleJson = @"{""key"":""value""}";

        [SetUp]
        public void Setup()
        {
            _fileReader = new FileReader();
            _textReaderMock = new Mock<TextReader>();
        }

        [Test, Ignore("Not Baked")]
        public void ReadFileIntoJObject_ReturnsCorrectJObject()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleJson));
            var reader = new StreamReader(stream);
            _textReaderMock.Setup(tr => tr.ReadToEnd()).Returns(reader.ReadToEnd());

            // Act
            var result = _fileReader.ReadFileIntoJObject("dummyPath");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("value", result["key"].ToString());
        }

        [Test]
        public void CorrectTarget_ReturnsCorrectedName()
        {
            // Arrange
            var packages = new List<Package>
            {
                new Package("Newtonsoft.Json", "1.0"),
                new Package("Moq", "1.0")
            };

            // Act
            var correctedName = FileReader.CorrectTarget("newtonsoft.json", packages);

            // Assert
            Assert.AreEqual("Newtonsoft.Json", correctedName);
        }

        [Test]
        public void CorrectTarget_PackageNotFound()
        {
            // Arrange
            var packages = new List<Package>
            {
                new Package("Newtonsoft.Json", "1.0"),
                new Package("Moq", "1.0")
            };

            // Act
            var correctedName = FileReader.CorrectTarget("oldtonsoft.json", packages);

            // Assert
            Assert.AreEqual("oldtonsoft.json", correctedName);
        }

        // Generate a test function for when the package name is not found in the list of packages


        [Test]
        public void DotNetVersionSupported_ReturnsTrueIfSupported()
        {
            // Arrange
            var jsonContent = JObject.Parse(@"{
                ""projectFileDependencyGroups"": { ""net5.0"": [] },
                ""targets"": { ""net5.0"": {} }
            }");

            // Act
            var isSupported = _fileReader.DotNetVersionSupported("net5.0", jsonContent);

            // Assert
            Assert.IsTrue(isSupported);
        }
    }
}
