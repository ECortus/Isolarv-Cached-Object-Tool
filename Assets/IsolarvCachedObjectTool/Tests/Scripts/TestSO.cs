using System;
using UnityEngine;

namespace IsolarvCachedObjectTool.Tests
{
    internal class TestSO : ScriptableObject
    {
        public TestOverrideData OverrideData;

        public TestOverrideData[] Array;

        public TestSubclass[] Subclass;
        
        [Serializable]
        public class TestSubclass
        {
            public TestOverrideData[] SubArray;
        }
    }
}