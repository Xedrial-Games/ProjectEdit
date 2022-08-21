using UnityEngine;
using MoonSharp.Interpreter;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using MoonSharp.Interpreter.Loaders;
using System;

namespace ProjectEdit
{
    public class ScriptEngine : MonoBehaviour
    {
        Script m_MinifyScript = new();
        DynValue m_Minify;

        public void LoadLua()
        {
            Dictionary<string, string> scripts = new();

            DirectoryInfo directoryInfo = new($"{Application.dataPath}/LuaScripts");

            foreach (var scriptFile in directoryInfo.GetFiles())
            {
                if (scriptFile.Extension == ".lua")
                    scripts.Add(scriptFile.Name, File.ReadAllText(scriptFile.FullName));
            }

            Script.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts);
            ((UnityAssetsScriptLoader)Script.DefaultOptions.ScriptLoader).ModulePaths
                = new string[] { $"{directoryInfo.FullName}/?", $"{directoryInfo.FullName}/?.lua" };

            m_MinifyScript = new();
            _ = m_MinifyScript.DoFile("minify.lua");

            m_Minify = m_MinifyScript.Globals.Get("MinifyCode");
        }

        public void ExecuteLua()
        {
            print(m_MinifyScript.Call(m_Minify, "a     =      5  +2  ;"));
        }
    }
}
