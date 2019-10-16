using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Penny
{
    public class ObjHaiDan : ObjBase
    {

        private bool m_isComplete = true;

        public Vector3 posInit;

        public bool IsComplete
        {
            get
            {
                return m_isComplete;
            }
        }

        public void SetComplete(bool bol)
        {
            m_isComplete = bol;
        }

        public override void ObjAction()
        {
            base.ObjAction();
            
            SetAnimation("effect_2", false).Complete += ObjHaiDan_Complete; ;
        }

        private void ObjHaiDan_Complete(Spine.TrackEntry trackEntry)
        {
            SetComplete(true);
            SetAnimation("effect_1", true);
            isCanTouch = true;
            transform.SetLocalPositionY(0);
            tween.Kill();
            gameObject.SetActive(false);
            
        }
        Tween tween;
        public void MoveY()
        {
           tween= transform.DOLocalMoveY(-1000, 15).SetEase(Ease.Linear) .OnComplete(()=> 
            {
                SetComplete(true);
                transform.SetLocalPositionY(0);
            });
        }
       public void RandomX()
       {
            transform.SetLocalPositionX(Random.Range(-220,220));
       }

        public void floatHaidan(Vector3 vc)
        {
            transform.DOLocalMove(transform.localPosition+vc, 2).SetEase(Ease.Linear).OnComplete(()=> 
            {
                float x, y;
                x = transform.localPosition.x > posInit.x + 50 ? Random.Range(-30, -10) : Random.Range(10, 30);
                y= transform.localPosition.y > posInit.y + 50 ? Random.Range(-30, -10) : Random.Range(10, 30);
                floatHaidan(new Vector3(x, y, 0));

            });
        }

        public void BeginMove()
        {
            SetComplete(false);
            gameObject.SetActive(true);
            RandomX();
            MoveY();
        }

    }
}

