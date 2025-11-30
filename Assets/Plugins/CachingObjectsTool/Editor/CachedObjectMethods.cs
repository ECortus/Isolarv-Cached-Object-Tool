using System.Collections.Generic;
using System.IO;
using System.Linq;
using CachingObjectsTool.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace CachingObjectsTool.Editor
{
    internal static class CachedObjectMethods
    {
        [MenuItem("Tools/Isolarv/Cached & Override/Initialize Cache Directory", false, 115)]
        public static void InitializeCacheDirectory()
        {
            var folder = EditorPaths.CACHED_OBJECTS_PATH;
            var name = "Cached Objects Directory";
            var path = folder + "/" + name + ".asset";
            
            if (!AssetDatabase.IsValidFolder(folder))
            {
                if (!AssetDatabase.IsValidFolder(EditorPaths.RESOURCES_PATH))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                
                AssetDatabase.CreateFolder(EditorPaths.RESOURCES_PATH, "Cached Objects");
            }

            if (AssetDatabase.LoadAssetAtPath<CachedObjectDirectory>(path) != null)
            {
                Debug.Log($"[Isolarv Cached Object Tool] Cached Object Directory already exists in Resources at path {path}");
                return;
            }
            
            EditorHelper.CreateNewScriptableAsset<CachedObjectDirectory>(name, folder);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Isolarv/Cached & Override/Clear Unused Cache", false, 120)]
        public static void ClearUnusedCache()
        {
            var cachedValid = new List<CachedObjectDirectory.CachedObject>();
            var inResourcesValid = new List<ScriptableObject>();
            
            var folder = EditorPaths.CACHED_OBJECTS_PATH;
            string[] lookingFor = new string[] { folder };
            string[] guids = AssetDatabase.FindAssets("glob:\"*.asset\"", lookingFor);

            var cachedInResources = new List<ScriptableObject>();
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                if (!obj)
                {
                    Debug.LogError($"Cached Object not found: {path}");
                    continue;
                }
                
                if (obj is CachedObjectDirectory)
                {
                    continue;
                }
                
                cachedInResources.Add(obj);
            }
            
            var cachedObjectsDirectory = CachedObjectDirectory.Objects;
            for (int i = 0; i < cachedObjectsDirectory.Count; i++)
            {
                var cached = cachedObjectsDirectory[i];
                for (int j = 0; j < cachedInResources.Count; j++)
                {
                    var cachedResource = cachedInResources[j];
                    if (cached.IsExistedCache() && cached.IsSameCache(cachedResource))
                    {
                        cachedValid.Add(cached);
                        inResourcesValid.Add(cachedResource);
                    }
                }
            }

            var cachedToDelete = cachedObjectsDirectory.Except(cachedValid).ToList();
            var resourcesToDelete = cachedInResources.Except(inResourcesValid).ToList();

            for (int i = 0; i < cachedToDelete.Count; i++)
            {
                var cached = cachedToDelete[i];
                CachedObjectDirectory.TryRemove(cached);
            }

            for (int i = 0; i < resourcesToDelete.Count; i++)
            {
                var cached = resourcesToDelete[i];
                
                var path = AssetDatabase.GetAssetPath(cached);
                if (path != "")
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[Isolarv Cached Object Tool] Clear Unused Cache completed.");
        }
    }
}