using CachingObjectsTool.Editor.Common;
using CachingObjectsTool.Runtime;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CachingObjectsTool.Editor.Modules
{
    public abstract class OverrideObjectPropertyDrawerModule<T> : PropertyDrawer
        where T : ScriptableObject, IValidationObject
    {
        protected abstract string FolderOfCachedOverride { get; }
        
        Rect _rect;
                
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            WriteRect(position);
            DrawProperty(property, label);

            UniTask.Create(async () =>
            {
                await TryGenerateOverrideNewData(property);
            });
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
            
            string newName = EditorUtils.CreateAndGetCachedAssetName(oldName, targetObject, property);
            EditorUtils.CreateOrGetNewCachedAsset(newName, FolderOfCachedOverride, property, newData, oldEffectData);

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            await UniTask.Yield();
        }
        
        void WriteRect(Rect rect)
        {
            _rect = rect;
        }

        #region Additional Methods

        protected delegate void DrawPropertyGUI(SerializedProperty property);
        protected void DrawAsFoldout(SerializedProperty property, GUIContent label, DrawPropertyGUI drawGUI)
        {
            var foldoutRect = new Rect(_rect.x, _rect.y, _rect.width, EditorGUIUtility.singleLineHeight);
            var foldout = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            if (foldout != property.isExpanded)
            {
                property.isExpanded = foldout;
            }
            
            AddSingleLine();
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                drawGUI(property);
                EditorGUI.indentLevel--;
            }
        }
        
        protected void AddHalfLine()
        {
            var modifier = EditorGUIUtility.singleLineHeight * 0.55f;
            _rect.y += modifier;
        }
        
        protected void AddSingleLine()
        {
            var modifier = EditorGUIUtility.singleLineHeight * 1.1f;
            _rect.y += modifier;
        }
        
        protected void DrawProperty(SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(_rect, property, label, true);
        }

        protected void DrawHelperBox(string message, MessageType messageType)
        {
            AddHalfLine();
            
            Rect rect = new Rect(this._rect.x, this._rect.y, this._rect.width, EditorGUIUtility.singleLineHeight);
            rect.y += EditorGUIUtility.singleLineHeight * 0.55f;
            
            EditorGUI.HelpBox(rect, message, messageType);
        }

        #endregion
    }
}