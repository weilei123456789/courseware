using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class CustomBullet : Model
    {
        [SerializeField]
        protected CustomBulletData m_CustomBulletData = null;

        private GameObject m_Aim = null;
        private Transform m_AimTF = null;
        private float Speed = 0.03f;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            m_CustomBulletData = userData as CustomBulletData;
            if (m_CustomBulletData == null)
            {
                Debug.LogWarning("CustomBulletData is Invalid");
            }

            m_Aim = m_CustomBulletData.AimGo;
            m_AimTF = m_Aim.transform;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            CachedTransform.localPosition = Vector3.Lerp(CachedTransform.localPosition, m_AimTF.localPosition, Speed);

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == m_Aim) {
                GameEntry.Sound.PlaySound(20002);
                GameEntry.Entity.HideEntity(this);
            }

        }
    }
}