using UnityEngine;

using Unity.Entities;
using Unity.Collections;

namespace ProjectEdit.Components
{
    public struct LogComponent : IComponentData
    {
        public FixedString32Bytes Message;

        public void Log() => Debug.Log(Message);
    }
}
