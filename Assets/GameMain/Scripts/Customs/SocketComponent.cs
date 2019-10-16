using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class SocketComponent : GameFrameworkComponent
    {
        [SerializeField]
        [Header("从Config中读取IP")]
        private string m_IP = "";
        [SerializeField]
        [Header("从Config中读取PORT")]
        private int m_Port = 0;
        [SerializeField]
        [Header("是否发送心跳")]
        private bool m_IsSendHeart = true;
        [SerializeField]
        [Header("心跳发送间隔")]
        private int m_HeartBeatInterval = 1;

        private SocketHelper m_SocketHelper = new SocketHelper();
        private ProtoManager m_ProtoManager = new ProtoManager();
        private float m_CurHeartBeatInterval = 0;
        /// <summary>
        /// 心跳包
        /// </summary>
        private HeartBeatReq m_HeartBeatReq = null;
        /// <summary>
        /// 进入老师人脸扫描
        /// </summary>
        public bool IsEnterTeacherFaceLogin { private set; get; }

        /// <summary>
        /// 老师登陆成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> TeacherLoginSuccessCallBack = null;
        /// <summary>
        /// Web选择大课件成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceCoursewareSuccessCallBack = null;
        /// <summary>
        /// Web切换小课件成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChangeCoursewareSuccessCallBack = null;
        /// <summary>
        /// Web切换学员登陆成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChangeStudentLoginSuccessCallBack = null;
        /// <summary>
        /// Web返回课件列表成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> BackCoursewareListSuccessCallBack = null;
        /// <summary>
        /// Web难度按钮成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> OperationDifficultySuccessCallBack = null;
        /// <summary>
        /// Web音量按钮成功的回调
        /// </summary>
        //public GameFrameworkAction<SocketData> OperationVolumeSuccessCallBack = null;
        /// <summary>
        /// Web选择主题曲成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceThemeSongSuccessCallBack = null;
        /// <summary>
        /// Web选择热身成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceWarmUpSuccessCallBack = null;
        /// <summary>
        /// Web选择屏保成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceScreenSaverSuccessCallBack = null;
        /// <summary>
        /// Web选择放松成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceRelaxSuccessCallBack = null;
        /// <summary>
        /// Web选择游戏成功的回调
        /// </summary>
        public GameFrameworkAction<SocketData> ChoiceGameSuccessCallBack = null;

        protected void Start()
        {
            IsEnterTeacherFaceLogin = false;
            TeacherLoginSuccessCallBack = null;
            ChoiceCoursewareSuccessCallBack = null;
            ChangeCoursewareSuccessCallBack = null;
            ChangeStudentLoginSuccessCallBack = null;
            BackCoursewareListSuccessCallBack = null;
            OperationDifficultySuccessCallBack = null;
            //OperationVolumeSuccessCallBack = null;
            ChoiceThemeSongSuccessCallBack = null;
            ChoiceWarmUpSuccessCallBack = null;
            ChoiceScreenSaverSuccessCallBack = null;
            ChoiceRelaxSuccessCallBack = null;

            Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
            {
                { "Heart", "Heart" }
            };
            m_HeartBeatReq = new HeartBeatReq("Heart", MiniJson.Serialize(CallJsonKeyValues));
            AddRespDelegates();
        }

        protected void OnDestroy()
        {
            IsEnterTeacherFaceLogin = false;
            TeacherLoginSuccessCallBack = null;
            ChoiceCoursewareSuccessCallBack = null;
            ChangeCoursewareSuccessCallBack = null;
            ChangeStudentLoginSuccessCallBack = null;
            BackCoursewareListSuccessCallBack = null;
            OperationDifficultySuccessCallBack = null;
            //OperationVolumeSuccessCallBack = null;
            ChoiceThemeSongSuccessCallBack = null;
            ChoiceWarmUpSuccessCallBack = null;
            ChoiceScreenSaverSuccessCallBack = null;
            ChoiceRelaxSuccessCallBack = null;
            SocketShutdown();
            DelRespDelegates();
        }

        protected void Update()
        {
            if (m_SocketHelper != null)
                m_SocketHelper.SocketUpdate(m_ProtoManager);
            if (m_IsSendHeart)
            {
                m_CurHeartBeatInterval += Time.deltaTime;
                if (m_CurHeartBeatInterval >= m_HeartBeatInterval)
                {
                    SendHeart();
                    m_CurHeartBeatInterval = 0;
                }
            }
        }

        public void CreateNetworkChannel(string ip, int port)
        {
            m_IP = ip;
            m_Port = port;
            m_SocketHelper.RegisterResp = RegisterResp;
            m_SocketHelper.Connect(m_IP, m_Port, ConnectSuccess, ConnectFailed);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="req"></param>
        public void SendMessage(Request req)
        {
            if (m_SocketHelper != null)
                m_SocketHelper.SendMessage(req);
            else
                Log.Warning("Socket Invalid!!!! ");
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        private void SocketShutdown()
        {
            Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
            {
                { "uid", GlobalData.SC_TeacherUid }
            };
            SocketDataReq callServer = new SocketDataReq(NetProtocols.CSGameRestartSuccessProtocol, "GameRestart", Utility.Json.ToJson(CallJsonKeyValues));
            callServer.Send();

            if (m_SocketHelper != null)
                m_SocketHelper.Closed();
        }

        /// <summary>
        /// 心跳包
        /// </summary>
        private void SendHeart()
        {
            if (m_HeartBeatReq != null)
                m_HeartBeatReq.Send();
        }

        /// <summary>
        /// 所有response必须要在这里注册
        /// </summary>
        private void RegisterResp()
        {
            m_ProtoManager.AddProtocol<HeartBeatResp>(NetProtocols.CSHeartBeatProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSHeartConnectProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSTeacherLoginSuccessProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSCallServerCoursewareListProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSSetGameStateSuccessProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSGameRestartSuccessProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSChangeTeacherSuccessProtocol);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSClassIsOverSuccessProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.CSGameListSuccessProtocols);

            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCLoginProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceCoursewareProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChangeCoursewareProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChangeStudentLoginProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCBackCoursewareListProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCOperationDifficultyProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCOperationVolumeProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCGameRestartProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChangeTeacherProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCClassIsOverProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceThemeSongProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceWarmUpProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceScreenSaverProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceRelaxProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCEnterGameProtocols);
            m_ProtoManager.AddProtocol<SocketDataResp>(NetProtocols.SCChoiceGameProtocols);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void AddRespDelegates()
        {
            m_ProtoManager.AddRespDelegate(NetProtocols.CSHeartBeatProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSHeartConnectProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSTeacherLoginSuccessProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSCallServerCoursewareListProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSSetGameStateSuccessProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSGameRestartSuccessProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSChangeTeacherSuccessProtocol, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSClassIsOverSuccessProtocols, CSResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.CSGameListSuccessProtocols, CSResponse);

            m_ProtoManager.AddRespDelegate(NetProtocols.SCLoginProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceCoursewareProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChangeCoursewareProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChangeStudentLoginProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCBackCoursewareListProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCOperationDifficultyProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCOperationVolumeProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCGameRestartProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChangeTeacherProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCClassIsOverProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceThemeSongProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceWarmUpProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceScreenSaverProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceRelaxProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCEnterGameProtocols, SCResponse);
            m_ProtoManager.AddRespDelegate(NetProtocols.SCChoiceGameProtocols, SCResponse);
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        private void DelRespDelegates()
        {
            m_ProtoManager.DelRespDelegate(NetProtocols.CSHeartBeatProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSHeartConnectProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSTeacherLoginSuccessProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSCallServerCoursewareListProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSSetGameStateSuccessProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSGameRestartSuccessProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSChangeTeacherSuccessProtocol, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSClassIsOverSuccessProtocols, CSResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.CSGameListSuccessProtocols, CSResponse);

            m_ProtoManager.DelRespDelegate(NetProtocols.SCLoginProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceCoursewareProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChangeCoursewareProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChangeStudentLoginProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCBackCoursewareListProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCOperationDifficultyProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCOperationVolumeProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCGameRestartProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChangeTeacherProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCClassIsOverProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceThemeSongProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceWarmUpProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceScreenSaverProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceRelaxProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCEnterGameProtocols, SCResponse);
            m_ProtoManager.DelRespDelegate(NetProtocols.SCChoiceGameProtocols, SCResponse);
        }

        private void ConnectSuccess()
        {
            Log.Info("Socket Connect Success, Start Send Login!!!");

            Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
            {
                { "Login", "Login" }
            };
            SocketDataReq m_LoginReq = new SocketDataReq(NetProtocols.CSHeartConnectProtocol, "Login", MiniJson.Serialize(CallJsonKeyValues));
            m_LoginReq.Send();
        }

        private void ConnectFailed()
        {
            Log.Info("<color=red>{0}</color>", "Socket Connect Failed!!!");
        }

        /// <summary>
        /// client to server 接受的数据
        /// </summary>
        /// <param name="response"></param>
        private void CSResponse(Resp response)
        {
            if (NetProtocols.CSHeartBeatProtocol == response.GetProtocol())
            {
                HeartBeatResp resp = response as HeartBeatResp;
                Log.Info("C - S : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
            }
            else if (NetProtocols.CSHeartConnectProtocol == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Info("C - S : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
            }
            else if (NetProtocols.CSTeacherLoginSuccessProtocol == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Info("C - S : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (TeacherLoginSuccessCallBack != null)
                    TeacherLoginSuccessCallBack(resp.RespSocketData);
            }

        }

        /// <summary>
        /// server to client 接受的数据
        /// </summary>
        /// <param name="response"></param>
        private void SCResponse(Resp response)
        {
            /// 可以登陆
            if (NetProtocols.SCLoginProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                //TODO： 进入老师人脸扫描
                Dictionary<string, object> JsonKeyValues = resp.RespSocketData.data as Dictionary<string, object>;
                if (JsonKeyValues != null)
                {
                    GlobalData.Web_Uid = Convert.ToString(JsonKeyValues["uid"]);
                    IsEnterTeacherFaceLogin = true;
                }

            }
            /// Web选择大课件
            else if (NetProtocols.SCChoiceCoursewareProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceCoursewareSuccessCallBack != null)
                    ChoiceCoursewareSuccessCallBack(resp.RespSocketData);
            }
            /// Web切换小课件
            else if (NetProtocols.SCChangeCoursewareProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChangeCoursewareSuccessCallBack != null)
                    ChangeCoursewareSuccessCallBack(resp.RespSocketData);
            }
            /// Web切换学员登陆
            else if (NetProtocols.SCChangeStudentLoginProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChangeStudentLoginSuccessCallBack != null)
                    ChangeStudentLoginSuccessCallBack(resp.RespSocketData);
            }
            /// Web返回课件列表
            else if (NetProtocols.SCBackCoursewareListProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                //game流程返回逻辑注册过事件
                if (BackCoursewareListSuccessCallBack != null)
                    BackCoursewareListSuccessCallBack(resp.RespSocketData);
                //而选择课件流程中，没有注册，服务器请求时需要返回课件信息
                else
                    GlobalData.CallServerCoursewareList();
            }
            /// Web难度按钮
            else if (NetProtocols.SCOperationDifficultyProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (OperationDifficultySuccessCallBack != null)
                    OperationDifficultySuccessCallBack(resp.RespSocketData);
            }
            /// Web音量按钮
            else if (NetProtocols.SCOperationVolumeProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                Dictionary<string, object> JsonKeyValues = resp.RespSocketData.data as Dictionary<string, object>;
                if (JsonKeyValues != null)
                {
                    int volume = Convert.ToInt32(JsonKeyValues["volume"]);

                    bool isOpen = GameEntry.UI.HasUIForm(UIFormId.SettingForm);
                    if (isOpen)
                    {
                        SettingForm settingForm = (SettingForm)GameEntry.UI.GetUIForm(UIFormId.SettingForm);
                        if (settingForm != null)
                        {
                            settingForm.Volume(volume * 0.01f);
                        }
                    }
                    else
                    {
                        GameEntry.UI.OpenUIForm(UIFormId.SettingForm, volume * 0.01f);
                    }
                }
            }
            /// Web发起重启
            else if (NetProtocols.SCGameRestartProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), "需要重启！");

                Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
                {
                    { "uid", GlobalData.SC_TeacherUid }
                };
                SocketDataReq callServer = new SocketDataReq(NetProtocols.CSGameRestartSuccessProtocol, "WebGameRestart", Utility.Json.ToJson(CallJsonKeyValues));
                callServer.Send();

                ProcedureInit.s_IsNeedInitResource = true;
                GlobalData.RestartGame();
                GameEntry.Sound.StopMusic();
                UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
            }
            /// Web切换教师顶替
            else if (NetProtocols.SCChangeTeacherProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                Dictionary<string, object> JsonKeyValues = resp.RespSocketData.data as Dictionary<string, object>;
                if (JsonKeyValues != null)
                {
                    string uid = Convert.ToString(JsonKeyValues["uid"]);
                    string userUid = Convert.ToString(JsonKeyValues["userUid"]);

                    ProcedureBase procedureBase = (ProcedureBase)GameEntry.Procedure.CurrentProcedure;
                    if (procedureBase != null)
                    {
                        GameEntry.Sound.StopMusic();
                        GlobalData.RestartGame();
                        procedureBase.GrabLoginTeacher();
                        //重新赋值
                        GlobalData.Web_Uid = uid;
                        Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
                        {
                            { "uid", uid },
                            { "userUid", userUid }
                        };
                        SocketDataReq callServer = new SocketDataReq(NetProtocols.CSChangeTeacherSuccessProtocol, "GrabLoginTeache", Utility.Json.ToJson(CallJsonKeyValues));
                        callServer.Send();

                    }
                    else
                    {
                        Log.Error("流程出错：{0}", GameEntry.Procedure.CurrentProcedure.ToString());
                    }
                }
            }
            /// Web下课（未用上）
            else if (NetProtocols.SCClassIsOverProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                Dictionary<string, object> JsonKeyValues = resp.RespSocketData.data as Dictionary<string, object>;
                if (JsonKeyValues != null)
                {
                    string uid = Convert.ToString(JsonKeyValues["uid"]);
                    ProcedureBase procedureBase = (ProcedureBase)GameEntry.Procedure.CurrentProcedure;
                    if (procedureBase != null)
                    {
                        GameEntry.Sound.StopMusic();
                        GlobalData.RestartGame();
                        procedureBase.BackInitProceduce();
                        ProcedureInit.s_IsNeedInitResource = false;
                        //告诉服务器下课成功
                        Dictionary<string, object> CallJsonKeyValues = new Dictionary<string, object>
                        {
                            { "uid", uid },
                        };
                        SocketDataReq callServer = new SocketDataReq(NetProtocols.CSClassIsOverSuccessProtocols, "ClassIsOver", Utility.Json.ToJson(CallJsonKeyValues));
                        callServer.Send();
                    }
                    else
                    {
                        Log.Error("流程出错：{0}", GameEntry.Procedure.CurrentProcedure.ToString());
                    }
                }
            }
            /// Web选择主题曲
            else if (NetProtocols.SCChoiceThemeSongProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceThemeSongSuccessCallBack != null)
                    ChoiceThemeSongSuccessCallBack(resp.RespSocketData);
            }
            /// Web选择热身
            else if (NetProtocols.SCChoiceWarmUpProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceWarmUpSuccessCallBack != null)
                    ChoiceWarmUpSuccessCallBack(resp.RespSocketData);
            }
            /// Web选择屏保
            else if (NetProtocols.SCChoiceScreenSaverProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceScreenSaverSuccessCallBack != null)
                    ChoiceScreenSaverSuccessCallBack(resp.RespSocketData);
            }
            /// Web选择放松
            else if (NetProtocols.SCChoiceRelaxProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceRelaxSuccessCallBack != null)
                    ChoiceRelaxSuccessCallBack(resp.RespSocketData);
            }
            /// Web进入选择游戏
            else if (NetProtocols.SCEnterGameProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                ProcedureSelCourseware procedureInit = (ProcedureSelCourseware)GameEntry.Procedure.CurrentProcedure;
                if (procedureInit != null)
                {
                    GlobalData.CallServerGameList();
                    procedureInit.EnterKinectGame();
                }
                else
                {
                    Log.Error("流程出错：{0}", GameEntry.Procedure.CurrentProcedure.ToString());
                }
            }
            /// Web选择放松
            else if (NetProtocols.SCChoiceGameProtocols == response.GetProtocol())
            {
                SocketDataResp resp = response as SocketDataResp;
                Log.Warning("S - C : protocol :{0} data : {1}", response.GetProtocol(), MiniJson.Serialize(resp.RespSocketData.data));
                if (ChoiceGameSuccessCallBack != null)
                    ChoiceGameSuccessCallBack(resp.RespSocketData);
            }
        }

    }

}