using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using DG.Tweening;

namespace Penny
{

    public class Lesson_4_1_GroundForm : LessonGroundUIFrame
    {

        private Vector3[] TFs = new Vector3[] { new Vector3(4,0.01f,-4.2f), new Vector3(4, 0.01f, 1.75f), new Vector3(-4, 0.01f, 4), new Vector3(-4, 0.01f, -4.2f), };

       
        
        //游戏回合数
        private int GameTurn;
        private int EndTurn;

        [SerializeField]
        private bool IsPop = false;

        private float CDTime = 0.8f;
        [SerializeField]
        private float m_Time;
        [SerializeField]
        private GameObject WaterGun;
        [SerializeField]
        private GameObject Truck;

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

            if (GameEntry.GameManager.IsInGame) {
                if (IsPop)
                {
                    m_Time -= Time.deltaTime;
                    if (m_Time < 0)
                    {
                        CreatePop();
                        m_Time = CDTime;
                    }
                }
            }




            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }

        private void InitGame() {

            ReInit();


            for (int i = 0; i < TFs.Length; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 300005)
                {
                    Name = "Point",
                    NewPostion = TFs[i],
                    Rotation = Quaternion.Euler(new Vector3(90,0,0)),
                    Scale = Vector3.one * 0.3f,
                    CodeID = i,
                    CDTime = 5f,
                });
            }

            GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300006)
            {
                Name = "Truck",
                Position = new Vector3(0, 0, 6),
                Rotation = Quaternion.Euler(new Vector3(0, 0, -180)),
                Scale = Vector3.one * 5,
                IsTouch = true,
            });

         

        }

        private void ReInit() {
            GameTurn = 0;
            EndTurn = 0;
        }

        private void ShowWaterGun()
        {
            if (WaterGun == null)
            {
                GameEntry.Entity.ShowWallModel(typeof(Model), m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 300007)
                {
                    Name = "WaterGun",
                    Position = new Vector3(-8.25f, -3, 3),
                    Rotation = Quaternion.Euler(new Vector3(0, 0, -180)),
                    Scale = new Vector3(2.5f, 2, 2),
                    IsTouch = true,
                });
            }
            else
            {
                WaterGun.SetActive(true);
            }
        }

        private void CloseWaterGun() {
            if (WaterGun == null)
            {
                return;
            }
            else {
                WaterGun.SetActive(false);
                GameEntry.Entity.ShowWallModel(typeof(CustomBullet), m_SeasonAssetPath, m_LessonAssetPath, new CustomBulletData(GameEntry.Entity.GenerateSerialId(), 300004)
                {
                    Name = "Bullet",
                    Position = WaterGun.transform.localPosition,
                    Scale = Vector3.one*1.5f,
                    AimGo = Truck,
                    
                });
            }
            
          
        }


        private void CreatePop() {

            float PosX = Random.Range(-2.5f, 4);
            GameEntry.Entity.ShowGroundModel(typeof(Lesson4Pop), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 300005)
            {
                Name = "Pop",
                NewPostion = new Vector3(PosX, 0.01f, 5.5f),
                Rotation = Quaternion.Euler(new Vector3(90, 0, 0)),
                Scale = Vector3.one * 0.15f,
                CDTime = 3f,
            });
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {

            if (go == null)
                return;

            if (go.name == "Point") {
                GroundModel mm = go.GetComponent<GroundModel>();
                if (!mm.m_IsTouch) {
                    mm.BeHit();
                    HitPoint(mm);

                }
            }

            if (go.name == "Pop")
            {
                Lesson4Pop mm = go.GetComponent<Lesson4Pop>();
                if (!mm.m_IsTouch) {
                    mm.BeHit();
                    GameEntry.Sound.PlaySound(20002);
                    GameEntry.Entity.HideEntity(mm);

                }

            }

            }

        private void HitPoint(GroundModel mm) {

            GameEntry.Sound.PlaySound(30002);
            int id = mm.CodeID;
            switch (id) {
                case 0:
                    GameTurn++;
                    break;
                case 1:
                    IsPop = true;
                    ShowWaterGun();
                    break;
                case 2:
                    IsPop = false;
                    CloseWaterGun();
                    break;
                case 3:
                    EndTurn++;
                    break;
            }

        }


        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;

            if (ne.Entity.Logic.name == "Truck")
            {
                Truck = ne.Entity.Logic.gameObject;
                if (Truck.transform.childCount > 0) {
                    Truck.transform.GetChild(0).gameObject.SetActive(false);

                }
            }


            if (ne.Entity.Logic.name == "WaterGun")
            {
                WaterGun = ne.Entity.Logic.gameObject;
            }
        }


    }
}