using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IsolarvCachedObjectTool.Editor
{
    internal abstract class CustomPropertyDrawerModule : PropertyDrawer
    {
        Rect _rect;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            WriteRect(position);
        }
        
        void WriteRect(Rect rect)
        {
            _rect = rect;
        }
        
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
    }
}