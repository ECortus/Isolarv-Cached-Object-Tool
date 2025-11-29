#if UNITY_EDITOR

using CachingObjectsTool.Editor.Modules;
using UnityEditor;

namespace CachingObjectsTool.Tests.Editor
{
    [CustomPropertyDrawer(typeof(TestOverrideData))]
    internal class TestOverrideDataDrawer : OverrideObjectPropertyDrawerModule<TestData>
    {
        protected override string FolderOfCachedOverride => "Test";
    }
}

#endif