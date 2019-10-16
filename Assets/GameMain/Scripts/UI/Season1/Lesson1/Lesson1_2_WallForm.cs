using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using GameFramework;
using DG.Tweening;

namespace Penny
{

    public class Lesson1_2_WallForm : LessonWallUIFrame
    {

        public Text txtFishHP;
        public Animator FishAni;
        [SerializeField]
        private GameObject FishHPGo = null;
        public Image FishHPTiao;

        //private Model m_Model;
        //public GameObject GoldFish;

        [SerializeField]
        private ModelLiYuW LiYuW = null;
        [SerializeField]
        private Transform CiWei = null;
        [SerializeField]
        private Animator CiWeiAni = null;

        [Header("休息环节")]
        //[SerializeField]
        //private RawImage ResetVideo;
        [SerializeField]
        private VideoClip ResetClip = null;
        [SerializeField]
        private VideoClip ResetBG = null;
        private VideoPlayer ResetVPlayer = null;
        [SerializeField]
        private Animator CiweiRest = null;
        [SerializeField]
        private Animator XKLRest = null;
        private int HugTrck = 0;


        [SerializeField]
        private GameObject EF_Boom1;
        [SerializeField]
        private GameObject EF_Boom2;

        private int GroundID;
        [SerializeField]
        private Lesson1_2_GroundForm GroundForm;

        private Vector3[] FishMoveTF = new Vector3[] {new Vector3(-535f, 0f, -30f),  new Vector3(255f, 0f, -30f), };    
        private Vector3 FishOriginTF = new Vector3(0, 0f, -30f);
        [SerializeField]
        private bool IsFishMove = false;
        [SerializeField]
        private bool IsBackOrigin = false;
        private float FishSpeed = 3f;
        //音频播放地址
        private string assetsPath = "lesson1_2_{0}";


        private int TotalFishHP;
        private int _FishHP;       
        public int FishHP
        {
            get
            {
                return _FishHP;
            }
            set
            {
                if (_FishHP == value) return;
                _FishHP = value;
                txtFishHP.text = _FishHP.ToString();
                FishHPTiao.fillAmount = ((float)_FishHP )/ ((float)TotalFishHP);
            }
        }

      
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIEventSubscribe();
            UIOpenEvent(userData);           
            GameEntry.Event.Subscribe(ModelPressEventArgs.EventId, OnModelPressSuccess);

            PlayBGM("Les1_2_BGM");



            IsPlayRandomVoice = false;
          

            InitGame();


        }


