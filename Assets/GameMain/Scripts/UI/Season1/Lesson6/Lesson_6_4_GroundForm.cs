using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Penny
{

    public class Lesson_6_4_GroundForm : LessonGroundUIFrame
    {
        [SerializeField]
        private ObjNormal[] objN = null;
        [SerializeField]
        private ObjNormal Shuimu = null;

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
            
            if (GameEntry.GameManager.IsMouseDebug)
                RayHitByPC();
        }


        private void InitGame()
        {
            

           
        }
        protected override void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            base.OnRayHitByLeida(go, vc);
            if (!GameEntry.GameManager.IsInGame) return;
            
            for (int i = 0; i < objN.Length; i++)
            {

                if (objN[i].isCanTouch&&go==objN[i].gameObject)
                {
                    objN[i].OnLiDarHitEvent(go, vc);
                        
                        
                        //ModelPressEventArgs ne = new ModelPressEventArgs(true);
                        //GameEntry.Event.Fire(this, ne);
                        if(!Shuimu.CanTouch)
                        {
                            Shuimu.CanTouch = true;
                            Shuimu.SetAnimation("effect_2", true).Complete+=(x)=> 
                            {
                                Shuimu.SetAnimation("effect_1", true);
                                Shuimu.CanTouch = false;
                            };

                        }
                        
                    
                }
            }

        }
        

       

       

       


       
      

       
    }
    
}