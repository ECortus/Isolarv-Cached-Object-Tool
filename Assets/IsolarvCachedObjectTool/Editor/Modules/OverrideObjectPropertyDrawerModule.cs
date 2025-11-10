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
        
        bool isGeneratingOverrideNewData = false;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            DrawProperty(property, label);

            if (!isGeneratingOverrideNewData)
            {
                UniTask.Create(async () =>
                {
                    isGeneratingOverrideNewData = true;
                    await TryGenerateOverrideNewData(property);
                    isGeneratingOverrideNewData = false;
                });
            }
        }

        async UniTask TryGenerateOverrideNewData(SerializedProperty property)
        {
            var defaultData = property.FindPropertyRelative("defaultData");
            if (!defaultData.objectReferenceValue)
                return;

            var oldEffectData = defaultData.objectReferenceValue as T;
            if (!oldEffectData)
                return;
            
            if (!EditorUtils.IsToolInitialized())
            {
                Debug.Log("[Isolarv Cached Object Tool] Tool is not initialized. Try to initialize...");
                CachedObjectMethods.InitializeCacheDirectory();
            }

            var newData = property.FindPropertyRelative("newData");

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