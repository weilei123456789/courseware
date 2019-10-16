using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_3_2_GroundForm : LessonGroundUIFrame
    {


        private Vector3[] PointTFs = new Vector3[] { new Vector3(1.3f, 0, -3.6f), new Vector3(-4f, 0, 4f), new Vector3(-3f, 0, -3.5f) };

        private Vector3[] LightTFs = new Vector3[] { new Vector3(3.43f, 0.3f, 3.12f), new Vector3(3.43f, 0.3f, 1.43f), new Vector3(3.43f, 0.3f, -0.27f), };

        private GameObject[] lightGo = new GameObject[3];

        private GameObject tempGo;


        private int Track = 0;

      //  private int GameTurn;

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


        private void InitGame()
        {

            GameTurn = 0;
            for (int i = 0; i < PointTFs.Length; i++)
            {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200003)
                {
                    Name = "Point",
                    NewPostion = PointTFs[i],
                    Scale = Vector3.one * 0.25f,
                    CDTime = 5f,
                    CodeID =i,
                });
            }

            for (int i = 0; i < LightTFs.Length; i++)
            {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201001)
                {
                    Name = "Light",
                    NewPostion = LightTFs[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    Scale = Vector3.one,
                    CDTime = 1f,
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

        }

        private void HitPoint(GameObject go)
        {
            GroundModel m_model = go.GetComponent<GroundModel>();
            if (!m_model.m_IsTouch)
            {
                GameEntry.Sound.PlaySound(30002);
                m_model.BeHit();
                switch (m_model.CodeID)
                {
                    case 0:
                        break;
                    case 1:
                        CloseLight();
                        break;
                    case 2:
                        GameTurn++;
                        break;
                }
            }
        }


        private void HitLight(GameObject go) {
            GroundModel m_model = go.GetComponent<GroundModel>();
            if (!m_model.m_IsTouch) {
                m_model.BeHit();
                if (go == tempGo)
                {
                    Track++;
                    if (Track == 5)
                    {
                        CloseChild(go);
                        go.transform.GetChild(4).gameObject.SetActive(true);
                        GameEntry.Sound.PlaySound(30001);
                       
                    } else if (Track > 5)
                    {
                        return;
                    }
                    else {
                        CloseChild(go);
                        go.transform.GetChild(Track - 1).gameObject.SetActive(true);
                    }
                }
                else {
                    Track = 0;
                    tempGo = go;
                }
                
            }
        }

        private void CloseChild(GameObject go) {
            for (int i = 0; i < go.transform.childCount; i++) {
                go.transform.GetChild(i).gameObject.SetActive(false);
            }
        }


        private void CloseLight()
        {
            
            for (int i = 0; i < lightGo.Length; i++)
            {
                lightGo[i].transform.GetChild(4).gameObject.SetActive(false);
            }
        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "Light") {
                GroundModel m_model = (GroundModel)ne.Entity.Logic;
                lightGo[m_model.CodeID] = m_model.gameObject;

            }

        }
    }
}