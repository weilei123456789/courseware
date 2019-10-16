using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using DG.Tweening;
using GameFramework;

namespace Penny
{

    public class Lesson1_1_GroundForm : LessonGroundUIFrame
    {

        private enum MoguGrowState
        {
            None,
            moguReady,
            moguOrderGrow,
            moguOrderGrowEnd,
            moguRefresh,
            moguClear,
        }

        [SerializeField]
        private MoguGrowState m_MoguGrowState = MoguGrowState.None;

        private GameObject MoguPart;


        [SerializeField]
        private List<bool> TempTerms = new List<bool>();
        [SerializeField]
        private bool IsGrowMoGu = false;
        //每回合出现蘑菇数目
        [SerializeField]
        private int EverNum = 0;

        /// <summary>
        /// 蘑菇跑路的循环音效
        /// </summary>
        private int MoguPaoSerid = -1;

        [SerializeField]
        private Lesson1_1_WallForm WallForm = null;

        [SerializeField]
        private Animator fazhenAni = null;

        [SerializeField]
        private List<ModelHasAni> Buckets = new List<ModelHasAni>();

        [SerializeField]
        private MoGuManager mogu1;
        [SerializeField]
        private MoGuManager mogu2;
        [SerializeField]
        private MoGuManager mogu3;
        [SerializeField]
        private GameObject EF_moguGrow1;
        [SerializeField]
        private GameObject EF_moguGrow2;
        [SerializeField]
        private GameObject EF_moguGrow3;

        private bool IsReadyGrow = true;

        //特效实体ID;
        private List<GameObject> EF_ID = new List<GameObject>();

        /// <summary>
        /// 当前活动蘑菇个数
        /// </summary>
        [SerializeField]
        private List<MoGuManager> MuguPool = new List<MoGuManager>();

        /// <summary>
        /// 刷新间隔
        /// </summary>
        [SerializeField]
        private float Refresh = 0.2f;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();

            if (GameEntry.UI.HasUIForm(drlesson.WallID, ""))
            {
                WallForm = (Lesson1_1_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "");
            }

            IsReadyGrow = true;
            ShowGroundEle(false);

            TurnTrack = 0;

            HumanNumber = 10;
            //Refresh = 5f / (1 + 0.5f * HumanNumber);
            Refresh = 0.2f;
        }

        protected override void OnClose(object userData)

        {


            TurnTrack = 0;
            //重置跑圈
            UIEventUnsubscribe();
            IsGrowMoGu = false;
            IsReadyGrow = true;
            IsPlayBucket = false;
            MoguPaoSerid = -1;
            //关闭游戏时重置场景
            InitGame();


            base.OnClose(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();


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
                        ((Lesson1_1_GroundForm)GameEntry.UI.GetUIForm(drlesson.GroundID, "")).ClearGroundPart();
                    }
                    break;
                default:
                    break;

            }

            switch (m_MoguGrowState)
            {

                case MoguGrowState.moguClear:
                    {
                        foreach (MoGuManager go in MuguPool)
                        {

                            Destroy(go.gameObject);
                        }
                        MuguPool.Clear();
                        TotalTrack = 0;
                        m_MoguGrowState = MoguGrowState.None;
                    }
                    break;
                case MoguGrowState.moguOrderGrow:
                    {
                        m_CurrentGrowtime += elapseSeconds;
                        if (m_CurrentGrowtime > m_GrowSpaceTime)
                        {
                            m_CurrentGrowtime = 0;
                            m_MoguGrowState = MoguGrowState.moguOrderGrowEnd;
                        }
                    }
                    break;
                case MoguGrowState.moguOrderGrowEnd:
                    {
                        GrowMoGuInTurn();
                        TotalTrack++;
                        if (TotalTrack > TotalNum)
                        {
                            m_MoguGrowState = MoguGrowState.None;
                        }
                        else
                        {
                            m_MoguGrowState = MoguGrowState.moguOrderGrow;
                        }
                    }
                    break;
                case MoguGrowState.moguRefresh:
                    {
                        IsGrowMoGu = true;
                    }
                    break;
                case MoguGrowState.moguReady:
                    {
                        TotalTrack = 0;
                        IsGrowMoGu = false;
                    }
                    break;

                default:
                    break;
            }


