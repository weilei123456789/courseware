using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Penny
{

    public class ArrowLineManager : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> TFs = new List<Transform>();

        [SerializeField]
        private Vector3[] pathPoint = new Vector3[] { };

        [SerializeField]
        private float dar = 1f;
        [SerializeField]
        private Ease easeType = Ease.Linear;



        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < TFs.Count; i++) {

                TFs[i].DOLocalPath(pathPoint, dar, PathType.Linear, PathMode.TopDown2D).SetLoops(-1, LoopType.Restart).SetLookAt(-1).SetEase(easeType).SetDelay(i * 1f);
            }

          
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}