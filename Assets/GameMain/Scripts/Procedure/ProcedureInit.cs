using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.DataTable;

namespace Penny
{
    public class ProcedureInit : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        private InitializationForm m_InitializationForm = null;

        public static bool s_IsNeedInitResource = true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            IsEnterNextProduce = false;
            GlobalData.GameStateType = GameStateType.Unknown;

            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            GameEntry.Face.GetFaceSets(0, this);

            //初始化为0.5f
            GameEntry.Sound.SetVolume("Music", 0.5f);
            GameEntry.Sound.SetVolume("Sound", 0.5f);
            GameEntry.Sound.SetVolume("UISound", 0.5f);
            GameEntry.VideoPlayer.Volume = 0.5f;
            //初始化配置
            //GameEntry.WindowsConfig.InitCustomConfig(InitializationWithConfig);

            //PreloadScene("Game");

#if TestLine
            ChangeState<ProcedureGame>(procedureOwner);
            return;
#endif

            InitializationWithConfig(GameEntry.WindowsConfig.Config);

            // 更具设备上的课件转成H5用的数据结构
            CoursewareManager.Instance.GetStructWithProduceInit();


            GameEntry.UI.OpenUIForm(UIFormId.InitializationForm, this);
        }

        /// <summary>
        /// 根据配置文件初始化
        /// </summary>
        /// <param name="config"></param>
        private void InitializationWithConfig(Config config)
        {
            if (!s_IsNeedInitResource) return;
            Debug.Log(CoursewareManager.Instance.Socket_IP+ CoursewareManager.Instance.Socket_Port+"ssss");
            if (GameEntry.Socket)
                GameEntry.Socket.CreateNetworkChannel(CoursewareManager.Instance.Socket_IP, CoursewareManager.Instance.Socket_Port);
            if (GameEntry.SerialPort)
                GameEntry.SerialPort.StartSerialPort(config.Serial_BaudRate, config.Serial_Port, config.Screen_Land_Width, config.Screen_Land_Height, config.Serial_Offset_Width, config.Serial_Offset_Height, (float)config.Serial_Scale);
            //if (GameEntry.Ethernet)
            //    GameEntry.Ethernet.StartEthernet(config.Screen_Land_Width, config.Screen_Land_Height, config.Ethernet_Address, config.Ethernet_Port, config.Ethernet_Offset_Width, config.Ethernet_Offset_Height, (float)config.Ethernet_Scale);
            if (GameEntry.Urg)
                GameEntry.Urg.StartUrgEthernet(config.URG_Address, config.URG_Port, config.URG_Offset_Width, config.URG_Offset_Height, (float)config.URG_Scale);
            s_IsNeedInitResource = false;

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_InitializationForm != null)
            {
                m_InitializationForm.Close(true);
                m_InitializationForm = null;
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //if (IsEnterNextProduce)
            //{
            //    ChangeState<ProcedureKinectGame>(procedureOwner);
            //    return;
            //}

            if (IsEnterNextProduce && GameEntry.Socket && GameEntry.Socket.IsEnterTeacherFaceLogin)
                ChangeState<ProcedureTeacherLogin>(procedureOwner);

        }

        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            string responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
            Log.Info("<color=lime>" + responseJson + "</color>");
            FaceSet faceSet = Utility.Json.ToObject<FaceSet>(responseJson);
            GameEntry.Face.AddFaceSetDatas(faceSet.facesets);
            int NextLen = faceSet.next == null ? 0 : int.Parse(faceSet.next);
            if (NextLen > 0)
            {
                GameEntry.Face.GetFaceSets(NextLen, this);
            }
            else
            {

                //GameEntry.Face.GetDatil("faceSetId_test", "faceSetId_test");
                NextProduce();
            }
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Warning("Web Request Failure. " + ne.ErrorMessage);

        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            m_InitializationForm = (InitializationForm)ne.UIForm.Logic;
        }

        private void PreloadScene(string sceneName)
        {
            GameEntry.Resource.LoadAsset(AssetUtility.GetSceneAsset(sceneName), Constant.AssetPriority.SceneAsset, new LoadAssetCallbacks(
                (assetName, asset, duration, userData) =>
                {
                    Log.Info("Preload scene '{0}' OK.", sceneName);
                },

                (assetName, status, errorMessage, userData) =>
                {
                    Log.Error("Can not Preload scene '{0}' from '{1}' with error message '{2}'.", sceneName, assetName, errorMessage);
                }));
        }




    }
}
