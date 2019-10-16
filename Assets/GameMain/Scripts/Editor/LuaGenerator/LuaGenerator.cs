using GameFramework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using UnityGameFramework.Runtime.XLua;

namespace Penny.Editor.LuaTools
{
    public class LuaGenerator
    {
        private static string LuaScriptPath = "Assets/GameMain/LuaScripts/";
        private static string LuaScriptListFileName = "LuaScriptList.bytes";
        private static List<string> s_LuaScriptNames = new List<string>();

        /// <summary>
        /// 生成lua列表文件
        /// </summary>
        public static void GeneratorLuaScriptFiles()
        {
            DirectoryInfo luaScriptDirectory = new DirectoryInfo(LuaScriptPath);
            // 路径不存在，创建路径
            if (!luaScriptDirectory.Exists)
            {
                luaScriptDirectory.Create();
            }
            // 删除lua列表文件
            if (File.Exists(Utility.Path.GetCombinePath(LuaScriptPath, LuaScriptListFileName)))
            {
                File.Delete(Utility.Path.GetCombinePath(LuaScriptPath, LuaScriptListFileName));
            }
            // 搜索lua脚本
            FileInfo[] luaFiles = luaScriptDirectory.GetFiles("*.bytes", SearchOption.AllDirectories);
            if (luaFiles.Length <= 0)
            {
                Debug.LogWarning("<Please Cheack File Suffix Is '.bytes'> Or <Is Empty Folder>");
                return;
            }
            string luaScriptFileName = string.Empty;
            s_LuaScriptNames.Clear();
            foreach (FileInfo luaScriptFile in luaFiles)
            {
                luaScriptFileName = GetName(luaScriptFile.Name);
                if (luaScriptFileName.Contains(LuaScriptListFileName))
                {
                    continue;
                }
                luaScriptFileName = luaScriptFileName.Replace(".lua.bytes", "");
                Debug.Log("FileName: " + luaScriptFileName);
                s_LuaScriptNames.Add(luaScriptFileName);
            }
            File.WriteAllText(Utility.Path.GetCombinePath(LuaScriptPath, LuaScriptListFileName), string.Join("\r\n", s_LuaScriptNames)/*, Encoding.ASCII*/);
            Debug.Log("Generate Lua Script List Complete. ");
        }

        /// <summary>
        /// 给lua文件追加.bytes后缀
        /// </summary>
        public static void LuaFileAppendedSuffix()
        {
            DirectoryInfo luaScriptDirectory = new DirectoryInfo(LuaScriptPath);
            // 路径不存在，提示
            if (!luaScriptDirectory.Exists)
            {
                Debug.LogWarning("Frist Generate Lua List File!!!! ");
                return;
            }
            // 搜索lua脚本
            FileInfo[] luaFiles = luaScriptDirectory.GetFiles("*.lua", SearchOption.AllDirectories);
            string luaFullName = string.Empty;
            foreach (FileInfo file in luaFiles)
            {
                if (GetName(file.Name).Contains(LuaScriptListFileName))
                {
                    continue;
                }
                luaFullName = file.FullName + XLuaComponent.LuaBundleExtensionName;
                Debug.Log("FullFile: " + luaFullName);
                file.MoveTo(luaFullName);
            }
        }

        /// <summary>
        /// 给lua文件取消.bytes后缀
        /// </summary>
        public static void LuaFileCancelSuffix()
        {
            DirectoryInfo luaScriptDirectory = new DirectoryInfo(LuaScriptPath);
            // 路径不存在，提示
            if (!luaScriptDirectory.Exists)
            {
                Debug.LogWarning("Frist Generate Lua List File!!!! ");
                return;
            }
            // 搜索lua脚本
            FileInfo[] luaFiles = luaScriptDirectory.GetFiles("*.bytes", SearchOption.AllDirectories);
            string luaFullName = string.Empty;
            foreach (FileInfo file in luaFiles)
            {
                if (GetName(file.Name).Contains(LuaScriptListFileName))
                {
                    continue;
                }
                luaFullName = file.FullName.Replace(XLuaComponent.LuaBundleExtensionName, "");
                Debug.Log("FullFile: " + luaFullName);
                file.MoveTo(luaFullName);
            }
        }


        private static string GetName(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            if (str.Length > 0)
            {
                if (str.LastIndexOf("/") > 0)
                {
                    str = str.Substring(str.LastIndexOf("`") + 1);
                }
                else if (str.LastIndexOf("\\") > 0)
                {
                    str = str.Substring(str.LastIndexOf("\\") + 1);
                }
            }
            return str;
        }
    }

}