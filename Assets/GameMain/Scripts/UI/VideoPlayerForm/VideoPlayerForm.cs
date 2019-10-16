using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using GameFramework;

namespace Penny
{

    public class VideoPlayerForm : UGuiForm
    {
        [SerializeField]
        private RawImage Video = null;

        private VideoPlayerParams VPParams;

        private int m_SerialId = -1;
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            GameEntry.VideoPlayer.VideoPlayEndHandler = null;

            Video.texture = GameEntry.VideoPlayer.Texture;
            VPParams = userData as VideoPlayerParams;

            PlayVideo(VPParams.Name, VPParams.IsLoop);

            GameEntry.VideoPlayer.VideoPlayEndHandler += VPParams.PlayEndCallBack;


            m_SerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.VideoPlayerGroundForm, VPParams);
        }

        private void PlayVideo(string videoName, bool IsLoop)
        {

            Video.gameObject.SetActive(true);

            GameEntry.VideoPlayer.PlayLoadMovice(videoName, IsLoop);



        }




        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_SerialId))
                GameEntry.UI.CloseUIForm(m_SerialId);
            GameEntry.VideoPlayer.Stop();


            GameEntry.VideoPlayer.VideoPlayEndHandler = null;



            base.OnClose(userData);

        }



        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}