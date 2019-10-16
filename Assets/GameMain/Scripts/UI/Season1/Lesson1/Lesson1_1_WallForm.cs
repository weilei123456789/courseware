using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using GameFramework;
using DG.Tweening;

namespace Penny
{

    public class Lesson1_1_WallForm : LessonWallUIFrame
    {

        public Text txtMoGuScore1;
        public Text txtMoGuScore2;

        public Transform AniTF1;
        public Transform AniTF2;

        //存放采到的蘑菇
        //private GameObject kuang1;
        public GameObject kuang2;
        [SerializeField]
        private Lesson1_1_GroundForm GroundForm = null;

        //游戏过程是否进行 
        [SerializeField]
        private int MaxMogu;

        private Model m_Model = null;

        /// <summary>
        /// 皮尼模型
        /// </summary>
        [SerializeField]
        private Animator PiNi = null;
        [SerializeField]
        private Animator ttg_show = null;

        private List<GameObject> moguScore = new List<GameObject>();


        private int _MoGu2;
        public int MoGuScore2
        {
            get
            {
                return _MoGu2;
            }
            set
            {
                if (_MoGu2 == value) return;
                _MoGu2 = value;

                txtMoGuScore2.text = _MoGu2.ToString();
            }
        }
        //private int m_GroundFormSerialId = -1;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIEventSubscribe();
            UIOpenEvent(userData);

            PlayBGM("Les1_1_BGM");

            ///皮尼模型设置初始位置
            PiNi.gameObject.SetActive(false);
            PiNi.transform.localPosition = new Vector3(1200, -135, -30);
            ttg_show.transform.localPosition = new Vector3(540, -77, -31);

        }




        protected override void OnClose(object userData)

