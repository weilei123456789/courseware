using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;

namespace Penny
{
    public enum PenPublicBtnClick {
        None = 0,
        OnClickReset = 1,
        OnClickSkip = 2,
        OnClickTwiceSkipOnce = 10,
        OnClickTwiceSkipTwice = 11,
    }

    public class PenPublicEventParams {
        //public bool OnClickReset { get; set; } = false;

        //public bool OnClickSkip { get; set; } = false;

        //public bool OnClickTwiceSkipOnce { get; set; } = false;

        //public bool OnClickTwiceSkipTwice { get; set; } = false;
        
        //循环OP是否是循环状态
        public bool VideoIsLoop { get; set; } = false;

        public PenPublicBtnClick BtnClick { get; set; } = PenPublicBtnClick.None;
    }


    public class PenPublicEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(PenPublicEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public PenPublicEventParams _PenParams {
            get;
            private set;
        }

        public PenPublicEventParams PenParams {
            get {
                return _PenParams;
            }
        }

        public override void Clear()
        {
            _PenParams = default(PenPublicEventParams);
        }

        public PenPublicEventArgs(PenPublicEventParams parms) {
            _PenParams = parms;
        }
    }
}