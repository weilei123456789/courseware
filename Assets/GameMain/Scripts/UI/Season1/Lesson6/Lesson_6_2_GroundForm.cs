
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using DG.Tweening;
namespace Penny
{

    public class Lesson_6_2_GroundForm : LessonGroundUIFrame
    {
        [SerializeField]
        private ObjNormal[] objsN = null;
        [SerializeField]
        private ObjNormal[] objZY = null;
        [SerializeField]
        private ObjNormal Yangqi;
        [SerializeField]
        private ObjNormal Hailuo;
        [SerializeField]
        private Vector3[] posNormal = new Vector3[] {
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
        };
        [SerializeField]
        private Vector3[] posInit = null;
        [SerializeField]
        private GameObject m_ResetImg;
        private bool isEnd = false;
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UIOpenEvent(userData);
            UIEventSubscribe();
            if (m_ResetImg.activeSelf)
                m_ResetImg.SetActive(false);

            InitGame();
        }

        protected override void OnClose(object userData)
        {
            base.OnClose(userData);
            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

          
        }


        private void InitGame()
        {
            for (int i = 0; i < objZY.Length; i++)
            {
                objZY[i].transform.localPosition = posNormal[i];
            }
            


        }


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            Debug.Log(GameEntry.GameManager.IsInGame);
            for (int i = 0; i < objsN.Length; i++)
            {
                objsN[i].OnLiDarHitEvent(go, vc);
            }
            if (isEnd&&Yangqi.isCanTouch&&go==Yangqi.gameObject)
            {
                PlayGameVoice("2_round_1", SoundLevel.Talk);
                isEnd = false;
            }
            Yangqi.OnLiDarHitEvent(go, vc);
            
            if(go==Hailuo.gameObject&&Hailuo.isCanTouch)
            {
                Hailuo.OnLiDarHitEvent(go, vc);
                Yangqi.SetAnimation("effect_3", true);
                isEnd = true;
                for (int i = 0; i < objZY.Length; i++)
                {
                    objZY[i].transform.DOLocalMove(posInit[i], 1).SetEase(Ease.Linear);
                }
                StartCoroutine(waitTime());
            }
            for (int i = 0; i < objZY.Length; i++)
            {
                objZY[i].OnLiDarHitEvent(go, vc);
            }

        }

        IEnumerator waitTime()
        {
            Yangqi.isCanTouch = true;
            yield return new WaitForSeconds(8);
            Hailuo.isCanTouch = true;
            Hailuo.SetAnimation("effect_1", true);
            for (int i = 0; i < objZY.Length; i++)
            {
                objZY[i].transform.DOLocalMove(posNormal[i], 1).SetEase(Ease.Linear);
            }

        }

        /// <summary>
        /// 地屏进入休息环节
        /// </summary>
        public void EntryRest()
        {
            m_ResetImg.SetActive(true);
        }
















    }
}