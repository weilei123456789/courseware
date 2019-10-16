using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class BGAniManager : MonoBehaviour
    {
        [SerializeField]
        private string AniName = string.Empty;
        [SerializeField]
        private Animator Ani = null;

        void Awake()
        {
            if (Ani == null)
                Ani = this.GetComponent<Animator>();
            //AniName = this.name;
        }

        // Use this for initialization
        void Start()
        {

            //Debug.Log(Ani.GetCurrentAnimatorClipInfoCount(0));
   
                Ani.Play(AniName);
           
        }

        void OnEnable()
        {
            Ani.Play(AniName);
        }

    }
}