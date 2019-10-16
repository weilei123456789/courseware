using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Penny
{

    public class GameCamManager : MonoBehaviour
    {
        [SerializeField]
        public GameObject WallPart;
        [SerializeField]
        public GameObject GroundPart;

        [SerializeField]
        public Camera WallCam;

        [SerializeField]
        public Camera GroundCam;

        [SerializeField]
        public GameObject WallBG;

        [SerializeField]
        public GameObject GroundBG;

        [SerializeField]
        public VideoPlayer BGVPlayer = null;

        [SerializeField]
        private RenderTexture m_videoTex = null;
        public RenderTexture BGVideoTex {
            get {
                return m_videoTex;
            }
        }



        public static GameCamManager _intance;
        public static GameCamManager Intance
        {
            get
            {
                if (_intance == null)
                {
                    _intance = FindObjectOfType(typeof(GameCamManager)) as GameCamManager;
                }
                return _intance;
            }
        }

        // Use this for initialization
        void Start()
        {
            _intance = this;

            InitGameCam();

        }

        // Update is called once per frame
        void Update()
        {

        }


        /// <summary>
        /// 填入对应对象防止为空
        /// </summary>
        public void InitGameCam()
        {
            if (WallPart == null)
            {
                WallPart = GameObject.Find("/RootPart/WallPart").gameObject;
                if (WallPart == null)
                {
                    Debug.LogError("WallPart Not Find！");
                }

            }

            if (GroundPart == null)
            {
                GroundPart = GameObject.Find("/RootPart/GroundPart").gameObject;
                if (GroundPart == null)
                {
                    Debug.LogError("GroundPart Not Find！");
                }

            }



            if (WallCam == null)
            {
                WallCam = GameObject.Find("/RootPart/WallPart/WallCamera").GetComponent<Camera>();
                if (WallCam == null)
                {
                    Debug.LogError("WallCam Not Find！");
                }
          
            }

            if (GroundCam == null)
            {
                GroundCam = GameObject.Find("/RootPart/GroundPart/GroundCamera").GetComponent<Camera>();
                if (GroundCam == null)
                {
                    Debug.LogError("GroundCam Not Find！");
                }
           
            }

            if (WallBG == null)
            {
                WallBG = GameObject.Find("/RootPart/WallPart/WallBG").gameObject;
                if (WallBG == null)
                {
                    Debug.LogError("WallBG Not Find！");
                }

#if UNITY_EDITOR
                WallBG.transform.localScale = new Vector3(2, 1, 1);
#else
                //BG 平铺摄像机
                WallBG.transform.localScale = new Vector3((float)WallCam.pixelWidth / (float)WallCam.pixelHeight, 1, 1);
#endif
            }

            if (GroundBG == null)
            {
                GroundBG = GameObject.Find("/RootPart/GroundPart/GroundBG").gameObject;
                if (GroundBG == null)
                {
                    Debug.LogError("GroundBG Not Find！");
                }
#if UNITY_EDITOR
                GroundBG.transform.localScale = Vector3.one;
#else
                //BG 平铺摄像机
                GroundBG.transform.localScale = new Vector3((float)GroundCam.pixelWidth / (float)GroundCam.pixelHeight, 1, 1);
#endif
            }
        }

        public Vector3 WallScale()
        {
#if UNITY_EDITOR
            return new Vector3(2, 1, 1);
#else
            //BG 平铺摄像机
            return new Vector3((float)WallCam.pixelWidth / (float)WallCam.pixelHeight, 1, 1);
#endif
        }

        public Vector3 GroundScle()
        {
#if UNITY_EDITOR
            return Vector3.one;
#else
             //BG 平铺摄像机
             return new Vector3((float)GroundCam.pixelWidth / (float)GroundCam.pixelHeight, 1, 1);
#endif
        }

    }
}