using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using UnityEngine;
using DG.Tweening;

namespace Penny
{
    public class Lesson_2_2_WallForm : LessonWallUIFrame
    {

    
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //数据存储
            drlesson = (DRLesson)userData;
            InitGame();

            UIOpenEvent(drlesson);

            UIEventSubscribe();
        }

        protected override void OnClose(object userData)

        {
            base.OnClose(userData);


            UICLoseEvent();
            UIEventUnsubscribe();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        private void InitGame() {
            
        }


      


       



    }
}