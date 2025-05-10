using Discord_Clone.Server.Tests.Utilities;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.UnitTests
{
    public class Utilities
    {
        public Utilities()
        {

        }

        [Fact]
        public void ImageValidation_IsFileExtensionAllowed_AllowedExtension_ReturnsTrue()
        {
            // Arrange
            string basePath = AppDomain.CurrentDomain.BaseDirectory; // Gets the directory of the test assembly
            string filePath = Path.Combine(basePath, "Assets", "Images", "TestImage.png");

            IFormFile file;
            using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                file = new FormFile(stream, 0, stream.Length, "TestImage", "TestImage.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };


                // Act
                bool result = ImageValidator.IsFileExtensionAllowed(file);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void ImageValidation_IsFileExtensionAllowed_DisallowedExtension_ReturnsFalse()
        {
            //Arrange
            string basePath = AppDomain.CurrentDomain.BaseDirectory; // Gets the directory of the test assembly
            string filePath = Path.Combine(basePath, "Assets", "Images", "FakeImage.png");

            IFormFile file;
            using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                file = new FormFile(stream, 0, stream.Length, "FakeImage", "FakeImage.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };


                //Act
                bool result = ImageValidator.IsFileExtensionAllowed(file);

                //Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void ImageValidation_IsFileSizeWithinRange_WithinRange_ReturnsTrue()
        {
            // Arrange
            string basePath = AppDomain.CurrentDomain.BaseDirectory; // Gets the directory of the test assembly
            string filePath = Path.Combine(basePath, "Assets", "Images", "TestImage.png");

            IFormFile file;
            using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                file = new FormFile(stream, 0, stream.Length, "TestImage", "TestImage.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };


                // Act
                bool result = ImageValidator.IsFileSizeWithinRange(file, 4097); //Allow up to 4KB

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void ImageValidation_IsFileSizeWithinRange_OutsideRange_ReturnsFalse()
        {
            string largeBasePath = AppDomain.CurrentDomain.BaseDirectory; // Gets the directory of the test assembly
            string largeFilePath = Path.Combine(largeBasePath, "Assets", "Images", "TestImage.png");
            string smallBasePath = AppDomain.CurrentDomain.BaseDirectory; // Gets the directory of the test assembly
            string smallFilePath = Path.Combine(smallBasePath, "Assets", "Images", "FakeImage.png");

            IFormFile largeFile;
            using (FileStream stream = new(largeFilePath, FileMode.Open, FileAccess.Read))
            {
                largeFile = new FormFile(stream, 0, stream.Length, "TestImage", "TestImage.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };


                bool result = ImageValidator.IsFileSizeWithinRange(largeFile, 5); //5 bytes to test if file is too large.

                Assert.False(result);
            }

            IFormFile smallFile;
            using (FileStream stream = new(smallFilePath, FileMode.Open, FileAccess.Read))
            {
                smallFile = new FormFile(stream, 0, stream.Length, "FakeImage", "FakeImage.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };

                bool result = ImageValidator.IsFileExtensionAllowed(smallFile);

                Assert.False(result);
            }
        }
    }
}
