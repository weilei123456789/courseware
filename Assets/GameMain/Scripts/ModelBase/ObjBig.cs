using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
namespace Penny
{
    public class ObjBig : ObjBase
    {
        public enum ZYState
        {
            Normal,
            Play,
            
        }

        public ZYState State = ZYState.Normal;

        protected float clipTime = 0;
        [SerializeField]
        private float CTime = 8;
        
        public bool isReplay = false;
       
        
        // Use this for initialization
        void Start()
        {

        }
        
        // Update is called once per frame
        void Update()
        {
            switch (State)
            {
                case ZYState.Normal:
                    
                        clipTime += Time.deltaTime;
                        if (clipTime >= CTime)
                        {
                            clipTime = 0;
                            ObjAction();

                        }
                    
                    break;
                case ZYState.Play:
                    ObjRestAction();
                    break;
                
                
            }
        }

        public void SetState(ZYState state)
        {
            State = state;
            

        }
    }
}


