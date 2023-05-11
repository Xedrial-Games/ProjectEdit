using Unity.Entities;

using MoonSharp.Interpreter;
using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public class ScriptInstance : IComponentData
    {
        private MScript m_ScriptHandle;
        
        private Closure m_StartFunction;
        private Closure m_UpdateFunction;

        public void Init(MScript scriptHandle)
        {
            m_ScriptHandle = scriptHandle;

            m_StartFunction = m_ScriptHandle.Globals.Get("Start").Function;
            m_UpdateFunction = m_ScriptHandle.Globals.Get("Update").Function;
        }

        public void CallStart() => m_StartFunction?.Call();

        public void CallUpdate(float ts) => m_UpdateFunction?.Call(ts);

        public DynValue this[object key]
        {
            get => m_ScriptHandle?.Globals.Get(key);
            set => m_ScriptHandle?.Globals.Set(key, DynValue.FromObject(m_ScriptHandle, value));
        }

        public static implicit operator bool(ScriptInstance scriptInstance)
            => scriptInstance.m_ScriptHandle != null;

        public static implicit operator MScript(ScriptInstance scriptInstance)
            => scriptInstance.m_ScriptHandle;
    }
}