using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using GameFramework;


namespace Penny
{

    public class LessonWallUIFrame : LessonBaseUIFrame
    {

        [Header("背景视频播放部分")]
        //[SerializeField]
        //protected VideoPlayer VPlayer = null;
        [SerializeField]
        protected VideoClip WallBGVideo = null;
        [SerializeField]
        protected RawImage VideoBG = null;
        /// <summary>
        /// 地屏对应界面UI ID
        /// </summary>
        protected int m_GroundFormSerialId = -1;

        /// <summary>
        /// 对应lesson表内数据
        /// </summary>
        protected DRLesson drlesson;
        /// <summary>
        /// 游戏回合数 开始时计数
        /// </summary>
        protected int GameTurn;
        /// <summary>
        /// 游戏回合数 结束时计数
        /// </summary>
        protected int EndTurn;

        /// <summary>
        /// 游戏人数
        /// </summary>
        protected int HumanNumber = 1;

      
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (GameEntry.GameManager.IsMouseDebug)
            {
                RayHitByPC();
            }


        }

        /// <summary>
        /// 常用UI事件订阅
        /// </summary>
        protected virtual void UIEventSubscribe()
        {

            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnUIFormOpenSuccess);
            // GameEntry.Event.Subscribe(LeiDaGameObjectEventArgs.EventId, OnRayHitByLeida);

            GameEntry.Windows.SubscribeUIWallEvent(OnRayHitByLeida);

