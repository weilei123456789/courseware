using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class Lesson_3_3_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TFs = new Vector3[] { new Vector3(-4.2f,0,-4), new Vector3(-4.2f, 0, 4.5f), new Vector3(4.35f, 0, 4.5f), new Vector3(4.35f, 0, -4f) };
        private int m_wallid;

        private Lesson_3_3_WallForm m_WallForm;

        [SerializeField]
        //完成回合数
        private int GameTurn;
        //private int IsNewTurn;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();

            m_wallid = drlesson.WallID;
            InitGame();
            m_WallForm = ((Lesson_3_3_WallForm)GameEntry.UI.GetUIForm(m_wallid, ""));
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
            GameEntry.GameManager.IsInGame = true;
            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200003)
                {
                    Name = "Rock",
                    NewPostion = TFs[i],
                    Scale = Vector3.one * 0.2f,
                    CDTime = 5f,
                    CodeID = i,
                });

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


        private void OnHitRock(GameObject go) {
            GroundModel m_model = go.GetComponent<GroundModel>();
            if (!m_model.m_IsTouch) {
                m_model.BeHit();
                GameEntry.Sound.PlaySound(30002);

                switch (m_model.CodeID) {
                    case 0:
                        break;
                    case 1:
                        PlayAni();
                        break;
                    case 2:
                        PlayAniBack();
                        break;
                    case 3:
                        GameTurn++;
                        break;
                }

            }

        }


        private void PlayAni() {
          
            m_WallForm.PalyAni();
        }

        private void PlayAniBack() {
           
            m_WallForm.PlayAniBack();
        }


        }
    }