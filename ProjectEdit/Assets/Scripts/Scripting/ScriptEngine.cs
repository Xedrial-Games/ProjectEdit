using UnityEngine;
using MoonSharp.Interpreter;
using System.Linq;
using System.IO;
using MoonSharp.Interpreter.Loaders;

namespace ProjectEdit.Scripting
{
    public class ScriptEngine : MonoBehaviour
    {
        private Script m_MinifyScript = new();
        private DynValue m_Minify;

        public void LoadLua()
        {
            DirectoryInfo directoryInfo = new($"{Application.dataPath}/LuaScripts");

            var scripts = directoryInfo.GetFiles()
                .Where(scriptFile => scriptFile.Extension == ".lua")
                .ToDictionary(scriptFile => scriptFile.Name,
                    scriptFile => File.ReadAllText(scriptFile.FullName));

            Script.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts);
            ((UnityAssetsScriptLoader)Script.DefaultOptions.ScriptLoader).ModulePaths
                = new[] { $"{directoryInfo.FullName}/?", $"{directoryInfo.FullName}/?.lua" };

            m_MinifyScript = new();
            m_MinifyScript.DoFile("minify.lua");

            m_Minify = m_MinifyScript.Globals.Get("MinifyCode");
        }

        public void ExecuteLua()
        {
            print(m_MinifyScript.Call(m_Minify, "a     =      5  +2  ;"));
        }
    }
}
