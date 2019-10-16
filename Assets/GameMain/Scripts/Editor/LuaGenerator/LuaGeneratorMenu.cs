using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.DataTableTools;

namespace Penny.Editor.LuaTools
{
    public sealed class LuaGeneratorMenu
    {
        [MenuItem("Star Force/☆Generate Lua List File☆")]
        private static void GenerateLuaListFile()
        {
            LuaGenerator.GeneratorLuaScriptFiles();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Star Force/☆Generate .Lua.bytes☆")]
        private static void GeanerateLuaFileByBytes()
        {
            LuaGenerator.LuaFileAppendedSuffix();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Star Force/☆Generate .Lua☆")]
        private static void GeanerateLuaFileByLua()
        {
            LuaGenerator.LuaFileCancelSuffix();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
