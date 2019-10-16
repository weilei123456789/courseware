
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_4_2_GroundForm : LessonGroundUIFrame
    {


        private Vector3[] PointTFs = new Vector3[] { new Vector3(-1.7f, 0.1f, 3.5f), new Vector3(-0.7f, 0.1f, 3.5f),
           new Vector3(0.36f, 0.1f, 3.5f), new Vector3(1.36f, 0.1f, 3.5f), new Vector3(2.36f, 0.1f, 3.5f), };

        private Vector3[] LightTFs = new Vector3[] { new Vector3(2f, 0.3f,-3.3f), new Vector3(1.3f, 0.3f, -4.3f),
            new Vector3(0.3f, 0.3f,-3.3f), new Vector3(-0.5f, 0.3f, -4.3f), new Vector3(-1.48f, 0.3f,-3.3f),};

        private Vector3[] TFs = new Vector3[] { new Vector3(-4f, 0.1f, -4f), new Vector3(3.67f, 0.1f, 3.67f), new Vector3(3.83f, 0.1f, -3.86f), };


        private Model Truck = null;

        private int GameTurn;
        private int EndTurn;

        private Lesson_4_2_WallForm WallForm = null;


        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
            UIEventSubscribe();

            WallForm = (Lesson_4_2_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "");
            InitGame();
        }

        protected override void OnClose(object userData)
        {
            base.OnClose(userData);
            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }


        private void InitGame()
        {

            GameTurn = 0;
            for (int i = 0; i < PointTFs.Length; i++)
            {
                GameEntry.Entity.ShowGroundModel(typeof(Lesson2_1_ground_Entity), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201001)
                {
                    Name = "Point",
                    NewPostion = PointTFs[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    Scale = new Vector3(0.5f,1.5f,1f),
                    CDTime = 3f,
                    CodeID =i,
                });
            }

            for (int i = 0; i < LightTFs.Length; i++)
            {
                GameEntry.Entity.ShowGroundModel(typeof(Lesson2_1_ground_Entity), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201001)
                {
                    Name = "Light",
                    NewPostion = LightTFs[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    Scale = Vector3.one * 0.5f,
                    CDTime = 1f,
                    CodeID = i,
                });
            }

            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200002)
                {
                    Name = "Cube",
                    NewPostion = TFs[i],

                    Scale = new Vector3(1.5f,0.2f,1.5f),
                    CDTime = 3f,
                    CodeID = i,
                });
            }

        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "Point")
            {
                HitPoint(go);
            }

            if (go.name == "Light") {
                HitLight(go);
            }

            if (go.name == "Cube")
            {
                HitCube(go);
            }
        }

        private void HitPoint(GameObject go) {
            Lesson2_1_ground_Entity mm = go.GetComponent<Lesson2_1_ground_Entity>();
            if (mm.m_IsTouch)
                return;

            mm.BeHit();
            GameEntry.Sound.PlaySound(30002);
            if (mm.CodeID == 0) {
                //可以拍击墙屏
                if (Truck == null)
                    Truck = WallForm.Truck.GetComponent<Model>();
                Truck.m_IsTouch = false;
            }

        }


        private void HitLight(GameObject go) {
            Lesson2_1_ground_Entity mm = go.GetComponent<Lesson2_1_ground_Entity>();
            if (mm.m_IsTouch)
                return;

            mm.BeHit();
            GameEntry.Sound.PlaySound(30002);

        }

        private void PlayBackAni() {
            if (WallForm.GameTurn == 0)
            {
                WallForm.PlayAniBak();
            }
        }


        private void HitCube(GameObject go) {
            GroundModel mm = go.GetComponent<GroundModel>();
            if (mm.m_IsTouch)
                return;

            mm.BeHit();
            GameEntry.Sound.PlaySound(30002);
            switch (mm.CodeID) {
                case 0:
                    GameTurn++;
                    break;
                case 1:
                    GameEntry.Sound.PlaySound(30000);
                    PlayBackAni();
                    break;
                case 2:
                    EndTurn++;
                    break;

            }
        }


    


     

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "Truck") {
                Truck = (Model)ne.Entity.Logic;
            }

        }
    }
}