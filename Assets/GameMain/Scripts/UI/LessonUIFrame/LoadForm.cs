using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Resource;


namespace Penny
{

    

    public class LoadForm : UGuiForm
    {
        private ProcedureSelCourseware m_ProcedureSelCourseware = null;

        private int m_LoadViceFormSerild = -1;

        private int m_ServerID = -1;

        private int m_MaxLoadLength = 0;

        private Dictionary<string, bool> AssetNames = new Dictionary<string, bool>();
        //private DRLesson m_drlesson;
        //private string WallUIName;
        //private string GroundUIName;


        private int FormTrack = 0;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            AssetNames.Clear();

            FormTrack = 0;

            //m_drlesson = userData as DRLesson;

            //WallUIName = GameEntry.GameManager.UIAssetName(m_drlesson.WallID);
            //GroundUIName = GameEntry.GameManager.UIAssetName(m_drlesson.GroundID);

            //设置UI界面层级
            if (OriginalDepth < (int)UIFormId.LoadForm)
            {
                OriginalDepth += (int)UIFormId.LoadForm;
            }

            m_ProcedureSelCourseware = userData as ProcedureSelCourseware;

            m_ServerID = m_ProcedureSelCourseware.m_GameLessonServerId;
            //GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, UIFormOpenSuccess);

            m_LoadViceFormSerild = (int)GameEntry.UI.OpenUIForm(UIFormId.LoadViceForm);

            if (!GameEntry.Base.EditorResourceMode)
            {
                PreLoadAsset();
            }
        }


        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_LoadViceFormSerild))
                GameEntry.UI.CloseUIForm(m_LoadViceFormSerild);

            m_LoadViceFormSerild = -1;
            m_ServerID = -1;

            AssetNames.Clear();
            //WallUIName = string.Empty;
            //GroundUIName = string.Empty;

            //GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, UIFormOpenSuccess);
            GameEntry.UI.CloseAllLoadingUIForms();

            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.Base.EditorResourceMode) {
                m_ProcedureSelCourseware.NextProduce();
            }

            if (m_MaxLoadLength != 0) {
                if (m_MaxLoadLength >= AssetNames.Count) {
                    m_ProcedureSelCourseware.NextProduce();
                }
            }

        }

        private void PreLoadAsset() {
            DRLesson[] drles =GameEntry.GameManager.LessonsByID(m_ServerID);
            m_MaxLoadLength = 0;

            for (int i = 0; i < drles.Length; i++) {
                if (drles[i].IsAni == 0)
                {
                    AssetNames.Add(GameEntry.GameManager.UIAssetName(drles[i].WallID), false);
                    AssetNames.Add(GameEntry.GameManager.UIAssetName(drles[i].GroundID), false);
                }         
            }


            //GameEntry.Resource.LoadAsset
            foreach (string str in AssetNames.Keys) {

                GameEntry.Resource.LoadAsset(AssetUtility.GetUIFormAsset(str), new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) => {
                        AssetNames[str] = true;
                        m_MaxLoadLength++;
                        Log.Info("<color=line>预加载资源'{0}'成功</color>", str);
                    },
                    (assetName, status, errorMessage, userData) => {
                        Log.Info("<color=red>预加载资源'{0}'from'{1}'失败'{2}'</color>", str,assetName, errorMessage);
                    }));
            }
        }



        //private void UIFormOpenSuccess(object sender, GameEventArgs e) {
        //    OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
        //    if (ne.UIForm.Logic.name == UINameReplace(WallUIName)) {
        //        FormTrack++;
        //    }
        //    if (ne.UIForm.Logic.name == UINameReplace(GroundUIName))
        //    {
        //        FormTrack++;
        //    }
        //    Log.Info("<color=SkyBlue>" + ne.UIForm.Logic.name + "</color>");
        //    if (FormTrack >= 2) {
        //        Close();
        //    }
           
        //}

        private string UINameReplace(string str) {
            return str.Replace("(Clone)", "");
        }
      
    }
}