using System;

using Unity.Entities;

using MoonSharp.Interpreter;
using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public struct ScriptComponent : ISharedComponentData, IEquatable<ScriptComponent>
    {
        public Closure StartFunction { get; private set; }
        
        public Closure UpdateFunction { get; private set; }
     
        public string ScriptName;

        private MScript m_ScriptHandle;

        public void Init(MScript scriptHandle)
        {
            m_ScriptHandle = scriptHandle;

            StartFunction = m_ScriptHandle.Globals.Get("Start").Function;
            UpdateFunction = m_ScriptHandle.Globals.Get("Update").Function;
        }
        
        public DynValue this[object key]
        {
            get => m_ScriptHandle?.Globals.Get(key);
            set => m_ScriptHandle?.Globals.Set(key, DynValue.FromObject(m_ScriptHandle, value));
        }

        public static implicit operator bool(ScriptComponent scriptComponent)
            => scriptComponent.m_ScriptHandle != null;

        public static implicit operator MScript(ScriptComponent scriptComponent)
            => scriptComponent.m_ScriptHandle;

        public bool Equals(ScriptComponent other)
            => Equals(m_ScriptHandle, other.m_ScriptHandle) && ScriptName == other.ScriptName && Equals(StartFunction, other.StartFunction) && Equals(UpdateFunction, other.UpdateFunction);

        public override bool Equals(object obj)
            => obj is ScriptComponent other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(m_ScriptHandle, ScriptName, StartFunction, UpdateFunction);
    }
}