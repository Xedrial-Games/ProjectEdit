using Timespawn.EntityTween;
using UnityEngine;

using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace ProjectEdit.Components
{
    public struct LogComponent : IComponentData
    {
        public FixedString32Bytes Message;

        public void Log() => Debug.Log(Message);
    }

    public struct MoveComponent : IComponentData
    {
        public float3 Start;
        public float3 End;
        public TweenParams TweenParams;
    }
}
