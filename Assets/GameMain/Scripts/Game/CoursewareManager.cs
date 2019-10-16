using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class CoursewareManager : MonoBehaviour
    {
        protected static CoursewareManager _instance = default(CoursewareManager);

        public static CoursewareManager Instance
        {
            get
            {
                return _instance;
            }
        }

        // 结束查询
        private GameFrameworkAction m_EndSearchCoursewareCallback = null;
        // 查询设备课件信息
        private GetDeviceWareInServer m_GetDeviceWareInServer = null;
        // 查询服务器课件信息
        private GetCoursewareInServer m_GetCoursewareInServer = null;
        // 查询全部课程
        private QueryCoursewareInServer m_QueryCoursewareInServer = null;

        // 设备课件信息
        private DeviceMap m_DeviceMap = null;
        // 服务器课件信息
        private CourseWareMap[] m_CourseWareMap = null;

        // 第一次登陆时间
        private DateTime m_FristLoginTime = DateTime.MinValue;
        // 今天的课程
        private CoursesDailyMap[] m_TodayCcourse = null;
        // 获取设备完成
        private bool m_IsGetDeviceWareFinish = false;
        // 获取服务器课件完成
        private bool m_IsGetCoursewareFinish = false;
        // 当前设备要下载的季资源名称
        private string[] m_CoursewareResourceNames = null;
        // 更具设备上的课件转成H5用的数据结构
        private WebCourseware[] m_GetWebCourseware = null;

        /// <summary>
        /// 当前设备上的季
        /// </summary>
        public CourseWareMap[] CourseWareMap
        {
            get
            {
                return m_CourseWareMap;
            }
        }

        /// <summary>
        /// 当前设备上拥有多少季
        /// </summary>
        public int SeasonLength
        {
            get
            {
                if (m_CourseWareMap != null)
                    return m_CourseWareMap.Length;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 当前设备要下载的季资源名称
        /// </summary>
        public string[] SeasonResourceDownloadName
        {
            get
            {
                return m_CoursewareResourceNames;
            }
        }

        /// <summary>
        /// 根据设备ID拉去的班级ID
        /// 开机拉一次和每天12点拉一次，“90,40,50”
        /// </summary>
        public string ClassId
        {
            get
            {
                if (m_DeviceMap == null)
                {
                    Log.Error("未获取到设备信息！！！");
                    return string.Empty;
                }
                return m_DeviceMap.classid;
            }
        }

        public string Socket_IP
        {
            get
            {
                if (m_DeviceMap == null)
                {
                    Log.Error("未获取到设备信息！！！");
                    return string.Empty;
                }
                return m_DeviceMap.lanip;
            }
        }

        public int Socket_Port
        {
            get
            {
                if (m_DeviceMap == null)
                {
                    Log.Error("未获取到设备信息！！！");
                    return -1;
                }
                return Convert.ToInt32(m_DeviceMap.socketport);
            }
        }

        /// <summary>
        /// 更具设备上的课件转成H5用的数据结构
        /// </summary>
        public WebCourseware[] WebCourseware
        {
            get
            {
                return m_GetWebCourseware;
            }
        }


        private void Awake()
        {
            _instance = this;
            m_FristLoginTime = DateTime.Now;
        }

        private void OnDisable()
        {
            if (_instance != null)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        /// <param name="endSearch"></param>
        public void InitCoursewareManager(GameFrameworkAction endSearch)
        {
            //设置课件资源查询标识
            if (!GameEntry.Base.EditorResourceMode)
            {
                GameEntry.Resource.CoursewareSearchPath = "gamescene/season";
            }
            m_IsGetDeviceWareFinish = false;
            m_IsGetCoursewareFinish = false;
            m_EndSearchCoursewareCallback = endSearch;
            // 查询设备课件信息
            GetDeviceWare();
            // 查询服务器课件信息
            GetCourseware();
        }

        /// <summary>
        /// 查询设备课件信息,功能已合并到查询服务器课件信息
        /// </summary>
        private void GetDeviceWare()
        {
            m_GetDeviceWareInServer = new GetDeviceWareInServer(GameEntry.WindowsConfig.Config.DeviceNumber, GetDeviceWareSuccess, GetDeviceWareFailed);
            m_GetDeviceWareInServer.SendMsg();
        }

        /// <summary>
        /// 查询服务器课件信息
        /// </summary>
        private void GetCourseware()
        {
            m_GetCoursewareInServer = new GetCoursewareInServer(GameEntry.WindowsConfig.Config.DeviceNumber, GetCoursewareSuccess, GetCoursewareFailed);
            m_GetCoursewareInServer.SendMsg();
        }

        /// <summary>
        /// 查询全部课程
        /// </summary>
        private void GetDeviceAllCourseware()
        {
            if (string.IsNullOrEmpty(ClassId))
            {
                m_TodayCcourse = null;
                // 结束查询
                if (m_EndSearchCoursewareCallback != null)
                    m_EndSearchCoursewareCallback();
                return;
            }
            m_QueryCoursewareInServer = new QueryCoursewareInServer(TimeUtility.GetTimeStamp(), ClassId, QueryCoursewareSuccess, QueryCoursewareFailed);
            m_QueryCoursewareInServer.SendMsg();
        }

        #region 查询设备课件信息 回调
        private void GetDeviceWareSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(GetDeviceWareData))
            {
                GetDeviceWareData data = userData as GetDeviceWareData;
                if (data.Response.respVo.resultCode == 0)
                {
                    m_DeviceMap = data.Response.devicesMap[0];
                }

                m_IsGetDeviceWareFinish = true;
            }

            CheckCallBack();
        }

        private void GetDeviceWareFailed(string error, IDictable userData)
        {
            Log.Info("SendFailed response : " + error);
            m_IsGetDeviceWareFinish = false;
        }
        #endregion

        #region 查询服务器课件信息 回调
        private void GetCoursewareSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(GetCoursewareData))
            {
                GetCoursewareData data = userData as GetCoursewareData;
                m_CourseWareMap = data.Response.courseWareMap;

                m_CoursewareResourceNames = new string[m_CourseWareMap.Length];
                for (int i = 0; i < m_CourseWareMap.Length; i++)
                {
                    m_CoursewareResourceNames[i] = m_CourseWareMap[i].resname;
                }
                m_IsGetCoursewareFinish = true;
            }

            CheckCallBack();
        }

        private void GetCoursewareFailed(string error, IDictable userData)
        {
            Log.Info("SendFailed response : " + error);
            m_IsGetCoursewareFinish = false;
        }
        #endregion

        #region 查询全部课程 回调
        private void QueryCoursewareSuccess(string response, IDictable userData)
        {
            Log.Info("SendSuccess response : " + response);
            if (userData.GetType() == typeof(QueryCoursewareData))
            {
                QueryCoursewareData data = userData as QueryCoursewareData;
                m_TodayCcourse = data.Response.coursesDailyMap;
                // 结束查询
                if (m_EndSearchCoursewareCallback != null)
                    m_EndSearchCoursewareCallback();
            }
        }

        private void QueryCoursewareFailed(string error, IDictable userData)
        {
            Log.Info("SendFailed response : " + error);
        }

        #endregion

        private void CheckCallBack()
        {
            if (m_IsGetCoursewareFinish && m_IsGetDeviceWareFinish)
            {
                GetDeviceAllCourseware();
            }
        }

        /// <summary>
        /// 获取当前时间有没有课(提前半小时判断)
        /// </summary>
        /// <returns></returns>
        public CoursesDailyMap QueryTodayNowTimeCourse()
        {
            if (m_TodayCcourse == null)
            {
                Log.Warning("There are no courses today. Please configure courses! ");
                return null;
            }
            int milli = 30 * 60 * 1000;
            foreach (var item in m_TodayCcourse)
            {
                Log.Info("起始:{0}     结束：{1}", TimeUtility.GetDateTime(item.starttime), TimeUtility.GetDateTime(item.endtime));
            }
            foreach (var item in m_TodayCcourse)
            {
                long cur = TimeUtility.GetTimeStamp();
                if (cur >= item.starttime - milli && cur < item.endtime)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 更具设备上的课件转成H5用的数据结构
        /// </summary>
        public void GetStructWithProduceInit()
        {
            m_GetWebCourseware = new WebCourseware[CourseWareMap.Length];
            for (int i = 0; i < CourseWareMap.Length; i++)
            {
                m_GetWebCourseware[i] = new WebCourseware()
                {
                    imgurl = CourseWareMap[i].imgurl,
                    name = CourseWareMap[i].name,
                    description = CourseWareMap[i].description,
                    id = CourseWareMap[i].id,
                };

                foreach (var item in CourseWareMap[i].courseWareDetailMap.courseWareDetailMap)
                {
                    WebCourseware.ServerInfo serverInfo = new WebCourseware.ServerInfo()
                    {
                        id = item.Value.id,
                        imgurl = item.Value.imgurl,
                        cdid = item.Value.cdid,
                        name = item.Value.name,
                    };

                    DRLesson[] dtLesson = GameEntry.DataTable.GetDataTable<DRLesson>().GetDataRows((x) => { return x.SeverID == item.Value.cdid; });
                    serverInfo.localInfo = new WebCourseware.LocalInfo[dtLesson.Length];
                    for (int j = 0; j < dtLesson.Length; j++)
                    {
                        serverInfo.localInfo[j] = new WebCourseware.LocalInfo()
                        {
                            id = dtLesson[j].Id,
                            isAni = dtLesson[j].IsAni,
                            serverId = dtLesson[j].SeverID,
                        };
                    }

                    m_GetWebCourseware[i].AddServerInfo(item.Key, serverInfo);
                }
            }
        }

        ///// <summary>
        ///// 根据cdid返回找到H5课件的本地id
        ///// </summary>
        ///// <param name="cdid"></param>
        ///// <returns></returns>
        //public WebCourseware.LocalInfo[] GetLocalInfoByCdid(int cdid)
        //{
        //    foreach (WebCourseware webCourseware in m_GetWebCourseware)
        //    {
        //        foreach (KeyValuePair<string, WebCourseware.ServerInfo> serverInfo in webCourseware.serverInfo)
        //        {
        //            if (serverInfo.Value.cdid == cdid)
        //            {
        //                return serverInfo.Value.localInfo;
        //            }
        //        }
        //    }
        //    return null;
        //}

    }
}