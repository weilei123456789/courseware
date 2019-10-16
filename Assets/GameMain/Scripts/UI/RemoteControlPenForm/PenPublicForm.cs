using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penny
{

    public class PenPublicForm : UGuiForm
    {
        [SerializeField]
        private ModelBase ResetBtn = null;
        [SerializeField]
        private ModelBase SkipBtn = null;
        [SerializeField]
        private ModelBase TwiceSkipBtn = null;

        private int m_PenPublicViceForm = -1;

        /// <summary>
        /// 根据开启界面配置当前界面元素
        /// </summary>
        private bool _mIsSkip = false;
        private bool _mIsGame = false;
        private bool _mIsLoop = false;
        private bool _mIsWarmup = false;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            PenPublicParams parms = userData as PenPublicParams;
            _mIsSkip = parms.IsSkip;
            _mIsGame = parms.IsGame;
            _mIsLoop = parms.IsLoopOP;
            _mIsWarmup = parms.IsWarmup;

            //设置界面层级
            if (OriginalDepth < (int)UIFormId.PenPublicForm)
            {
                OriginalDepth += (int)UIFormId.PenPublicForm;
            }


            GameEntry.Windows.SubscribeUIWallEvent(OnRayHitByLeida);

            m_PenPublicViceForm = (int)GameEntry.UI.OpenUIForm(UIFormId.PenPublicViceForm, parms);

            InitPanel();
        }
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            InputControlPen();
        }

        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_PenPublicViceForm))
                GameEntry.UI.CloseUIForm(m_PenPublicViceForm);

            GameEntry.Windows.UnSubscribeUIWallEvent(OnRayHitByLeida);

            base.OnClose(userData);
        }


        public void InitPanel()
        {

            if (_mIsSkip)
            {
                SkipBtn.gameObject.SetActive(true);
                TwiceSkipBtn.gameObject.SetActive(false);
            }
            else
            {
                SkipBtn.gameObject.SetActive(false);
                TwiceSkipBtn.gameObject.SetActive(true);
                TwiceSkipBtn.transform.GetChild(0).gameObject.SetActive(false);
                TwiceSkipTrack = 0;
            }

            if (_mIsWarmup)
            {
                ResetBtn.gameObject.SetActive(false);
            }
            else
            {
                ResetBtn.gameObject.SetActive(true);
            }

            //可循环界面一开始默认不在循环
            if (_mIsLoop)
            {
                ResetBtn.transform.GetChild(0).gameObject.SetActive(true);
                IsLoop = false;
            }
            else
            {
                ResetBtn.transform.GetChild(0).gameObject.SetActive(false);
            }


        }


        private void OnRayHitByLeida(GameObject go, Vector3 ve)
        {

            if (!ResetBtn.m_IsTouch)
            {
                if (ResetBtn.OnLidarHitEvent(go, ve) != null)
                {
                    OnClickReset();
                }
            }
            if (!SkipBtn.m_IsTouch)
            {
                if (SkipBtn.OnLidarHitEvent(go, ve) != null)
                {
                    OnClickSkip();
                }
            }
            if (!TwiceSkipBtn.m_IsTouch)
            {
                if (TwiceSkipBtn.OnLidarHitEvent(go, ve) != null)
                {
                    OnClickTwiceSkip();
                }
            }

        }


        /// <summary>
        /// 遥控笔操控
        /// </summary>
        private void InputControlPen()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickReset();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_mIsSkip)
                {
                    OnClickSkip();
                }
                else
                {
                    OnClickTwiceSkip();
                }
            }
        }


        private bool IsLoop = false;
        public void OnClickReset()
        {

            if (_mIsLoop)
            {
                if (IsLoop)
                {
                    IsLoop = false;
                    ResetBtn.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    IsLoop = true;
                    ResetBtn.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            PenPublicEventParams parms = new PenPublicEventParams()
            {
                VideoIsLoop = IsLoop,
                BtnClick = PenPublicBtnClick.OnClickReset,
            };
            PenPublicEventArgs ne = new PenPublicEventArgs(parms);
            GameEntry.Event.Fire(this, ne);
        }

        public void OnClickSkip()
        {
            PenPublicEventParams parms = new PenPublicEventParams()
            {
                BtnClick = PenPublicBtnClick.OnClickSkip,
            };
            PenPublicEventArgs ne = new PenPublicEventArgs(parms);
            GameEntry.Event.Fire(this, ne);
        }

        //多次点击按钮计数器
        private int TwiceSkipTrack = 0;
        public void OnClickTwiceSkip()
        {
            PenPublicEventParams parms = new PenPublicEventParams();
            TwiceSkipTrack++;
            if (TwiceSkipTrack % 2 == 1)
            {
                parms.BtnClick = PenPublicBtnClick.OnClickTwiceSkipOnce;
                TwiceSkipBtn.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                parms.BtnClick = PenPublicBtnClick.OnClickTwiceSkipTwice;
                TwiceSkipBtn.transform.GetChild(0).gameObject.SetActive(false);
            }
            PenPublicEventArgs ne = new PenPublicEventArgs(parms);
            GameEntry.Event.Fire(this, ne);
        }

        //private void OnPenPublicListen(object sender, GameEventArgs e) {
        //    PenPublicEventArgs ne = e as PenPublicEventArgs;

        //}
    }
}