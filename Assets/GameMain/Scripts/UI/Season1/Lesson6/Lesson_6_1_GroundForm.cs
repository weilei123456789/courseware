using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using DG.Tweening;
using System;

namespace Penny
{

    public class Lesson_6_1_GroundForm : LessonGroundUIFrame
    {
        [SerializeField]
        private ObjNormal[] objsN = null;
        [SerializeField]
        private ObjHaiDan[] objsH = null;
        [SerializeField]
        private ObjNormal[] objFish = null;
        [SerializeField]
        private GameObject haidan = null;
        [SerializeField]
        private Transform haidanParent = null;
        [SerializeField]
        private ObjNormal end;
        float CDTime = 3;
        float curTime = 3;

        protected override void OnOpen(object userData) 
        {
            base.OnOpen(userData);

            UIOpenEvent(userData);
            UIEventSubscribe();
            

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

            

            if (curTime>0)
            {
                curTime -= elapseSeconds;
                if (curTime<0)
                {
                    for (int i = 0; i < objsH.Length; i++)
                    {
                        if (objsH[i].IsComplete)
                        {
                            curTime = CDTime;
                            objsH[i].BeginMove();
                            break;
                        }
                    }
                }
            }





        }

        private void InitGame()
        {
            //初始化11个海胆
            for (int i = 0; i < 11; i++)
            {
                GameObject go = Instantiate(haidan);
                Transform trans = go.GetComponent<Transform>();
                trans.SetParent(haidanParent);
                trans.localScale = Vector3.one;
                trans.position = haidanParent.position;
                trans.localRotation = Quaternion.identity;
                objsH[i] = trans.GetComponent<ObjHaiDan>();
            }
            m_ProduceingState = Produceing.DelayEnd;

            for (int i = 0; i < objFish.Length; i++)
            {
                objFish[i].initState();
                objFish[i].gameObject.SetActive(false);
                objFish[i].transform.localScale = Vector3.zero;
            }


        }

       



        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {

            base.OnRayHitByLeida(go, vc);

            for (int i = 0; i < objsN.Length; i++)
            {
                objsN[i].OnLiDarHitEvent(go, vc);
            }

           

            for (int i = 0; i < objsH.Length; i++)
            {
                if (go == objsH[i].gameObject && objsH[i].isCanTouch)
                {
                    PlayGameVoice("Les1_1_HitMoGu", SoundLevel.Once);
                }
                objsH[i].OnLiDarHitEvent(go, vc);
                
            }
            //触发结束点
            if (go == end.gameObject&&end.isCanTouch)
            {
                end.isCanTouch = false;
                end.SetAnimation("effect_2", true);
                //objFish[0].SetAnimation("effect_1", true);
                PlayGameVoice("1_round_1");
                StartCoroutine(AddFish());
            }
        }

        public IEnumerator AddFish()
        {
            for (int i = 0; i < objFish.Length; i++)
            {
                objFish[i].transform.localScale = Vector3.one;
                objFish[i].SetAnimation("effect_1", false);
                objFish[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                
            }
            
            for (int i = 0; i < objFish.Length; i++)
            {

                objFish[i].SetAnimation("effect_2", true);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(5);
            for (int i = 0; i < objFish.Length; i++)
            {
                objFish[i].SetAnimation("effect_3", false);
            }
            end.isCanTouch = true;
            end.SetAnimation("effect_1", true);
        }



       


       


       

       

        


    }
}