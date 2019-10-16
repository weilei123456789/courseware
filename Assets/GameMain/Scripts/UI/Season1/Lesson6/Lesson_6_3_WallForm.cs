using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using Spine.Unity;
using GameFramework;

namespace Penny
{

    public class Lesson_6_3_WallForm : LessonWallUIFrame
    {
        [SerializeField]
        private ObjBase Tubiao = null;
        
        [SerializeField]
        private ObjBigZY objBigZY = null;

       
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
            UIEventSubscribe();
            PlayBGM("Les1_3_BGM");
            InitGame();
        }

        protected override void OnClose(object userData)
        {
            base.OnClose(userData);

            UICLoseEvent();
            UIEventUnsubscribe();
           
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);


            if (IsPlayRandomVoice)
            {
                m_RandomTrack += elapseSeconds;
                if (m_RandomTrack > m_RandomLenth)
                {
                    PlayRandomVoice();
                    m_RandomTrack = 0;
                }
            }


            
        }

        private void InitGame() {

            //GameEntry.GameManager.IsInGame = false;
            

        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            
            if (go == Tubiao.gameObject && Tubiao.isCanTouch)
            {

                Tubiao.isCanTouch = false;

                
                Tubiao.SetAnimation("effect_1", false).Complete+=(x)=> { Tubiao.isCanTouch = true; };

                if (objBigZY.State == ObjBigZY.ZYState.Play)
                {
                    objBigZY.isReplay = true;
                }
                else
                {
                    objBigZY.SetAnimation("dzy_zhua3", false);
                    objBigZY.SetState(ObjBigZY.ZYState.Play);
                }
                
                
            }

        }
        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            m_VoiceTrack = 5000;
            m_ProduceingState = Produceing.Delaying;
        }
        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 5000:
                    PlayGameVoice("3_lead_1", SoundLevel.Talk);
                    m_VoiceTrack = 1;
                    m_CurrentVoiceTimeLength = 8f;
                    m_ProduceingState = Produceing.PlayingVoice;
                    break;
                case 1:
                    GameEntry.GameManager.IsInGame = true;
                    break;
                case 1001:
                    m_ProduceingState = Produceing.EndGame;
                    break;
            }
        }

        /// <summary>
        /// 随机语音播放
        /// </summary>
        private bool IsPlayRandomVoice = false;
        private float m_RandomTrack = 0;
        private float m_RandomLenth = 10f;
        private int VoiceTrack = 1;
        private void PlayRandomVoice()
        {
            VoiceTrack++;
            if (VoiceTrack > 3)
            {
                VoiceTrack = 1;
            }
            string soundName = Utility.Text.Format("3_random_{0}", VoiceTrack.ToString());
            PlayGameVoice(soundName, SoundLevel.Talk);
        }
        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("3_over_1", SoundLevel.Once);
            m_VoiceTrack = 1001;
            m_ProduceingState = Produceing.PlayingVoice;
            m_CurrentVoiceTimeLength = 2f;
        }


    }
}