using UnityEngine;

namespace IsolarvCachedObjectTool.Runtime
{
    public interface IValidationObject
    {
        public void OnCreateValidate<T>(T oldObject) where T : ScriptableObject, IValidationObject;
        public void OnUpdateValidate<T>(T oldObject) where T : ScriptableObject, IValidationObject;
    }
}