using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class TeacherFaceLoginForm : UGuiForm
    {
        [SerializeField]
        private GameObject m_CourseObject = null;
        [SerializeField]
        private GameObject m_ScanFaceObject = null;
        [SerializeField]
        private GameObject m_RoleObject = null;

        [SerializeField]
        private Text m_TitleText = null;
        [SerializeField]
        private Text m_MessageText = null;
        [SerializeField]
        private Image m_OKButtonImage = null;
        [SerializeField]
        private Image m_CancelButtonImage = null;


        [SerializeField]
        private RawImage m_RawImage = null;
        [SerializeField]
        private RawImage m_TestRawImage = null;
        [SerializeField]
        private RectTransform m_RoleTransform = null;
        [SerializeField]
        private Animator m_RolePenny = null;
        [SerializeField]
        private int m_PlayerIndex = 0;

        private bool m_CollectionSuccess = false;
        private byte[] m_ImageBase64 = null;
        //private LoginData m_LoginData = null;
        //private TeachSignData m_TeachSignData = new TeachSignData();
        private string m_TeachNames = string.Empty;
        /// <summary>
        /// 理应登陆的老师信息
        /// </summary>
        private UserData m_ShouldBeTeacherUseData = null;
        /// <summary>
        /// 抢登陆的老师信息
        /// </summary>
        private UserData m_GrabTeacherUseData = null;

        private ProcedureTeacherLogin m_ProcedureLogin = null;

        private int m_TeacherFaceLoginViceSerialId = -1;
        private long m_UserId = 0;
        private Rect m_BackgroundRect = Rect.zero;
        private bool m_HasCourseInTime = false;
        private WebCamManager m_WebCamManager = null;
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_ProcedureLogin = (ProcedureTeacherLogin)userData;
            GameEntry.Socket.TeacherLoginSuccessCallBack = TeacherLoginSuccessCallBack;
            GameEntry.Windows.SubscribeUIWallEvent(OnLidarHitEvent);
            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            // 是否开启课程提示
            GlobalData.CoursesDailyMap = CoursewareManager.Instance.QueryTodayNowTimeCourse();
            m_HasCourseInTime = GlobalData.CoursesDailyMap != null;

            m_CourseObject.SetActive(m_HasCourseInTime);
            m_ScanFaceObject.SetActive(!m_HasCourseInTime);
            m_RoleObject.SetActive(!m_HasCourseInTime);
            if (m_HasCourseInTime)
            {
                State_Course();
            }
            else
            {
                State_ScanFace();
            }
            m_TeacherFaceLoginViceSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.TeacherFaceLoginViceForm, this);
        }

        protected override void OnClose(object userData)
        {
            m_RawImage.texture = null;
            Again();
            GameEntry.Windows.UnSubscribeUIWallEvent(OnLidarHitEvent);
            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            GameEntry.Socket.TeacherLoginSuccessCallBack = null;
            if (KinectManager.Instance)
                KinectManager.Instance.enabled = false;
            if (GameEntry.UI.HasUIForm(m_TeacherFaceLoginViceSerialId))
                GameEntry.UI.CloseUIForm(m_TeacherFaceLoginViceSerialId);
            if (m_WebCamManager != null)
            {
                m_WebCamManager.Clear();
                m_WebCamManager = null;
            }
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                GameEntry.Windows.WallUICameraRay(Input.mousePosition);

            KinectBehavior();
        }

        private void State_Course()
        {
            m_TitleText.text = "[课程提示]";
            m_MessageText.text = Utility.Text.Format("课程名称:[{0}]\n课程介绍:[{1}]\n上课时间:[{2}]\n结束时间:[{3}]",
                GlobalData.CoursesDailyMap.name,
                GlobalData.CoursesDailyMap.description,
                TimeUtility.GetDateTime(GlobalData.CoursesDailyMap.starttime),
                TimeUtility.GetDateTime(GlobalData.CoursesDailyMap.endtime));
        }

        private void State_ScanFace()
        {
            Again();

            if (GameEntry.WindowsConfig.Config.IsKinect)
            {
                KinectManager.Instance.enabled = true;
                m_RawImage.texture = KinectManager.Instance.GetUsersClrTex();
            }
            else
            {
                m_WebCamManager = m_RawImage.gameObject.GetOrAddComponent<WebCamManager>();
            }

            GlobalData.ClearTeacher();
            GameEntry.XFTTS.MultiSpeak("请老师将脸部对准圆圈！");
            m_ShouldBeTeacherUseData = null;
        }

        private void OnLidarHitEvent(GameObject go, Vector3 vec)
        {
            if (go == null) return;

            if (m_OKButtonImage.gameObject == go || m_CancelButtonImage.gameObject == go)
            {
                m_CourseObject.SetActive(false);
                m_ScanFaceObject.SetActive(true);
                m_RoleObject.SetActive(true);
                State_ScanFace();
                GameEntry.Windows.UnSubscribeUIWallEvent(OnLidarHitEvent);
            }

        }

        private void KinectBehavior()
        {
            if (KinectManager.Instance && KinectManager.Instance.IsInitialized() && GameEntry.Windows.WallUICamera)
            {
                m_BackgroundRect = GameEntry.Windows.WallUICamera.pixelRect;
                if (PortraitBackground.Instance && PortraitBackground.Instance.enabled)
                {
                    m_BackgroundRect = PortraitBackground.Instance.GetBackgroundRect();
                }

                if (KinectManager.Instance.IsUserDetected())
                {
                    m_UserId = KinectManager.Instance.GetUserIdByIndex(m_PlayerIndex);

                    //if (KinectManager.Instance.IsJointTracked(m_UserId, (int)KinectInterop.JointType.Head))
                    //{
                    //    Vector3 posJoint = KinectManager.Instance.GetJointPosColorOverlay(m_UserId, (int)KinectInterop.JointType.Head, GameEntry.Windows.WallUICamera, m_BackgroundRect);

                    //    m_RolePenny.transform.localPosition = posJoint;
                    //}
                    KinectTextureHelper.OverlayJoint(GameEntry.Windows.WallUICamera, m_UserId, (int)KinectInterop.JointType.Head, m_RoleTransform, m_BackgroundRect);

                    m_RoleTransform.localPosition = new Vector3(m_RoleTransform.localPosition.x, m_RoleTransform.localPosition.y, 0);

                    //Quaternion jointRotation = kinectManager.GetJointOrientation(userId, joint, flip);
                }
                else
                {
                    m_UserId = 0;
                    m_RoleTransform.anchoredPosition = Vector2.zero;
                }
                // id > 0 && [50,0,-50]
                if (m_UserId > 0 && m_RoleTransform.anchoredPosition.x < 50 && m_RoleTransform.anchoredPosition.x > -50
                    && m_RoleTransform.anchoredPosition.y < 50 && m_RoleTransform.anchoredPosition.y > -50)
                {
                    TakePhoto();
                    //Log.Info("TakePhoto-");
                }
            }
        }

        public void TakePhoto()
        {
            if (!m_CollectionSuccess)
            {
                m_CollectionSuccess = true;
                if (GameEntry.WindowsConfig.Config.IsKinect)
                {
                    Texture2D clip = KinectTextureHelper.ScaleTextureCutOut(KinectManager.Instance.GetUsersClrTex(), 535, 115, 850, 850);
                    m_ImageBase64 = clip.EncodeToJPG();
                    m_TestRawImage.texture = KinectTextureHelper.Base64ToTexter2d(m_ImageBase64);
                }
                else
                {
                    Texture2D clip = KinectTextureHelper.TextureToTexture2D(m_RawImage.texture);

                    m_ImageBase64 = clip.EncodeToJPG();
                    m_TestRawImage.texture = KinectTextureHelper.Base64ToTexter2d(m_ImageBase64);
                }


                //m_TestRawImage.SetNativeSize();
                string faceset_token = GameEntry.Face.GetFacesetTokenByOuterId("faceSetId_test");
                GameEntry.Face.SearchFace(m_ImageBase64, faceset_token, this);

                //Destroy(clip);
            }
        }

        private void Again()
        {
            m_CollectionSuccess = false;
            m_TestRawImage.texture = null;
        }

        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData != this) return;
            string responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
            Log.Info("<color=lime>" + responseJson + "</color>");
            SearchFace search = Utility.Json.ToObject<SearchFace>(responseJson);
            if (search.faces.Count > 0)
            {
                SearchFace.Results results = GameEntry.Face.MaxConfidence(search.results);
                if (results != null)
                {
                    //老师登陆为flag = 2
                    LoginServer loginServer = new LoginServer(results.face_token, GlobalData.Web_Uid, 2, LoginServerSuccess, LoginServerFailure);
                    loginServer.SendMsg();
                }
                else
                {
                    GameEntry.UI.OpenDialog(new DialogParams
                    {
                        PauseGame = false,
                        UserData = null,
                        CloseTime = 2f,
                        Title = "登陆失败",
                        Message = "该人员未注册！",
                        OnFinish = delegate (object userData) { Again(); },
                    });
                    GameEntry.XFTTS.MultiSpeak("登陆失败,该人员未注册！");
                }
            }
            else
            {
                GameEntry.UI.OpenDialog(new DialogParams
                {
                    PauseGame = false,
                    UserData = null,
                    CloseTime = 2f,
                    Title = "登陆失败",
                    Message = "请老师对准镜头！",
                    OnFinish = delegate (object userData) { Again(); },
                });
                GameEntry.XFTTS.MultiSpeak("请看镜头！");
            }
            //else if (search.faces.Count > 1)
            //{
            //    GameEntry.UI.OpenDialog(new DialogParams
            //    {
            //        PauseGame = false,
            //        UserData = null,
            //        CloseTime = 2f,
            //        Title = "登陆失败",
            //        Message = "当前登陆人员数量过多！",
            //        OnFinish = delegate (object userData) { Again(); },
            //    });
            //    GameEntry.XFTTS.MultiSpeak("登陆失败,当前登陆人员数量过多！");
            //}
            //else
            //{
            //    GameEntry.UI.OpenDialog(new DialogParams
            //    {
            //        PauseGame = false,
            //        UserData = null,
            //        CloseTime = 2f,
            //        Title = "登陆失败",
            //        Message = "请老师对准镜头！",
            //        OnFinish = delegate (object userData) { Again(); },
            //    });
            //    GameEntry.XFTTS.MultiSpeak("请看镜头！");
            //}
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData != this) { return; }
            Again();
            Log.Warning("Face++ SearchFace failure. " + ne.ErrorMessage);
        }

        private void LoginServerSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(LoginData))
            {
                LoginData data = userData as LoginData;
                if (data.Response.respVo.resultCode == 0)
                {
                    GlobalData.LoginTeacherDatas.Clear();
                    GlobalData.LoginTeacherDatas.AddRange(data.Response.userMap);
                    m_ShouldBeTeacherUseData = GlobalData.LoginTeacherDatas.Find(x =>
                    {
                        //if (string.IsNullOrEmpty(x.auid))
                        //    Log.Warning("Teacher auid is null !!!");
                        return GlobalData.Web_Uid.Equals(x.id.ToString());
                    });
                    m_GrabTeacherUseData = GlobalData.LoginTeacherDatas.Find(x => { return !GlobalData.Web_Uid.Equals(x.id.ToString()); });
                    if (m_ShouldBeTeacherUseData != null && m_GrabTeacherUseData == null)
                    {
                        if (m_ShouldBeTeacherUseData.flag == 2)
                        {
                            m_TeachNames = m_ShouldBeTeacherUseData.name;
                            // 如果有课,老师需要登录
                            if (m_HasCourseInTime)
                            {
                                HttpSendTeachSignIn(m_ShouldBeTeacherUseData.id, m_ShouldBeTeacherUseData.tel, m_ImageBase64);
                            }
                            // 没课,老师直接登录,GlobalData.SC_TeacherClassDaily = classDaily = null
                            else
                            {
                                CSTeacherLogin(null, m_ShouldBeTeacherUseData.id, m_TeachNames);
                            }
                        }
                        else
                        {
                            GameEntry.UI.OpenDialog(new DialogParams
                            {
                                PauseGame = false,
                                UserData = null,
                                CloseTime = 2f,
                                Title = "登陆失败",
                                Message = "现在是老师登陆，学员请离开采集区域！",
                                OnFinish = delegate (object user) { Again(); },
                            });
                            GameEntry.XFTTS.MultiSpeak("现在是老师登陆，学员请离开采集区域！");
                        }

                    }
                    else if (m_ShouldBeTeacherUseData != null && m_GrabTeacherUseData != null)
                    {
                        string message = "请" + m_ShouldBeTeacherUseData.name + "老师登陆!";
                        GameEntry.UI.OpenDialog(new DialogParams
                        {
                            PauseGame = false,
                            UserData = null,
                            CloseTime = 2f,
                            Title = "登陆失败",
                            Message = message,
                            OnFinish = delegate (object user) { Again(); },
                        });
                        GameEntry.XFTTS.MultiSpeak(message);
                    }
                    else
                    {
                        Again();
                    }
                }
                else
                {
                    Log.Warning("code:{0}. desc:{1}.", data.Response.respVo.resultCode, data.Response.respVo.resultDesc);
                    Again();
                }
            }
        }

        private void LoginServerFailure(string response, object userData)
        {
            if (!string.IsNullOrEmpty(response))
            {
                Log.Warning("SendFailure response : " + response);
            }
        }

        /// <summary>
        /// 发送老师登陆成功消息的回掉
        /// 进入下一流程
        /// </summary>
        /// <param name="obj"></param>
        private void TeacherLoginSuccessCallBack(SocketData socketData)
        {
            m_ProcedureLogin.NextProduce();
        }

        /// <summary>
        /// 老师签到
        /// </summary>
        /// <param name="teachId"></param>
        /// <param name="tel"></param>
        /// <param name="photo"></param>
        private void HttpSendTeachSignIn(int teachId, string tel, byte[] photo)
        {
            int cdid = GlobalData.CoursesDailyMap.id;
            string cid = CoursewareManager.Instance.ClassId;
            TeacherSignInServer teachSignInServer = new TeacherSignInServer(teachId, tel, TimeUtility.GetTimeStamp(), cdid, cid, photo,
                TeachSignInSendSuccess, TeachSignInSendFailure);
            teachSignInServer.SendMsgForm();
        }

        private void TeachSignInSendSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(TeacherSignData))
            {
                TeacherSignData m_TeachSignData = userData as TeacherSignData;
                if (m_TeachSignData.Response.respVo.resultCode == 0)
                {
                    string course = string.Format("{0}老师登陆成功!\n当前课程:{1}\n课程介绍:{2}",
                        m_TeachNames,
                        m_TeachSignData.Response.coursesDailyMap[0].name,
                        m_TeachSignData.Response.coursesDailyMap[0].description);
                    GameEntry.UI.OpenDialog(new DialogParams
                    {
                        PauseGame = false,
                        UserData = null,
                        CloseTime = 4f,
                        Title = "登陆成功",
                        Message = course,
                        OnFinish = delegate (object user)
                        {
                            CSTeacherLogin(m_TeachSignData.Response.coursesDailyMap[0].classDaily, m_ShouldBeTeacherUseData.id, m_TeachNames);
                        },
                    });
                    GameEntry.XFTTS.MultiSpeak(m_TeachNames + "老师登陆成功！");
                }
                else
                {
                    Again();
                }
            }
        }

        private void TeachSignInSendFailure(string response, object userData)
        {
            if (!string.IsNullOrEmpty(response))
            {
                Log.Warning("SendFailure response : " + response);
            }
        }

        /// <summary>
        /// 发送老师登陆成功消息
        /// </summary>
        /// <param name="teacher"></param>
        private void CSTeacherLogin(int? classDaily, int teacher, string name)
        {
            GlobalData.SC_TeacherClassDaily = classDaily;
            GlobalData.SC_TeacherUid = teacher.ToString();
            //切换状态
            GlobalData.GameStateType = GameStateType.WaitWebChoiceCourseware;
            ////本地课件数据
            //DRLesson[] dtLesson = GameEntry.DataTable.GetDataTable<DRLesson>().GetAllDataRows();
            //json数据
            Dictionary<string, object> JsonKeyValues = new Dictionary<string, object>
            {
                { "uid", GlobalData.SC_TeacherUid },
                { "state", (int)GlobalData.GameStateType },
                { "lesson", CoursewareManager.Instance.WebCourseware },
                { "name", name },
            };
            string json = Utility.Json.ToJson(JsonKeyValues);
            //发送老师登陆成功消息
            SocketDataReq teacherLoginSuccess = new SocketDataReq(NetProtocols.CSTeacherLoginSuccessProtocol, "TeacherLoginSuccess", json);
            teacherLoginSuccess.Send();
        }

    }
}