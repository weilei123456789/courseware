using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_4_3_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TFs = new Vector3[] { new Vector3(2.8f,0,-1.7f), new Vector3(4.5f, 0, -3.3f),
            new Vector3(4.43f, 0, -1.65f), new Vector3(3f, 0, -2.6f),
            new Vector3(2.36f, 0, 0.36f), new Vector3(4.25f, 0, -0.76f),
            new Vector3(4.42f, 0, 0.4f), new Vector3(4f, 0, -0.07f),
            new Vector3(2.5f, 0, -4.45f), new Vector3(2.63f, 0, -3.45f),};

        [SerializeField]
        private GameObject RunMan = null;
        private Lesson_4_3_WallForm WallForm = null;


        [SerializeField]
        //完成回合数
        private int GameTurn;
        private int EndTurn;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();

         
            InitGame();
            WallForm = (Lesson_4_3_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "");
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

            ReInitGame();

            GameEntry.GameManager.IsInGame = true;
            
            GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200002)
            {
                Name = "StartPoint",
                NewPostion = new Vector3(-4,0,4.3f),
                Scale = new Vector3(1,0.1f,1),
                CDTime = 5f,


            });

            GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200002)
            {
                Name = "EndPoint",
                NewPostion = new Vector3(4, 0, -4.2f),
                Scale = new Vector3(1, 0.1f, 1),
                CDTime = 5f,
            });

            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(Lesson4Mogu), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 100001)
                {
                    Name = "Les4Mogu",
                    NewPostion = TFs[i],
                    Scale = Vector3.one,
                    CDTime =3f,
                });


            }

        }

        private void ReInitGame() {
            GameTurn = 0;
            EndTurn = 0;

        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {

            if (go == null)
                return;

            if (go.name == "StartPoint") {
                GroundModel mm = go.GetComponent<GroundModel>();
                if (!mm.m_IsTouch) {
                    mm.BeHit();
                    GameEntry.Sound.PlaySound(30001);
                    GameTurn++;
                }
             
               
            }

            if (go.name == "EndPoint")
            {
                GroundModel mm = go.GetComponent<GroundModel>();
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                    GameEntry.Sound.PlaySound(30001);
                    HitEndPoint();
                    EndTurn++;
                }


            }



            if (go.transform.parent.name == "Les4Mogu")
            {
                Lesson4Mogu mm = go.transform.parent.GetComponent<Lesson4Mogu>();
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                  
                }


            }
        }

        private void HitEndPoint() {

            if (RunMan == null)
                RunMan = WallForm.RunMan;

            Animator ani = RunMan.GetComponent<Animator>();
            bool temp = ani.GetBool("OnArea");
            ani.SetBool("OnArea", (temp != true));
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