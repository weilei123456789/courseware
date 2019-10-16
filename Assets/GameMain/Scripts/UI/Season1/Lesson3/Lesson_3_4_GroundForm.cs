using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class Lesson_3_4_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TFs = new Vector3[] { new Vector3(-3.5f, 0, 3.75f), new Vector3(0, 0, 3.75f), new Vector3(3.5f, 0, 3.75f) };

        private int wallid; 


        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();


            wallid = drlesson.WallID;

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



        private void InitGame() {


            for (int i = 0; i< TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200003)
                {
                    Name = "Rock",
                    NewPostion = TFs[i],
                    Scale = new Vector3(0.2f, 0.2f, 0.2f),
                    CodeID = i,
                    CDTime = 5f,
                });

            }
        }


        private void OnHitRock(GameObject go) {
            GroundModel mm = go.GetComponent<GroundModel>();
            if (!mm.m_IsTouch) {
                mm.BeHit();
                ((Lesson_3_4_WallForm)GameEntry.UI.GetUIForm(wallid, "")).Tris[mm.CodeID].GetComponent<Model>().m_IsTouch = false;
                GameEntry.Sound.PlaySound(30003);
            }

        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "Rock") {
                OnHitRock(go);

            }
        }
    }
}