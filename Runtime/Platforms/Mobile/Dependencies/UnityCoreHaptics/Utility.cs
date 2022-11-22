using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityCoreHaptics {
    public static class Utility
    {
        /// <summary>
        /// Checks if relative path from Streaming Assets correctly identifies an existing AHAP file
        /// </summary>
        /// <param name="relativePath">Path relative to streaming assets to AHAP file</param>
        /// <returns></returns>
        public static bool FileExists(string relativePath) {
            var fullPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, relativePath);
            return File.Exists(fullPath);
        }

        /// <summary>
        /// Checks if relative path from Streaming Assets correctly identifies an existing AHAP file
        /// </summary>
        /// <param name="relativePath">Path relative to streaming assets to AHAP file</param>
        /// <returns></returns>
        public static void AssertFileExists(string relativePath) {
            #if UNITY_EDITOR
                try {
                    Assert.IsTrue(FileExists(relativePath));
                }
                catch {
                    throw new System.Exception(
                        "The relative path to file " + relativePath + " does."
                        + " Please check that this path is valid relative to Assets/StreamingAssets."
                        + " If this is your first time using this asset, please upload an AHAP file"
                        + " inside Assets/StreamingAssets/path/to/myfile.ahap and set AhapRelativePath to"
                        + " path/to/myfile.ahap"
                    );
                }
            #endif
        }
    }
}
