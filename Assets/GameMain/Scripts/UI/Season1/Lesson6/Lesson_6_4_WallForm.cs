using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using DG.Tweening;
using GameFramework;

namespace Penny
{

    public class Lesson_6_4_WallForm : LessonWallUIFrame
    {
        [SerializeField]
        private ObjBase Tubiao = null;
        [SerializeField]
        private ObjBigLan objBigLan = null;
        [SerializeField]
        private GameObject Qianshui = null;
        [SerializeField]
        private ObjBase[] objFish = null;
        private Lesson_6_4_GroundForm GroundForm;
        private Vector3 fishPosInit = new Vector3 ( -250, 0, 0 );
        private Vector3 fishPosMiddle = new Vector3(0,0,0);
        private Vector3 fishPosEnd = new Vector3(250,0,0);
        private int index = 0;
        private bool isQSCanTouch = true;
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
            if (!Tubiao.gameObject.activeSelf)
            {
                Tubiao.gameObject.SetActive(true);
            }
            

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

            
        }


        private void InitGame() {
            
            objFish[0].transform.localPosition = fishPosMiddle;
            objFish[1].transform.localPosition = fishPosInit;
            objFish[2].transform.localPosition = fishPosInit;
        }
     


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            if (!GameEntry.GameManager.IsInGame) return;
            if (go == Tubiao.gameObject && Tubiao.isCanTouch)
            {
                

                //Tubiao.isCanTouch = false;

                Tubiao.OnLiDarHitEvent(go, vc);
                Tubiao.SetAnimation("effect_1", false).Complete += (x) => 
                {
                    //Tubiao.gameObject.SetActive(false);
                    //Tubiao.isCanTouch = true;
                    PlayGameVoice("4_round_1", SoundLevel.Talk);
                };

                if (objBigLan.State != ObjBigLan.ZYState.Play)
                {
                    objBigLan.SetAnimation("xiaoqing_jushou", false);
                    objBigLan.SetState(ObjBigLan.ZYState.Play);
                }


            }
            if (go==Qianshui&&isQSCanTouch)
            {
                isQSCanTouch = false;
                PlayRandomVoice();
                objFish[index].transform.DOLocalMoveX(fishPosEnd.x, 1).SetEase(Ease.Linear).OnComplete(()=>
                {
                    objFish[index].transform.localPosition = fishPosInit;
                    index++;
                    index= (index+2)/2;
                    if (index >= 2)
                        index = 0;
                    objFish[index].transform.DOLocalMoveX(fishPosMiddle.x, 1).SetEase(Ease.Linear).OnComplete(()=> 
                    {
                        isQSCanTouch = true;
                    });
                });
            }





        }


        protected override void OnUIFormOpenSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = e as OpenUIFormSuccessEventArgs;
            if (ne.UIForm.SerialId == m_GroundFormSerialId)
            {

                GroundForm = ne.UIForm.Logic as Lesson_6_4_GroundForm;

                m_VoiceTrack = 5000;
                m_ProduceingState = Produceing.Delaying;


            }
            
        }
        protected override void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                case 5000:

                    PlayGameVoice("4_lead_1", SoundLevel.Talk);
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
       
        private int VoiceTrack = 1;
        private void PlayRandomVoice()
        {
            VoiceTrack = Utility.Random.GetRandom(1, 4);
            string soundName = Utility.Text.Format("4_random_{0}", VoiceTrack.ToString());
            PlayGameVoice(soundName, SoundLevel.Talk);
        }
        protected override void SkipGame()
        {
            GameEntry.GameManager.IsInGame = false;

            PlayGameVoice("4_over_1", SoundLevel.Once);
            m_VoiceTrack = 1001;
            m_ProduceingState = Produceing.PlayingVoice;
            m_CurrentVoiceTimeLength = 4f;
        }
    }
}