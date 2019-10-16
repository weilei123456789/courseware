using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class SelCoursewareForm : UGuiForm
    {
        [SerializeField]
        private RawImage m_RawImage = null;

        private int SC_Id = 0;
        private int SC_ServerId = 0;
        private ProcedureSelCourseware m_ProcedureSelCourseware = null;
        private int m_SerialId = -1;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            SC_Id = 0;
            SC_ServerId = 0;
            m_RawImage.texture = GameEntry.VideoPlayer.Texture;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_ProcedureSelCourseware = (ProcedureSelCourseware)userData;
            GameEntry.VideoPlayer.PlayLoadMovice("xPiNiMV");
            GameEntry.Socket.ChoiceCoursewareSuccessCallBack = ChoiceCoursewareSuccessCallBack;
            m_SerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.SelCoursewareViceForm);
        }

        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_SerialId))
                GameEntry.UI.CloseUIForm(m_SerialId);
            GameEntry.Socket.ChoiceCoursewareSuccessCallBack = null;
            GameEntry.VideoPlayer.Stop();
            base.OnClose(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 发送课件选择成功消息的回掉
        /// 进入下一流程
        /// </summary>
        /// <param name="obj"></param>
        private void ChoiceCoursewareSuccessCallBack(SocketData socketData)
        {
            Dictionary<string, object> JsonKeyValues = socketData.data as Dictionary<string, object>;
            if (JsonKeyValues != null)
            {
                // SC_Id = CourseWareDetailMap.id
                SC_Id = Convert.ToInt32(JsonKeyValues["id"]);
                // serverID = CourseWareDetailMap.cdid
                SC_ServerId = Convert.ToInt32(JsonKeyValues["serverID"]);

                m_ProcedureSelCourseware.LoadLessonRes(SC_Id, SC_ServerId);
              
                //资源预加载
                //GameEntry.GameManager.PreloadDrLessonBGRes(SC_ServerId);
                Log.Info("!!!!!!!!!!!!!!!!!!!!!  进入游戏啦！！！！！！！！！  {0}----{1}", SC_Id, SC_ServerId);
            }
        }

        /// <summary>
        /// 发送老师登陆成功消息的回掉
        /// 进入下一流程
        /// </summary>
        /// <param name="obj"></param>
        private void TeacherLoginSuccessCallBack(SocketDataResp obj)
        {
            m_ProcedureSelCourseware.NextProduce();
        }
    }
}