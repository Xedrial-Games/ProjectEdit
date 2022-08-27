using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using ProjectEdit.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using MScript = MoonSharp.Interpreter.Script;

namespace ProjectEdit.Scripting
{
    public class ScriptEngine : MonoBehaviour
    {
        public static Dictionary<string, Script> Scripts => s_Scripts;

        private static readonly Dictionary<string, Script> s_Scripts = new();

        private static MScript s_MinifyScript = new();
        private static DynValue s_Minify;

        public static void LoadLua()
        {
            s_Scripts.Clear();
            DirectoryInfo directoryInfo = new($"{Application.dataPath}/LuaScripts");

            var scripts = directoryInfo.GetFiles()
                .Where(scriptFile => scriptFile.Extension == ".lua")
                .ToDictionary(scriptFile => scriptFile.Name,
                    scriptFile => File.ReadAllText(scriptFile.FullName));

            MScript.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts);
            ((UnityAssetsScriptLoader)MScript.DefaultOptions.ScriptLoader).ModulePaths
                = new[] { $"{directoryInfo.FullName}/?", $"{directoryInfo.FullName}/?.lua" };

            RegisterTypes();
            RegisterValues(scripts.Keys);
            
            s_MinifyScript = new MScript();
            s_MinifyScript.DoFile("minify.lua");

            s_Minify = s_MinifyScript.Globals.Get("MinifyCode");
        }

        public static void ExecuteLua()
        {
            print(s_Scripts["Test.lua"].StartFunction.Call().Number);
        }

        private static void Log(object message) => Debug.Log(message);

        private static void RegisterTypes()
        {
            UserData.RegisterType<Transform>();
        }

        private static void RegisterValues(IEnumerable<string> scripts)
        {
            foreach (string name in scripts)
            {
                var script = new MScript
                {
                    Globals =
                    {
                        ["Log"] = (Action<object>)Log
                    }
                };
                
                script.DoFile(name);
                s_Scripts.Add(name, new Script(script));
            }
        }
    }

    public class Transform
    {
        [MoonSharpHidden]
        public Transform(Entity entity) => m_Entity = entity;

        public Table position
        {
            get
            {
                float3 value = m_Entity.GetComponentData<Translation>().Value;
                var table = new Table(m_Entity.GetComponentData<Script>().ScriptHandle)
                {
                    ["x"] = value.x,
                    ["y"] = value.y,
                    ["z"] = value.z
                };

                return table;
            }
            set => m_Entity.SetComponentData(new Translation
            {
                Value = new float3
                (
                    (float)value["x"],
                    (float)value["y"],
                    (float)value["z"]
                )
            });
        }
        
        private Entity m_Entity;
    }
}
