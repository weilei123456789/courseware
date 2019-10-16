using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class ModelLiYuW : ModelHasAni
    {
        [SerializeField]
        private GameObject Crit = null;
        [SerializeField]
        private List<GameObject> Body = new List<GameObject>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public override int? OnLidarHitEvent(GameObject obj, Vector3 screenPos)
        {

            if (obj == Crit)
            {
                m_IsTouch = true;
                m_Time = CDTime;
                return 10;
            }

            for (int i = 0; i < Body.Count; i++) {
                if (obj == Body[i]) {
                    m_IsTouch = true;
                    m_Time = CDTime;
                    return 1;
                }
            }

           
                return null;
           
          
        }
    }
}