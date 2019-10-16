using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace Penny
{
    public class GeneralLimitTime : Model
    {

        //防止多射线重复点击
        [SerializeField]
        private float Duration = 3f;
        private float DurTime;

        private ModelLimitTimeEventArgs ne ;

        protected override void OnShow(object userData) {

            base.OnShow(userData);
         
         
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (!m_IsTouch)
            {
                DurTime -= Time.deltaTime;
                if (DurTime < 0)
                {
                    m_IsTouch = true;
                    transform.GetChild(0).gameObject.SetActive(false);
                    ne = new ModelLimitTimeEventArgs(false);
                    GameEntry.Event.Fire(this, ne);
                }
            }

        }

        /// <summary>
        /// 开始限制时间
        /// </summary>
        public void ReadyLimitTime() {
            transform.GetChild(0).gameObject.SetActive(true);
            m_IsTouch = false;
            DurTime = Duration;
        }


        public override void BeHit() {
            m_IsTouch = true;
            transform.GetChild(0).gameObject.SetActive(false);
            ne = new ModelLimitTimeEventArgs(true);
            GameEntry.Event.Fire(this, ne);
        }

        //public override ImpactData GetImpactData()
        //{
        //    return new ImpactData();
        //}
    }
}