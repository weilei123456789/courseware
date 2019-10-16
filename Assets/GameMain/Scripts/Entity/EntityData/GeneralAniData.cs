using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{
    public enum AniStateType
    {
        TypeInt = 0,
        TypeFloat = 1,
        TypeBool = 2,
    }


    

    public class GeneralAniData : GroundModelData
    {

        private bool m_anistate = false;
        private string m_aniControlName;

        public GeneralAniData(int entityId, int typeId) 
            :base(entityId, typeId)
        {
        }

        
        public bool AniState {
            get {
                return m_anistate;
            }
            set {
                m_anistate = value;
            }
        }

        public string ControlName {

            get {
                return m_aniControlName;
            }
            set {
                m_aniControlName = value;
            }
        }

    }
}