using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
//using System;
using DG.Tweening;
using Spine;
using GameFramework;

namespace Penny
{

    public class Lesson_6_1_WallForm : LessonWallUIFrame
    {
        [SerializeField]
        private Lesson_6_1_GroundForm GroundForm = null;
        [SerializeField]
        private ObjNormal daPao;
        [SerializeField]
        private ObjHaiDan[] objH;
        private int haidanNum = 5;
        private int HaidanNumMax = 5;
        private bool isCanAddHaidan = false;

        private float clipTime = 0;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
            UIEventSubscribe();
            GameEntry.Event.Subscribe(ModelPressEventArgs.EventId, OnModelPressSuccess);

            PlayBGM("Les1_1_BGM");
            InitGame();
        }

        private void OnModelPressSuccess(object sender, GameEventArgs e)
        {
            daPao.transform.DOLocalRotate(new Vector3(0, 0, -20), 0.5f).SetEase(Ease.Linear).OnComplete(()=> 
            {
                daPao.CanTouch = true;
            });

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

            if (isCanAddHaidan)
            {
                clipTime += elapseSeconds;
                if (clipTime>=5)
                {
                    clipTime = 0;
                    for (int i = 0; i < objH.Length; i++)
                    {
                        if (!objH[i].gameObject.activeSelf)
                        {
                            objH[i].gameObject.SetActive(true);
                            objH[i].transform.SetLocalPositionX(Random.Range(-580, 790));
                            objH[i].transform.SetLocalPositionY(Random.Range(-250, 250));
                            haidanNum++;
                            break;
                        }
                    }
                    if (haidanNum==HaidanNumMax)
                    {
                        isCanAddHaidan = false;
                    }
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

        private void InitGame()
        {

            GameEntry.GameManager.IsInGame = true;

            for (int i = 0; i < objH.Length; i++)
            {
                objH[i].transform.SetLocalPositionX(Random.Range(-580, 790));
                objH[i].transform.SetLocalPositionY(Random.Range(-250, 250));
                objH[i].posInit = objH[i].transform.localPosition;
                objH[i].floatHaidan(new Vector3(30,30,0));
            }

        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);

            if (go==daPao.gameObject&&daPao.CanTouch)
            {
                daPao.CanTouch = false;
                daPao.SetAnimation("effect_2", false).Complete += Fire;
            }

            
        }

        private void Fire(TrackEntry trackEntry)
        {
            haidanNum--;
            if(haidanNum==3)
                isCanAddHaidan = true;
            int randomNum = Random.Range(0, 5);
            PlayGameVoice("MagicDis", SoundLevel.Once);
            objH[randomNum].SetAnimation("effect_2", false).Complete += (e) => 
            {
                objH[randomNum].gameObject.SetActive(false);
                objH[randomNum].SetAnimation("effect_1", true);
            };
            daPao.transform.DOLocalRotate(new Vector3(0, 0, -50), 0.5f).SetEase(Ease.Linear);
        }

        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson_6_1_GroundForm;

                m_VoiceTrack = 5000;
                m_ProduceingState = Produceing.Delaying;
               

            }

        }

        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 5000:
                    PlayGameVoice("1_lead_1", SoundLevel.Talk);
                    m_VoiceTrack = 1;
                    m_CurrentVoiceTimeLength = 5f;
                    m_ProduceingState = Produceing.PlayingVoice;
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
            if (VoiceTrack > 2)
            {
                VoiceTrack = 1;
            }
            string soundName = Utility.Text.Format("1_random_{0}", VoiceTrack.ToString());
            PlayGameVoice(soundName, SoundLevel.Talk);
        }

        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("1_over_1", SoundLevel.Once);
            m_VoiceTrack = 1001;
            m_ProduceingState = Produceing.PlayingVoice;
            m_CurrentVoiceTimeLength = 5f;
        }

    }
}
