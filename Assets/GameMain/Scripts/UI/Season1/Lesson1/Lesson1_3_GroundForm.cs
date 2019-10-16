using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using DG.Tweening;

namespace Penny
{

    public class Lesson1_3_GroundForm : LessonGroundUIFrame
    {
        [SerializeField]
        private List<ModelAniOnce> MuBan = new List<ModelAniOnce>();
        [SerializeField]
        private List<ModelIsLight> qishui = new List<ModelIsLight>();
        [SerializeField]
        private List<ModelIsLight> EleProp = new List<ModelIsLight>();


        [SerializeField]
        private ModelBase QiDian = null;
        [SerializeField]
        private ModelIsLight Star = null;

        private GameObject LightTriger = null;


        [SerializeField]
        private Lesson1_3_WallForm WallForm = null;

     
        //[SerializeField]
        //private GameObject LanternTarget = null;
      
      
        //根据难度改变速度
        private float DifSpeed = 4;
        

        private GameObject TriBox = null;

        public static bool OnArea = false;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
        

            UIEventSubscribe();
            UIOpenEvent(userData);




            InitGround();
            WallForm = GameEntry.UI.GetUIForm(drlesson.WallID, "") as Lesson1_3_WallForm;
            GameEntry.Event.Subscribe(ModelPressEventArgs.EventId, OnPressArea);
        }


        protected override void OnClose(object userData)

        {
         

            UIEventUnsubscribe();
            GameEntry.Event.Unsubscribe(ModelPressEventArgs.EventId, OnPressArea);
            base.OnClose(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

            //if (GameEntry.GameManager.IsInGame)
            //    //PlayDifAni();


            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();


            switch (m_ProduceingState)
            {

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
            switch (m_VoiceTrack) {
                case 5001:
                    {
                        ModelPressEventArgs ne = new ModelPressEventArgs(false);
                        GameEntry.Event.Fire(this, ne);
                    }
                    break;
                default:
                    break;
            }
        }

        public void InitGround() {

          
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {
            base.OnRayHitByLeida(go, ve);
         

            if (go == null)
                return;


            for (int i = 0; i < EleProp.Count; i++) {
                if (!EleProp[i].m_IsTouch)
                {
                    if (EleProp[i].OnLidarHitEvent(go, ve) != null) {
                        switch (EleProp[i].OnLidarHitEvent(go, ve))
                        {
                            case -1:
                                PlayGameVoice("Les1_3_cookie", SoundLevel.Once);
                                break;
                            case 1:
                                PlayGameVoice("Les1_3_1_do", SoundLevel.Once);
                                break;
                            case 2:
                                PlayGameVoice("Les1_3_2_re", SoundLevel.Once);
                                break;
                            case 3:
                                PlayGameVoice("Les1_3_3_mi", SoundLevel.Once);
                                break;
                            case 4:
                                PlayGameVoice("Les1_3_4_fa", SoundLevel.Once);
                                break;
                            case 5:
                                PlayGameVoice("Les1_3_5_sol", SoundLevel.Once);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }


            if (go.name == "baoziQidian")
            {
                if (!QiDian.m_IsTouch) {
                    if (QiDian.OnLidarHitEvent(go, ve) != null) {
                        HitStartPoint();
                    }
                }
            }

            if (go.name == "qishui")
            {
                for (int i = 0; i < qishui.Count; i++) {
                    if (!qishui[i].m_IsTouch)
                    {
                        if (qishui[i].OnLidarHitEvent(go, ve) != null)
                        {
                            Hitqishui(qishui[i]);
                        }
                    }

                }          
            }
         
            if (go.name == "MuBan") {
                for (int i = 0; i < MuBan.Count; i++) {
                    if (!MuBan[i].m_IsTouch) {
                        if (MuBan[i].OnLidarHitEvent(go, ve) != null) {
                            HitMuBan(MuBan[i]);
                        }
                    }

                }             
            }

            if (!Star.m_IsTouch) {
                if (Star.OnLidarHitEvent(go, ve) != null)
                {
                    BoxTigHit();
                }
             
            }

        }


        private void HitStartPoint() {
           
         
        }

        public void Hitqishui(ModelIsLight modLg)
        {

            PlayGameVoice("Les1_3_ZhuanPin", SoundLevel.Once);

            //WallForm.PlayTTSWord();

            modLg.modelAni.Play("qishuizhuan");

            int id = modLg.CodeID;
        }



        //private GameObject tempgo;
        public void BoxTigHit()
        {
                          
                ModelPressEventArgs ne = new ModelPressEventArgs(true);
                GameEntry.Event.Fire(this, ne);

                Star.transform.DOShakePosition(2f, 20f);
                PlayGameVoice("Les1_3_Star", SoundLevel.Once);

            ///延时抛出不在地点判断 大于CD时间
            m_ProduceingState = Produceing.Delaying;
            m_VoiceTrack = 5001;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 4f;

           
            

        }

        private void HitMuBan(ModelHasAni go) {
                  
            PlayGameVoice("Les1_3_MuBan", SoundLevel.Once);

         
        }




        //是否在可触发区域内
        public void OnPressArea(object sender, GameEventArgs e)
        {
            ModelPressEventArgs ne = (ModelPressEventArgs)e;

            if (ne.IsPress)
            {
                if (!Star.transform.GetChild(0).gameObject.activeSelf) {
                    Star.transform.GetChild(0).gameObject.SetActive(true);                
                }
            }
            else
            {
                if (Star.transform.GetChild(0).gameObject.activeSelf)
                {
                    Star.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
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
