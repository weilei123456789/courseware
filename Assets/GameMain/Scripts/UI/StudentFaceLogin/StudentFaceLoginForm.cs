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
    public class StudentFaceLoginForm : UGuiForm
    {
        [SerializeField]
        private RawImage m_RawImage = null;

        [SerializeField]
        private RawImage m_TestRawImage = null;

        [SerializeField]
        private Transform m_RoleTransform = null;

        [SerializeField]
        private Animator m_RolePenny = null;

        [SerializeField]
        private int m_PlayerIndex = 0;

        [SerializeField]
        private Text m_StudentNum = null;

        [SerializeField]
        private Text m_StudentLoginTips = null;

        private bool m_CollectionSuccess = false;
        private byte[] m_ImageBase64 = null;
        private string m_TipsConext = string.Empty;
        private int m_StudentFaceLoginViceSerialId = -1;

        private bool m_IsAgain = false;
        private float m_AgainTime = 0;

        private long m_UserId = 0;
        private Rect m_BackgroundRect = Rect.zero;
        private CoursesDailyMap m_CoursesDailyMap = null;

        private WebCamManager m_WebCamManager = null;
        /// <summary>
        /// 登陆学员信息
        /// </summary>
        private UserData m_LoginStudentData = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            Again();
            StudentNum(0);
            LoginTips(string.Empty, Color.white);
            //KinectManager.Instance.enabled = true;
            //m_RawImage.texture = KinectManager.Instance.GetUsersClrTex();
            SwitchCamera();

            GameEntry.XFTTS.MultiSpeak("请学员将脸部对准圆圈！");
            m_StudentFaceLoginViceSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.StudentFaceLoginViceForm, this);
            m_CoursesDailyMap = CoursewareManager.Instance.QueryTodayNowTimeCourse();
            m_IsAgain = false;
            m_CollectionSuccess = false;
            m_AgainTime = 0;

        }

        protected override void OnClose(object userData)
        {
            m_RawImage.texture = null;
            Again();
            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            if (KinectManager.Instance)
                KinectManager.Instance.enabled = false;
            if (GameEntry.UI.HasUIForm(m_StudentFaceLoginViceSerialId))
                GameEntry.UI.CloseUIForm(m_StudentFaceLoginViceSerialId);
            if (m_WebCamManager != null)
            {
                m_WebCamManager.Clear();
                m_WebCamManager = null;
            }
            m_IsAgain = false;
            m_AgainTime = 0;
            m_LoginStudentData = null;
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_IsAgain)
            {
                m_AgainTime += elapseSeconds;
                if (m_AgainTime > 2f)
                {
                    m_CollectionSuccess = false;
                    m_AgainTime = 0;
                    m_IsAgain = false;
                }
            }

            KinectBehavior();
        }
        /// <summary>
        /// 切换选择Kinect还是普通camera
        /// </summary>
        private void SwitchCamera()
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

                    KinectTextureHelper.OverlayJoint(GameEntry.Windows.WallUICamera, m_UserId, (int)KinectInterop.JointType.Head, m_RoleTransform, m_BackgroundRect);
                    m_RoleTransform.localPosition = new Vector3(m_RoleTransform.localPosition.x, m_RoleTransform.localPosition.y, 0);
                }
                else
                {
                    m_UserId = 0;
                    m_RoleTransform.localPosition = Vector3.zero;
                }
                // id > 0 && [50,0,-50]
                if (m_UserId > 0 && m_RoleTransform.localPosition.x < 50 && m_RoleTransform.localPosition.x > -50
                    && m_RoleTransform.localPosition.y < 50 && m_RoleTransform.localPosition.y > -50)
                {
                    TakePhoto();
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

                //m_ImageBase64 = KinectManager.Instance.GetUsersClrTex().EncodeToJPG();
                //Texture2D clip = KinectTextureHelper.ScaleTextureCutOut(KinectManager.Instance.GetUsersClrTex(), 535, 115, 850, 850);
                //m_ImageBase64 = clip.EncodeToJPG();
                //m_TestRawImage.texture = KinectTextureHelper.Base64ToTexter2d(m_ImageBase64);
                //m_TestRawImage.SetNativeSize();
                string faceset_token = GameEntry.Face.GetFacesetTokenByOuterId("faceSetId_test");
                GameEntry.Face.SearchFace(m_ImageBase64, faceset_token, this);

                //Destroy(clip);
            }
        }

        /// <summary>
        /// 学生人数
        /// </summary>
        /// <param name="num"></param>
        private void StudentNum(int num)
        {
            m_StudentNum.text = string.Format("当前学员数：{0}", num);
        }

        /// <summary>
        /// 登陆提示
        /// </summary>
        /// <param name="num"></param>
        private void LoginTips(string tips, Color color)
        {
            m_StudentLoginTips.color = color;
            m_StudentLoginTips.text = tips;
            StartCoroutine(SmoothValue(m_StudentLoginTips, 0, 1));
        }

        public IEnumerator SmoothValue(Text slider, float value, float duration)
        {
            yield return new WaitForSeconds(1);
            float time = 0f;
            float originalValue = slider.color.a;
            float alpha = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                alpha = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }
            alpha = value;
            slider.color = new Color(slider.color.r, slider.color.g, slider.color.b, alpha);
        }

        private void Again()
        {
            m_IsAgain = true;
        }

        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData != this) { return; }
            string responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
            Log.Info("<color=lime>" + responseJson + "</color>");
            SearchFace search = Utility.Json.ToObject<SearchFace>(responseJson);
            if (search.faces.Count > 0)
            {
                SearchFace.Results results = GameEntry.Face.MaxConfidence(search.results);
                if (results != null)
                {
                    //学员登陆为flag = 1
                    LoginServer loginServer = new LoginServer(results.face_token, "-1", 1, SendSuccess, SendFailure);
                    loginServer.SendMsg();
                }
                else
                {
                    //学员登陆失败,该人员未注册
                    //Again();
                    //LoginTips("ERROR: -101", Color.red);
                    GameEntry.UI.OpenDialog(new DialogParams
                    {
                        PauseGame = false,
                        UserData = null,
                        CloseTime = 2f,
                        Title = "登陆失败",
                        Message = "该学员未注册！",
                        OnFinish = delegate (object userData) { Again(); },
                    });
                    GameEntry.XFTTS.MultiSpeak("登陆失败,该学员未注册！");
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
                    Message = "请学员对准镜头！",
                    OnFinish = delegate (object userData) { Again(); },
                });
                GameEntry.XFTTS.MultiSpeak("请看镜头！");
            }
            //else if (search.faces.Count > 1)
            //{
            //    //学员登陆失败,登陆人员数量过多
            //    Again();
            //    LoginTips("ERROR: -102", Color.red);
            //}
            //else
            //{
            //    //学员登陆失败,人脸识别错误
            //    Again();
            //    LoginTips("ERROR: -103", Color.red);
            //}
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData != this) { return; }
            Again();
            Log.Warning("Face++ SearchFace failure. " + ne.ErrorMessage);
        }

        private void SendSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(LoginData))
            {
                LoginData data = userData as LoginData;
                if (data.Response.respVo.resultCode == 0)
                {
                    m_LoginStudentData = data.Response.userMap[0];

                    if (m_LoginStudentData.flag == 1)
                    {
                        if (GlobalData.HasStudent(m_LoginStudentData))
                        {
                            //学员登陆失败,该学员已登录
                            //Again();
                            //LoginTips("ERROR: -1000", Color.red);
                            GameEntry.UI.OpenDialog(new DialogParams
                            {
                                PauseGame = false,
                                UserData = null,
                                CloseTime = 2f,
                                Title = "登陆失败",
                                Message = "该学员已登录",
                                OnFinish = delegate (object user)
                                {
                                    Again();
                                },
                            });
                        }
                        else
                        {
                            // 如果老师刷脸时候有课, 允许学员签到
                            if (GlobalData.CoursesDailyMap != null)
                            {
                                // 学生签到
                                HttpSendStudentSignIn(m_LoginStudentData.id, m_LoginStudentData.tel, m_ImageBase64);
                            }
                            else
                            {
                                // 显示信息
                                m_TipsConext = string.Format("{0}小朋友,现在不是上课时间哦!", m_LoginStudentData.name);
                                LoginTips(m_TipsConext, Color.white);
                                Again();
                            }
                        }
                    }
                    else
                    {
                        //学员登陆失败,请老师离开采集区域
                        Again();
                        LoginTips("学员登陆失败,请老师离开采集区域", Color.red);
                    }
                }
                else
                {
                    Log.Warning("code:{0}. desc:{1}.", data.Response.respVo.resultCode, data.Response.respVo.resultDesc);
                    Again();
                }
            }
        }

        private void SendFailure(string response, object userData)
        {
            if (!string.IsNullOrEmpty(response))
            {
                Log.Warning("SendFailure response : " + response);
            }
        }

        /// <summary>
        /// 学生签到
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="tel"></param>
        /// <param name="photo"></param>
        private void HttpSendStudentSignIn(int studentId, string tel, byte[] photo)
        {
            int cdid = GlobalData.CoursesDailyMap.id;
            string cid = CoursewareManager.Instance.ClassId;
            StudentSignInServer teachSignInServer = new StudentSignInServer(studentId, tel, cdid, cid, photo, TimeUtility.GetTimeStamp(), StudentSignInSendSuccess, StudentSignInSendFailure);
            teachSignInServer.SendMsgForm();
        }

        private void StudentSignInSendSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(StudentSignData))
            {
                StudentSignData data = userData as StudentSignData;
                // -3 = 课时不足,不记录
                if (data.Response.resultCode == -3)
                {
                    m_TipsConext = string.Format("{0}学员课时不足！", m_LoginStudentData.name);
                    GameEntry.UI.OpenDialog(new DialogParams
                    {
                        PauseGame = false,
                        UserData = null,
                        CloseTime = 5f,
                        Title = "提示",
                        Message = m_TipsConext,
                    });
                    GameEntry.XFTTS.MultiSpeak(m_TipsConext);
                    Again();
                }
                // 0 = 认证通过 学员阔以登录
                else if (data.Response.resultCode == 0)
                {
                    // 显示信息
                    m_TipsConext = string.Format("欢迎{0}小朋友来上课！", m_LoginStudentData.name);

                    // 记录学员上课人数
                    GlobalData.LoginStudentDatas.Add(m_LoginStudentData);
                    StudentNum(GlobalData.LoginStudentDatas.Count);

                    GameEntry.DataNode.SetData<VarInt>(Constant.ProcedureData.NumberDifficulty, GlobalData.LoginStudentDatas.Count);
                    GlobalData.HumanNumber = GlobalData.LoginStudentDatas.Count;

                    // 成功提示
                    LoginTips(m_TipsConext, Color.white);
                    GameEntry.UI.OpenDialog(new DialogParams
                    {
                        PauseGame = false,
                        UserData = null,
                        CloseTime = 2f,
                        Title = "登陆成功",
                        Message = m_TipsConext,
                        OnFinish = delegate (object user)
                        {
                            Again();
                        },
                    });
                    GameEntry.XFTTS.MultiSpeak(m_TipsConext);
                }
                else
                {
                    Again();
                }
            }
        }

        private void StudentSignInSendFailure(string response, IDictable userData)
        {
            Log.Warning("SendFailure response : " + response);
            Again();
        }
    }
}