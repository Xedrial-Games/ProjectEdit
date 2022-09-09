using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using Unity.Entities;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MScript = MoonSharp.Interpreter.Script;

using ProjectEdit.Components;
using ProjectEdit.Entities;
using Unity.Collections;

namespace ProjectEdit.Scripting
{
    public partial class ScriptsSystem : SystemBase
    {
        public static Dictionary<Entity, Script> Scripts { get; } = new();
        private static ref EntityCommandBuffer CommandBuffer => ref EntitiesManager.CommandBuffer;

        protected override void OnCreate()
        {
            Scripts.Clear();
            DirectoryInfo modulesDi = new($"{Application.streamingAssetsPath}/LuaScripts/");
            DirectoryInfo directoryInfo = new($"{Application.streamingAssetsPath}/LuaScripts/");

            var scripts = directoryInfo.GetFiles("*.lua", SearchOption.AllDirectories)
                .ToDictionary(scriptFile => scriptFile.Name,
                    scriptFile => File.ReadAllText(scriptFile.FullName));

            MScript.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts)
            {
                ModulePaths = new[]
                {
                    $"{modulesDi.FullName}/?", $"{modulesDi.FullName}/?.lua"
                },
                IgnoreLuaPathGlobal = true
            };
            MScript.DefaultOptions.DebugPrint = Debug.Log;

            RegisterTypes();
        }

        protected override void OnStartRunning()
        {
            CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var scripts = Scripts;

            Entities.ForEach((Entity entity, ref ScriptComponent _) =>
            {
                if (!scripts.TryGetValue(entity, out Script script))
                    return;
                
                script.StartFunction?.Call();

                DynValue export = script.ScriptHandle.Globals.Get("export");
                if (export.IsNil())
                    return;

                foreach (TablePair pair in export.Table.Pairs)
                {
                    Debug.Log($"{pair.Key}:{pair.Value}");
                }
                
            }).WithoutBurst().Run();
            
            if (CommandBuffer.IsEmpty)
            {
                CommandBuffer.Dispose();
                return;
            }
            
            CommandBuffer.Playback(EntityManager);
            CommandBuffer.Dispose();
        }

        protected override void OnUpdate()
        {
            float ts = Time.DeltaTime;
            CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities.ForEach((Entity entity, ref ScriptComponent _) =>
            {
                if (Scripts.TryGetValue(entity, out Script script))
                    script.UpdateFunction?.Call(ts);
            }).WithoutBurst().Run();

            if (CommandBuffer.IsEmpty)
            {
                CommandBuffer.Dispose();
                return;
            }
            
            CommandBuffer.Playback(EntityManager);
            CommandBuffer.Dispose();
        }

        public static void AddScript(Entity entity)
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

            script.DoFile(EntitiesManager.EntityManager.GetComponentData<ScriptComponent>(entity).Script.ToString());
            Scripts.Add(entity, new Script(script, entity));
        }

        private static void RegisterTypes()
        {
            UserData.RegisterAssembly();
            UserData.RegisterType<Entity>();
            UserData.RegisterType<Input>();
        }
    }
}
