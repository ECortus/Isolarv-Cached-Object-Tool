using Cysharp.Threading.Tasks;
using IsolarvCachedObjectTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace IsolarvCachedObjectTool.Editor
{
    public abstract class OverrideObjectPropertyDrawerModule<T, TS> : CustomPropertyDrawerModule
        where T : ScriptableObject, IValidationObject
        where TS : OverrideActorData<T>
    {
        protected abstract string FolderOfCachedOverride { get; }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            DrawProperty(property, label);

            UniTask.Create(async () =>
            {
                await TryGenerateOverrideNewData(property);
            });
        }

        async UniTask TryGenerateOverrideNewData(SerializedProperty property)
        {
            var defaultData = property.FindPropertyRelative("defaultData");
            if (!defaultData.objectReferenceValue)
                return;

            var oldEffectData = defaultData.objectReferenceValue as T;
            if (!oldEffectData)
                return;

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