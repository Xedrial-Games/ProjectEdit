using MoonSharp.Interpreter;

using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public struct Script
    {
        public Closure StartFunction { get; }
        
        public Closure UpdateFunction { get; }
        
        public Script(MScript scriptHandle)
        {
            m_ScriptHandle = scriptHandle;

            StartFunction = m_ScriptHandle.Globals.Get("Start").Function;
            UpdateFunction = m_ScriptHandle.Globals.Get("Update").Function;
        }
        
        private MScript m_ScriptHandle;
    }
}
