using IsolarvCachedObjectTool.Runtime;
using UnityEngine;

namespace IsolarvCachedObjectTool.Tests
{
    [CreateAssetMenu(fileName = "New Test Data", menuName = "Isolarv/Cached Object Tool/Test Data")]
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