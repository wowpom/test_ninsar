using System.IO;
using Game.Infrastructure;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor
{
    public sealed class GridFileBuildHook : IPostprocessBuildWithReport
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var source = GridFilePath.Full;
            if (!File.Exists(source))
            {
                Debug.LogWarning($"{GridFilePath.FileName} нет в корне проекта ({source}), копировать нечего.");
                return;
            }

            var outputFolder = ResolveOutputFolder(report);
            if (string.IsNullOrEmpty(outputFolder))
            {
                Debug.LogWarning($"Не понял папку сборки. {GridFilePath.FileName} положите рядом с плеером сами.");
                return;
            }

            var destination = Path.Combine(outputFolder, GridFilePath.FileName);
            File.Copy(source, destination, true);

            Debug.Log($"{GridFilePath.FileName} скопирован в сборку: {destination}");
        }

        private static string ResolveOutputFolder(BuildReport report)
        {
            if (report == null)
            {
                return null;
            }

            var outputPath = report.summary.outputPath;
            if (string.IsNullOrEmpty(outputPath))
            {
                return null;
            }

            return Directory.Exists(outputPath) ? outputPath : Path.GetDirectoryName(outputPath);
        }
    }
}
