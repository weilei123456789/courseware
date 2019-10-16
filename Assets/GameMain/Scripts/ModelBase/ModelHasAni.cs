using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Penny
{

    public class ModelHasAni : ModelBase
    {

        public Animator modelAni = null;

        // Use this for initialization
        protected virtual void Start()
        {
            if (modelAni == null)
                modelAni = this.GetComponent<Animator>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }


        public override int? OnLidarHitEvent(GameObject obj, Vector3 screenPos)
        {
             return base.OnLidarHitEvent(obj, screenPos);
        }
    }
}