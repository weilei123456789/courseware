using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Penny
{

    public class Lesson_4_3_WallForm : LessonWallUIFrame
    {

        public GameObject RunMan = null;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
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

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }

        private void InitGame() {

            GameEntry.GameManager.IsInGame = true;
            GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300001)
            {
                Name = "RunMan",
                Position = new Vector3(0,-4.5f,3),
                Rotation = Quaternion.Euler(new Vector3(0, -90, 0)),
                Scale = Vector3.one * 3,
              
            });


        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "RunMan")
            {

                RunMan = ne.Entity.Logic.gameObject;
            }
        }








    }
}