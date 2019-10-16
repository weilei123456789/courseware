using GameFramework.Event;
using UnityGameFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Penny
{

    public class Lesson_2_1_GroundForm : LessonGroundUIFrame
    {

        //判定点
        private Vector3[] PointTF = new Vector3[] { new Vector3(-3.75f,0.1f,3.75f),
                                                    new Vector3(-3.75f, 0.1f, -3.75f),
                                                    new Vector3(3.75f, 0.1f, -3.75f),
                                                    new Vector3(3.75f, 0.1f, 3.75f), };

        //花纹
        private Vector3[] FigureTF = new Vector3[] { new Vector3(-2f, 0.1f, -4.3f), new Vector3(0f, 0.1f, -4.3f), new Vector3(2f, 0.1f, -4.3f),
                                                     new Vector3(-2f, 0.1f, 4.3f), new Vector3(0f, 0.1f, 4.3f), new Vector3(2f, 0.1f, 4.3f),
                                                     new Vector3(-4.2f, 0.1f, 2f), new Vector3(-4.2f, 0.1f,0f), new Vector3(-4.2f, 0.1f, -2f),
                                                     new Vector3(4.17f, 0.1f, 2f), new Vector3(4.17f, 0.1f,0f), new Vector3(4.17f, 0.1f, -2f), };

        private string SoundPath = "Lesson_2_1_{0}";

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //数据存储
            UIOpenEvent(userData);
            //drlesson = (DRLesson)userData;
            //m_LessonAssetPath = drlesson.LessonPath;
            InitGame();
            UIEventSubscribe();
        }

        protected override void OnClose(object userData)

        {
            base.OnClose(userData);


          
            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (GameEntry.GameManager.IsNowCam)
                RayHitByPC();
        }


        private void InitGame() {


            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 2));
            GameEntry.Sound.PlaySound(path, "Sound");

            for (int i = 0; i < PointTF.Length; i++) {
                GameEntry.Entity.ShowCustomEntity(typeof(Lesson2_1_ground_Entity), "GroundModel", m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201001)
                {
                    Name = "Point",
                    NewPostion = PointTF[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    CodeID = i,
                    CDTime = 1f,
                });
            }

            for (int i = 0; i < FigureTF.Length; i++)
            {
                GameEntry.Entity.ShowCustomEntity(typeof(Lesson2_1_ground_Entity), "GroundModel", m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201001)
                {
                    Name = "Unuse",
                    NewPostion = FigureTF[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    CodeID = -1,
                    CDTime = 1f,
                });
            }
        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 vc) {
          

            if (go == null)
                return;
          
            Lesson2_1_ground_Entity mm =go.GetComponent<Lesson2_1_ground_Entity>();
            if (mm == null)
                return;

            if (go.name.Equals("Unuse")) {
                if (!mm.m_IsTouch) {
                    mm.BeHit();
                }
            }

            if (go.name.Equals("Point"))
            {
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                    string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 1));
                    GameEntry.Sound.PlaySound(path, "Sound");
                    ((Lesson_2_1_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "")).PlayRunmanAni(mm.CodeID);
                }
            }
        }


    }
}