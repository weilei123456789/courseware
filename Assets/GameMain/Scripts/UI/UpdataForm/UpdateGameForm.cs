using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class UpdateGameForm : MonoBehaviour
    {
        [SerializeField]
        private Text m_UpdateLog = null;
        [SerializeField]
        private Text m_DownloadMessage = null;
        [SerializeField]
        private Image m_DownloadProcess = null;

        // 是否更新通用资源
        private bool IsUpdateGeneralResources = false;
        // 是否更新XXX季资源
        private bool IsUpdateXXXSeasonResources = false;
        // 更新进度
        private float m_UpdateProgress = 0;
        // 下载速度
        private float m_UpdateSpeed = 0;
        //// 下载总大小
        //private float m_UpdateTotalSize = 0;
        // 下载状态
        private UpdateType m_UpdateType = UpdateType.ReadyUpdate;

        private void Awake()
        {
            IsUpdateGeneralResources = false;
            IsUpdateXXXSeasonResources = false;
            m_UpdateProgress = 0;
            m_UpdateSpeed = 0;
            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            UpdateLog();
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        #region 获取公告
        private void UpdateLog()
        {
            GameEntry.WebRequest.AddWebRequest(GameEntry.BuiltinData.BuildInfo.UpdateLogUrl, this);
        }

        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData != this) return;
            string responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
            m_UpdateLog.text = responseJson;
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData != this) return;
            Log.Warning("UpdateLog failure, error message： '{0}'.", ne.ErrorMessage);
        }
        #endregion

        /// <summary>
        /// 设置更新状态
        /// </summary>
        /// <param name="updateType"></param>
        public void SetUpdateType(UpdateType updateType, GameFrameworkAction call)
        {
            m_UpdateType = updateType;
            switch (updateType)
            {
                case UpdateType.ReadyUpdate:
                    {
                        m_DownloadProcess.fillAmount = 0;
                        m_DownloadMessage.text = "";
                    }
                    break;
                case UpdateType.CheackVersion:
                    {
                        m_DownloadProcess.fillAmount = 0;
                        m_DownloadMessage.text = "正在检查是否有新版本。";
                        WaitDelayCall(call);
                    }
                    break;
                case UpdateType.CheackVersionFinish:
                    {
                        m_DownloadProcess.fillAmount = 0;
                        m_DownloadMessage.text = "检查版本完成,准备下载资源。";
                        WaitDelayCall(call);
                    }
                    break;
                case UpdateType.UpdateResource:
                    {
                        SetUpdateValue(0, 0);
                        if (call != null) call();
                    }
                    break;
                case UpdateType.CheackCourseware:
                    {
                        m_DownloadProcess.fillAmount = 0;
                        m_DownloadMessage.text = "正在检查课件。";
                        WaitDelayCall(call);
                    }
                    break;
                case UpdateType.UpdateCourseware:
                    {
                        SetUpdateValue(0, 0);
                        if (call != null) call();
                    }
                    break;
                case UpdateType.UpdateCoursewareFinish:
                    {
                        m_DownloadProcess.fillAmount = 1;
                        m_DownloadMessage.text = "准备进入未来教室。";
                        WaitDelayCall(call);
                    }
                    break;
            }
        }

        private void WaitDelayCall(GameFrameworkAction call, float value = 1.0f, float duration = 1.0f)
        {
            if (call == null) return;
            StartCoroutine(DelayedCall(call, value, duration));
        }

        private IEnumerator DelayedCall(GameFrameworkAction call, float value, float duration)
        {
            float time = 0f;
            float originalValue = m_DownloadProcess.fillAmount;
            while (time < duration)
            {
                time += Time.deltaTime;
                m_DownloadProcess.fillAmount = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }
            m_DownloadProcess.fillAmount = 1;
            if (call != null) call();
        }

        //正在更新：90% 下载速度：200k/s 总大小：200MB
        public void SetUpdateValue(float _progress, float _speed)
        {
            bool isMB = _speed / 1024f > 1024f;
            m_UpdateProgress = _progress;
            m_UpdateSpeed = isMB ? (_speed / 1024f / 1024f) : (_speed / 1024f);
            m_DownloadProcess.fillAmount = _progress;
            m_DownloadMessage.text = Utility.Text.Format("正在更新:{0}% 下载速度:{1}{2}", _progress * 100, m_UpdateSpeed.ToString("F2"), isMB ? "M/S" : "K/S");
        }
    }
}
