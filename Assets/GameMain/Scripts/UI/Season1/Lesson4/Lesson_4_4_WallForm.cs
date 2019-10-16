using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson_4_4_WallForm : LessonWallUIFrame
    {

        public GameObject Clover;
        public List<GameObject> Box = new List<GameObject>();

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
            Box.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();

        }


        private void InitGame() {
            GameEntry.GameManager.IsInGame = true;
            Box.Clear();

            GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300001)
            {
                Name = "RunModel",
                Position = new Vector3(-4, -4, 6),
                Rotation = Quaternion.Euler(new Vector3(0, 180, 0)),
                Scale = Vector3.one * 3,
                CDTime = 5f,
            });

            GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 201001)
            {
                Name = "Clover",
                Position = new Vector3(5, -2, 6),
                
                Scale = Vector3.one,
                Rotation = Quaternion.Euler(new Vector3(0,180,0)),
                CDTime = -1f,
            });

        }
     


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (go.name == "RunModel") {
                Model mm = go.GetComponent<Model>();
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();                 
                    OnHitRunman(go);
                }              
            }

            if (go.name == "Clover")
            {
                Model mm = go.GetComponent<Model>();
                if (!mm.m_IsTouch)
                {
                    mm.m_IsTouch = true;
                    OnHitClover(go);
                }
            }

        }

        private void OnHitRunman(GameObject go) {
            GameEntry.Sound.PlaySound(30001);
            bool temp = go.GetComponent<Animator>().GetBool("OnArea");

            go.GetComponent<Animator>().SetBool("OnArea", (temp != true));
        }

        private void OnHitClover(GameObject go) {

            GameEntry.Sound.PlaySound(30001);

            Clover.transform.GetChild(0).gameObject.SetActive(false);

            for (int i = 0; i < Box.Count; i++) {
                GroundModel mm = Box[i].GetComponent<GroundModel>();
                mm.m_IsTouch = false;
                Box[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }


        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.Logic.name == "Clover") {
                Clover = ne.Entity.Logic.gameObject;
            }

            if (ne.Entity.Logic.name == "Les4Box")
            {
                Box.Add(ne.Entity.Logic.gameObject);
            }
        }

    }
}