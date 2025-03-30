using FileSignatures;
using FileSignatures.Formats;

namespace Discord_Clone.Server.Utilities
{
    public static class ImageValidator
    {
        private static readonly FileFormatInspector fileFormatInspector = new();

        /// <summary>
        /// Validates the type of file is an image with its magic number.
        /// </summary>
        /// <param name="file">The file to validate.</param>
        /// <returns>True if the file is an image, else false.</returns>
        public static bool IsFileExtensionAllowed(IFormFile file)
        {
            FileFormat? format = fileFormatInspector.DetermineFileFormat(file.OpenReadStream());

            if (format != null && format is Image)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the file is smaller or equal to a max file size.
        /// </summary>
        /// <param name="file">The file to validate.</param>
        /// <param name="maxSizeInBytes">The max file size in bytes.</param>
        /// <returns>True if the file size is within the required range.</returns>
        public static bool IsFileSizeWithinRange(IFormFile file, long maxSizeInBytes)
        {
            return file.Length <= maxSizeInBytes;
        }
    }
}
