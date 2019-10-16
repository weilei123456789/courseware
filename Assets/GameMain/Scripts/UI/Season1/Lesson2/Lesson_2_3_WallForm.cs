using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using UnityEngine;
using DG.Tweening;

namespace Penny
{
    public class Lesson_2_3_WallForm : LessonWallUIFrame
    {

        [SerializeField]
        private GameObject m_GroundGo = null;

        [SerializeField]
        private float LandValue = 0.1f;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //数据存储
            drlesson = (DRLesson)userData;
            InitGame();

            UIOpenEvent(drlesson);

            UIEventSubscribe();
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
            
        }


        protected override void GameGroundBgLoad(GameObject obj) {
            base.GameGroundBgLoad(obj);
            m_GroundGo = GroundBgPart.transform.GetChild(0).gameObject;
            m_GroundGo.transform.localScale = Vector3.one * 0.25f;
        }


        public void LandByRun() {
            m_GroundGo.transform.localScale = m_GroundGo.transform.localScale * (1 + LandValue);

        }




    }
}