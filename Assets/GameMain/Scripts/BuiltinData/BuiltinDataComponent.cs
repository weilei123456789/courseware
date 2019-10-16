//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class BuiltinDataComponent : GameFrameworkComponent
    {
        [SerializeField]
        private TextAsset m_BuildInfoTextAsset = null;

        [SerializeField]
        private TextAsset m_DefaultDictionaryTextAsset = null;

        [SerializeField]
        private GameObject m_UpdateGameFormInstanceObject = null;

        [SerializeField]
        private GameObject m_UpdateGameViceFormInstanceObject = null;

        private BuildInfo m_BuildInfo = null;

        public BuildInfo BuildInfo
        {
            get
            {
                return m_BuildInfo;
            }
        }

        public UpdateGameForm m_UpdateGameForm
        {
            get;
            private set;
        }

        public UpdateGameViceForm m_UpdateGameViceForm
        {
            get;
            private set;
        }

        public void InitBuildInfo()
        {
            if (m_BuildInfoTextAsset == null || string.IsNullOrEmpty(m_BuildInfoTextAsset.text))
            {
                Log.Info("Build info can not be found or empty.");
                return;
            }

            m_BuildInfo = Utility.Json.ToObject<BuildInfo>(m_BuildInfoTextAsset.text);
            if (m_BuildInfo == null)
            {
                Log.Warning("Parse build info failure.");
                return;
            }
        }

        public void InitDefaultDictionary()
        {
            if (m_DefaultDictionaryTextAsset == null || string.IsNullOrEmpty(m_DefaultDictionaryTextAsset.text))
            {
                Log.Info("Default dictionary can not be found or empty.");
                return;
            }

            if (!GameEntry.Localization.ParseDictionary(m_DefaultDictionaryTextAsset.text))
            {
                Log.Warning("Parse default dictionary failure.");
                return;
            }
        }

        public void InitResourceUIForm()
        {
            GameFramework.UI.IUIGroup group1 = GameEntry.UI.GetUIGroup("WallUI");
            if (group1 == null)
            {
                Log.Warning("Not find IUIGroup by name : GameUI");
                return;
            }
            UGuiGroupHelper WallUI = (UGuiGroupHelper)group1.Helper;

            GameFramework.UI.IUIGroup group2 = GameEntry.UI.GetUIGroup("GroundUI");
            if (group2 == null)
            {
                Log.Warning("Not find IUIGroup by name : GameUI");
                return;
            }
            UGuiGroupHelper GroundUI = (UGuiGroupHelper)group2.Helper;

            //更新界面
            GameObject go = Instantiate(m_UpdateGameFormInstanceObject, WallUI.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            m_UpdateGameForm = go.GetComponent<UpdateGameForm>();
            m_UpdateGameForm.gameObject.SetActive(false);

            //更新副界面
            go = Instantiate(m_UpdateGameViceFormInstanceObject, GroundUI.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            m_UpdateGameViceForm = go.GetComponent<UpdateGameViceForm>();
            m_UpdateGameViceForm.gameObject.SetActive(false);
        }

        public void ClearResourceUI()
        {
            Destroy(m_UpdateGameForm.gameObject);
            Destroy(m_UpdateGameViceForm.gameObject);
        }
    }
}
