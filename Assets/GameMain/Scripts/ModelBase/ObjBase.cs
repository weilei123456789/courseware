using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;

namespace Penny
{
    public class ObjBase : MonoBehaviour
    {
        private SkeletonGraphic m_Sg;
        private Spine.AnimationState m_AnimationState;
        [Header("是否可以触碰")]
        public bool isCanTouch = true;
        [SerializeField]
        [Header("连续点击CD时间")]
        public float CDTime = 3;

        public float m_Time = 0;

        public TrackEntry te;

        public Spine.AnimationState AnimationState
        {
            get
            {
                if (m_Sg==null)
                {
                    m_Sg = GetComponentInChildren<SkeletonGraphic>();
                    AnimationState = m_Sg.AnimationState;
                }
                return m_AnimationState;
            }

            set
            {
                m_AnimationState = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            if (null==m_Sg)
            {
                m_Sg = GetComponentInChildren<SkeletonGraphic>();
                AnimationState = m_Sg.AnimationState;
                
            }
        }
        public void initState()
        {
            if (null == m_Sg)
            {
                m_Sg = GetComponentInChildren<SkeletonGraphic>();
                AnimationState = m_Sg.AnimationState;

            }
        }
       

       

        // Update is called once per frame
        void Update()
        {
            if (CDTime < 0) return;
            if (!isCanTouch)
            {
                m_Time -= Time.deltaTime;
                if (m_Time<0)
                {
                    isCanTouch = true;
                    m_Time = CDTime;
                    ObjRestAction();
                }
            }
        }

        public virtual void ObjAction() { }
        public virtual void ObjRestAction() { }

        public virtual void OnLiDarHitEvent(GameObject go,Vector3 screenPos)
        {
            if (go == gameObject)
            {
                if (!isCanTouch) return;
                isCanTouch = false;
                m_Time = CDTime;
                ObjAction();
            }
            
            
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名字</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <returns></returns>
        public TrackEntry SetAnimation(string name,bool isLoop)
        {
            return te=AnimationState.SetAnimation(0, name, isLoop);
        }


    }
}