        {
            m_ProduceingState = Produceing.EndGame;

            UIEventUnsubscribe();
            UICLoseEvent();
            base.OnClose(userData);



        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

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
                        GroundForm.ClearGroundPart();
                    }
                    break;
                default:
                    break;

            }


            switch (m_ViceProduceingState)
            {
                case ViceProduceing.None:
                    break;
                case ViceProduceing.Playing:
                    {
                        m_CurrentViceTime += elapseSeconds;
                        if (m_CurrentViceTime > m_CurrentViceTimeLength)
                        {
                            m_CurrentViceTime = 0;
                            m_ViceProduceingState = ViceProduceing.PlayingEnd;
                        }
                    }
                    break;
                case ViceProduceing.PlayingEnd:
                    {
                        ViceOnComplete();
                        m_ViceProduceingState = ViceProduceing.None;
                    }
                    break;
                default:
                    break;
            }

        }



        /// <summary>
        /// 皮尼入场动画
        /// </summary>
        private void EnterPiNiAni()
        {
            m_VoiceTrack = 5000;
            m_ProduceingState = Produceing.Delaying;
            m_CurrentVoiceTimeLength = 1f;
        }

        public void InitGame()
        {

            GameEntry.GameManager.IsInGame = true;

            moguScore.Clear();
            MoGuScore2 = 0;
            txtMoGuScore1.text = 0.ToString();
            txtMoGuScore2.text = 0.ToString();

            MaxMogu = HumanNumber * 40;

        }

        /// <summary>
        /// 显示蘑菇分数
        /// </summary>     
        public void ShowScoreAni(int MoguID)
        {

            //大于一定数值不在增加蘑菇
            if (moguScore.Count >= 200)
            {
                return;
            }
            //PlayTTSWord();
            AddMogu(MoguID);

            //本回合游戏结束
            //if ( MoGuScore2 >= MaxMogu)
            //{
            //    PlayGameVoice("Les1_1_Victor", SoundLevel.Once);
            //    m_VoiceTrack = 1001;
            //    m_ProduceingState = Produceing.PlayingVoice;
            //    m_CurrentVoiceTimeLength = 5f;            
            //}                    
        }

        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 5000:
                    {
                        PlayGameVoice("lesson1_1_1", SoundLevel.Talk);
                        m_VoiceTrack = 1;
                        m_CurrentVoiceTimeLength = 5f;
                        m_ProduceingState = Produceing.PlayingVoice;
                        PiNi.gameObject.SetActive(true);
                        PiNi.transform.DOLocalMoveX(600, 2.2f);
                    }
                    break;
                case 1:
                    {
                        PlayGameVoice("lesson1_1_2", SoundLevel.Talk);
                        m_VoiceTrack = 2;
                        m_CurrentVoiceTimeLength = 4f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 2:
                    {
                        PiNi.SetInteger("state", (int)PublicState.Speak);
                        PlayGameVoice("lesson1_1_3", SoundLevel.Talk);
                        m_VoiceTrack = 3;
                        m_CurrentVoiceTimeLength = 3f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 3:
                    {

                        PlayGameVoice("lesson1_1_4", SoundLevel.Talk);
                        m_VoiceTrack = 4;
                        m_CurrentVoiceTimeLength = 4f;
                        ttg_show.SetInteger("state", (int)PublicState.MoguGrow);
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 4:
                    {
                        ttg_show.transform.DOLocalMove(new Vector3(178f, -77f, -29), 3f);
                        m_VoiceTrack = 5001;
                        m_ProduceingState = Produceing.Delaying;
                        m_CurrentVoiceTimeLength = 2.2f;
                    }
                    break;
                case 5001:
                    {
                        PlayGameVoice("lesson1_1_5", SoundLevel.Talk);
                        m_VoiceTrack = 5;
                        m_ProduceingState = Produceing.PlayingVoice;
                        m_CurrentVoiceTimeLength = 3f;
                        //ttg_show.transform.SetAsFirstSibling();
                        PiNi.transform.DOLocalMoveX(225f, 1f);
                        //pini扑跳跳菇 1s后蘑菇隐藏
                        m_ViceTrack = 1;
                        m_CurrentViceTimeLength = 1f;
                        m_ViceProduceingState = ViceProduceing.Playing;
                    }
                    break;
                case 5:
                    {
                        PlayGameVoice("lesson1_1_6", SoundLevel.Talk);
                        m_VoiceTrack = 6;
                        m_ProduceingState = Produceing.PlayingVoice;
                        m_CurrentVoiceTimeLength = 5f;
                    }
                    break;
                case 6:
                    {
                        PiNi.SetInteger("state", (int)PublicState.Idle);
                        m_ProduceingState = Produceing.None;
                        m_ViceTrack = 2;
                        m_CurrentViceTimeLength = 3f;
                        m_ViceProduceingState = ViceProduceing.Playing;
                    }
                    break;
                case 1001:
                    {
                        PlayGameVoice("lesson1_1_15", SoundLevel.Talk);
                        m_VoiceTrack = 15;
                        m_CurrentVoiceTimeLength = 5f;
                        m_ProduceingState = Produceing.PlayingVoice;

                    }
                    break;

                case 15:
                    {
                        PlayGameVoice("lesson1_1_16", SoundLevel.Talk);
                        m_VoiceTrack = 16;
                        m_CurrentVoiceTimeLength = 5f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 16:
                    {
                        m_ProduceingState = Produceing.EndGame;
                    }
                    break;

                default:
                    break;
            }
        }

        protected override void ViceOnComplete()
        {
            switch (m_ViceTrack)
            {
                case 1:
                    {
                        ttg_show.gameObject.SetActive(false);
                    }
                    break;

                case 2:
                    {
                        //开始游戏
                        InitGame();
                        GroundForm.InitGame();
                    }
                    break;
                default:
                    break;

            }


        }
        /// <summary>
        /// 添加作为分数的蘑菇
        /// </summary>
        public void AddMogu(int MoguID)
        {
            float min = -0.5f;
            float max = 0.5f;
            float x = Random.Range(min, max);
            float z = Random.Range(min, max);
            GameEntry.Entity.ShowModeTypeOnce(m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), MoguID)
            {
                Name = "mogu",
                Position = kuang2.transform.position + new Vector3(x, 2, z),
                Scale = new Vector3(1f, 1f, 1f),
                CodeID = 2,
                Layer = 5,
            });

        }

        protected override void ResetGame()
        {
            MoGuScore2 = 0;
            GameEntry.GameManager.IsInGame = true;

            foreach (GameObject go in moguScore)
            {
                GameEntry.Entity.HideEntity(go.GetComponent<Model>());
            }

            moguScore.Clear();
        }

        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("Les1_1_Victor", SoundLevel.Once);
            m_VoiceTrack = 1001;
            m_ProduceingState = Produceing.PlayingVoice;
            m_CurrentVoiceTimeLength = 5f;
        }

        /// <summary>
        /// 分数增加时 有几率发出声音
        /// </summary>
        private void PlayTTSWord()
        {
            int i = Random.Range(8, 15);
            if (i >= 12 && i < 15)
            {
                string name = Utility.Text.Format("lesson1_1_{0}", i.ToString());
                PlayGameVoice(name, SoundLevel.Talk);
            }
        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(Model))
            {
                m_Model = (Model)ne.Entity.Logic;

                if (m_Model.Name.Equals("mogu"))
                {                 
                    //加入分数计数器方便重置
                    moguScore.Add(m_Model.gameObject);
                }

            }
        }

        protected override void OnPenPublicChange(object sender, GameEventArgs e)
        {
            base.OnPenPublicChange(sender, e);
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

        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson1_1_GroundForm;
                EnterPiNiAni();
            }

        }

        protected override void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

    }
}