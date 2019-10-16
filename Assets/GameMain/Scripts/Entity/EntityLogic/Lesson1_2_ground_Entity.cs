using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class Lesson1_2_ground_Entity:GroundModel
    {
        [SerializeField]
        private Collider RockCol = null;
        [SerializeField]
        private Collider ViceRockCol = null;

        [SerializeField]
        private bool IsPress = false;

        public List<bool> terms = new List<bool>();

        float m_pressCDTime = 0;
        float pressCDTime = 0;
        public bool IsPressTouch = false;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);

            m_GroundModeldata = userData as GroundModelData;
            if (m_GroundModeldata == null)
            {
                Debug.LogWarning("GroundModelData is Invalid");
            }

            CDTime = m_GroundModeldata.CDTime;
            pressCDTime = m_GroundModeldata.CDTime * 2;
            InitColider();

        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            //Debug.Log("ViceRockCol is " + ViceRockCol.enabled);

            if (m_IsTouch)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    m_IsTouch = false;
                    //ViceRockCol.enabled = true;
                }
            }

            if (!IsPressTouch) {
                m_pressCDTime -= Time.deltaTime;
                if (m_pressCDTime < 0) {
                    IsPressTouch = true;
                    //ViceRockCol.enabled = true;
                }
            }
         
        }

        private void InitColider() {
            RockCol = GetComponent<Collider>();
            if (RockCol == null) {
                Debug.LogWarning("RoclCol is Null");
            }
            RockCol.enabled = false;

            ViceRockCol = transform.GetChild(0).GetComponent<Collider>();
            if (ViceRockCol == null) {
                Debug.LogWarning("ViceRockCol is Null");
            }

        }


        public void JudgeTerms() {

            if (terms != null) {

                foreach (bool te in terms) {
                    if (!te)
                        return;
                }

                RockCol.enabled = true;
            }

        }

        //结束本次砸鱼
        public void EndJudge() {
            RockCol.enabled = false;
            IsPress = false;
            for (int i = 0; i < terms.Count; i++)
            {
                terms[i] = false;
            }
            ModelPressEventArgs ne = new ModelPressEventArgs(IsPress);
            GameEntry.Event.Fire(this, ne);
        }


        public void BeHitRock() {
            if (IsPress)
                return;

            IsPress = true;
            ModelPressEventArgs ne = new ModelPressEventArgs(IsPress);
            GameEntry.Event.Fire(this,ne);
            //ViceRockCol.enabled = false;
            Again();
        }

        /// <summary>
        /// 每间隔一段时间监测是否离开
        /// </summary>
        public void BeHitViceRock() {

            IsPressTouch = false;
            m_pressCDTime = pressCDTime;

          
            if (!IsPress) 
                return;

            IsPress = false;
           
            EndJudge();
            ModelPressEventArgs ne = new ModelPressEventArgs(IsPress);
            GameEntry.Event.Fire(this, ne);
           
        }

        //private void Again()
        //{
        //    m_IsTouch = true;
        //    m_Time = CDTime;
        //}

    }
}
