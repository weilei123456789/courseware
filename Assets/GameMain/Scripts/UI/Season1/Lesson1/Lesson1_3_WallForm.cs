using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using GameFramework;
using GameFramework.Event;


namespace Penny
{

    public class Lesson1_3_WallForm : LessonWallUIFrame
    {

        public Animator RunManAni;
        public ModelHasAni RunMan;

        [SerializeField]
        private Lesson1_3_GroundForm GroundForm;

        //最大跑圈数
        private int MaxTurns;
        private int nowTurns = 0;

        [SerializeField]
        private GameObject HandFlash;
        [SerializeField]
        private Animator EF_HitHand;

        private Model m_Model = null;

        private bool IsPlayRandomVoice = false;
     

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);


            UIEventSubscribe();
            UIOpenEvent(userData);

       
            //制定区域到达监听
            GameEntry.Event.Subscribe(ModelPressEventArgs.EventId, OnPressArea);
        

          
            InitRunMan();
        

            PlayBGM("Les1_3_BGM");

          
           
        }


        protected override void OnClose(object userData)

        {
            base.OnClose(userData);
            UIEventUnsubscribe();
            UICLoseEvent();

        
            IsPlayRandomVoice = false;

          
            GameEntry.Event.Unsubscribe(ModelPressEventArgs.EventId, OnPressArea);
          
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();

            if (GameEntry.GameManager.IsInGame && IsPlayRandomVoice)
                PlayTTSWord();

          
            ///跑步声
            PlayIntervalVoice(15f, GameEntry.GameManager.IsInGame, "Les1_3_Pao");


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
                        PlayGameVoice("lesson1_3_9", SoundLevel.Talk);
                        m_ProduceingState = Produceing.EndGame;
                    }
                    break;
                case 1:
                    {
                        PlayGameVoice("lesson1_3_2", SoundLevel.Talk);
                        m_VoiceTrack = 2;
                    
                        m_CurrentVoiceTimeLength = 5f;
                        m_ProduceingState = Produceing.PlayingVoice;
                    }
                    break;
                case 2:
                    {
                        IsPlayRandomVoice = true;
                        m_ProduceingState = Produceing.None;
                    }
                    break;
                default:
                    break;

            }
        }


        private void InitRunMan()
        {
            //豹子位置初始化
            RunMan.transform.localPosition = new Vector3(-130, -115, -30);
         
        }

        public void InitGame()
        {

            GameEntry.GameManager.IsInGame = true;
            //nowTurns = 0;
            //if (GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty) == null)
            //{
            //    MaxTurns = 2;
            //}
            //else {
            //    int temp = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty);
            //    MaxTurns = temp * 2;
            //}

            PlayGameVoice("lesson1_3_1", SoundLevel.Talk);
            m_VoiceTrack = 1;         
            m_CurrentVoiceTimeLength = 5f;
            m_ProduceingState = Produceing.PlayingVoice;

        }

    

        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {

            base.OnRayHitByLeida(go, ve);

            if (go == null)
                return;


      

            if (go.name.Equals("baozi"))
            {

                if (!RunMan.m_IsTouch)
                {
                    if (RunMan.OnLidarHitEvent(go, ve) != null)
                    {


                        HitRunMan();
                        
                        //ShowEffectEntity(new EffectData(GameEntry.Entity.GenerateSerialId(), 70102)
                        //{
                        //    Name = "HitHands",
                        //    Position = RunMan.transform.position,
                        //    KeepTime = 3f,
                        //    //Parent = go.transform,
                        //});
                    }
                }              
            }
        }

      

        //是否在可触发区域内
        public void OnPressArea(object sender, GameEventArgs e)
        {
            ModelPressEventArgs ne = (ModelPressEventArgs)e;

            

            if (ne.IsPress) {
                RunManAni.SetInteger("state", 1);
                //ChangDiAni.speed = 0;
                RunMan.m_IsTouch = false;
                RunMan.CDTime = 1f;
                HandFlash.SetActive(true);
            }
            else{

                RunManAni.SetInteger("state", 0);
                //ChangDiAni.speed = 1;
                RunMan.m_IsTouch = true;
                RunMan.CDTime = -1f;
                HandFlash.SetActive(false);
            }

        
        }


        public void HitRunMan()
        {
            RunMan.CDTime = -1;
            RunMan.m_IsTouch = true;

            RunManAni.SetInteger("state", 0);
            //ChangDiAni.speed = 1;
            PlayGameVoice("Les1_3_HitHand", SoundLevel.Once);
            EF_HitHand.Play("HitHands");


            //nowTurns++;

            //if (nowTurns == MaxTurns)
            //{


            //}
            //else
            //{
            //} 
                PlayGameVoice("Les1_3_Jiasu", SoundLevel.Once);
            
           
        }

       /// <summary>
       /// 按键游戏结束
       /// </summary>
        protected override void SkipGame()
        {
            //游戏结束
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("Les1_3_End", SoundLevel.Once);
            m_CurrentVoiceTime = 0f;
            m_CurrentVoiceTimeLength = 5f;
            m_ProduceingState = Produceing.PlayingVoice;
            m_VoiceTrack = 5000;
        }


        /// <summary>
        /// 播放随机语音
        /// </summary>

        private float CaehTime = 10f;
        private float m_Time;
        private int s_Track = 5;
        private void PlayTTSWord()
        {
            //if (GameEntry.GameManager.IsInGame)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    string ss = Utility.Text.Format("lesson1_3_{0}", s_Track.ToString());
                    PlayGameVoice(ss, SoundLevel.Talk);
                    s_Track++;
                    if (s_Track > 8)
                    {
                        s_Track = 5;
                    }
                    m_Time = CaehTime;
                }
            }
        }

        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson1_3_GroundForm;
                InitGame();
            }

        }



    }
}