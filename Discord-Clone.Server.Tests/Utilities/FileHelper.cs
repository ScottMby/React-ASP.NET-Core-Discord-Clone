using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace Discord_Clone.Server.Tests.Utilities
{
    public static class FileHelper
    {
        public static IFormFile CreateFormFile(string fileName, string content, string contentType = "text/plain")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}