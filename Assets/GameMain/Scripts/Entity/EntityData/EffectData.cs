//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;

namespace Penny
{
    [Serializable]
    public class EffectData : ModelData
    {
        [SerializeField]
        private float m_KeepTime = 0f;
        [SerializeField]
        private Transform m_Parent = null;


        public EffectData(int entityId, int typeId)
            : base(entityId, typeId)
        {
            m_KeepTime = 3f;
        }

        public float KeepTime
        {
            get
            {
                return m_KeepTime;
            }
            set {
                m_KeepTime = value;
            }
        }
   

        public Transform Parent {

            get {
                return m_Parent;
            }
            set {
                m_Parent = value;
            }
        }
    }
}
