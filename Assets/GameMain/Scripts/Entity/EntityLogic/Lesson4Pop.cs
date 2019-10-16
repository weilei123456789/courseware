using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class Lesson4Pop : GroundModel
    {

        private float speed = 0.05f;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            m_IsTouch = false;
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            CachedTransform.localPosition = Vector3.Lerp(CachedTransform.localPosition, new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, CachedTransform.localPosition.z - 1), speed);
            CachedTransform.localRotation = Quaternion.Lerp(CachedTransform.localRotation,Quaternion.Euler( new Vector3(CachedTransform.localEulerAngles.x, CachedTransform.localEulerAngles.y + 1, CachedTransform.localEulerAngles.z )),5f);

            if (CachedTransform.localPosition.z < -5.5f||CachedTransform.localPosition.z > 7.5f) {

                GameEntry.Entity.HideEntity(this);
            }

        }




    }
}