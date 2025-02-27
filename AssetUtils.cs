﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        
        public static AssetBundle LoadFromEmbeddedResources(string assetBundleResourceName)
        {
            return AssetBundle.LoadFromMemory(ReadFully(Assembly.GetCallingAssembly().GetManifestResourceStream(assetBundleResourceName)));
        }
    
        private static byte[] ReadFully(Stream input)
        {
            using var ms = new MemoryStream();
            var buffer = new byte[81920];
            int read;
        
            while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, read);
            }
        
            return ms.ToArray();
        }
    }
}
