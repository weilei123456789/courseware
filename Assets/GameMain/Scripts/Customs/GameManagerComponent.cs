using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityGameFramework.Runtime;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Resource;
using DG.Tweening;
using GameFramework.Event;

namespace Penny
{

    public class GameManagerComponent : GameFrameworkComponent
    {
   

        [SerializeField]
        //场景加载完成
        public bool IsNowCam;
        public bool IsInGame;

        [SerializeField]
        public bool IsMouseDebug = true;

        //private GameFrameworkAction<GameObject, Vector3> WallRayCallBack = null;
        //private GameFrameworkAction<GameObject, Vector3> GroundRayCallBack = null;

        //[SerializeField]
        //public VideoPlayer BGVPlayer = null;




        ///// <summary>
        ///// 墙面雷达输入端口
        ///// </summary>
        ///// <param name="ve"></param>
        //public void WallCreateRay(Vector3 ve)
        //{
        //    if (!IsNowCam || !GameCamManager.Intance)
        //    {
        //        return;
        //    }

        //    //从摄像机发出到点击坐标的射线
        //    Ray ray = GameCamManager.Intance.WallCam.ScreenPointToRay(ve);
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(ray, out hitInfo))
        //    {
        //        //划出射线，只有在scene视图中才能看到
        //        Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1);
        //        //GameObject gameObj = hitInfo.collider.gameObject;

        //        //播放点击特效 1秒只能播放一次
        //        // ShowWallEFByClick(hitInfo.point);

        //        //抛出事件
        //        //LeiDaGameObjectEventArgs ne = new LeiDaGameObjectEventArgs(hitInfo.collider.gameObject, hitInfo.point);
        //        //GameEntry.Event.Fire(this, new LeiDaGameObjectEventArgs(hitInfo.collider.gameObject, hitInfo.point));
        //        if (WallRayCallBack != null)
        //            WallRayCallBack(hitInfo.collider.gameObject, hitInfo.point);
        //    }
        //}

