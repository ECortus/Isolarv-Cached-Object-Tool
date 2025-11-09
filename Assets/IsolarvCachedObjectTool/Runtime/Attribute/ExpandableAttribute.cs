using UnityEngine;

namespace IsolarvCachedObjectTool.Runtime
{
    public class ExpandableAttribute : PropertyAttribute
    {
        public bool ReadOnly { get; private set; }
        
        public ExpandableAttribute(bool readOnly = false)
        {
            ReadOnly = readOnly;
        }
    }
}