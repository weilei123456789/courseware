using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class Lesson_2_4_Wall_GoldFish_Entity : Model
    {


        public List<bool> Terms = new List<bool>();
        public bool IsReadyTouch = false;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);

            m_Modeldata = userData as ModelData;
            if (m_Modeldata == null)
            {
                Debug.LogWarning("GroundModelData is Invalid");
            }

            CDTime = m_Modeldata.CDTime;
         

        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            //if (!m_IsTouch) return;
            //m_Time -= Time.deltaTime;
            //if (m_Time < 0)
            //{
            //    m_IsTouch = false;
            //}
        }


        public void BeHitFish() {
            Again();
        }

        public void TermJudge() {
            foreach (bool t in Terms) {

            }
        }

        //private void Again()
        //{
        //    m_IsTouch = true;
        //    m_Time = CDTime;
        //}


    }
}