using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;

using UnityGameFramework.Runtime;

namespace Penny
{
    public class Lesson2_2_Kedou : GroundModel
    {

        [SerializeField]
        Lesson2_2_Ice RightIce = null;

        [SerializeField]
        Lesson2_2_Ice LeftIce = null;

        private bool RightReady = false;
        private bool LeftReady = false;
        private float RightCD = 0;
     
        private float LeftCD = 0;


        private NormalDifficulty NowDif = NormalDifficulty.Normal;
        private float TimeMin = 5;
        private float TimeMax = 8;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(NormalDifficultyEventArgs.EventId, OnNormalDifficultyChange);

            int temp = GameEntry.DataNode.GetData<VarInt>(Constant.ProcedureData.NormalDifficulty);
            NowDif = (NormalDifficulty)temp;

            RightReady = false;
            LeftReady = false;
            AttachRightIce();
            AttachLeftIce();
        }

        protected override void OnHide(object userData)
        {
            base.OnHide(userData);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(NormalDifficultyEventArgs.EventId, OnNormalDifficultyChange);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (RightReady) {
                RightCD -= Time.deltaTime;
                if (RightCD < 0)
                {
                    AttachRightIce();
                }
                
            }


            if (LeftReady)
            {
                LeftCD -= Time.deltaTime;
                if (LeftCD < 0)
                {
                    AttachLeftIce();
                }
            }
        }

        public void AttachRightIce() {

            RightReady = false;
            GameEntry.Entity.ShowWallModel(typeof(Lesson2_2_Ice), "Season1", "Lesson2",
                    new UseDiffcultyData(GameEntry.Entity.GenerateSerialId(), 201004)
                    {
                        Name = "RightIce",
                        Rotation = Quaternion.Euler(new Vector3(0, 0, 0)),
                        Position = new Vector3(0, 0, -100),
                        Scale = Vector3.one * 0.25f,
                        CodeID = this.CodeID,
                        CDTime = 1f,
                        NormalDif = NowDif,

                    });
        
        }

        public void AttachLeftIce() {

            LeftReady = false;
            GameEntry.Entity.ShowWallModel(typeof(Lesson2_2_Ice), "Season1", "Lesson2",
                new UseDiffcultyData(GameEntry.Entity.GenerateSerialId(), 201004)
                {
                    Name = "LeftIce",
                    Rotation = Quaternion.Euler(new Vector3(0, 0, 0)),
                    Position = new Vector3(0, 0, -100),
                    Scale = Vector3.one * 0.25f,
                    CodeID = this.CodeID,
                    CDTime = 1f,
                    NormalDif = NowDif,
                });

        }


        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
           
            float time = Random.Range(TimeMin, TimeMax);
            if (childEntity.name == "RightIce") {

                RightCD = time;
                RightReady = true;
               
            }

            if (childEntity.name == "LeftIce") {

                LeftCD = time;
                LeftReady = true;
               
            }
        }

      

        private void OnShowEntitySuccess(object sender, GameEventArgs e) {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.Entity.name == "RightIce") {
                if (((Lesson2_2_Ice)ne.Entity.Logic).CodeID == this.CodeID) {
                    RightIce = (Lesson2_2_Ice)ne.Entity.Logic;
                    GameEntry.Entity.AttachEntity(RightIce.Id, this.Id, transform.GetChild(1));
                }
             
            }

            if (ne.Entity.name == "LeftIce") {
                if (((Lesson2_2_Ice)ne.Entity.Logic).CodeID == this.CodeID)
                {
                    LeftIce = (Lesson2_2_Ice)ne.Entity.Logic;
                    GameEntry.Entity.AttachEntity(LeftIce.Id, this.Id, transform.GetChild(0));
                }                 
            }            
        }

        private void OnNormalDifficultyChange(object sender,GameEventArgs e) {
            NormalDifficultyEventArgs ne = (NormalDifficultyEventArgs)e;
            NowDif = ne.Difficulty;
            switch (NowDif) {
                case NormalDifficulty.Easy:
                    TimeMax = 3f;
                    TimeMin = 2f;
                    break;
                case NormalDifficulty.Normal:
                    TimeMax = 2f;
                    TimeMin = 1f;
                    break;
                case NormalDifficulty.Hard:
                    TimeMax = 1.5f;
                    TimeMin = 0.5f;
                    break;


            }


        }


    }
}