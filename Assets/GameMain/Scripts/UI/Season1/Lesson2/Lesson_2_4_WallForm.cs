using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using GameFramework.Event;
using GameFramework;


namespace Penny
{

    public class Lesson_2_4_WallForm: LessonWallUIFrame
    {

        public Text txtFishHP;
        public Animator FishAni;
        public Image FishHPTiao;

        private Lesson_2_4_Wall_GoldFish_Entity m_Model;
        public GameObject GoldFish;

        //音频播放地址
        private string SoundPath = "Lesson_2_4_{0}";


        private int TotalFishHP;
        private int _FishHP;       
        public int FishHP
        {
            get
            {
                return _FishHP;
            }
            set
            {
                if (_FishHP == value) return;

                _FishHP = value;
                txtFishHP.text = _FishHP.ToString();
                FishHPTiao.fillAmount = ((float)_FishHP )/ ((float)TotalFishHP);
            }
        }


     

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            


            UIEventSubscribe();

            UIOpenEvent(userData);

            //条件模型触发监听

            GameEntry.Event.Subscribe(ModelPressEventArgs.EventId, OnModelPressSuccess);

            GameEntry.GameManager.IsInGame = true;

            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 1));
            GameEntry.Sound.PlaySound(path, "Sound");

            InitGameHP();
            InitGoldFish();

        }


        protected override void OnClose(object userData)

        {
            base.OnClose(userData);

            GameEntry.GameManager.IsInGame = false;
            //退订事件

            GameEntry.Event.Unsubscribe(ModelPressEventArgs.EventId, OnModelPressSuccess);

            UICLoseEvent();
            UIEventUnsubscribe();

         

        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();

        }


        //根据人数增加鱼血量
        private void InitGameHP()
        {
            if (GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty) == null)
            {
                TotalFishHP = 10;
                FishHP = 10;
            }
            else
            {
                int hp = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NumberDifficulty);
                TotalFishHP = hp * 4;
                FishHP = hp * 4;
            }

        }

        private void InitGoldFish()
        {

            GameEntry.Entity.ShowCustomEntity(typeof(Lesson_2_4_Wall_GoldFish_Entity), "WallModel", m_SeasonAssetPath, m_LessonAssetPath, new ModelData(GameEntry.Entity.GenerateSerialId(), 200001) {
                Name = "Goldfish",
                Position = new Vector3(0, 0, 7),
                Rotation = Quaternion.Euler(new Vector3(0, 0, 0)),
                Scale = new Vector3(15,15,15),
                CDTime = 1f,
             });
    
        }

        protected override void OnRayHitByLeida(GameObject go, Vector3 ve)
        {

         
            if (go == null)
                return;


            if (!GameEntry.GameManager.IsNowCam)
                return;

            string temp = go.transform.parent.parent.name;

            if (temp.Equals("Goldfish"))
            {
                //CD
                if (m_Model.IsReadyTouch) {
                    if (!m_Model.m_IsTouch) {
                        m_Model.BeHitFish();
                        if (FishHP > 0)
                        {
                            GameEntry.Sound.PlaySound(30001);
                            PlayTTSWord();
                            FishAni.SetBool("isattack", true);
                            Invoke("AniEnd", 0.5f);
                        }
                        //HitFish();
                    }
                }
          
            }
        }


        public void OnModelPressSuccess(object sender, GameEventArgs e) {
            ModelPressEventArgs ne = (ModelPressEventArgs)e;
            if (GoldFish == null) return;

            //ModeTypeBase gfish = GoldFish.GetComponent<ModeTypeBase>();
            if (ne.IsPress)
            {

               m_Model.IsReadyTouch = true;
            }
            else {

                m_Model.IsReadyTouch = false;
              
            }
          
        }

   
        public void HitFish()
        {
           

            if (FishHP <= 0)
                return;

            FishHP -= 2;


            if (FishHP == 0)
            {
               
                GameEntry.Sound.PlaySound(30000);
                FishAni.SetBool("isdead", true);
                txtFishHP.text = "GameOver";
                GameEntry.GameManager.IsInGame = false;

                string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, 4));
                GameEntry.Sound.PlaySound(path, "Sound");

            }
          
        }

        private void AniEnd()
        {
            FishAni.SetBool("isattack", false);
        }

        private void PlayTTSWord()
        {
            int i = Random.Range(1, 3);
            //string name = "Assets/GameMain/Sounds/lesson1_2_" + i+".wav";

            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, Utility.Text.Format(SoundPath, i.ToString()));
            GameEntry.Sound.PlaySound(path, "Sound");
            

        }



        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(Lesson_2_4_Wall_GoldFish_Entity))
            {
                m_Model = (Lesson_2_4_Wall_GoldFish_Entity)ne.Entity.Logic;

                m_Model.IsReadyTouch = false;

                GoldFish = m_Model.gameObject;
             
                //GoldFish.GetComponent<ModeTypeBase>().AllColliderOpen(false);
                FishAni = GoldFish.GetComponent<Animator>();
               
            }
        }

      

       
    }
}