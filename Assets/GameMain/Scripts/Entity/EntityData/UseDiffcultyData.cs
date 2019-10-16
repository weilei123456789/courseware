using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public class UseDiffcultyData : GroundModelData
    {
        public UseDiffcultyData(int entityId, int typeId) 
            : base(entityId, typeId)
        {
        }

        [SerializeField]
        private  int m_NumDif;

        [SerializeField]
        private NormalDifficulty m_NormalDif;

        public int NumberDifficulty {
            get {
                return m_NumDif;
            }
            set {
                m_NumDif = value;
            }
        }

        public NormalDifficulty NormalDif {
            get {
                return m_NormalDif;
            }
            set {
                m_NormalDif = value;
            }
        }

    }
}