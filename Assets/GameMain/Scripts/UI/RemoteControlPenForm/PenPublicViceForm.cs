using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class PenPublicViceForm : UGuiForm
    {
        [SerializeField]
        private GameObject starPart = null;

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
            if (OriginalDepth < (int)UIFormId.PenPublicViceForm)
            {
                OriginalDepth += (int)UIFormId.PenPublicViceForm;
            }

            if (_mIsGame)
            {
                starPart.SetActive(true);
                GameEntry.Event.Subscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);
                int level = GlobalData.GameDifficulty;
                ShowStar(level);
            }
            else {
                starPart.SetActive(false);
            }
        }
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
        protected override void OnClose(object userData)
        {
            if (_mIsGame)
            {
                GameEntry.Event.Unsubscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);
            }
            base.OnClose(userData);
        }

        /// <summary>
        /// 显示对应难度星星
        /// </summary>
        /// <param name="num"></param>
        private void ShowStar(int num) {
           for(int i = 0; i < starPart.transform.childCount; i++)
            {
                GameObject temp = starPart.transform.GetChild(i).gameObject;
                if (i < num)
                {
                    if (!temp.activeSelf)
                    {
                        temp.SetActive(true);
                    }
                }
                else
                {
                    if (temp.activeSelf)
                    {
                        temp.SetActive(false);
                    }
                }
            }
        }

        protected virtual void OnDiffcultyChange(object sender, GameEventArgs e)
        {
            NormalDifficultyEventArgs ne = e as NormalDifficultyEventArgs;
            int level = (int)ne.Difficulty ;
            ShowStar(level);
        }
    }
}