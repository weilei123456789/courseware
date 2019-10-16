using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using GameFramework.Event;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System;
using GameFramework;

namespace Penny
{

    public class ProcedureGame : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public int m_GameLessonLocalId = -1;
        public int m_GameLessonServerId = -1;
        public int m_GameDifficultyIndex = 1;



        /// <summary>
        /// 按键操作和遥控器界面
        /// </summary>
        private int m_PenPublicFormId = -1;

        private int SC_Id = 0;
        private int SC_ServerId = 0;
        private int SC_IsAni = 0;
        private int SC_HasFollow;


        private int m_StudentFaceLoginFormSerialId = -1;
        private int m_VideoPlayerFormSerialId = -1;
        private Dictionary<string, object> m_JsonKeyValues = new Dictionary<string, object>();
        private string[] m_Diff = new string[] { "简单难度", "中等难度", "困难难度" };


        private static DRLesson[] NowDrLessonList = null;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            IsEnterNextProduce = false;
            IsGrabLoginTeacher = false;
            IsBackInitProceduce = false;

            GameEntry.GameManager.IsNowCam = false;


#if TestLine
            //GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NormalDifficulty, 7);
            //GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NumberDifficulty, 4);
            GlobalData.GameDifficulty = 1;
            GlobalData.HumanNumber = 10;
            GameEntry.Event.Subscribe(PenPublicEventArgs.EventId, OnPenPublicChange);
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);

            GameEntry.GameManager.OpenGameUIByID(5002);

            LoadGameScene();
            return;
