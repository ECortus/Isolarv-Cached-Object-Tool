using System;
using System.Collections.Generic;
using CachingObjectsTool.Editor.Common;
using CachingObjectsTool.Runtime;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CachingObjectsTool.Editor
{
    internal class CachedObjectDirectory : ScriptableObject
    {
        static CachedObjectDirectory _instance;
        static CachedObjectDirectory Instance
        {
            get
            {
                if (!_instance)
                    _instance = Resources.Load<CachedObjectDirectory>("Cached Objects/Cached Objects Directory");

                if (!_instance)
                    Debug.LogError("[Cached Object Tool] Non directory in Resources. Please, initialize new directory.");
                
                return _instance;
            }
        }

        public static bool IsExist() => Instance != null;
        
        [Serializable]
        public class CachedObject
        {
            [SerializeField, ReadOnly] Object owner;
            [SerializeField, ReadOnly] Object primaryObject;
            
            [Header("Property Data:")]
            [SerializeField, ReadOnly] private string propertyName;
            [SerializeField, ReadOnly] private bool isArrayElement;
            [SerializeField, ReadOnly] private int arrayElementIndex;
            
            [Header("Object Data:")]
            [SerializeField, ReadOnly] int instanceId;
            [SerializeField, ReadOnly] string assetName;
            [SerializeField, ReadOnly] string assetPath;
            
            public Object GetOwner() => owner;
            public Object GetPrimaryObject() => primaryObject;
            public Object GetAsset() => AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            public CachedObject(Object obj, SerializedProperty parentProperty)
            {
                this.owner = parentProperty.serializedObject.targetObject;
                primaryObject = parentProperty.FindPropertyRelative("defaultData").objectReferenceValue;

                propertyName = parentProperty.displayName;
                isArrayElement = EditorHelper.IsArrayElement(parentProperty);
                arrayElementIndex = EditorHelper.GetArrayIndex(parentProperty);
                
                instanceId = obj.GetInstanceID();
                assetName = obj.name;
                assetPath = AssetDatabase.GetAssetPath(obj);
            }
            
            public bool IsSameCache(Object obj, SerializedProperty parentProperty)
            {
                var sameOwner = this.owner == parentProperty.serializedObject.targetObject;

                var otherPropertyName = parentProperty.name;
                var otherIsArrayElement = EditorHelper.IsArrayElement(parentProperty);
                var otherArrayElementIndex = EditorHelper.GetArrayIndex(parentProperty);
                
                var sameProperty = this.propertyName == otherPropertyName;
                var sameArrayElement = this.isArrayElement == otherIsArrayElement;
                var sameArrayElementIndex = this.arrayElementIndex == otherArrayElementIndex;
                
                return sameOwner && sameProperty && sameArrayElement && sameArrayElementIndex;
            }
            
            public bool IsSameCache(Object obj)
            {
                var sameInstanceId = this.instanceId == obj.GetInstanceID();
                var sameAssetName = this.assetName == obj.name;
                var sameAssetPath = this.assetPath == AssetDatabase.GetAssetPath(obj);
                
                return sameInstanceId && sameAssetName && sameAssetPath;
            }

            public bool IsSameCache(CachedObject other)
            {
                var sameOwner = this.owner == other.owner;
                
                var sameProperty = this.propertyName == other.propertyName;
                var sameArrayElement = this.isArrayElement == other.isArrayElement;
                var sameArrayElementIndex = this.arrayElementIndex == other.arrayElementIndex;
                
                return sameOwner && sameProperty && sameArrayElement && sameArrayElementIndex;
            }

            public bool IsExistedCache()
            {
                bool isExisted = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                
                bool isOwnerExisted = owner;
                bool isDefaultExisted = primaryObject;
                
                return isExisted && isOwnerExisted && isDefaultExisted;
            }
            
            public void DestroyCache()
            {
                if (AssetDatabase.DeleteAsset(assetPath))
                {
                    AssetDatabase.Refresh();
                }
            }
        }

        [SerializeField] private List<CachedObject> objects = new List<CachedObject>();
        public static List<CachedObject> Objects => Instance.objects;

        public static void SingleAdd(Object obj, SerializedProperty parentProperty)
        {
            if (Contains(obj, parentProperty, out var index))
            {
                RemoveAt(index);
            }
            
            Add(obj, parentProperty);
        }
        
        public static void TryAdd(Object obj, SerializedProperty parentProperty)
        {
            if (Contains(obj, parentProperty))
            {
                return;
            }
            
            Add(obj, parentProperty);
        }
        
        public static void TryRemove(Object obj, SerializedProperty parentProperty)
        {
            if (!Contains(obj, parentProperty, out var index))
            {
                return;
            }
            
            RemoveAt(index);
        }
        
        public static void TryRemove(Object cached)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].GetAsset() == cached)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }
        
        public static void TryRemove(CachedObject cached)
        {
            var index = Objects.IndexOf(cached);
            RemoveAt(index);
        }
        
        public static bool Contains(Object obj, SerializedProperty parentProperty)
        {
            return Contains(obj, parentProperty, out _);
        }
        
        static void Add(Object obj, SerializedProperty parentProperty)
        {
            Objects.Add(new CachedObject(obj, parentProperty));
            EditorUtility.SetDirty(Instance);
        }
        
        static void RemoveAt(int index)
        {
            var cached = Objects[index];
            if (cached.IsExistedCache())
            {
                cached.DestroyCache();
            }
            
            Objects.RemoveAt(index);
            EditorUtility.SetDirty(Instance);
        }

        static bool Contains(Object obj, SerializedProperty parentProperty, out int index)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                var cached = Objects[i];
                if (cached.IsSameCache(obj, parentProperty))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }
        
        static void UpdateCachedObjects()
        {
            if (Objects.Count == 0)
            {
                return;
            }
            
            for (int i = 0; i < Objects.Count; i++)
            {
                var cached = Objects[i];
                if (!cached.IsExistedCache())
                {
                    RemoveAt(i);
                    i--;
                }
            }
        }

        private void OnValidate()
        {
            UpdateCachedObjects();
        }
    }
}