using UnityEngine;

namespace IsolarvCachedObjectTool.Tests
{
    [CreateAssetMenu(fileName = "New Test SO", menuName = "Isolarv/Cached Object Tool/Test SO")]
    public class TestSO : ScriptableObject
    {
        public TestOverrideData OverrideData;
    }
}