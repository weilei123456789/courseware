using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class GeneralLight : Model
    {
        [SerializeField]
        protected GameObject LightGo = null;
        [SerializeField]
        protected GeneralLightData m_GeneralLight = null;

        private bool IsUseLight = false;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);
            m_GeneralLight = userData as GeneralLightData;

            IsUseLight = m_GeneralLight.IsUseLight;
            
            LightGo = this.transform.GetChild(0).gameObject;
            
        }
       

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            //base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (CDTime < 0) return;

            if (!m_IsTouch) return;
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
                IsLight(IsUseLight);
                m_IsTouch = false;
              
            }
        }

        


        protected override void OnHide(object userData)
        {
            base.OnHide(userData);
            IsLight(false);
        }

      


        public override void BeHit()
        {
            Again();
            IsLight(!IsUseLight);
        }

        public void IsLight(bool IsL)
        {

            LightGo.SetActive(IsL);

        }
    }
}