using System;
using UnityEngine;

namespace Penny
{
    [Serializable]
    public class CustomBulletData : GroundModelData
    {
        [SerializeField]
        private GameObject m_AimGo = null;

        public CustomBulletData(int entityId, int typeId)
           : base(entityId, typeId)
        {

        }


        public GameObject AimGo
        {
            get
            {
                return m_AimGo;
            }
            set
            {
                m_AimGo = value;
            }

        }


    }
}