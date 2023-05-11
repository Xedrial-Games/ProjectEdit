using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Unity.Entities;
using Unity.Collections;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public partial class ScriptEngine : SystemBase
    {
        protected override void OnCreate()
        {
            DirectoryInfo modulesDi = new($"{Application.streamingAssetsPath}/LuaScripts/");
            DirectoryInfo directoryInfo = new($"{Application.streamingAssetsPath}/LuaScripts/");

            FileInfo[] filesInfo = directoryInfo.GetFiles("*.lua", SearchOption.AllDirectories);
            Dictionary<string, string> scripts = new(filesInfo.Length);
            foreach (FileInfo file in filesInfo)
                scripts.Add(file.Name, File.ReadAllText(file.FullName));

            MScript.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts)
            {
                ModulePaths = new[]
                {
                    $"{modulesDi.FullName}/?", $"{modulesDi.FullName}/?.lua"
                },
                IgnoreLuaPathGlobal = true,
            };
            MScript.DefaultOptions.DebugPrint = Debug.Log;

            RegisterTypes();
        }

        protected override void OnUpdate()
        {
            float ts = SystemAPI.Time.DeltaTime;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach ((RefRO<ScriptComponent> script, Entity entity)
                     in SystemAPI.Query<RefRO<ScriptComponent>>()
                         .WithNone<ScriptInstance>()
                         .WithEntityAccess()
            )
            {
                ScriptInstance scriptInstance = CreateScriptInstance(
                    entity,
                    script.ValueRO.ScriptName.ToString()
                );
                
                if (scriptInstance != null)
                    ecb.AddComponent(entity, scriptInstance);
            }
            
            ecb.Playback(EntityManager);
            ecb.Dispose();

            foreach (ScriptInstance si in SystemAPI.Query<ScriptInstance>())
                si.CallUpdate(ts);
        }

        private ScriptInstance CreateScriptInstance(Entity entity, string scriptName)
        {
            var script = new MScript
            {
                Globals =
                {
                    ["Input"] = typeof(Input),
                    ["EntityHandle"] = entity,
                    ["InternalCalls"] = UserData.CreateStatic<InternalCalls>()
                }
            };

            DynValue scriptValue = script.LoadFile(scriptName);
            if (scriptValue == null || scriptValue.IsNil())
                return null;

            script.Call(scriptValue);
            
            var scriptInstance = new ScriptInstance();
            scriptInstance.Init(script);
            
            DynValue export = script.Globals.Get("export");
            if (export.IsNotNil())
            {
                foreach (TablePair pair in export.Table.Pairs)
                    Debug.Log($"{pair.Key}:{pair.Value}");
            }

            scriptInstance.CallStart();
            return scriptInstance;
        }

        private static void RegisterTypes()
        {
            UserData.RegisterAssembly();
            UserData.RegisterType<Entity>();
            UserData.RegisterType<Input>();
        }
    }
}
