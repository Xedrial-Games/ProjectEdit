using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using ProjectEdit.Components;
using Unity.Burst;
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

            foreach (var pair in scripts)
            {
                var script = new MScript
                {
                    Globals =
                    {
                        ["Log"] = (Action<object>)Log
                    }
                };
                
                script.DoFile(pair.Key);
                s_Scripts.Add(pair.Key, new Script(script));
            }
            
            s_MinifyScript = new MScript();
            s_MinifyScript.DoFile("minify.lua");

            s_Minify = s_MinifyScript.Globals.Get("MinifyCode");
        }

        public static void ExecuteLua()
        {
            print(s_Scripts["Test.lua"].StartFunction.Call().Number);
        }

        private static void Log(object message) => Debug.Log(message);
    }
}
