using GameFramework;
using System;
using System.Collections.Generic;

namespace Penny
{
    /// <summary>
    /// 告诉遥控器H5的课程信息
    /// </summary>
    public class WebCourseware
    {
        public class ServerInfo
        {
            public int id { set; get; }
            public string imgurl { set; get; }
            public int      cdid { set; get; }
            public string name      { set; get; }
            public LocalInfo[] localInfo { set; get; }

        }

        public class LocalInfo
        {
            public int id { set; get; }
            public int serverId { set; get; }
            public int isAni { set; get; }
        }

        public string imgurl { set; get; }

        public string name { set; get; }

        public string description { set; get; }

        public int id { set; get; }

        public Dictionary<string, ServerInfo> serverInfo { set; get; }

        public override string ToString()
        {
            return Utility.Json.ToJson(this);
        }

        public void AddServerInfo(string key, ServerInfo value)
        {
            if (serverInfo == null)
            {
                serverInfo = new Dictionary<string, ServerInfo>();
            }
            if (serverInfo.ContainsKey(key))
                throw new Exception(key + " -> has in Dictionary! ");
            serverInfo.Add(key, value);
        }
    }
}