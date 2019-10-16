using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using DG.Tweening;

namespace Penny
{

    public class Lesson1_2_GroundForm : LessonGroundUIFrame
    {

        //荷叶坐标   
        [SerializeField]
        private List<ModelIsLight> HeYes = new List<ModelIsLight>();

        //乌龟
        [SerializeField]
        private List<ModelIsLight> WuGuis = new List<ModelIsLight>();

        //private Dictionary<int, GameObject> lightPool = new Dictionary<int, GameObject>();

        /// <summary>
        /// 荷叶位置信息
        /// </summary>
        //private List<LessonPropTFParams> HeYeTF = new List<LessonPropTFParams>() {
        //    new LessonPropTFParams { Postion = new Vector3(-4,-43,-3.94f),Rotation = new Vector3(90,0,132),Scale = Vector3.one*0.675f},
        //    new LessonPropTFParams { Postion = new Vector3(-1.67f,-43,-4.14f),Rotation = new Vector3(90,0,-0.8f),Scale = Vector3.one*0.5f},
        //    new LessonPropTFParams { Postion = new Vector3(0.35f,-43,-4.21f),Rotation = new Vector3(90,0,-194.9f),Scale = Vector3.one*0.5f},
        //    new LessonPropTFParams { Postion = new Vector3(2.24f,-43,-4.19f),Rotation = new Vector3(90,170,0),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(3.77f,-43,-3.44f),Rotation = new Vector3(90,50,-62.6f),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(4.44f,-43,-2.02f),Rotation = new Vector3(90,-152,-90f),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(2.76f,-43,-2.3f),Rotation = new Vector3(90,3.2f,-90f),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(3.61f,-43,-0.78f),Rotation = new Vector3(90,-90.5f,-90f),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(1.71f,-43,-1.31f),Rotation = new Vector3(90,-70.5f,-90f),Scale = Vector3.one*0.4f},
        //    new LessonPropTFParams { Postion = new Vector3(2.42f,-43,0.33f),Rotation = new Vector3(90,-177f,-90f),Scale = Vector3.one*0.4f},
        //};

        /// <summary>
        /// 乌龟位置信息
        /// </summary>
        //private Vector3[] WuGuiTF = new Vector3[] { new Vector3(0.03f,-43,2.06f), new Vector3(-0.75f, -43, 1.48f), new Vector3(-0.78f, -43, 2.95f),
        //    new Vector3(-1.41f, -43, 2.05f), new Vector3(-1.65f, -43, 3.7f), new Vector3(-2.18f, -43, 2.71f), };

        //摆放道具点
        //private Vector3[] PointTF = new Vector3[] { new Vector3(1.9f,-5,4.1f), new Vector3(0, -5, 4.1f) , new Vector3(-1.9f, -5, 4.1f) };


        [SerializeField]
        private ModelHasAni Rock = null;

        [SerializeField]
        private ModelBase Qidian = null;

        private Animator ciweiAni;

        //private List<bool> temp = new List<bool>();

        //根据难度改变速度
        private float DifSpeed = 4;

        [SerializeField]
        private Animator YanMo;
        [SerializeField]
        private Transform LiYuW_Top;

        private Lesson1_2_WallForm WallForm = null;

        [SerializeField]
        private GameObject m_ResetImg;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            InitGame();

            UIEventSubscribe();
            //lightPool.Clear();

            WallForm = GameEntry.UI.GetUIForm(drlesson.WallID, "") as Lesson1_2_WallForm;

            if (m_ResetImg.activeSelf)
                m_ResetImg.SetActive(false);

            InitGround();

            ShowEFYanMo();
        }


        protected override void OnClose(object userData)

