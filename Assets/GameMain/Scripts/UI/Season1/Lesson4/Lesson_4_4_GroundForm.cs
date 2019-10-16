using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Penny
{

    public class Lesson_4_4_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TriTF = new Vector3[] { new Vector3(-3.5f, 0, 1.8f), new Vector3(3.5f, 0, 1.8f), };
        private Lesson_4_4_WallForm WallForm;
        private GameObject Clover;
       

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();

          

            InitGame();
            WallForm = (Lesson_4_4_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "");
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
            for (int i = 0; i < TriTF.Length; i++)
            {

                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 300002)
                {
                    Name = "Les4Box",
                    NewPostion = TriTF[i],
                    Rotation = Quaternion.Euler(Vector3.zero),
                    Scale = new Vector3(2, 0.3f, 3),
                    CodeID = i,
                    CDTime = -1,
                    IsTouch = false,
                });
            }
        }
        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "Les4Box")
            {
                GroundModel mm = go.GetComponent<GroundModel>();
                if (!mm.m_IsTouch)
                {
                    mm.m_IsTouch = true;
                    OnHitBOX(go);
                }
            }

        }


        private void OnHitBOX(GameObject go)
        {
            if (Clover == null)
                Clover = WallForm.Clover;

            go.transform.GetChild(0).gameObject.SetActive(true);
            Model mm = Clover.GetComponent<Model>();
            mm.m_IsTouch = false;
            Clover.transform.GetChild(0).gameObject.SetActive(true);

        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "Clover")
            {
                Clover = ne.Entity.Logic.gameObject;
            }
        }
    }
    
}