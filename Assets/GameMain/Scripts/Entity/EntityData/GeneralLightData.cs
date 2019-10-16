using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Penny
{
    public class GeneralLightData : ModelData
    {
        [SerializeField]
        private bool m_IsUseLight = false;

        public GeneralLightData(int entityId, int typeId)
            : base(entityId, typeId)
        {
        }

        /// <summary>
        /// false 踩亮 true 踩灭
        /// </summary>
        public bool IsUseLight{
            get {
                return m_IsUseLight;
            }
            set {
                m_IsUseLight = value;
            }
        }

    }
}