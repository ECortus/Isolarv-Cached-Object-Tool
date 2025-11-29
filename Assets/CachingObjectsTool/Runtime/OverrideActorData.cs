using System;
using UnityEngine;

namespace CachingObjectsTool.Runtime
{
    [Serializable]
    public abstract class OverrideActorData<T>
    {
        [SerializeField] private T defaultData;
        [SerializeField, Expandable(true)] private T newData;

        public T GetData() => newData;
    }
}