            GameEntry.Event.Subscribe(PenPublicEventArgs.EventId, OnPenPublicChange);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);

        
        }

        /// <summary>
        /// 常用UI事件退订
        /// </summary>
        protected virtual void UIEventUnsubscribe()
        {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            //GameEntry.Event.Unsubscribe(LeiDaGameObjectEventArgs.EventId, OnRayHitByLeida);
            GameEntry.Windows.UnSubscribeUIWallEvent(OnRayHitByLeida);

            GameEntry.Event.Unsubscribe(PenPublicEventArgs.EventId, OnPenPublicChange);

            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnUIFormOpenSuccess);
        }

        /// <summary>
        /// 常用UI开启时执行函数
        /// </summary>
        /// <param name="userData"></param>
        protected virtual void UIOpenEvent(object userData)
        {

            drlesson = (DRLesson)userData;

            m_LessonAssetPath = drlesson.LessonPath;
            m_SeasonAssetPath = drlesson.SeasonPath;

            m_VoiceTrack = -1;

            //播放背景视频部分
            //VPlayer = GameEntry.VideoPlayer.VPSource;
            //VideoBG.texture = GameEntry.VideoPlayer.Texture;

            //GameEntry.VideoPlayer.PlayVideoClip(WallBGVideo, true);

            //if (GameEntry.GameManager.IsNowCam)
            //    InitGameBG(drlesson);
            m_GroundFormSerialId = (int)GameEntry.UI.OpenUIForm(drlesson.GroundID, drlesson);


            HumanNumber = GlobalData.HumanNumber;
        }

        /// <summary>
        /// 常用UI关闭时执行函数
        /// </summary>
        protected virtual void UICLoseEvent()
        {

            GameEntry.GameManager.IsInGame = false;

            //关闭当前背景音乐
            StopLessonMusic();
            // 停止播放背景视频并清理视频缓存
            GameEntry.VideoPlayer.Stop();
            //GameBGUnLoad();

            if (GameEntry.UI.HasUIForm(m_GroundFormSerialId))
                GameEntry.UI.CloseUIForm(m_GroundFormSerialId);

        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        protected virtual void InitGameBG(DRLesson dr)
        {
            if (dr == null)
            {
                Log.Warning("UI Data Is Null! LoadGameSence Error ");
            }

            ResourceUtility.LoadGameVideo(dr.SeasonPath, dr.LessonPath, dr.WallBG, GameWallBgLoad);
            ResourceUtility.LoadGameSence(dr.SeasonPath, dr.LessonPath, dr.GroundBG, GameGroundBgLoad);

        }

   
     
      
        /// <summary>
        /// 场景储存
        /// </summary>
        protected GameObject ThreeDArea = null;
        protected GameObject WallBgPart = null;
        protected GameObject GroundBgPart = null;

  
   

        protected virtual void GameWallBgLoad(VideoClip obj)
        {
            if (obj == null)
            {
                Log.Info("BG Load Error");
                return;
            }

            GameCamManager.Intance.BGVPlayer.clip = obj;
            GameCamManager.Intance.BGVPlayer.isLooping = true;
            GameCamManager.Intance.BGVPlayer.Play();
          
          
            //设置墙屏模型位置
            GameFramework.Entity.IEntityGroup wallModel = GameEntry.Entity.GetEntityGroup("WallModel");
            DefaultEntityGroupHelper Whelper = wallModel.Helper as DefaultEntityGroupHelper;
            Whelper.transform.localPosition = GameCamManager.Intance.WallPart.transform.localPosition;
            Whelper.transform.localRotation = GameCamManager.Intance.WallPart.transform.localRotation;
        }


        protected virtual void GameGroundBgLoad(GameObject obj)
        {
            if (obj == null)
            {
                Log.Info("BG Load Error");
                return;
            }
            GroundBgPart = Instantiate(obj);
            GroundBgPart.transform.SetParent(GameCamManager.Intance.GroundBG.transform);
            GroundBgPart.transform.localPosition = Vector3.zero;
          
          
            //设置地屏模型位置
            GameFramework.Entity.IEntityGroup groundModel = GameEntry.Entity.GetEntityGroup("GroundModel");
            DefaultEntityGroupHelper Ghelper = groundModel.Helper as DefaultEntityGroupHelper;
            Ghelper.transform.localPosition = GameCamManager.Intance.GroundPart.transform.localPosition;
            Ghelper.transform.localRotation = GameCamManager.Intance.GroundPart.transform.localRotation;
        }
    

        protected virtual void GameBGUnLoad()
        {
            //清除渲染图片缓存
            if (GameCamManager.Intance.BGVideoTex != null)
                GameCamManager.Intance.BGVideoTex.Release();

            if (WallBgPart != null)
                Destroy(WallBgPart);
            if (GroundBgPart != null)
                Destroy(GroundBgPart);
            if (ThreeDArea != null) {
                Destroy(ThreeDArea);
            }
        }

        /// <summary>
        /// 墙屏创建射线
        /// </summary>
        protected virtual void RayHitByPC()
        {
            if (!GameEntry.GameManager.IsNowCam)
                return;
            GameEntry.Windows.WallUICameraRay(Input.mousePosition);
            GameEntry.Windows.WallUICameraRay(Input.mousePosition + Vector3.up);
            GameEntry.Windows.WallUICameraRay(Input.mousePosition + Vector3.down);
            GameEntry.Windows.WallUICameraRay(Input.mousePosition + Vector3.right);
            GameEntry.Windows.WallUICameraRay(Input.mousePosition + Vector3.left);
        }

        /// <summary>
        /// 重置游戏
        /// </summary>
        protected virtual void ResetGame()
        {

        }
        /// <summary>
        /// 跳过游戏
        /// </summary>
        protected virtual void SkipGame()
        {

        }


        #region 事件执行函数部分
        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

        protected virtual void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;


            if (!GameEntry.GameManager.IsInGame)
                return;
        }

        protected virtual void OnDiffcultyChange(object sender, GameEventArgs e)
        {


        }

        protected virtual void OnPenPublicChange(object sender, GameEventArgs e) {
            PenPublicEventArgs ne = e as PenPublicEventArgs;
            PenPublicEventParams parms = ne.PenParams;

            switch (parms.BtnClick)
            {
                case PenPublicBtnClick.OnClickReset:
                    ResetGame();
                    break;
                case PenPublicBtnClick.OnClickTwiceSkipOnce:
                    SkipGame();
                    break;

            }
        }

        protected virtual void OnUIFormOpenSuccess(object sender, GameEventArgs e) {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
          
            
        }


        protected virtual void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.SceneAssetName != AssetUtility.GetSceneAsset("Game"))
            {
                return;
            }
            //InitGameBG(drlesson);
        }
        #endregion
    }
}