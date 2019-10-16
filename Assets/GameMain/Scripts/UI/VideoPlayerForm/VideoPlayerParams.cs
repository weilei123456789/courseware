using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityEngine.Video;

namespace Penny
{

    public class VideoPlayerParams
    {
        public string Name { get; set; } = string.Empty;

        public bool HasCallBack { get; set; } = false;

        public bool IsLoop { get; set; } = false;

        public VideoPlayer.EventHandler PlayEndCallBack;

        public bool HasGSprite { get; set; } = false;

        public DRLesson delesson { get; set; } = null;
    }
}