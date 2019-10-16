using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class Lesson2_2_Ice : Model
    {

        private bool IsAutoHide = false;
        private float M_CDTime = 0;

        [SerializeField]
        private UseDiffcultyData m_UseDifData;

        private NormalDifficulty m_NormalDif;

        private float TimeMin = 5;
        private float TimeMax = 8;


        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_UseDifData = (UseDiffcultyData)userData;

            m_NormalDif = m_UseDifData.NormalDif;

            AutoHide();
        }

        protected override void OnHide(object userData)

        {
            base.OnHide(userData);

        }

     

        /// <summary>
        ///绑定后坐标归0 
        /// </summary>   
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)

        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            this.CachedTransform.localPosition = Vector3.zero;
            this.CachedTransform.localScale = Vector3.one * 0.3f;
        }


     


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)

        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (IsAutoHide) {
                M_CDTime -= Time.deltaTime;
                if (M_CDTime < 0)
                {
                    GameEntry.Entity.HideEntity(this.Entity);
                }
            }


        }

        public void BeHit() {

            GameEntry.Entity.HideEntity(this.Entity);
            Again();
        }

        public void AutoHide() {
            IsAutoHide = true;
            switch (m_NormalDif) {
                case NormalDifficulty.Easy:
                    TimeMax = 8f;
                    TimeMin = 6f;
                    break;
                case NormalDifficulty.Normal:
                    TimeMax = 5f;
                    TimeMin = 3f;
                    break;
                case NormalDifficulty.Hard:
                    TimeMin = 2f;
                    TimeMin = 1f;
                    break;


            }


            M_CDTime = Random.Range(TimeMin, TimeMax);

        }

    }
}
