using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace Penny
{
    public class Model : Entity
    {
        [SerializeField]
        protected ModelData m_Modeldata = null;

        //防止多射线重复点击
        public  bool m_IsTouch = true;
        public  float CDTime = 3f;
        protected float m_Time = 0;

        public int TypeID = 0;

        private int m_CodeID = -1;
        public int CodeID {
            get
            {
                return m_CodeID;
            }
            set {
                m_CodeID = value;
            }
        }


        protected override void OnShow(object userData) {

            base.OnShow(userData);

            m_Modeldata = userData as ModelData;
            if (m_Modeldata == null) {
                Debug.LogWarning("ModelData is Invalid");
            }

            Name = m_Modeldata.Name;
            CodeID = m_Modeldata.CodeID;
            CDTime = m_Modeldata.CDTime;
            m_IsTouch = m_Modeldata.IsTouch;
            TypeID = m_Modeldata.TypeId;

            this.gameObject.SetLayerRecursively(m_Modeldata.Layer);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (CDTime < 0) return;

            if (!m_IsTouch) return;
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
                m_IsTouch = false;

            }
        }

        protected virtual void Again()
        {
            m_IsTouch = true;
            m_Time = CDTime;
        }


        public virtual void BeHit()
        {
            Again();
        }

        //public override ImpactData GetImpactData()
        //{
        //    return new ImpactData();
        //}
    }
}