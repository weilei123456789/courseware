using System;
using UnityEngine;

namespace Penny
{
    [Serializable]
    public class ModelData : EntityData
    {

        [SerializeField]
        private string m_Name = null;

        [SerializeField]
        private int m_CodeID = -1;

        [SerializeField]
        private float m_CDTime = 0;

        [SerializeField]
        private bool m_isTouch = true;

        [SerializeField]
        private int m_Layer = 5;

        public ModelData(int entityId, int typeId)
            : base(entityId, typeId)
        {
          
        }
       
        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }


        public int CodeID
        {
            get
            {
                return m_CodeID;
            }
            set
            {
                m_CodeID = value;
            }
        }


        public float CDTime {

            get {
                return m_CDTime;
            }
            set {
                m_CDTime = value;
            }
        }


        public bool IsTouch
        {

            get
            {
                return m_isTouch;
            }
            set
            {
                m_isTouch = value;
            }
        }


        public int Layer
        {

            get
            {
                return m_Layer;
            }
            set
            {
                m_Layer = value;
            }
        }
    }
}