            //增加蘑菇
            if (GameEntry.GameManager.IsInGame)
            {
                if (IsGrowMoGu)
                {
                    MoguPool();
                    PlayIntervalVoice(10f, IsGrowMoGu, Utility.Text.Format("lesson1_1_{0}", GameEntry.GameManager.RandomInt(12, 15).ToString()), SoundLevel.Talk);
                }
                PlayRandomBucket();
            }
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void InitGame()
        {
            foreach (MoGuManager go in MuguPool)
            {

                Destroy(go.gameObject);
            }
            MuguPool.Clear();

            for (int i = 0; i < TempTerms.Count; i++)
            {
                TempTerms[i] = false;
            }

            //关闭生长特效
            for (int i = 0; i < EF_ID.Count; i++)
            {
                Destroy(EF_ID[i]);
            }
            EF_ID.Clear();


            ShowGroundEle(true);
        }

        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {

                case 5000:
                    {
                        PlayGameVoice("Les1_1_jiaoshui", SoundLevel.Once);
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                case 10:
                    {
                        PlayGameVoice("lesson1_1_11", SoundLevel.Talk);
                        m_VoiceTrack = 11;
                        m_ProduceingState = Produceing.PlayingAniVoice;
                        m_CurrentViceTimeLength = 5f;
                    }
                    break;
                case 11:
                    {
                        PlayGameVoice("lesson1_1_18", SoundLevel.Talk);
                        m_VoiceTrack = 18;
                        m_ProduceingState = Produceing.PlayingAniVoice;
                        m_CurrentViceTimeLength = 2f;
                    }
                    break;
                case 18:
                    {
                        PlayGameVoice("lesson1_1_17", SoundLevel.Talk);
                        m_VoiceTrack = 17;
                        m_ProduceingState = Produceing.PlayingAniVoice;
                        m_CurrentViceTimeLength = 18f;
                    }
                    break;
                case 17:
                    {
                        GameEntry.GameManager.IsInGame = true;
                        //反向并开启地屏元素
                        SwitchEleDir();
                        ShowGroundEle(true);
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                default:
                    break;
            }
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {

            if (go == null)
                return;


            if (!GameEntry.GameManager.IsInGame)
                return;

            //if (go.name.Equals("mogu"))
            {
                for (int i = 0; i < MuguPool.Count; i++)
                {
                    if (!MuguPool[i].m_IsTouch)
                    {
                        if (MuguPool[i].OnLidarHitEvent(go, ve) != null)
                        {
                            HitMoGu(MuguPool[i]);
                            if (MuguPool.Count > i)
                            {
                                MuguPool.Remove(MuguPool[i]);
                            }
                        }
                    }

                }
                //HitMoGu(go);
            }
            if (go.name.Equals("Bucket"))
            {
                for (int i = 0; i < Buckets.Count; i++)
                {
                    if (!Buckets[i].m_IsTouch)
                    {
                        if (Buckets[i].OnLidarHitEvent(go, ve) != null)
                        {
                            HitBucket(Buckets[i]);

                        }
                    }
                }
            }

        }
        public void HitMoGu(MoGuManager mmg)
        {

            int typeid = mmg.TypeID;


            //根据不同蘑菇生成不通被踩特效
            int efId = 0;
            switch (typeid)
            {
                //蓝蘑菇
                case 100001:
                    efId = 100038;
                    break;
                //红蘑菇
                case 100002:
                    efId = 100039;
                    break;
                //黄蘑菇
                case 100003:
                    efId = 100040;
                    break;
            }



            ShowEffectEntity(new EffectData(GameEntry.Entity.GenerateSerialId(), efId)
            {
                Name = "EF_Boom",
                Position = mmg.transform.position,
                Rotation = Quaternion.Euler(new Vector3(0, 0, 0)),
                Scale = Vector3.one * 0.3f,
                KeepTime = 3f,
                Layer = 5,
            });

            PlayGameVoice("Les1_1_MoguBoom", SoundLevel.Once);

            mmg.DestroyMoGu();
            WallForm.MoGuScore2++;
            WallForm.ShowScoreAni(typeid);


            //本回合结束
            EverNum--;
            if (EverNum <= 0)
            {
                EndGameTurn();
            }

        }

        private int TurnTrack = 0;
        private void HitBucket(ModelHasAni molani)
        {
            //游戏环节直接跳过
            if (IsGrowMoGu)
                return;
            if (IsReadyGrow)
            {
                InitTurn();
                IsReadyGrow = false;
            }

            Animator ani = molani.modelAni;
            ani.Play("shuitong");

            //PlayRandomBucket();

            PlayGameVoice("Les1_1_HitMoGu", SoundLevel.Once);
            m_VoiceTrack = 5000;
            m_CurrentVoiceTimeLength = 1f;
            m_ProduceingState = Produceing.PlayingVoice;

            int id = molani.CodeID;
            if (!TempTerms[id])
            {
                TempTerms[id] = true;
            }

            foreach (bool te in TempTerms)
            {
                if (!te)
                    return;
            }

            TurnTrack++;

            if (TurnTrack % 2 == 1)
            {
                for (int i = 0; i < TempTerms.Count; i++)
                {
                    TempTerms[i] = false;
                }
            }
            if (TurnTrack % 2 == 0)
            {

                //开始游戏环节
                // int human = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty);
                fazhenAni.speed = 0;
                EverNum = HumanNumber * 5;
                IsGrowMoGu = true;

                IsPlayBucket = false;

                //清理蘑菇生成
                m_MoguGrowState = MoguGrowState.moguRefresh;

                //关闭生长特效
                for (int i = 0; i < EF_ID.Count; i++)
                {
                    Destroy(EF_ID[i]);
                }
                EF_ID.Clear();

                PlayGameVoice("Les1_1_MoguGrow", SoundLevel.Once);

                foreach (MoGuManager mogu in MuguPool)
                {

                    mogu.m_IsTouch = false;
                    mogu.GrowMoGu();
                }

                //关闭地屏元素
                ShowGroundEle(false);
                //播放蘑菇跑路音效
                MoguPaoSerid = (int)PlayGameVoice("Les1_1_moguPao", new GameFramework.Sound.PlaySoundParams { Priority = 50, Loop = true, });
            }
        }


        float Growtime = 10f;
        float m_GrowSpaceTime = 0;
        float m_CurrentGrowtime;
        int TotalNum = 0;
        int TotalTrack = 0;
        //List<Coroutine> lisCor = new List<Coroutine>();
        /// <summary>
        /// 每回合初始生成蘑菇
        /// </summary>
        public void InitTurn()
        {
            TotalNum = HumanNumber * 2;
            m_GrowSpaceTime = Growtime / TotalNum;
            m_MoguGrowState = MoguGrowState.moguOrderGrow;

        }

        /// <summary>
        /// 回合结束 清理协程中未生成的蘑菇以及残留在游戏场景中蘑菇
        /// </summary>
        private void ClearMogu()
        {

            foreach (MoGuManager go in MuguPool)
            {

                Destroy(go.gameObject);
            }
            MuguPool.Clear();

            //关闭生长特效
            for (int i = 0; i < EF_ID.Count; i++)
            {
                Destroy(EF_ID[i]);
            }
            EF_ID.Clear();
        }

        /// <summary>
        /// 回合根据人数生成有生长特效的蘑菇
        /// </summary>
        private void GrowMoGuInTurn()
        {
            int id = 100000 + Random.Range(1, 4);
            Vector3 ve = RandomVc();

            CreateMogu(id, ve, 0);

            GameObject ef_go = null;
            switch (id)
            {
                //蓝蘑菇
                case 100001:
                    ef_go = Instantiate(EF_moguGrow1);
                    break;
                //红蘑菇
                case 100002:
                    ef_go = Instantiate(EF_moguGrow2);
                    break;
                //黄蘑菇
                case 100003:
                    ef_go = Instantiate(EF_moguGrow3);
                    break;
            }
            ef_go.SetActive(true);
            ef_go.transform.SetParent(MidPart);
            ef_go.transform.localScale = Vector3.one;
            ef_go.transform.localPosition = ve;
            EF_ID.Add(ef_go);


        }
        /// <summary>
        ///生成蘑菇
        /// </summary>
        /// <param name="typeid">蘑菇类型</param>
        /// <param name="ve">蘑菇坐标</param>
        /// <param name="codeID">蘑菇codeid</param>
        private void CreateMogu(int typeid, Vector3 ve, int codeID)
        {

            MoGuManager go = null;
            switch (typeid)
            {
                case 100001:
                    go = Instantiate(mogu1);
                    break;
                case 100002:
                    go = Instantiate(mogu2);
                    break;
                case 100003:
                    go = Instantiate(mogu3);
                    break;
            }

            go.CodeID = codeID;
            go.gameObject.SetActive(true);
            go.transform.SetParent(FrontPart);
            go.transform.localPosition = ve;
            go.transform.localScale = new Vector3(1, 1, 1);
            //go.transform.GetChild(0).localScale = new Vector3(0, 0, 0);
            go.IsHit(false);
            go.name = "mogu";

            MuguPool.Add(go);

            if (codeID == 1)
            {
                go.GrowMoGu();
            }
        }


        /// <summary>
        /// 圆范围内随机坐标
        /// </summary>
        float min = -250;
        float max = 250;
        private Vector3 RandomVc()
        {

            float x = Random.Range(min, max);
            float z = Random.Range(min, max);

            float dis = Vector3.Distance(new Vector3(x, z, 0), new Vector3(0, 0, 0));
            if (dis < 250)
            {
                return new Vector3(x, z, 0);
            }
            else
            {
                return RandomVc();
            }
        }

        /// <summary>
        /// 方形区域内随机坐标
        /// </summary>
        /// <returns></returns>
        private Vector3 RandomSqVc(float min, float max)
        {
            float x = Random.Range(min, max);
            float z = Random.Range(min, max);
            return new Vector3(x, z, 0);
        }

        //切换地屏元素方向
        private void SwitchEleDir()
        {

            fazhenAni.transform.parent.localEulerAngles = fazhenAni.transform.parent.localEulerAngles + new Vector3(180, 0, 0);
            fazhenAni.speed = 1;

            foreach (ModelHasAni en in Buckets)
            {
                en.transform.localScale = new Vector3(en.transform.localScale.x, en.transform.localScale.y * -1, en.transform.localScale.z);
            }
        }


        //本回合游戏结束
        public void EndGameTurn()
        {

            //第一次绕场2圈语音
            if (TurnTrack == 2)
            {
                PlayGameVoice("lesson1_1_9", SoundLevel.Talk);
            }


            ////中场休息
            //if (TurnTrack == 8)
            //{

            //    GameEntry.GameManager.IsInGame = false;
            //    //好累啊，小朋友们，我们先原地休息一下吧~
            //    PlayGameVoice("lesson1_1_10", SoundLevel.Talk);
            //    fazhenAni.speed = 0;
            //    //暂时关闭地屏元素
            //    ShowGroundEle(false);
            //    m_VoiceTrack = 10;
            //    m_ProduceingState = Produceing.PlayingAniVoice;
            //    m_CurrentViceTimeLength = 30f;
            //    //休息30s

            //}
            //else
            {
                //反向并开启地屏元素
                SwitchEleDir();
                ShowGroundEle(true);
            }

            IsGrowMoGu = false;
            IsReadyGrow = true;
            IsPlayBucket = true;

            m_MoguGrowState = MoguGrowState.moguReady;

            for (int i = 0; i < TempTerms.Count; i++)
            {
                TempTerms[i] = false;
            }

            //暂停蘑菇跑音效
            GameEntry.Sound.StopSound(MoguPaoSerid);
            MoguPaoSerid = -1;

            //清理还未长出蘑菇
            ClearMogu();
        }

        /// <summary>
        /// 踩水桶随机播放语音
        /// </summary>
        private bool IsPlayBucket = false;
        private float CaehTime = 10f;
        private float m_Time;
        private int s_Track = 7;
        private void PlayRandomBucket()
        {
            if (IsPlayBucket)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    string ss = Utility.Text.Format("lesson1_1_{0}", s_Track.ToString());
                    PlayGameVoice(ss, SoundLevel.Talk);
                    s_Track++;
                    if (s_Track > 9)
                    {
                        s_Track = 7;
                    }
                    m_Time = CaehTime;
                }
            }
        }



