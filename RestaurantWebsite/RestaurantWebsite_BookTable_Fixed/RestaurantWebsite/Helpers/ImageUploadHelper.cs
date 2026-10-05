using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RestaurantWebsite.Helpers
{
    public static class ImageUploadHelper
    {
        public const int MaxFileSizeBytes = 5 * 1024 * 1024;
        public const int MaxDishImages = 10;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public static List<string> ValidateFiles(
            IEnumerable<HttpPostedFileBase> files,
            int maxFiles = MaxDishImages)
        {
            var errors = new List<string>();

            var validFiles = (files ?? Enumerable.Empty<HttpPostedFileBase>())
                .Where(f => f != null && f.ContentLength > 0)
                .ToList();

            if (validFiles.Count > maxFiles)
            {
                errors.Add("You can upload up to " + maxFiles + " images at a time.");
            }

            for (int i = 0; i < validFiles.Count; i++)
            {
                var file = validFiles[i];
                string extension = Path.GetExtension(file.FileName ?? string.Empty)
                    .ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                {
                    errors.Add(
                        "File '" +
                        Path.GetFileName(file.FileName) +
                        "' is not a supported image. Use JPG, JPEG, PNG or WEBP.");
                }

                if (file.ContentLength > MaxFileSizeBytes)
                {
                    errors.Add(
                        "File '" +
                        Path.GetFileName(file.FileName) +
                        "' exceeds the 5 MB limit.");
                }
            }

            return errors;
        }

        public static List<string> SaveImages(
            IEnumerable<HttpPostedFileBase> files,
            HttpServerUtilityBase server,
            string folderName,
            string filePrefix,
            int maxFiles = MaxDishImages)
        {
            var savedPaths = new List<string>();

            var validFiles = (files ?? Enumerable.Empty<HttpPostedFileBase>())
                .Where(f => f != null && f.ContentLength > 0)
                .Take(maxFiles)
                .ToList();

            if (!validFiles.Any())
            {
                return savedPaths;
            }

            string relativeFolder =
                "~/Resources/User/images/" +
                folderName.Trim('/') +
                "/";

            string physicalFolder = server.MapPath(relativeFolder);

            if (string.IsNullOrWhiteSpace(physicalFolder))
            {
                throw new InvalidOperationException(
                    "The image upload folder could not be resolved.");
            }

            Directory.CreateDirectory(physicalFolder);

            foreach (var file in validFiles)
            {
                string extension =
                    Path.GetExtension(file.FileName ?? string.Empty)
                        .ToLowerInvariant();

                string safeFileName =
                    filePrefix + "_" +
                    Guid.NewGuid().ToString("N") +
                    extension;

                string physicalPath =
                    Path.Combine(physicalFolder, safeFileName);

                file.SaveAs(physicalPath);

                savedPaths.Add(
                    relativeFolder + safeFileName);
            }

            return savedPaths;
        }

        public static string SaveSingleImage(
            HttpPostedFileBase file,
            HttpServerUtilityBase server,
            string folderName,
            string filePrefix)
        {
            var paths = SaveImages(
                new[] { file },
                server,
                folderName,
                filePrefix,
                1);

            return paths.FirstOrDefault();
        }

        public static void DeleteLocalImage(
            string imageUrl,
            HttpServerUtilityBase server,
            string folderName)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            string normalized = imageUrl.Trim();
            string expectedPrefix =
                "~/Resources/User/images/" +
                folderName.Trim('/') + "/";

            if (!normalized.StartsWith(
                expectedPrefix,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string physicalPath = server.MapPath(normalized);

            if (string.IsNullOrWhiteSpace(physicalPath))
            {
                return;
            }

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}
