using GameFramework.Event;
using UnityGameFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;


namespace Penny
{

    public class Lesson_2_3_GroundForm : LessonGroundUIFrame
    {

        //判定点
        private Vector3[] PointTF = new Vector3[] { new Vector3(-3.5f,0.1f,4f),
                                                    new Vector3(-3.5f, 0.1f, -4f),
                                                    new Vector3(3.5f, 0.1f, -4f),
                                                    new Vector3(3.5f, 0.1f, 4f), };


        private string SoundPath = "Lesson_2_3_{0}";

        private List<bool> Terms = new List<bool>();

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

            Terms.Clear();
          
            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (GameEntry.GameManager.IsNowCam)
                RayHitByPC();
        }


        private void InitGame() {

            for (int i = 0; i < PointTF.Length; i++) {
                GameEntry.Entity.ShowCustomEntity(typeof(GroundModel), "GroundModel", m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201003)
                {
                    Name = "YunPoint",
                    NewPostion = PointTF[i],
                    Rotation = Quaternion.Euler(new Vector3(-90, 0, 0)),
                    Scale = Vector3.one * 2,
                    CodeID = i,
                    CDTime = 3f,
                });
                Terms.Add(false);
            }

            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 1));
            GameEntry.Sound.PlaySound(path, "Sound");


        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 vc) {
          

            if (go == null)
                return;

            GroundModel mm =go.GetComponent<GroundModel>();
            if (mm == null)
                return;



            if (go.name.Equals("YunPoint"))
            {
                if (!mm.m_IsTouch)
                {
                    mm.BeHit();
                    Terms[mm.CodeID] = true;

                    HitYun();
                }
            }
        }


        public void HitYun() {

            GameEntry.Sound.PlaySound(30002);

            foreach (bool bl in Terms) {
                if (!bl)
                    return;
            }

            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 2));
            GameEntry.Sound.PlaySound(path, "Sound");

            ((Lesson_2_3_WallForm)GameEntry.UI.GetUIForm(drlesson.WallID, "")).LandByRun();
            for (int i = 0; i < Terms.Count; i++) {
                Terms[i] = false;
            }
        }

    }
}