        protected override void ResetGame()
        {
            IsReadyGrow = true;
            IsGrowMoGu = false;
            IsPlayBucket = true;

            if (fazhenAni.speed == -1)
            {
                SwitchEleDir();
            }
            else
            {
                fazhenAni.speed = 1;
            }

            for (int i = 0; i < TempTerms.Count; i++)
            {
                TempTerms[i] = false;
            }

            foreach (MoGuManager go in MuguPool)
            {

                Destroy(go.gameObject);
            }
            MuguPool.Clear();
        }

        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;
            IsGrowMoGu = false;

            if (MoguPaoSerid != -1)
            {
                GameEntry.Sound.PauseSound(MoguPaoSerid);
            }

            //重置游戏
            InitGame();
        }

        float time;
        public void MoguPool()
        {

            if (Time.time - time > Refresh)
            {
                if (ShowCount() >= HumanNumber * 2)
                    return;

                if (EverNum <= ShowCount())
                    return;

                AddMoGu();

                time = Time.time;
            }
        }

        /// <summary>
        /// 蘑菇当前屏幕数量i
        /// </summary>
        public int ShowCount()
        {
            return MuguPool.Count;
        }

        /// <summary>
        /// 随机长出蘑菇
        /// </summary>
        public void AddMoGu()
        {
            int id = 100000 + Random.Range(1, 4);
            Vector3 ve = RandomSqVc(-500, 500);
            CreateMogu(id, ve, 1);
        }

        /// <summary>
        /// 关闭或开启显示地屏元素
        /// </summary>
        /// <param name="IsShow"></param>
        public void ShowGroundEle(bool IsShow)
        {
            for (int i = 0; i < Buckets.Count; i++)
            {
                Buckets[i].gameObject.SetActive(IsShow);

            }
            fazhenAni.gameObject.SetActive(IsShow);
        }

        /// <summary>
        /// 隐藏地屏元素
        /// </summary>
        public void ClearGroundPart()
        {
            for (int i = 0; i < Buckets.Count; i++)
            {
                Buckets[i].gameObject.SetActive(false);
            }
            fazhenAni.gameObject.SetActive(false);
            SkipGame();
        }




        protected override void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

    }
}
