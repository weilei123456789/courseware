using GameFramework;
using UnityGameFramework.Runtime;

namespace Penny
{

    public class TeacherLoginServer : HttpBase
    {
        public TeacherLoginServer(string[] _facetoken, string _auid, int _flag, SuccessCallBack success, FailedCallBack failure)
            :base(GlobalData.Server_Getuserbyfacetokenlist, success, failure)
        {
            m_HttpSendData = new TeacherLoginData(_facetoken, _auid, _flag);
        }
    }

}
