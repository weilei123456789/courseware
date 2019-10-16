using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class Lesson1_1_ground_Entity:GroundModel
    {
       
        protected override void OnShow(object userData)
        {

            base.OnShow(userData);

            m_GroundModeldata = userData as GroundModelData;
            if (m_GroundModeldata == null)
            {
                Debug.LogWarning("GroundModelData is Invalid");
            }

          

        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

    }
}
