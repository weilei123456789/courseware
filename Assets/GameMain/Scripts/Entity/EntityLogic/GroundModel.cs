using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class GroundModel : Model
    {
        [SerializeField]
        protected GroundModelData m_GroundModeldata = null;

        protected override void OnShow(object userData)
        {

            base.OnShow(userData);

            m_GroundModeldata = userData as GroundModelData;
            if (m_GroundModeldata == null)
            {
                Debug.LogWarning("GroundModelData is Invalid");
            }

            Name = m_GroundModeldata.Name;
            CodeID = m_GroundModeldata.CodeID;
            //CachedTransform.localPosition = m_GroundModeldata.NewPostion;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

        }

        public override void BeHit()
        {
            Again();
        }

    }
}