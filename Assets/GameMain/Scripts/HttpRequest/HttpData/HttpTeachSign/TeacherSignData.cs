using System;
using GameFramework;
using LitJson;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class TeacherSignReq
    {
        public int teacher { set; get; }
        public long timestamp { set; get; }
        public string tel { set; get; }
    }

    public class TeacherSignResponse
    {
        public RespVo respVo { set; get; }
        public CoursesDailyMap[] coursesDailyMap { set; get; }
    }

    public class TeacherSignData : IDictable, ICloneable
    {
        private TeacherSignReq m_Req = new TeacherSignReq();
        public TeacherSignResponse Response { get; private set; }

        public TeacherSignData(int _teacher, long _timestamp,string tel)
        {
            m_Req.teacher = _teacher;
            m_Req.timestamp = _timestamp;
            m_Req.tel = tel;
        }

        #region 序列化

        public string ToJson()
        {
            return Utility.Json.ToJson(m_Req);
        }

        public byte[] ToJsonData()
        {
            return Utility.Json.ToJsonData(m_Req);
        }

        public void fromDict(string responseJson)
        {
            Response = Utility.Json.ToObject<TeacherSignResponse>(responseJson);
        }

        #endregion

        #region 克隆,拷贝方法需添加赋值操作

        public object Clone()
        {
            return new TeacherSignData(m_Req.teacher, m_Req.timestamp, m_Req.tel);
        }

        #endregion
    }
}