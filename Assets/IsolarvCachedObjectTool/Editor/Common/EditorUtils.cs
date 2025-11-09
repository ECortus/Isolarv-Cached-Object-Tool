using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IsolarvCachedObjectTool.Editor
{
    public static class EditorUtils
    {
        public static string GetCurrentAssetDirectory()
        {
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                    continue;

                if (Directory.Exists(path))
                    return path;
                
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);
            }

            return @"Assets\_Project\Data";
        }

        public static string CreateAndGetAssetPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = "Assets";
            if (path[0] == '/')
                path = path.Substring(1);
            if (path.Substring(0, 6).ToLower() != "assets")
                path = path.Insert(0, "Assets/");
            if (!Directory.Exists(Path.GetFullPath(path)))
                Directory.CreateDirectory(path);

            return path;
        }
        
        public static string CreateAndGetCachedAssetName(string name, Object targetObject)
        {
            string newName = $"{name}-cached-by-{targetObject.name}-of-type-{targetObject.GetType().Name}";
            newName = newName.ToLower().Replace(" ", "-");
            
            return newName;
        }

        public static Object CreateOrGetNewCachedAsset<T>(string newName, string folder, 
            SerializedProperty parentProperty, SerializedProperty newDataProperty, T oldData) where T : ScriptableObject, IValidationObject
        {
            string path = EditorPaths.CACHED_OBJECTS_PATH;
            path += $"/{folder}";

            T instance = null;
            
            var loadedData = EditorHelper.GetScriptableAsset<T>(newName, path);
            if (loadedData != null)
            {
                if (newDataProperty.objectReferenceValue == loadedData)
                {
                    instance = loadedData;
                }
            }

            if (!instance)
            {
                instance = EditorHelper.CreateNewScriptableAsset(newName, path, newDataProperty, oldData) as T;
                CachedObjectDirectory.SingleAdd(instance, parentProperty);
            }

            if (!instance)
                throw new Exception($"Failed to create or get new cached asset: {newName}");
            
            instance.OnUpdateValidate(oldData);
            return instance;
        }
        
        public static GUIContent GetWindowTitle(string windowName)
        {
            return new GUIContent(windowName);
        }
    }
}