#endif

            SC_Id = 0;
            SC_ServerId = 0;
            GameEntry.Socket.ChangeCoursewareSuccessCallBack = ChangeCoursewareSuccessCallBack;
            GameEntry.Socket.ChangeStudentLoginSuccessCallBack = ChangeStudentLoginSuccessCallBack;
            GameEntry.Socket.BackCoursewareListSuccessCallBack = BackCoursewareListSuccessCallBack;
            GameEntry.Socket.OperationDifficultySuccessCallBack = OperationDifficultySuccessCallBack;
            GameEntry.Socket.ChoiceThemeSongSuccessCallBack = ChoiceThemeSongSuccessCallBack;
            GameEntry.Socket.ChoiceWarmUpSuccessCallBack = ChoiceWarmUpSuccessCallBack;
            GameEntry.Socket.ChoiceScreenSaverSuccessCallBack = ChoiceScreenSaverSuccessCallBack;
            GameEntry.Socket.ChoiceRelaxSuccessCallBack = ChoiceRelaxSuccessCallBack;

            m_GameLessonLocalId = procedureOwner.GetData<VarInt>(Constant.ProcedureData.GameLessonLocalId).Value;
            m_GameLessonLocalId = -1;
            m_GameLessonServerId = procedureOwner.GetData<VarInt>(Constant.ProcedureData.GameLessonServerId).Value;
            m_GameDifficultyIndex = 1;

            m_StudentFaceLoginFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.StudentFaceLoginForm, this);
            m_VideoPlayerFormSerialId = -1;

            //GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NormalDifficulty, m_GameDifficultyIndex);
            //GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NumberDifficulty, 4);
            GlobalData.GameDifficulty = m_GameDifficultyIndex;
            GlobalData.HumanNumber = 10;

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Subscribe(PenPublicEventArgs.EventId, OnPenPublicChange);

            LoadGameScene();

            GlobalData.GameStateType = GameStateType.EnterCourseware;
            {
                //本地课件数据
                DRLesson[] dtLesson = GameEntry.DataTable.GetDataTable<DRLesson>().GetDataRows((x) => { return x.SeverID == m_GameLessonServerId; });
                m_JsonKeyValues.Clear();
                m_JsonKeyValues.Add("uid", GlobalData.SC_TeacherUid);
                m_JsonKeyValues.Add("state", (int)GlobalData.GameStateType);
                m_JsonKeyValues.Add("serverID", m_GameLessonServerId);
                m_JsonKeyValues.Add("lesson", dtLesson);
                SocketDataReq m_LoginReq = new SocketDataReq(NetProtocols.CSSetGameStateSuccessProtocol, "SetState", Utility.Json.ToJson(m_JsonKeyValues));
                m_LoginReq.Send();
            }

            //根据serverID 建立课件列表
            if (NowDrLessonList != null && NowDrLessonList.Length > 0)
            {
                if (NowDrLessonList[0].SeverID != m_GameLessonServerId)
                {
                    NowDrLessonList = GameEntry.GameManager.LessonsByID(m_GameLessonServerId);
                }
            }
            else
            {
                NowDrLessonList = GameEntry.GameManager.LessonsByID(m_GameLessonServerId);
            }

            TeacherStartClass();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Socket.ChangeCoursewareSuccessCallBack = null;
            GameEntry.Socket.ChangeStudentLoginSuccessCallBack = null;
            GameEntry.Socket.BackCoursewareListSuccessCallBack = null;
            GameEntry.Socket.OperationDifficultySuccessCallBack = null;
            GameEntry.Socket.ChoiceThemeSongSuccessCallBack = null;
            GameEntry.Socket.ChoiceWarmUpSuccessCallBack = null;
            GameEntry.Socket.ChoiceScreenSaverSuccessCallBack = null;
            GameEntry.Socket.ChoiceRelaxSuccessCallBack = null;

            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(PenPublicEventArgs.EventId, OnPenPublicChange);

            ClearUIFormById();

            GameEntry.GameManager.IsNowCam = false;
            m_GameLessonLocalId = -1;
            m_GameLessonServerId = -1;

            GlobalData.ClearStudents();

            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);

            TeacherEndClass();



            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);


            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    LessonIndex--;              
            //    SwitchLesson();
            //}

            //if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    LessonIndex++;              
            //    SwitchLesson();
            //}

            ///返回课件列表界面
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextProduce();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (GameEntry.UI.HasUIForm(m_StudentFaceLoginFormSerialId))
                {
                    LessonIndex = 0;
                    SwitchLesson();
                }
            }



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


        }

        /// <summary>
        /// 遥控笔切换课件
        /// </summary>
        private int LessonIndex = 0;
        public void SwitchLesson()
        {
            ClearUIFormById();
            if (LessonIndex >= NowDrLessonList.Length)
            {

                LessonIndex = 0;
            }

            if (LessonIndex < 0)
            {
                LessonIndex = NowDrLessonList.Length - 1;
            }
            DRLesson drlesson = NowDrLessonList[LessonIndex];
            SC_Id = drlesson.Id;
            SC_ServerId = drlesson.SeverID;
            SC_IsAni = drlesson.IsAni;

            if (SC_IsAni == 0)
            {
                GameEntry.GameManager.OpenGameUIByID(m_GameLessonLocalId = SC_Id);
                //打开遥控笔界面  
                m_PenPublicFormId = (int)GameEntry.UI.OpenUIForm(UIFormId.PenPublicForm, new PenPublicParams
                {
                    IsGame = true,
                    IsSkip = false,
                });
            }
            if (SC_IsAni == 1)
            {
                OpenLessonMovieForm(drlesson);
            }
        }




        private void OnPenPublicChange(object sender, GameEventArgs e)
        {
            PenPublicEventArgs ne = e as PenPublicEventArgs;
            PenPublicEventParams parms = ne.PenParams;

            switch (parms.BtnClick)
            {
                case PenPublicBtnClick.OnClickSkip:

                    break;

                case PenPublicBtnClick.OnClickTwiceSkipTwice:
                    LessonIndex++;
                    SwitchLesson();
                    break;

            }
        }



        private void ClearUIFormById()
        {
            //关闭学员登陆
            if (GameEntry.UI.HasUIForm(m_StudentFaceLoginFormSerialId))
            {
                GameEntry.UI.CloseUIForm(m_StudentFaceLoginFormSerialId);
                m_StudentFaceLoginFormSerialId = -1;
            }
            //关闭video
            if (GameEntry.UI.HasUIForm(m_VideoPlayerFormSerialId))
            {
                GameEntry.UI.CloseUIForm(m_VideoPlayerFormSerialId);
                m_VideoPlayerFormSerialId = -1;
            }
            //关闭课件
            if (m_GameLessonLocalId != -1)
            {
                GameEntry.GameManager.CloseUIByID(m_GameLessonLocalId);
            }

            //关闭遥控笔界面
            if (GameEntry.UI.HasUIForm(m_PenPublicFormId))
            {
                GameEntry.UI.CloseUIForm(m_PenPublicFormId);
                m_PenPublicFormId = -1;
            }
        }

        /// <summary>
        /// 切换小课件成功的回调
        /// </summary>
        private void ChangeCoursewareSuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                SC_Id = Convert.ToInt32(JsonKeyValues["id"]);
                SC_ServerId = Convert.ToInt32(JsonKeyValues["serverID"]);
                if (SC_ServerId != m_GameLessonServerId)
                {
                    //根据serverID 建立课件列表
                    if (NowDrLessonList != null && NowDrLessonList.Length > 0)
                    {
                        if (NowDrLessonList[0].SeverID != SC_ServerId)
                        {
                            NowDrLessonList = GameEntry.GameManager.LessonsByID(SC_ServerId);
                        }
                    }
                    else
                    {
                        NowDrLessonList = GameEntry.GameManager.LessonsByID(SC_ServerId);
                    }
                }

                for (int i = 0; i < NowDrLessonList.Length; i++)
                {
                    if (NowDrLessonList[i].Id == SC_Id)
                    {
                        LessonIndex = i;
                        break;
                    }
                }

                DRLesson drlesson = GameEntry.GameManager.LessonByID(SC_Id);
                SC_IsAni = drlesson.IsAni;
                //SC_HasFollow = drlesson.HasFollow;

                ClearUIFormById();

                if (SC_IsAni == 0)
                {
                    GameEntry.GameManager.OpenGameUIByID(m_GameLessonLocalId = SC_Id);
                    //打开遥控笔界面  
                    m_PenPublicFormId = (int)GameEntry.UI.OpenUIForm(UIFormId.PenPublicForm, new PenPublicParams
                    {
                        IsGame = true,
                        IsSkip = false,
                    });
                }
                if (SC_IsAni == 1)
                {
                    OpenLessonMovieForm(drlesson);
                }
            }
        }

        /// <summary>
        /// 开启视频界面
        /// </summary>
        private void OpenLessonMovieForm(DRLesson drlesson)
        {


            VideoPlayer.EventHandler playbak = null;
            //Sprite sp = new Sprite();
            //ResourceUtility.LoadLessonUISprite(drlesson.GroundBG, drlesson.SeasonPath, drlesson.LessonPath, sp);
            SC_HasFollow = drlesson.HasFollow;

            if (SC_HasFollow == 1)
            {
                playbak = (TempSource) =>
                {
                    TempSource = GameEntry.VideoPlayer.VPSource;
                    if (SC_HasFollow == 1)
                    {
                        LessonIndex++;
                        SwitchLesson();
                    }
                };
            }
            else
            {
                playbak = null;
            }

            m_VideoPlayerFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerForm, new VideoPlayerParams
            {
                Name = drlesson.AniName,
                HasCallBack = true,
                PlayEndCallBack = playbak,
                HasGSprite = true,
                delesson = drlesson,
            });
        }

        /// <summary>
        /// 切换学员登陆成功的回调
        /// </summary>
        private void ChangeStudentLoginSuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                ClearUIFormById();
                //打开学员登陆
                if (m_StudentFaceLoginFormSerialId == -1)
                    m_StudentFaceLoginFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.StudentFaceLoginForm, this);
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

        /// <summary>
        /// Web难度按钮成功的回调
        /// </summary>
        private void OperationDifficultySuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                m_GameDifficultyIndex = Convert.ToInt32(JsonKeyValues["difficulty"]);

                GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NormalDifficulty, m_GameDifficultyIndex);
                GlobalData.GameDifficulty = m_GameDifficultyIndex;

                GameEntry.UI.OpenDialog(new DialogParams
                {
                    PauseGame = false,
                    UserData = null,
                    CloseTime = 1.5f,
                    Title = "难度更改",
                    Message = "难度：" + m_Diff[m_GameDifficultyIndex - 1],
                    OnFinish = delegate (object userData) { },
                });
                GameEntry.XFTTS.MultiSpeak("难度：" + m_Diff[m_GameDifficultyIndex - 1]);

                //TODO： 难度改变事件
                NormalDifficultyEventArgs ne = new NormalDifficultyEventArgs(m_GameDifficultyIndex);
                GameEntry.Event.Fire(this, ne);
            }
        }

        /// <summary>
        /// Web选择主题曲
        /// </summary>
        private void ChoiceThemeSongSuccessCallBack(SocketData socketData)
        {
            ClearUIFormById();
            m_VideoPlayerFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerForm, new VideoPlayerParams { Name = "xPiNiMV", IsLoop = true });
        }

        /// <summary>
        /// Web选择热身
        /// </summary>
        private void ChoiceWarmUpSuccessCallBack(SocketData socketData)
        {
            ClearUIFormById();
            m_VideoPlayerFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerForm, new VideoPlayerParams { Name = "xPiNiMV", IsLoop = true });
        }

        /// <summary>
        /// Web选择屏保
        /// </summary>
        private void ChoiceScreenSaverSuccessCallBack(SocketData socketData)
        {
            ClearUIFormById();
            m_VideoPlayerFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerForm, new VideoPlayerParams { Name = "xPiNiMV", IsLoop = true });
        }

        /// <summary>
        /// Web选择放松
        /// </summary>
        private void ChoiceRelaxSuccessCallBack(SocketData socketData)
        {
            ClearUIFormById();
            m_VideoPlayerFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerForm, new VideoPlayerParams { Name = "xPiNiMV", IsLoop = true });
        }

        /// <summary>
        ///  老师开始上课
        /// </summary>
        private void TeacherStartClass()
        {
            if (GlobalData.SC_TeacherClassDaily == null) return;
            UpdateClassDailyStarttimeInServer updateClassDailyStarttimeInServer = new UpdateClassDailyStarttimeInServer(
                (int)GlobalData.SC_TeacherClassDaily, SendSuccess, SendFailed);
            updateClassDailyStarttimeInServer.SendMsg();
        }

        /// <summary>
        /// 老师结束上课
        /// </summary>
        private void TeacherEndClass()
        {
            if (GlobalData.SC_TeacherClassDaily == null) return;
            UpdateClassDailyEndtimeInServer updateClassDailyEndtimeInServer = new UpdateClassDailyEndtimeInServer(
                (int)GlobalData.SC_TeacherClassDaily, SendSuccess, SendFailed);
            updateClassDailyEndtimeInServer.SendMsg();
        }

        private void SendSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
        }

        private void SendFailed(string error, IDictable userData)
        {
            Log.Info("SendFailed Error : " + error);
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


        /// <summary>
        /// 加载游戏场景
        /// </summary>
        public void LoadGameScene()
        {
            //加载场景
            int sceneId = GameEntry.Config.GetInt("Scene.Game");
            IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            DRScene drScene = dtScene.GetDataRow(sceneId);
            if (drScene == null)
            {
                Log.Warning("Can not load scene '{0}' from data table.", sceneId.ToString());
                return;
            }
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset, this);
        }
    }
}