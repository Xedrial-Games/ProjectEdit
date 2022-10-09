using System;
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
                IgnoreLuaPathGlobal = true
            };
            MScript.DefaultOptions.DebugPrint = Debug.Log;

            RegisterTypes();
        }

        protected override void OnStartRunning()
        {
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, in ScriptComponent scriptComponent) =>
            {
                if (!scriptComponent)
                    InitScript(entity);

                if (scriptComponent)
                {
                    DynValue export = scriptComponent["export"];
                    if (export.IsNil())
                        return;

                    foreach (TablePair pair in export.Table.Pairs)
                    {
                        Debug.Log($"{pair.Key}:{pair.Value}");
                    }
                }

                scriptComponent.StartFunction?.Call();
            }).Run();
        }

        protected override void OnUpdate()
        {
            float ts = Time.DeltaTime;

            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, in ScriptComponent scriptComponent) =>
            {
                if (!scriptComponent)
                    InitScript(entity);

                scriptComponent.UpdateFunction?.Call(ts);
            }).Run();
        }

        private void InitScript(Entity entity)
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

            var scriptComponent = EntityManager.GetSharedComponentData<ScriptComponent>(entity);
            
            script.DoFile(scriptComponent.ScriptName);
            scriptComponent.Init(script);
            
            EntityManager.SetSharedComponentData(entity, scriptComponent);
        }

        private static void RegisterTypes()
        {
            UserData.RegisterAssembly();
            UserData.RegisterType<Entity>();
            UserData.RegisterType<Input>();
        }
    }
}
