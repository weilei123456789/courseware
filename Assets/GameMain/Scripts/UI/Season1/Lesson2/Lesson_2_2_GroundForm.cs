using GameFramework.Event;
using UnityGameFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Penny
{

    public class Lesson_2_2_GroundForm : LessonGroundUIFrame
    {

        //判定点
        //private Vector3[] PointTF = new Vector3[] { new Vector3(-2f,0.1f,-2.5f),
        //                                            new Vector3(-2f,0.1f,0f),
        //                                            new Vector3(-2f,0.1f,2.5f),
        //                                            new Vector3(2f,0.1f,-2.5f),
        //                                            new Vector3(2f,0.1f,0f),
        //                                            new Vector3(2f,0.1f,2.5f), };

        private string SoundPath = "Lesson_2_2_{0}";


        private int ScroeTrack = 0;

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

            ScroeTrack = 0;

            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (GameEntry.GameManager.IsNowCam)
                RayHitByPC();
        }


        private void InitGame() {
            int Num = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty);
            List<Vector3> PPTF = PointTFByNum(Num);

            for (int i = 0; i < PPTF.Count; i++) {
                GameEntry.Entity.ShowGroundModel(typeof(Lesson2_2_Kedou), m_SeasonAssetPath, m_LessonAssetPath,
                    new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201005)
                {
                    Name = "KeDouPoint",
                    NewPostion = PPTF[i],
                    Rotation = Quaternion.Euler(new Vector3(90, 0, 0)),
                    Scale = Vector3.one * 0.5f,
                    CodeID = i,
                    CDTime = 3f,
                });
              
            }


            ScroeTrack = 0;

            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 1));
            GameEntry.Sound.PlaySound(path, "Sound");

        }

        private void OnHitIce() {

            ScroeTrack++;
            if (ScroeTrack == 10) {

                string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 4));
                GameEntry.Sound.PlaySound(path, "Sound");
            }

            if (ScroeTrack >= 30) {

                string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 6));
                GameEntry.Sound.PlaySound(path, "Sound");


                ScroeTrack = 0;
            }

        }

        private List<Vector3> PointTFByNum(int Num)
        {
            if (Num > 10) {
                Num = 10;
            }

            if (Num <= 0) {
                Num = 1;
            }

            List<Vector3> PTF = new List<Vector3>();
            int Line = Num / 2 + Num % 2;
           
            for (int i = 0; i < Line; i++) {
                float zz = 6f - ((float)(12f / (float)(Line + 1)) * (i+1));
                Log.Info("坐标" + zz);
                PTF.Add(new Vector3(-2f, 0.1f, zz));
                PTF.Add(new Vector3(2f, 0.1f, zz));
            }

            return PTF;
        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc) {
          

            if (go == null)
                return;


            if (go.name.Equals("LeftIce")|| go.name.Equals("RightIce"))
            {
                Lesson2_2_Ice mm = go.GetComponent<Lesson2_2_Ice>();
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                    OnHitIce();
                    GameEntry.Sound.PlaySound(30003);
                }
            }
        }


      

    }
}