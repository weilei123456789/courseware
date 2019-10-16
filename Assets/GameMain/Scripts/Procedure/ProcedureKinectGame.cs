using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System;
using GameFramework;
using UnityEngine.SceneManagement;

namespace Penny
{

    public class ProcedureKinectGame : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        //private string KinectGameSceneName = "KinectGame";
        private string KinectGameSceneName = "SkiingGame";
        private int BackgroundMusicId = 0;

        private int m_OpenFormSerialId = -1;
        private Dictionary<string, bool> m_UnLoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            IsEnterNextProduce = false;
            IsGrabLoginTeacher = false;
            IsBackInitProceduce = false;
            //初始化
            m_OpenFormSerialId = -1;
            m_UnLoadedFlag.Clear();

            GameEntry.Socket.ChoiceGameSuccessCallBack = ChoiceGameSuccessCallBack;
            GameEntry.Socket.BackCoursewareListSuccessCallBack = BackCoursewareListSuccessCallBack;

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);

            GameEntry.Event.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
            GameEntry.Event.Subscribe(UnloadSceneFailureEventArgs.EventId, OnUnloadSceneFailure);

            LoadKincetGameScene(5);

            GlobalData.GameStateType = GameStateType.EnterGame;
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Socket.ChoiceGameSuccessCallBack = null;
            GameEntry.Socket.BackCoursewareListSuccessCallBack = null;
            //取消事件
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
            GameEntry.Event.Unsubscribe(UnloadSceneFailureEventArgs.EventId, OnUnloadSceneFailure);
            //隐藏实体
            GameEntry.Entity.HideAllLoadedEntities();
            GameEntry.Entity.HideAllLoadingEntities();
            //关闭所用音效
            GameEntry.Sound.StopMusic();
            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            //关闭UI
            if (GameEntry.UI.HasUIForm(m_OpenFormSerialId))
                GameEntry.UI.CloseUIForm(m_OpenFormSerialId);

            GameEntry.GameManager.IsNowCam = false;

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (IsBackInitProceduce)
            {
                ChangeState<ProcedureInit>(procedureOwner);
                return;
            }

            if (IsGrabLoginTeacher)
            {
                ChangeState<ProcedureTeacherLogin>(procedureOwner);
                return;
            }

            if (IsEnterNextProduce)
            {
                ChangeState<ProcedureSelCourseware>(procedureOwner);
            }

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    m_ChooseGameIndex++;
            //    if (m_ChooseGameIndex >= m_DRGameList.Length) m_ChooseGameIndex = m_DRGameList.Length - 1;
            //    LoadKincetGameScene(m_ChooseGameIndex);
            //}
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    m_ChooseGameIndex--;
            //    if (m_ChooseGameIndex < 0) m_ChooseGameIndex = 0;
            //    LoadKincetGameScene(m_ChooseGameIndex);
            //}
        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        public void LoadKincetGameScene(int id)
        {
            //
            DRGame dRGame = GameEntry.DataTable.GetDataTable<DRGame>().GetDataRow(id);
            if (dRGame == null)
            {
                Log.Warning("DRGame Id:{0} is invalid！！！", id);
                return;
            }
            KinectGameSceneName = dRGame.AssetName;
            BackgroundMusicId = dRGame.BackgroundMusicId;
            GameEntry.GameManager.IsNowCam = false;
            //隐藏实体
            GameEntry.Entity.HideAllLoadedEntities();
            GameEntry.Entity.HideAllLoadingEntities();
            //关闭所用音效
            GameEntry.Sound.StopMusic();
            // 卸载所有场景
            m_UnLoadedFlag.Clear();
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                m_UnLoadedFlag.Add(Utility.Text.Format("UnScene.{0}", loadedSceneAssetNames[i]), false);
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i], this);
            }
            //关闭UI
            if (GameEntry.UI.HasUIForm(m_OpenFormSerialId))
                GameEntry.UI.CloseUIForm(m_OpenFormSerialId);
            //是否加载UI
            if (dRGame.NeedLoadUI && dRGame.UIFormId != -1)
            {
                m_OpenFormSerialId = (int)GameEntry.UI.OpenUIForm(dRGame.UIFormId, this);
            }
            //体感
            KinectManager.Instance.enabled = (dRGame.GameType == 1);
            //如果要卸载的场景为空
            if (loadedSceneAssetNames.Length == 0)
            {
                //加载新场景
                GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(KinectGameSceneName), Constant.AssetPriority.SceneAsset, this);
                if (BackgroundMusicId > 0)
                    GameEntry.Sound.PlayMusic(BackgroundMusicId);
            }
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
            GameEntry.GameManager.IsNowCam = true;
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
            GameEntry.GameManager.IsNowCam = false;
        }

        private void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            m_UnLoadedFlag[Utility.Text.Format("UnScene.{0}", ne.SceneAssetName)] = true;

            IEnumerator<bool> iter = m_UnLoadedFlag.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (!iter.Current) return;
            }
            //加载新场景
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(KinectGameSceneName), Constant.AssetPriority.SceneAsset, this);
            if (BackgroundMusicId > 0)
                GameEntry.Sound.PlayMusic(BackgroundMusicId);
        }

        private void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
        }

        /// <summary>
        /// 返回课件列表成功的回调
        /// </summary>
        private void BackCoursewareListSuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                GlobalData.CallServerCoursewareList();
                NextProduce();
            }
        }

        private void ChoiceGameSuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                LoadKincetGameScene(Convert.ToInt32(JsonKeyValues["id"]));
            }
        }

    }
}