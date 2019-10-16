using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Penny
{

    public class Lesson_3_3_WallForm : LessonWallUIFrame
    {

        public GameObject RunMan;

        private Vector3 StartTF = new Vector3(0, -2, 0);
        private Vector3 EndTF = new Vector3(0, -12, 10);

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
                Position = StartTF,
                Rotation = Quaternion.Euler(new Vector3(0, 180, 0)),
                Scale = Vector3.one * 3,
              
            });


        }




        private Tweener twe;
        private Tweener tweT;
        public void PalyAni() {
            twe.Kill();
            tweT.Kill();
            twe = RunMan.transform.DOLocalMove(EndTF, 3f);
            tweT = RunMan.transform.DOScale(Vector3.one * 10, 3f);
        }

        public void PlayAniBack()
        {
            twe.Kill();
            tweT.Kill();
            twe = RunMan.transform.DOLocalMove(StartTF, 3f);
            tweT = RunMan.transform.DOScale(Vector3.one * 3, 3f);
        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "RunMan") {

                RunMan = ne.Entity.Logic.gameObject;
            }
        }

    }
}