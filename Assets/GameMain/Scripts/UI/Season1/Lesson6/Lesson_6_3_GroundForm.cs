using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using Spine.Unity;
using Spine;
namespace Penny
{

    public class Lesson_6_3_GroundForm : LessonGroundUIFrame
    {
        [SerializeField]
        private ObjNormal[] ObjN = null;
        [SerializeField]
        private ObjHaiDanNormal[] objH=null;
        [SerializeField]
        private GameObject haidan = null;
        [SerializeField]
        private ObjNormal End;


        private int haidanNum = 8;
        private int HaidanNumMax = 8;
        private bool isCanAddHaidan = false;

        private float clipTime = 0;




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
            if (isCanAddHaidan)
            {
                clipTime += elapseSeconds;
                if (clipTime >= 3)
                {
                    clipTime = 0;
                    for (int i = 0; i < objH.Length; i++)
                    {
                        if (!objH[i].gameObject.activeSelf)
                        {
                            objH[i].gameObject.SetActive(true);
                            objH[i].isCanTouch = true;
                            objH[i].transform.SetLocalPositionX(Random.Range(290, 460));
                            objH[i].transform.SetLocalPositionY(Random.Range(-427, 64));
                            haidanNum++;
                            break;
                        }
                    }
                    if (haidanNum == HaidanNumMax)
                    {
                        isCanAddHaidan = false;
                    }
                }
            }


            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }

        private void InitGame()
        {
            //GameEntry.GameManager.IsInGame = true;
            Transform transP = transform.Find("MidPart/Haidans");
            //初始化8个海胆
            for (int i = 0; i < 8; i++)
            {
                GameObject go = Instantiate(haidan);
                Transform trans = go.GetComponent<Transform>();
                trans.SetParent(transP);
                trans.localScale = Vector3.one*0.5f;
                trans.position = transP.position;
                trans.localRotation = Quaternion.identity;
                objH[i] = trans.GetComponent<ObjHaiDanNormal>();
            }
            for (int i = 0; i < objH.Length; i++)
            {
                objH[i].transform.SetLocalPositionX(Random.Range(290, 460));
                objH[i].transform.SetLocalPositionY(Random.Range(-427, 64));
            }
            



        }

        


        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            if (!GameEntry.GameManager.IsInGame) return;
            for (int i = 0; i < ObjN.Length; i++)
            {
                ObjN[i].OnLiDarHitEvent(go, vc);
            }
            for (int i = 0; i < objH.Length; i++)
            {
                if (go==objH[i].gameObject&&objH[i].isCanTouch)
                {
                    PlayGameVoice("Les1_1_HitMoGu", SoundLevel.Once);
                    objH[i].OnLiDarHitEvent(go, vc);
                    haidanNum--;
                    if (haidanNum == 5)
                        isCanAddHaidan = true;
                }
            }

            if (go==End.gameObject&&End.isCanTouch)
            {
                End.OnLiDarHitEvent(go, vc);
                
                
                PlayGameVoice("3_round_1",SoundLevel.Talk);
            }

        }












    }
}