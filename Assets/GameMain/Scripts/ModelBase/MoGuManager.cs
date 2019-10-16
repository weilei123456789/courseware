using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityGameFramework.Runtime;

namespace Penny
{


    public class MoGuManager : ModelBase
    {

        public float MaxX = 2.5f;
        public float MinX = -2.5f;
        public float MaxZ = 2.5f;
        public float MinZ = -2.5f;
        //蘑菇位移速度
        public float MoveSpeed = 0.8f;

        public  Animator AniMan;
        public bool IsMove = true;


       
        private void Awake()
        {
            if (AniMan == null)
            {
                AniMan = transform.GetChild(0).GetComponent<Animator>();
            }
            AniMan.SetInteger("state", -1);
     
        }

        // Use this for initialization
        void Start()
        {


        }

        private void OnEnable()
        {
         
        }

        // Update is called once per frame
        protected override  void Update()
        {

            if (IsMove)
            {
                MoveMoGu();
               
            }

            if (CDTime < 0) return;
            if (!m_IsTouch) return;
            m_Time -= Time.deltaTime;
            if (m_Time < 0)
            {
                m_IsTouch = false;
              
            }
        }


        public void GrowMoGu() {

            AniMan.SetInteger("state", 0);
            IsHit(true);
            //this.transform.DOScale(new Vector3(1, 1, 1), 1f);
            //this.transform.GetChild(0).DOScale(new Vector3(1, 1, 1), 2f);

            StartCoroutine(GameEntry.GameManager.DelayWork(1f, () =>
            {
               IsMove = true;
                ReSetTF();
            }));
        }


        public void CycReset()
        {
            AniMan.SetInteger("state", 1);
        }

        public void ReSetTF()
        {

            float x = Random.Range(MinX, MaxX);
            float z = Random.Range(MinZ, MaxZ);

            //float dis = Vector3.Distance(new Vector3(x, z, 0), new Vector3(0, 0, 0));
           
          
            Vector3 nor = (this.transform.localPosition - new Vector3(x, z, 0)).normalized;
            //if (dis < 350f)
            {
                TargetTF = new Vector3(x, z, 0);
               
                float angle = Vector3.Angle(Vector3.up, nor);
                this.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
            //else {

            //    ReSetTF();
            //}
        }
        
        private Vector3 TargetTF;
        private void MoveMoGu() {
           
            Vector3 ve = (TargetTF - this.transform.localPosition).normalized;
            this.transform.Translate(ve * MoveSpeed * Time.deltaTime, Space.World);
            float dis = Vector3.Distance(TargetTF, this.transform.localPosition);

            if (dis < 10)
            {
                ReSetTF();
            }
        }

        /// <summary>
        /// 是否可以点击
        /// </summary>
        /// <param name="IsHit"></param>
        public void IsHit(bool IsHit) {
            if (IsHit)
            {
                CDTime = 3;
                m_IsTouch = false;
            }
            else {
                CDTime = -1;
                m_IsTouch = true;
            }

        }
       

        public void DestroyMoGu()
        {
            //Debug.Log("蘑菇被踩了");
         
          
            IsMove = false;
            AniMan.SetBool("beAtk", true);
            StartCoroutine(GameEntry.GameManager.DelayWork(1f, CloseModule));
          
        }

        public void CloseModule()
        {
           // Debug.Log("动画播放完毕~");
            AniMan.SetBool("beAtk", false);
            Destroy(this.gameObject);           
        }



    }
}
