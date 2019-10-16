using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Penny
{
    public class PenPublicParams
    {
        /// <summary>
        /// True 直接跳过 false 2次跳过
        /// </summary>
        public bool IsSkip { get; set; } = false;

        /// <summary>
        /// 是否游戏场景 显示难度星级
        /// </summary>
        public bool IsGame { get; set; } = false;

        /// <summary>
        /// 是否是循环OP场景 控制ICON显示
        /// </summary>
        public bool IsLoopOP { get; set; } = false;

        /// <summary>
        /// 热身操环节 禁用或不显示重置环节
        /// </summary>
        public bool IsWarmup { get; set; } = false;
    }
}