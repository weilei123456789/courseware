using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class Lesson2_1_ground_Entity : GroundModel
    {
        [SerializeField]
        protected Collider m_col = null;
        protected GameObject LightGo = null;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);
          
            m_GroundModeldata = userData as GroundModelData;
            if (m_GroundModeldata == null)
            {
                Debug.LogWarning("GroundModelData is Invalid");
            }

            m_col = this.GetComponent<Collider>();
            LightGo = this.transform.GetChild(0).gameObject;

            IsLight(false);

            CDTime = m_GroundModeldata.CDTime;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            //base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (!m_IsTouch) return;
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
                m_IsTouch = false;
                IsLight(false);
            }
        }

        public override void BeHit()
        {
            IsLight(true);
            GameEntry.Sound.PlaySound(30002);
            Again();

        }

        public void IsLight(bool IsL) {

            LightGo.SetActive(IsL);
          
        }


     

    }
}