using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using DG.Tweening;

namespace Penny
{

    public class Lesson_4_2_WallForm : LessonWallUIFrame
    {
        [SerializeField]
        public  int GameTurn;

        public GameObject Truck;
        private GameObject Chuang;

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
            GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300006)
            {
                Name = "Truck",
                Position = new Vector3(0, 0, 6),
                Rotation = Quaternion.Euler(new Vector3(0, 0, -180)),
                Scale = new Vector3(10,6,6),
                CDTime = -1,
                IsTouch = true,
            });

        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "Truck") {
                Model mm = go.GetComponent<Model>();
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                    HitTruck();
                }
            }

        }

        private void HitTruck() {
            GameEntry.Sound.PlaySound(30001);
            GameTurn++;
            if (GameTurn > 2)
            {
                GameTurn = 0;
               
                PlayAni();
            }
            else {
                Chuang.transform.localScale = new Vector3(0.1f, Chuang.transform.localScale.y+0.05f, 0.1f);
            }

        }

        public void PlayAni() {
            Truck.transform.DOLocalMoveX(16, 1.5f);
        }

        public void PlayAniBak() {
            Truck.transform.DOLocalMoveX(0, 3f);
            Chuang.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
        }


        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "Truck")
            {
                Truck = ne.Entity.Logic.gameObject;
                Chuang =    Truck.transform.GetChild(0).gameObject;
                Chuang.SetActive(true);
                Chuang.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
            }

        }

    }
}