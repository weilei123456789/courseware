using System;
using UnityEngine;

namespace Penny
{
    [Serializable]
    public class GroundModelData : ModelData
    {

  
        [SerializeField]
        private Vector3 m_Postion = Vector3.zero;

       
        public GroundModelData(int entityId, int typeId)
            : base(entityId, typeId)
        {
          
        }
       

        /// <summary>
        /// 地屏世界坐标本地坐标 Y轴-150
        /// </summary>
        public  Vector3 NewPostion {
            get
            {
                return m_Postion;
            }
            set
            {
                m_Postion = value + new Vector3(0,-150,0);
            }
        }

    }
}