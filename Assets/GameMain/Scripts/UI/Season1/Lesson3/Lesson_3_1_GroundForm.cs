using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_3_1_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TFs = new Vector3[] { new Vector3(4,0,-4), new Vector3(4, 0, 4), new Vector3(-4, 0, 4), new Vector3(-4, 0, -4), };

        private GameObject slm1;
        private GameObject slm2;
        
        //游戏回合数
     //   private int GameTurn;

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

            UIEventUnsubscribe();
         
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }

        private void InitGame() {

            GameTurn = 0;

            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200003)
                {
                    Name = "Point",
                    NewPostion = TFs[i],
                    Scale = Vector3.one * 0.25f,
                    CodeID = i,
                    CDTime = 10f,
                });
            }

            GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 300004)
            {
                Name = "shilaimu",
                NewPostion = new Vector3(0, 0, 3.4f),
                Scale = Vector3.one,
                CodeID = 1,
                CDTime = -1,
               
            });

            GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 300004)
            {
                Name = "shilaimu",
                NewPostion = new Vector3(-3.6f, 0, 0),
                Scale = Vector3.one,
                CodeID = 2,
                CDTime = -1,

            });
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {

            if (go == null)
                return;

            if (go.name == "shilaimu") {

                HitSLM(go);
            }

            if (go.name == "Point") {
                HitPoint(go);

            }
        }

        private void HitSLM(GameObject go) {
            GroundModel m_model = go.GetComponent<GroundModel>();
            Animator ani = go.GetComponent<Animator>();
            if (!m_model.m_IsTouch)
            {
                m_model.m_IsTouch = true;
                GameEntry.Sound.PlaySound(30001);              
               ani.Play("behit");
            }
        }

        private void HitPoint(GameObject go)
        {
            GroundModel m_model = go.GetComponent<GroundModel>();
            if (!m_model.m_IsTouch)
            {
                m_model.BeHit();
                GameEntry.Sound.PlaySound(30002);
                switch (m_model.CodeID) {
                    case 0:
                        break;
                    case 1:
                        slm1.GetComponent<GroundModel>().m_IsTouch = false;
                        break;
                    case 2:                     
                        break;
                    case 3:
                        slm2.GetComponent<GroundModel>().m_IsTouch = false;
                        GameTurn++;
                        break;

                }
            }
        }


        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;

            if (ne.Entity.Logic.name == "shilaimu")
            {
                GroundModel m_model = (GroundModel)ne.Entity.Logic;
                if (m_model.CodeID == 1) {
                    slm1 = ne.Entity.Logic.gameObject;
                }
                if (m_model.CodeID == 2)
                {
                    slm2 = ne.Entity.Logic.gameObject;
                }
            }
        }


    }
}