using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using DG.Tweening;
using Spine;
using Spine.Unity;
using GameFramework;

namespace Penny
{

    public class Lesson_6_2_WallForm : LessonWallUIFrame
    {
        [SerializeField]
        private Lesson_6_2_GroundForm GroundForm = null;
        [SerializeField]
        private ObjNormal Tubiao=null;
        [SerializeField]
        private SkeletonGraphic sg = null;
        private int index = 0;
        private float clipTime = 0;
        private int CDTimeMax = 3;
        private bool isRest = false;
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
            UIEventSubscribe();
            PlayBGM("Les1_2_BGM");
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

            if (!GameEntry.GameManager.IsInGame) return;

            if (Tubiao.isCanTouch)
            {
                clipTime += elapseSeconds;
                if (clipTime > CDTimeMax)
                {
                    clipTime = 0;
                    sg.AnimationState.SetAnimation(0, "dzy_zhua1", false);
                }
            }

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
            
            
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            if (!GameEntry.GameManager.IsInGame)
                return;
            if (go == Tubiao.gameObject && Tubiao.isCanTouch)
            {

                Tubiao.isCanTouch = false;
                
                clipTime = 0;
                Tubiao.SetAnimation("effect_1", false);
                sg.AnimationState.SetAnimation(0, "dzy_zhua2", false).Complete+=(e)=> 
                {
                    sg.AnimationState.SetEmptyAnimation(0, 0);
                    Tubiao.AnimationState.SetEmptyAnimation(0, 0);
                    Tubiao.isCanTouch = true;
                };
            }
            

        }

        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson_6_2_GroundForm;
                GuideVoice();
            }

        }
        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 1:
                    IsPlayRandomVoice = true;
                    GameEntry.GameManager.IsInGame = true;
                    m_ProduceingState = Produceing.None;
                    break;
                
                case 6000:
                       

                    m_VoiceTrack = 6001;

                    break;
                case 6001:
                    clipTime += Time.deltaTime;
                    if (clipTime>5)
                    {
                        clipTime = 0;
                        index++;
                        index = (index + 2) / 2;
                        if (index >= 3)
                            index = 0;
                        string name = Utility.Text.Format("dzy_zhua{0}", index.ToString());
                        sg.AnimationState.SetAnimation(0, name, false);
                    }
                    break;
                case 1001:
                    Tubiao.gameObject.SetActive(false);
                    m_ProduceingState = Produceing.None;
                    EntryRest();
                    break;
            }
        }
        /// <summary>
        /// 游戏结束
        /// </summary>
        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("2_over_1", SoundLevel.Talk);
            m_VoiceTrack = 1001;
            m_ProduceingState = Produceing.PlayingVoice;
            m_CurrentVoiceTimeLength = 5f;
        }

        /// <summary>
        /// 墙屏进入休息环节
        /// </summary>
        public void EntryRest()
        {
            GroundForm.EntryRest();
            //地屏动画播放
            PlayRestAni();
        }


        private void PlayRestAni()
        {
           
            
            PlayGameVoice("r_lead_1", SoundLevel.Talk);
            sg.AnimationState.SetAnimation(0, "dzy_zhua2", false).Complete += (e) =>
            {
                sg.AnimationState.SetAnimation(0, "dzy_zhua1", false).Complete += (x) =>
                {
                    sg.AnimationState.SetAnimation(0, "dzy_zhua2", false).Complete += (y) =>
                    {
                        isRest = true;
                        clipTime = 0;
                    };
                };
            };
            m_ProduceingState = Produceing.PlayingVoice;
            m_VoiceTrack = 6000;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 19f;
            

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
            if (VoiceTrack > 2)
            {
                VoiceTrack = 1;
            }
            string soundName = Utility.Text.Format("2_random_{0}", VoiceTrack.ToString());
            PlayGameVoice(soundName, SoundLevel.Talk);
        }
        private void GuideVoice()
        {
            PlayGameVoice("2_lead_1", SoundLevel.Talk);
            m_VoiceTrack = 1;
            m_CurrentVoiceTime = 0;
            m_CurrentVoiceTimeLength = 2f;
            m_ProduceingState = Produceing.PlayingVoice;
        }
    }
}