        {
            //temp.Clear();
            //lightPool.Clear();
            //退订事件
            UIEventUnsubscribe();

            base.OnClose(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);


            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();


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


        protected override void ViceOnComplete()
        {
            switch (m_ViceTrack)
            {
                case 5001:
                    {
                        Rock.modelAni.SetBool("IsLoop", false);
                        if (m_FazhenSound != -1)
                        {
                            GameEntry.Sound.StopSound(m_FazhenSound);
                            m_FazhenSound = -1;
                        }
                        ModelPressEventArgs ne = new ModelPressEventArgs(false);
                        GameEntry.Event.Fire(this, ne);
                    }
                    break;
                case 5002:
                    {
                        LiYuW_Top.DOLocalMoveY(845, 3f).OnComplete(() =>
                        {
                            YanMo.gameObject.SetActive(false);
                            WallForm.PlayFishAni();
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 游戏初始化
        /// </summary>
        private void InitGame()
        {


        }

        //初始化地屏
        public void InitGround()
        {
            YanMo.gameObject.SetActive(false);
            LiYuW_Top.localPosition = new Vector3(0, -830, 0);
        }

        //显示淹没转场动画
        private void ShowEFYanMo()
        {
            YanMo.gameObject.SetActive(true);
            PlayGameVoice("Les1_2_YanMo", SoundLevel.Once);

            WallForm.PlayCiWeiAni();

            m_ViceProduceingState = ViceProduceing.Playing;
            m_ViceTrack = 5002;
            m_CurrentViceTime = 0;
            m_CurrentViceTimeLength = 4f;
        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {

            base.OnRayHitByLeida(go, ve);

            if (go == null)
                return;


            if (!GameEntry.GameManager.IsInGame)
                return;

            if (go.name == "HeYe")
            {
                for (int i = 0; i < HeYes.Count; i++)
                {
                    if (!HeYes[i].m_IsTouch)
                    {
                        if (HeYes[i].OnLidarHitEvent(go, ve) != null)
                        {
                            HeYeHit(HeYes[i].transform.position);
                        }
                    }
                }
            }

            if (go.name == "wugui")
            {
                for (int i = 0; i < WuGuis.Count; i++)
                {
                    if (!WuGuis[i].m_IsTouch)
                    {
                        if (WuGuis[i].OnLidarHitEvent(go, ve) != null)
                        {
                            HitWugui(WuGuis[i]);
                        }
                    }
                }
            }

            if (go.name == "QiDian")
            {
                if (!Qidian.m_IsTouch)
                {
                    if (Qidian.OnLidarHitEvent(go, ve) != null)
                    {
                        HitQidian();
                    }
                }
            }

            if (go.name == "Rock")
            {

                if (!Rock.m_IsTouch)
                {
                    if (Rock.OnLidarHitEvent(go, ve) != null)
                    {
                        HitRock();
                    }
                }

            }
        }

        public void HeYeHit(Vector3 ve)
        {
            PlayGameVoice("Les1_3_MuBan", SoundLevel.Once);
        }




        private void HitRock()
        {
            Rock.modelAni.SetBool("IsLoop", true);
            if (m_FazhenSound == -1)
            {
                m_FazhenSound = PlayGameVoice("Les1_2_FaZhen", new GameFramework.Sound.PlaySoundParams
                {
                    Priority = 50,
                    Loop = true,
                });
            }
            ModelPressEventArgs ne = new ModelPressEventArgs(true);
            GameEntry.Event.Fire(this, ne);


            ///延时抛出不在地点判断 大于CD时间
            m_ViceProduceingState = ViceProduceing.Playing;
            m_ViceTrack = 5001;
            m_CurrentViceTime = 0;
            m_CurrentViceTimeLength = 4f;
        }


        private void HitWugui(ModelIsLight mod)
        {
            PlayGameVoice("Les1_2_HitWugui2", SoundLevel.Once);
        }

        private void HitQidian()
        {
            PlayGameVoice("Les1_3_MuBan", SoundLevel.Once);
        }

        private int m_FazhenSound = -1;

        /// <summary>
        /// 停止播放魔法阵语音
        /// </summary>
        public void EndGame()
        {
            if (m_FazhenSound != -1)
            {
                GameEntry.Sound.StopSound(m_FazhenSound);
                m_FazhenSound = -1;
            }
        }


        /// <summary>
        /// 地屏进入休息环节
        /// </summary>
        public void EntryRest()
        {
            m_ResetImg.SetActive(true);

        }

        protected override void OnDiffcultyChange(object sender, GameEventArgs e)
        {
            NormalDifficultyEventArgs ne = (NormalDifficultyEventArgs)e;
            switch (ne.Difficulty)
            {
                case NormalDifficulty.Easy:
                    DifSpeed = 8;
                    break;
                case NormalDifficulty.Normal:
                    DifSpeed = 4;
                    break;
                case NormalDifficulty.Hard:
                    DifSpeed = 2;
                    break;
            }

        }

    }
}
