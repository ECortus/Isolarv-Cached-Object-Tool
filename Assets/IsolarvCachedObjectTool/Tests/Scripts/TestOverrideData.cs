using System;
using IsolarvCachedObjectTool.Runtime;

#if UNITY_EDITOR
using IsolarvCachedObjectTool.Editor;
#endif

using UnityEditor;

namespace IsolarvCachedObjectTool.Tests
{
    [Serializable]
    public class TestOverrideData : OverrideActorData<TestData> { }
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TestOverrideData))]
    public class TestOverrideDataDrawer : OverrideObjectPropertyDrawerModule<TestData, TestOverrideData>
    {
        protected override string FolderOfCachedOverride => "Test";
    }
    
#endif
}