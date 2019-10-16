//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    /// <summary>
    /// 特效类。
    /// </summary>
    public class Effect : Model
    {
        [SerializeField]
        private EffectData m_EffectData = null;

        [SerializeField]
        private Transform m_Parent = null;
   

        private float m_ElapseSeconds = 0f;

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userData)
#else
        protected internal override void OnShow(object userData)
#endif
        {
            base.OnShow(userData);

            m_EffectData = userData as EffectData;
            if (m_EffectData == null)
            {
                Log.Error("Effect data is invalid.");
                return;
            }
       


            m_ElapseSeconds = 0f;
            //设置特效父级
            m_Parent = m_EffectData.Parent;
            if (m_Parent != null)
            {
                this.transform.SetParent(m_Parent);
            }
           
            if (m_EffectData.Name != null) {
                Name = m_EffectData.Name;
            }

        

        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= m_EffectData.KeepTime)
            {
                GameEntry.Entity.HideEntity(this);
            }
        }
    }
}
