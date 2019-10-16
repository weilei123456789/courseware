using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class ModelIsLight : ModelHasAni
    {

        public bool LightFlag = false;

        [SerializeField]
        protected GameObject LightGo = null;

        protected override void Start()
        {
            
        }

        // Update is called once per frame
        protected  override void Update()
        {
            if (CDTime < 0) return;

            if (m_IsTouch)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    m_IsTouch = false;
                    IsLight(LightFlag);
                }
            }
        }

        public override int? OnLidarHitEvent(GameObject obj, Vector3 screenPos)
        {

            if (obj == this.gameObject)
            {
                m_IsTouch = true;
                m_Time = CDTime;
                LightGo.SetActive(!LightFlag);
                return CodeID;
            }
            else
            {
                return null;
            }
        }
        public void IsLight(bool IsL)
        {
            LightGo.SetActive(IsL);
        }
    }
}