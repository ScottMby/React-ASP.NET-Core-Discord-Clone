using Discord_Clone.Server.Tests.Utilities;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
            //Arrange
            IFormFile file;
            using (Bitmap b = new Bitmap(50, 50)) //Create an actual png file
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(Color.Green);
                }

                using (var stream = new MemoryStream())
                {
                    b.Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    file = new FormFile(stream, 0, stream.Length, "file", "green.png");

                    //Act
                    bool result = ImageValidator.IsFileExtensionAllowed(file);
                    //Assert
                    Assert.True(result);
                }
            }
        }

        [Fact]
        public void ImageValidation_IsFileExtensionAllowed_DisallowedExtension_ReturnsFalse()
        {
            //Arrange
            IFormFile file = FileHelper.CreateFormFile("text.png", "test");
            //Act
            bool result = ImageValidator.IsFileExtensionAllowed(file);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public void ImageValidation_IsFileSizeWithinRange_WithinRange_ReturnsTrue()
        {

        }

        [Fact]
        public void ImageValidation_IsFileSizeWithinRange_OutsideRange_ReturnsFalse()
        {

        }
    }
}
