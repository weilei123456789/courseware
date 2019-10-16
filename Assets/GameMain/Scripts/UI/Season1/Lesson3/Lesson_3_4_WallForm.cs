using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_3_4_WallForm : LessonWallUIFrame
    {

        //触控点坐标
        private Vector3[] TFs = new Vector3[] {new Vector3(-6.5f,-0.3f,0), new Vector3(0, -0.3f, 0), new Vector3(6.5f, -0.3f, 0), };

        //替代资源坐标变量
        private Vector3 ChangeTF = new Vector3(0, -4, 4);

        private GameObject[] GOs = new GameObject[3];
        public GameObject[] Tris = new GameObject[3];

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

            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowCustomEntity(typeof(Model), "WallModel", m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300003)
                {
                    Name = "Box",
                    Position = TFs[i],
                    Scale = new Vector3(5, 1, 8),
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    CodeID = i,
                    CDTime = -1,
                });

                GameEntry.Entity.ShowCustomEntity(typeof(Model), "WallModel", m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300001)
                {
                    Name = "RunMan",
                    Position = TFs[i]+ChangeTF,
                    Scale = new Vector3(5, 5, 5),
                    Rotation = Quaternion.Euler(new Vector3(0,180,0)),
                    CodeID = i,
                  
                });
            }

        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "Box") {
                Model mm = go.GetComponent<Model>();
                if (!mm.m_IsTouch) {
                    mm.m_IsTouch = true;
                    OnClickTri(mm.CodeID);
                }
            }

        }

        private void OnClickTri(int id) {

            GameEntry.Sound.PlaySound(30001);
            bool temp = GOs[id].GetComponent<Animator>().GetBool("OnArea");
          
            GOs[id].GetComponent<Animator>().SetBool("OnArea", (temp!= true));

        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "RunMan") {
                Model mm = (Model)ne.Entity.Logic;
                GOs[mm.CodeID] = mm.gameObject;
            }

            if (ne.Entity.Logic.name == "Box")
            {
                Model mm = (Model)ne.Entity.Logic;
                Tris[mm.CodeID] = mm.gameObject;
            }
        }

    }
}