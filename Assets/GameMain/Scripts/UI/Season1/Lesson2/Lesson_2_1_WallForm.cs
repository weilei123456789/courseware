using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using UnityEngine;
using DG.Tweening;

namespace Penny
{
    public class Lesson_2_1_WallForm : LessonWallUIFrame
    {

        [SerializeField]
        private GameObject m_PennyGo = null;

        private Vector3[] MoveTF = new Vector3[] {  new Vector3(-9f, -4.8f, 8f), new Vector3(9f, -4.8f, 8f), };

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //数据存储
            drlesson = (DRLesson)userData;
           

            UIOpenEvent(drlesson);
            UIEventSubscribe();

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
        }

        private void InitGame() {

            GameEntry.Entity.ShowCustomEntity(typeof(Model), "WallModel", m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300001) {

                Name = "Penny",
                Position = new Vector3(-9f,-4.8f,8f),
                Rotation = Quaternion.Euler(new Vector3(0f,90f,0f)),
                Scale = new Vector3 (3f,3f,3f),
            });
            
        }

        private Tweener twe;
        private float AniSpeed = 5;
        public void PlayRunmanAni(int key)
        {

     
            if (key == 1)
            {
                twe.Kill();
                m_PennyGo.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                twe = m_PennyGo.transform.DOLocalMove(MoveTF[1], AniSpeed);

            }

            if (key == 3) {

                twe.Kill();
                m_PennyGo.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
                twe = m_PennyGo.transform.DOLocalMove(MoveTF[0], AniSpeed);
            }
        }


        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(Model))
            {
                if (ne.Entity.Logic.Name.Equals("Penny"))
                {
                    m_PennyGo = ne.Entity.Logic.gameObject;
                }
            }
        }


        protected override void OnDiffcultyChange(object sender, GameEventArgs e)
        {
            NormalDifficultyEventArgs ne = (NormalDifficultyEventArgs)e;
            NormalDifficulty dif = ne.Difficulty;
            switch (dif) {
                case NormalDifficulty.Easy:
                    AniSpeed = 8;
                    break;
                case NormalDifficulty.Normal:
                    AniSpeed = 5;
                    break;
                case NormalDifficulty.Hard:
                    AniSpeed = 2;
                    break;
                   

            }
        }
    }
}