        ///// <summary>
        ///// 地面雷达输入端口
        ///// </summary>
        ///// <param name="ve"></param>
        //public void GroundCreateRay(Vector3 ve)
        //{
        //    if (!IsNowCam || !GameCamManager.Intance)
        //    {
        //        return;
        //    }
        //    //从摄像机发出到点击坐标的射线
        //    Ray ray = GameCamManager.Intance.GroundCam.ScreenPointToRay(ve);
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(ray, out hitInfo))
        //    {
        //        //划出射线，只有在scene视图中才能看到
        //        Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 0.1f);
        //        //GameObject gameObj = hitInfo.collider.gameObject;

        //        //   ShowGroundEFByClick(hitInfo.point);
        //        //抛出事件
        //        //LeiDaGameObjectEventArgs ne = new LeiDaGameObjectEventArgs(hitInfo.collider.gameObject, hitInfo.point);
        //        //GameEntry.Event.Fire(this, new LeiDaGameObjectEventArgs(hitInfo.collider.gameObject, hitInfo.point));
        //        if (GroundRayCallBack != null)
        //            GroundRayCallBack(hitInfo.collider.gameObject, hitInfo.point);
        //    }
        //}

        ///// <summary>
        ///// 注册墙屏射线回调
        ///// </summary>
        ///// <param name="_WallRayCallBack"></param>
        //public void SubscribeWallEvent(GameFrameworkAction<GameObject, Vector3> _WallRayCallBack)
        //{
        //    WallRayCallBack += _WallRayCallBack;
        //}
        ///// <summary>
        ///// 取消墙屏射线回调
        ///// </summary>
        ///// <param name="_WallRayCallBack"></param>
        //public void UnSubscribeWallEvent(GameFrameworkAction<GameObject, Vector3> _WallRayCallBack)
        //{
        //    WallRayCallBack -= _WallRayCallBack;
        //}
        ///// <summary>
        ///// 注册地屏射线回调
        ///// </summary>
        ///// <param name="_GroundRayCallBack"></param>
        //public void SubscribeGroundEvent(GameFrameworkAction<GameObject, Vector3> _GroundRayCallBack)
        //{
        //    GroundRayCallBack += _GroundRayCallBack;
        //}
        ///// <summary>
        ///// 取消地屏射线回调
        ///// </summary>
        ///// <param name="_GroundRayCallBack"></param>
        //public void UnSubscribeGroundEvent(GameFrameworkAction<GameObject, Vector3> _GroundRayCallBack)
        //{
        //    GroundRayCallBack -= _GroundRayCallBack;
        //}


        /// <summary>
        /// 根据id打开UI 后续读表填入UIFormID
        /// </summary>   
        public void OpenGameUIByID(int id)
        {
            DRLesson drLesson = LessonByID(id);

            int _severID = drLesson.SeverID;


            int WallUIID = drLesson.WallID;
            int GroundID = drLesson.GroundID;

            //GameEntry.UI.OpenUIForm(UIFormId.LoadForm, drLesson);
            GameEntry.UI.OpenUIForm(WallUIID, drLesson);
          
        }

        /// <summary>
        /// 根据ID关闭UI界面
        /// </summary>    
        public void CloseUIByID(int id)
        {
            if (id == -1) return;
            DRLesson drLesson = LessonByID(id);

            int WallUIID = drLesson.WallID;

            if (GameEntry.UI.GetUIForm(WallUIID, "") != null)
                GameEntry.UI.GetUIForm(WallUIID, "").Close(true);
  
            //回收全部模型
            RecoveryAll();

         
        }

      


        public void CloseVideoUIForm()
        {
            if (GameEntry.UI.GetUIForm(UIFormId.VideoPlayerForm) != null)
                GameEntry.UI.GetUIForm(UIFormId.VideoPlayerForm).Close(true);
            if (GameEntry.UI.GetUIForm(UIFormId.VideoPlayerGroundForm) != null)
                GameEntry.UI.GetUIForm(UIFormId.VideoPlayerGroundForm).Close(true);
        }


        /// <summary>
        /// 根据ID查找对应数据
        /// </summary>
        public DRLesson LessonByID(int id)
        {

            IDataTable<DRLesson> dtLesson = GameEntry.DataTable.GetDataTable<DRLesson>();
            DRLesson drLesson = dtLesson.GetDataRow(id);
            if (drLesson == null)
            {
                Log.Warning("Can not load Lesson '{0}' from data table.", id.ToString());
                return null;
            }


            return drLesson;
        }


        /// <summary>
        /// 根据serverID查找全部课件
        /// </summary>
        public DRLesson[] LessonsByID(int serverID)
        {
            IDataTable<DRLesson> dtLesson = GameEntry.DataTable.GetDataTable<DRLesson>();
            Predicate<DRLesson> item = i => { return i.SeverID == serverID; };
            DRLesson[] ddrLesson = dtLesson.GetDataRows(item);

            if (ddrLesson == null)
            {
                Log.Warning("Can not load Lessons '{0}' from data table.", serverID.ToString());
                return null;
            }

            return ddrLesson;
        }


        public string UIAssetName(int uiid)
        {

            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiid);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiid.ToString());
                return string.Empty;
            }
            return drUIForm.AssetName;
        }


        /// <summary>
        /// 协成延时执行函数
        /// </summary>
        /// <param name="time">延迟时间</param>
        /// <param name="action">需要执行的方法</param>
        public IEnumerator DelayWork(float time, GameFrameworkAction action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        /// <summary>
        /// 开始延时执行函数的协程
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public Coroutine StartDelayWork(float time, GameFrameworkAction action) {
             return  StartCoroutine(DelayWork(time, action));        
        }



        /// <summary>
        /// 预加载全部背景资源
        /// </summary>
        /// <param name="drles"></param>
        public void PreloadDrLessonBGRes(int serverID) {

            DRLesson[] drles = LessonsByID(serverID);

            for (int i = 0; i < drles.Length; i++) {
                if (drles[i].IsAni == 0)
                {
                    ResourceUtility.LoadGameVideo(drles[i].SeasonPath, drles[i].LessonPath, drles[i].WallBG);
                    ResourceUtility.LoadGameSence(drles[i].SeasonPath, drles[i].LessonPath, drles[i].GroundBG);
                }
            }
        }

        /// <summary>
        /// 清理全部
        /// </summary>
        public void RecoveryAll()
        {

            //隐藏全部实体
            GameEntry.Entity.HideAllLoadedEntities();
            GameEntry.Entity.HideAllLoadingEntities();
            //停止所有声音
            GameEntry.Sound.StopAllLoadedSounds();
            GameEntry.Sound.StopAllLoadingSounds();
        }


        public Vector3 vectorByString(string str) {
            string[] strArray = str.Split('|');
            if (strArray.Length == 3)
            {
                float x = float.Parse(strArray[0]);
                float y = float.Parse(strArray[1]);
                float z = float.Parse(strArray[2]);
                return new Vector3(x, y, z);
            }
            else {
                Log.Error("字符串转换坐标失败！~~~");
                return Vector3.zero;
            }
           
        }

        private int r_TempID = -1;
        /// <summary>
        /// 不连续重复随机整数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public int RandomInt(int min,int max) {
            int id = UnityEngine.Random.Range(min, max);
            if (id != r_TempID)
            {
                r_TempID = id;
                return r_TempID;
            }
            else {
                return RandomInt(min, max);
            }
         
        }

    }
}