        protected override void OnClose(object userData)
        {
          
            UIEventUnsubscribe();
            UICLoseEvent();
            GameEntry.GameManager.IsInGame = false;
            IsPlayRandomVoice = false;
            GameEntry.Event.Unsubscribe(ModelPressEventArgs.EventId, OnModelPressSuccess);

            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();

            if (IsFishMove)
                PlayDifAni();

            if (GameEntry.GameManager.IsInGame)
                PlayTTSWord();


            ///水浪声
            PlayIntervalVoice(15f, GameEntry.GameManager.IsInGame, "Les1_2_Lang");

            switch (m_ProduceingState)
            {
                case Produceing.PlayingVoice:
                    {
                        m_CurrentVoiceTime += elapseSeconds;
                        if (m_CurrentVoiceTime > m_CurrentVoiceTimeLength)
                        {
                            m_CurrentVoiceTime = 0;
                            m_ProduceingState = Produceing.PlayingVoiceEnd;
                        }
                    }
                    break;
                case Produceing.PlayingVoiceEnd:
                    {
                        VoiceOnComplete();
                    }
                    break;
                case Produceing.Delaying:
                    {
                        m_CurrentVoiceTime += elapseSeconds;
                        if (m_CurrentVoiceTime > m_CurrentVoiceTimeLength)
                        {
                            m_CurrentVoiceTime = 0;
                            m_ProduceingState = Produceing.DelayEnd;
                        }
                    }
                    break;
                case Produceing.DelayEnd:
                    {
                        m_ProduceingState = Produceing.None;
                        VoiceOnComplete();
                    }
                    break;
                case Produceing.EndGame:
                    {
                        GameEntry.GameManager.IsInGame = false;                    
                    }
                    break;
                default:
                    break;

            }
        }

        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 5000:
                    {
                        CiWeiAni.SetInteger("state", (int)PublicState.Idle);
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                case 5001:
                    {
                        FishAni.SetInteger("state", (int)Liyw.Idle);
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                case 5002:
                    {
                        PlayGameVoice("lesson1_2_11", SoundLevel.Talk);
                        m_VoiceTrack = 11;
                        m_CurrentVoiceTime = 0;
                        m_CurrentVoiceTimeLength = 10f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 11:
                    {
                        LiYuW.gameObject.SetActive(false);
                        CiWei.gameObject.SetActive(false);
                        m_ProduceingState = Produceing.None;
                        EntryRest();
                    }
                    break;
                case 6000:
                    {
                        CiweiRest.SetInteger("state", (int)PublicState.Idle);
                        XKLRest.SetInteger("state", (int)PublicState.Idle);
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                case 6001:
                    {
                        CiweiRest.Play("ciwei_hug");
                        PlayGameVoice("Lesson1_2_Rest_2", SoundLevel.Talk);
                        m_CurrentVoiceTime = 0;
                        m_CurrentVoiceTimeLength = 2f;
                        HugTrck++;
                        if (HugTrck < 3)
                        {
                            m_VoiceTrack = 6002;
                            m_ProduceingState = Produceing.PlayingVoice;
                        }
                        else {
                            m_VoiceTrack = 6003;
                            m_ProduceingState = Produceing.PlayingVoice;
                        }

                    }
                    break;
                case 6002:
                    {
                        XKLRest.Play("XKL_hug");
                        PlayGameVoice("Lesson1_2_Rest_3", SoundLevel.Talk);
                        m_VoiceTrack = 6001;
                        m_CurrentVoiceTime = 0;
                        m_CurrentVoiceTimeLength = 2f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 6003:
                    {
                        XKLRest.gameObject.SetActive(false);
                        CiweiRest.Play("ciwei_XKL_hug");
                        m_VoiceTrack = 6004;
                        m_CurrentVoiceTime = 0;
                        m_CurrentVoiceTimeLength = 3.1f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 6004:
                    {
                        XKLRest.gameObject.SetActive(true);
                        XKLRest.transform.localScale = new Vector3(-50, 50, 50);
                        XKLRest.SetInteger("state", (int)PublicState.Walk);
                        XKLRest.transform.DOLocalMoveX(-1200, 8f);
                        CiweiRest.SetInteger("state", (int)PublicState.CustomAction);
                        m_ProduceingState = Produceing.None;
                        //XKLRest.gameObject.SetActive(true);
                        //CiweiRest.Play("ciwei_XKL_hug");
                        //m_VoiceTrack = 6005;
                        //m_CurrentVoiceTime = 0;
                        //m_CurrentVoiceTimeLength = 3.1f;
                        //m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                //case 6005:
                //    {
                //        XKLRest.transform.localScale = new Vector3(-50, 50, 50);
                //        XKLRest.SetInteger("state", (int)PublicState.Walk);
                //        XKLRest.transform.DOLocalMoveX(-1200, 8f);
                //        CiweiRest.SetInteger("state", (int)PublicState.CustomAction);
                //        m_ProduceingState = Produceing.None;
                //    }
                //    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化游戏人物位置
        /// </summary>
        private void InitGame() {
            CiWei.localPosition = new Vector3(-1200, -146, -30);
            LiYuW.transform.localPosition = new Vector3(0, -380, -30);
            LiYuW.gameObject.SetActive(false);
            if (!CiWei.gameObject.activeSelf)
            {
                CiWei.gameObject.SetActive(true);
            }

            XKLRest.transform.localPosition = new Vector3(-1200, -217, -31);
            XKLRest.transform.localScale = new Vector3(50, 50, 50);
            if (XKLRest.gameObject.activeSelf) {
                XKLRest.gameObject.SetActive(false);
            }
            CiweiRest.transform.localPosition = new Vector3(0, -195, -30);
            if (CiweiRest.gameObject.activeSelf) {
                CiweiRest.gameObject.SetActive(false);
            }

            //求抱计数器
            HugTrck = 0;
        }


        public void PlayCiWeiAni()
        {

            PlayGameVoice("lesson1_2_1", SoundLevel.Talk);
            CiWei.DOLocalMoveX(690f, 4f).OnComplete(() =>
            {
                CiWeiAni.SetInteger("state", (int)PublicState.Speak);
                m_ProduceingState = Produceing.Delaying;
                m_VoiceTrack = 5000;
                m_CurrentVoiceTime = 0;
                m_CurrentVoiceTimeLength = 1f;
            });
        }


        //根据人数增加鱼血量
        private void InitGameHP()
        {
            //if (GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty) == null)
            //{               
            //    TotalFishHP = 10;
            //    FishHP = 10;
            //}
            //else
            //{
            //int hp = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty);
            //    TotalFishHP = hp * 6;
            //    FishHP = hp * 6;
            //}

        }


        /// <summary>
        /// 鲤鱼王出厂动画
        /// </summary>
        public void PlayFishAni() {


                LiYuW.gameObject.SetActive(true);
                FishAni.SetInteger("state", (int)Liyw.Jump);
                //FishHPGo.SetActive(true);
                LiYuW.transform.DOLocalMoveY(0f, 0.8f).OnComplete(() =>
                {
                    FishAni.SetInteger("state", (int)Liyw.Idle);
                    GameEntry.GameManager.IsInGame = true;
                    IsFishMove = true;
                    IsPlayRandomVoice = true;                                         
                });
          
        }
      

        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {
            base.OnRayHitByLeida(go, ve);
            if (go == null)
                return;

            if (!GameEntry.GameManager.IsInGame)
                return;

            if (!LiYuW.m_IsTouch)
            {
                if (LiYuW.OnLidarHitEvent(go, ve) != null)
                {
                    //打到周围空白
                    if (LiYuW.OnLidarHitEvent(go, ve) == 10)
                    {
                        FishAni.SetInteger("state", (int)Liyw.CalHit);
                        EF_Boom2.SetActive(false);
                        EF_Boom2.SetActive(true);

                    }
                    //打到身体
                    if (LiYuW.OnLidarHitEvent(go, ve) == 1)
                    {
                        FishAni.SetInteger("state", (int)Liyw.Hit);
                        EF_Boom1.SetActive(false);
                        EF_Boom1.SetActive(true);
                    }

                    HitFish();
                }
            }

        }

        int LanternTrack = 0;
        float DifSpeed;
        public void PlayDifAni()
        {

            if (IsFishMove)
            {
                if (!IsBackOrigin)
                {
                    float dis = Vector3.Distance(LiYuW.transform.localPosition, FishMoveTF[LanternTrack]);
                 
                    if (dis < 0.1)
                    {
                        LanternTrack++;
                        if (LanternTrack > (FishMoveTF.Length - 1))
                        {
                            LanternTrack = 0;
                        }
                    }
                 
                    ShowLantern(FishMoveTF[LanternTrack], LiYuW.gameObject);
                }
                else {
                    FishAni.SetInteger("state", (int)Liyw.Idle);
                    float dis = Vector3.Distance(LiYuW.transform.localPosition, FishOriginTF);
                    if (dis <0.1)
                    {
                        IsFishMove = false;
                        LiYuW.gameObject.transform.localPosition = FishOriginTF;
                        return;
                    }
                    ShowLantern(FishOriginTF, LiYuW.gameObject);
                }
            }          
        }

      

        private  Tweener twe;
        private Vector3 ve;
        public void ShowLantern(Vector3 tf, GameObject target)
        {
            twe.Kill();
            GameEntry.GameManager.StartDelayWork(1f, () =>
            {
                twe = target.transform.DOLocalMove(tf, FishSpeed);
            });
        }

        public void OnModelPressSuccess(object sender, GameEventArgs e) {
            ModelPressEventArgs ne = (ModelPressEventArgs)e;
                 
            if (ne.IsPress)
            {
                IsBackOrigin = true;
            }
            else
            {
                IsFishMove = true;
                IsBackOrigin = false;
            }          
        }

        public void HitFish()
        {

            //if (FishHP <= 0)
            //    return;

            //FishHP -= 3;

            PlayGameVoice("Les1_2_HitFish", SoundLevel.Once);

            //if (FishHP == 0)
            //{

            //}
            //else {

            //动画返回待机状态
         
            m_VoiceTrack = 5001;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 1f;
            m_ProduceingState = Produceing.Delaying;
            //}

        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        protected override void SkipGame()
        {
            IsFishMove = false;
            PlayGameVoice("Les1_2_Victor", SoundLevel.Once);


            m_ProduceingState = Produceing.Delaying;
            m_VoiceTrack = 5002;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 2f;

          
            //鲤鱼王死亡动画
            FishAni.SetInteger("state", (int)Liyw.Die);

            txtFishHP.text = "GameOver";
            GameEntry.GameManager.IsInGame = false;

            GroundForm.EndGame();
        }


        private float CaehTime = 10f;
        private float m_Time=0;
        private bool IsPlayRandomVoice = false;
        /// <summary>
        /// 播放随机语音
        /// </summary>
        private void PlayTTSWord()
        {
            if (IsPlayRandomVoice)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    int i = GameEntry.GameManager.RandomInt(3, 11);
                    if (i >= 3 && i <= 10)
                    {
                        string name = Utility.Text.Format(assetsPath, i.ToString());
                        PlayGameVoice(name, SoundLevel.Talk);
                    }

                    if (i == 8 || i == 9)
                    {
                        PlayGameVoice("Les1_2_FishLang",SoundLevel.Once);
                    }
                    m_Time = CaehTime;
                }
            }
        }

        /// <summary>
        /// 墙屏进入休息环节
        /// </summary>
        public void EntryRest() {

            GroundForm.EntryRest();
            //ResetVideo.texture = GameEntry.VideoPlayer.Texture;
            ResetVPlayer = GameEntry.VideoPlayer.VPSource;

            GameEntry.VideoPlayer.PlayVideoClip(ResetClip);

            //ResourceUtility.LoadGameVideo(drlesson.SeasonPath, drlesson.LessonPath, "lesson1_resetWall", GameWallBgLoad);

            GameEntry.VideoPlayer.VideoPlayEndHandler += (TempSource) =>
            {
                TempSource = ResetVPlayer;
                //ResetVideo.gameObject.SetActive(false);            
                GameEntry.VideoPlayer.VideoPlayEndHandler = null;

                GameEntry.VideoPlayer.PlayVideoClip(ResetBG, true);
                //地屏动画播放
                PlayRestAni();
            };

        }

        private void PlayRestAni() {

            CiweiRest.gameObject.SetActive(true);
            XKLRest.gameObject.SetActive(true);


            //5f
            PlayGameVoice("Lesson1_2_Rest_1", SoundLevel.Talk);

            m_ProduceingState = Produceing.Delaying;
            m_VoiceTrack = 6000;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 5f;

            XKLRest.SetInteger("state", (int)PublicState.Walk);
            XKLRest.transform.DOLocalMoveX(-82, 8f).OnComplete(()=> {
                XKLRest.Play("XKL_hug");
                PlayGameVoice("Lesson1_2_Rest_3", SoundLevel.Talk);
                m_VoiceTrack = 6001;
                m_CurrentVoiceTime = 0;
                m_CurrentVoiceTimeLength = 2f;
                m_ProduceingState = Produceing.PlayingVoice;
            });

        }

        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson1_2_GroundForm;
               
            }

        }


    }
}