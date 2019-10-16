using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class ModelAniOnce : ModelHasAni
    {
        public bool AniFlag = false;

        public string AniName = "IsPlay";

        // Use this for initialization
        void Start()
        {

        }

        protected override void Update()
        {
            if (CDTime < 0) return;

            if (m_IsTouch)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0)
                {
                    m_IsTouch = false;
                    PlayAni(AniFlag);
                }
            }
        }

        public override int? OnLidarHitEvent(GameObject obj, Vector3 screenPos)
        {

            if (obj == this.gameObject)
            {
                m_IsTouch = true;
                m_Time = CDTime;
                PlayAni(!AniFlag);
                return CodeID;
            }
            else
            {
                return null;
            }
        }


        public void PlayAni(bool Isplay)
        {
            modelAni.SetBool(AniName, Isplay);
        }
    }
}