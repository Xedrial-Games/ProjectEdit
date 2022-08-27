using MoonSharp.Interpreter;
using Unity.Entities;
using Entity = ProjectEdit.Entities.Entity;
using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public struct Script : IComponentData
    {
        public Closure StartFunction { get; }
        
        public Closure UpdateFunction { get; } 
        
        public readonly MScript ScriptHandle;

        public Script(MScript scriptHandle)
        {
            ScriptHandle = scriptHandle;

            StartFunction = ScriptHandle.Globals.Get("Start").Function;
            UpdateFunction = ScriptHandle.Globals.Get("Update").Function;
        }
        
        public Script(string fileName, Entity entity)
        {
            ScriptHandle = new MScript();
            ScriptHandle.DoFile(fileName);

            ScriptHandle.Globals["transform"] = new Transform(entity);

            StartFunction = ScriptHandle.Globals.Get("Start").Function;
            UpdateFunction = ScriptHandle.Globals.Get("Update").Function;
        }
        
        public object this[object key]
        {
            get => ScriptHandle.Globals.Get(key).ToObject();
            set => ScriptHandle.Globals.Set(key, DynValue.FromObject(ScriptHandle, value));
        }
    }
}
