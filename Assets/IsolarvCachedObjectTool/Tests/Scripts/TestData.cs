using IsolarvCachedObjectTool.Runtime;
using UnityEngine;

namespace IsolarvCachedObjectTool.Tests
{
    public class TestData : ScriptableObject, IValidationObject
    {
        public int Value;

        public void OnCreateValidate<T>(T oldObject) where T : ScriptableObject, IValidationObject
        {

        }

        public void OnUpdateValidate<T>(T oldObject) where T : ScriptableObject, IValidationObject
        {

        }
    }
}