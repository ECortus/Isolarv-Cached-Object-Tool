using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IsolarvCachedObjectTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace IsolarvCachedObjectTool.Editor
{
    public abstract class OverrideObjectPropertyDrawerModule<T> : CustomPropertyDrawerModule
        where T : ScriptableObject, IValidationObject
    {
        protected abstract string FolderOfCachedOverride { get; }
        
        List<SerializedProperty> busyProperties = new List<SerializedProperty>();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            DrawProperty(property, label);

            if (!busyProperties.Contains(property))
            {
                UniTask.Create(async () =>
                {
                    busyProperties.Add(property);
                    await TryGenerateOverrideNewData(property);
                    busyProperties.Remove(property);
                });
            }
        }

        async UniTask TryGenerateOverrideNewData(SerializedProperty property)
        {
            var defaultData = property.FindPropertyRelative("defaultData");
            var newData = property.FindPropertyRelative("newData");
            
            if (!defaultData.objectReferenceValue)
            {
                if (newData.objectReferenceValue)
                {
                    CachedObjectDirectory.TryRemove(newData.objectReferenceValue);
                }
                
                newData.objectReferenceValue = null;
                return;
            }

            var oldEffectData = defaultData.objectReferenceValue as T;
            if (!oldEffectData)
                return;
            
            if (!EditorUtils.IsToolInitialized())
            {
                Debug.Log("[Isolarv Cached Object Tool] Tool is not initialized. Try to initialize...");
                CachedObjectMethods.InitializeCacheDirectory();
            }

            var targetObject = property.serializedObject.targetObject;
            var oldName = oldEffectData.name;

            if (EditorHelper.IsArrayElement(property))
            {
                var index = EditorHelper.GetArrayIndex(property);
                oldName += $"-array-index-{index}";
            }
            
            string newName = EditorUtils.CreateAndGetCachedAssetName(oldName, targetObject);
            EditorUtils.CreateOrGetNewCachedAsset(newName, FolderOfCachedOverride, property, newData, oldEffectData);

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            await UniTask.Yield();
        }
    }
}