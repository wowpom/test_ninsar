using System.IO;
using UnityEngine;

namespace Game.Infrastructure
{
    public static class GridFilePath
    {
        public const string FileName = "grid.txt";

        public static string ApplicationFolder
        {
            get
            {
                var parent = Directory.GetParent(Application.dataPath);
                return parent != null ? parent.FullName : Application.dataPath;
            }
        }

        public static string Full
        {
            get
            {
                return Path.Combine(ApplicationFolder, FileName);
            }
        }
    }
}
