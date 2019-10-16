using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static GameFramework.Utility;

namespace Penny.Editor.LuaTools
{
    public class OfflineSpeechGenerator : EditorWindow
    {
        [MenuItem("Star Force/☆Generate Offline Speech☆")]
        private static void XunFeiOfflineSpeechGenerator()
        {
            GetWindow<OfflineSpeechGenerator>(true, "语音合成", true);
            EditorApplication.isPlaying = true;
        }

        private void OnDestroy()
        {
            EditorApplication.isPlaying = false;
        }

        private string m_SavePath = string.Empty;
        private string m_Text = string.Empty;
        private string m_FileName = string.Empty;

        private void OnGUI()
        {
            //***************************************************************************************************************************************************************************
            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width));
                {
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height - 52f));
                    {
                        //*****************************************************************
                        if (GUILayout.Button("选择文件夹路径", GUILayout.Width(position.width)))
                        {
                            m_SavePath = EditorUtility.SaveFolderPanel("选择目标路径", "确定选择", "");
                        }
                        EditorGUILayout.LabelField(m_SavePath);
                        //*****************************************************************
                        GUILayout.Space(25f);
                        EditorGUILayout.LabelField("文字：");
                        m_Text = EditorGUILayout.TextField(m_Text);
                        //*****************************************************************
                        GUILayout.Space(25f);
                        EditorGUILayout.LabelField("文件名：");
                        m_FileName = EditorGUILayout.TextField(m_FileName);
                        //*****************************************************************
                        GUILayout.Space(25f);
                        if (GUILayout.Button("合成", GUILayout.Height(50)))
                        {
                            string newPath = Path.GetCombinePath(m_SavePath, m_FileName); //m_SavePath + "/" + m_FileName;

                            if (!System.IO.File.Exists(newPath))
                            {
                                GameEntry.XFTTS.SaveMultiSpeak(m_Text, newPath);
                            }
                            else
                            {
                                Debug.Log("[" + m_FileName + "]  文件已存在！");
                            }
                        }

                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            //***************************************************************************************************************************************************************************

        }
    }

}
