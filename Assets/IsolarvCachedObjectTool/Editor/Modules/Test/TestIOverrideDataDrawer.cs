using IsolarvCachedObjectTool.Tests;
using UnityEditor;

namespace IsolarvCachedObjectTool.Editor
{
    [CustomPropertyDrawer(typeof(TestOverrideData))]
    internal class TestOverrideDataDrawer : OverrideObjectPropertyDrawerModule<TestData>
    {
        protected override string FolderOfCachedOverride => "Test";
    }
}