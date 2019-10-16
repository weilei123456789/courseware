using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using DG.Tweening;
using GameFramework;

namespace Penny
{

    public class Lesson_2_4_GroundForm: LessonGroundUIFrame
    {

        [SerializeField]
        public List<Transform> AniTF = new List<Transform>();

        //荷叶坐标
        [SerializeField]
        private Vector3[] HeyeTF = new Vector3[] {new Vector3(-4,0.2f,4),new Vector3(-4, 0.2f, -4),new Vector3(4, 0.2f, -4),new Vector3(4, 0.2f, 0) };

        [SerializeField]
        private Vector3[] beikeTF = new Vector3[] { new Vector3(-4, 0.2f, 2), new Vector3(-4, 0.2f, 0), new Vector3(-4, 0.2f, -2) };

        [SerializeField]
        public GameObject Rock = null;
        private Vector3 PressRockTF = new Vector3(0, 0.2f, 0.3f);



        [SerializeField]
        private GameObject LanternTarget = null;
        private int LanternTrack = 0;
        private bool IsPlayLantern = true;
    //    private float STime;

        private List<bool> temp = new List<bool>();

        //根据难度改变速度
        private float DifSpeed = 4;

        private int WallUIID = 0;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            //订阅事件


            UIEventSubscribe();
            UIOpenEvent(userData);
            //drlesson = (DRLesson)userData;
            //m_LessonAssetPath = drlesson.LessonPath;

            WallUIID = drlesson.WallID;

            GameEntry.Event.Subscribe(ModelTermEventArgs.EventId, OnModelTermSuccess);


            InitGround();
        }


        protected override void OnClose(object userData)

        {
            base.OnClose(userData);

            temp.Clear();
            //退订事件
            UIEventUnsubscribe();
         
            GameEntry.Event.Unsubscribe(ModelTermEventArgs.EventId, OnModelTermSuccess);

        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

       

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }

        public void InitGround() {
            for (int i = 0; i < HeyeTF.Length; i++) {


                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200002) {
                    Name = "heye",
                    NewPostion = HeyeTF[i],
                    Scale = new Vector3(1.5f, 0.2f, 1.5f),
                    CodeID = i,
                });

                temp.Add(false);
            }

            for (int i = 0; i< beikeTF.Length; i++) {

                GameEntry.Entity.ShowGroundModel(typeof(GroundModel), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 201002)
                {
                    Name = "Beike",
                    NewPostion = beikeTF[i],
                    Scale = new Vector3(1.5f, 0.2f, 1.5f),
                    CDTime = 3f,
                    CodeID = i,
                });

            }


            GameEntry.Entity.ShowGroundModel(typeof(Lesson1_2_ground_Entity), m_SeasonAssetPath, m_LessonAssetPath, new GroundModelData(GameEntry.Entity.GenerateSerialId(), 200003)
            {
                Name = "Rock",
                NewPostion = PressRockTF,
                Scale = new Vector3(0.25f, 0.25f, 0.25f),
                CDTime = 3f,
                CodeID = -1,
            
            });

           
        }

        private bool IsHitFish = false;

        protected  override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {

            if (go == null)
                return;

          

            if (!GameEntry.GameManager.IsNowCam)
                return;
            
            if (go.name == "heye")
            {
                HeYeHit(go);
            }

            if (go.name == "Beike") {
                GroundModel gm = go.GetComponent<GroundModel>();
                if (!gm.m_IsTouch) {

                    GameEntry.Sound.PlaySound(30002);
                    gm.BeHit();

                    gm.gameObject.transform.DOScale(new Vector3(1.2f, 0.2f, 1.2f), 1).OnComplete(() => gm.gameObject.transform.DOScale(new Vector3(1.5f, 0.2f, 1.5f), 1));
                    
                  
                  
                }

            }


            if (go.name == "Rock") {
                Lesson1_2_ground_Entity mm = go.GetComponent<Lesson1_2_ground_Entity>();
                if (mm == null)
                    return;

                if (!mm.m_IsTouch)
                {
                    mm.BeHitRock();
                    IsHitFish = true;
                }

            }
            if (go.name == "ViceRock")
            {
                Lesson1_2_ground_Entity mm = go.transform.parent.GetComponent<Lesson1_2_ground_Entity>();

                if (mm.IsPressTouch)
                {
                    mm.BeHitViceRock();
                    if (IsHitFish)
                    {
                        ((Lesson_2_4_WallForm)GameEntry.UI.GetUIForm(WallUIID, "")).HitFish();
                        IsHitFish = false;
                    }
                }
             

            }

        }

        public void HeYeHit(GameObject go) {
            Model mm = go.GetComponent<Model>();       
            int id= mm.CodeID;
            //TempGoldfish.GetComponent<ModeTypeTerm>().Terms[id].

            if (temp[id])
            {
                return;
            }
            else {
                temp[id] = true;
            }
            GameEntry.Sound.PlaySound(30002);

            ModelTermEventArgs ne = new ModelTermEventArgs(temp);
            GameEntry.Event.Fire(this, ne);

        
        }



        /// <summary>
        /// 播放走马灯
        /// </summary>
        //public void PlayDifAni() {
        //    if (IsPlayLantern)
        //    {
        //        LanternTrack++;
        //        if (LanternTrack > (AniTF.Count - 1))
        //        {
        //            LanternTrack = 0;
        //        }
            
        //        GameEntry.GameManager.ShowLantern(AniTF[LanternTrack].position, LanternTarget, DifSpeed, LanternEnd);
        //        IsPlayLantern = false;
        //    }
        //}

        public void LanternEnd()
        {
            IsPlayLantern = true;
        }
      
        //条件判断
        public void OnModelTermSuccess(object sender, GameEventArgs e)
        {
            ModelTermEventArgs ne = (ModelTermEventArgs)e;

            if (Rock == null) return;

            Lesson1_2_ground_Entity mm = Rock.GetComponent<Lesson1_2_ground_Entity>();
            mm.terms = ne.Terms ;
            mm.JudgeTerms();
          
        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(Lesson1_2_ground_Entity))
            {
                Lesson1_2_ground_Entity m_Model = (Lesson1_2_ground_Entity)ne.Entity.Logic;
                Rock = m_Model.gameObject;
            }
        }

       


        protected override void OnDiffcultyChange(object sender, GameEventArgs e) {
            NormalDifficultyEventArgs ne = (NormalDifficultyEventArgs)e;
            switch (ne.Difficulty) {
                case NormalDifficulty.Easy:
                    DifSpeed = 8;
                    break;
                case NormalDifficulty.Normal:
                    DifSpeed = 4;
                    break;
                case NormalDifficulty.Hard:
                    DifSpeed = 2;
                    break;
            } 
           
        }

    }
}
