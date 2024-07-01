using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShadowLib
{
    public static class AssetUtils
    {
        public static AssetBundle LoadAssetBundleFromPluginsFolder(string assetBundleName)
        {
            string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assetBundleName);
            return AssetBundle.LoadFromFile(text);
        }

        public static AssetBundle LoadAssetBundleFromPath(string assetBundlePath)
        {
            return AssetBundle.LoadFromFile(assetBundlePath);
        }
    }
}
