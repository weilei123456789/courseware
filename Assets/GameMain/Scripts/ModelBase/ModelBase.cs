using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
   

    public class ModelBase: MonoBehaviour
    {
        [Header("ID")]
        public int CodeID = -1;

        [Header("类型ID")]
        public int TypeID = -1;

        [Header("连续点击间隔时间")]
        public float CDTime = 3f;
     
        [Header("重复点击开关")]
        public bool m_IsTouch = false;

        protected float m_Time = 0;

        protected virtual  void Update()
        {
            if (CDTime < 0) return;

            if (m_IsTouch)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    m_IsTouch = false;
                }
            }
        }

        public virtual int? OnLidarHitEvent(GameObject obj, Vector3 screenPos)
        {

            if (obj == this.gameObject)
            {
                m_IsTouch = true;
                m_Time = CDTime;
                return CodeID;
            }
            else {
                return null;
            }
        }




    }

}
