using GameFramework;
using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GFResource = GameFramework.Resource;
using System;

namespace Penny
{
    public class ProcedureUpdateGame : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get { return true; }
        }

        private bool m_UpdateAllComplete = false;
        private int m_UpdateCount = 0;
        private long m_UpdateTotalZipLength = 0;
        private int m_UpdateSuccessCount = 0;
        private List<GFResource.CoursewareUpdate> ServerCourseware = null;
        private UpdateType m_UpdateType = UpdateType.ReadyUpdate;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("<color=lime>进入<资源更新下载>流程.</color>");
            IsEnterNextProduce = false;
            m_UpdateCount = 0;
            m_UpdateTotalZipLength = 0;
            m_UpdateSuccessCount = 0;
            m_UpdateAllComplete = false;
            ServerCourseware = new List<GFResource.CoursewareUpdate>();

            GameEntry.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntry.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntry.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntry.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

            OpenUpdateUIForm();

            SetUpdateType(UpdateType.CheackVersion, () =>
            {
                GameEntry.Resource.CheckResources(CheckResourcesCompleteCallback);
            });
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntry.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntry.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntry.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);
            CloseUpdateUIForm();
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_UpdateType == UpdateType.UpdateResource || m_UpdateType == UpdateType.UpdateCourseware)
                UpdateResource(m_UpdateSuccessCount / (float)m_UpdateCount, GameEntry.Download.CurrentSpeed);

            if (!m_UpdateAllComplete)
            {
                return;
            }
            ChangeState<ProcedurePreload>(procedureOwner);
        }

        /// 使用可更新模式并检查资源。
        /// 参数:
        /// needUpdateResources:
        ///   是否需要进行资源更新。
        ///
        /// removedCount:
        ///   已移除的资源数量。
        ///
        /// updateCount:
        ///   要更新的资源数量。
        ///
        /// updateTotalLength:
        ///   要更新的资源总大小。
        ///
        /// updateTotalZipLength:
        ///   要更新的压缩包总大小。
        private void CheckResourcesCompleteCallback(bool needUpdateResources, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            if (needUpdateResources)
            {
                Log.Info("<color=lime> 已移除的资源数量:'{0}',要更新的资源数量: '{1}',要更新的资源总大小:'{2}',要更新的压缩包总大小:'{3}'.</color>", removedCount, updateCount, updateTotalLength, updateTotalZipLength);
                m_UpdateCount = updateCount;
                m_UpdateTotalZipLength = updateTotalZipLength;
                if (m_UpdateCount <= 0)
                {
                    Log.Error("要更新的资源数量小于0");
                    return;
                }
                //如果没有网络提示
                if (Application.internetReachability == NetworkReachability.NotReachable)
                    return;
                Log.Info("<color=lime> Start update resources...</color>");
                SetUpdateType(UpdateType.CheackVersionFinish, () =>
                {
                    SetUpdateType(UpdateType.UpdateResource, () =>
                    {
                        GameEntry.Resource.UpdateResources(OnResourceUpdateAllComplete);
                    });
                });
            }
            else
            {
                Log.Info("不需要进行资源更新");
                SetUpdateType(UpdateType.CheackVersionFinish, () =>
                {
                    GameEntry.Resource.UpdateResources(OnResourceUpdateAllComplete);
                });
            }
        }

        // 资源更新开始事件。
        private void OnResourceUpdateStart(object sender, GameEventArgs e)
        {
            ResourceUpdateStartEventArgs ne = (ResourceUpdateStartEventArgs)e;
            Log.Warning("Update resource '{0}' is start.", ne.Name);
        }

        // 资源更新改变事件。
        private void OnResourceUpdateChanged(object sender, GameEventArgs e)
        {
            //ResourceUpdateChangedEventArgs ne = (ResourceUpdateChangedEventArgs)e;
        }

        // 资源更新成功事件。
        private void OnResourceUpdateSuccess(object sender, GameEventArgs e)
        {
            ResourceUpdateSuccessEventArgs ne = (ResourceUpdateSuccessEventArgs)e;
            Log.Warning("Update resource '{0}' success.", ne.Name);
            m_UpdateSuccessCount++;
        }

        // 资源更新失败事件。
        private void OnResourceUpdateFailure(object sender, GameEventArgs e)
        {
            ResourceUpdateFailureEventArgs ne = (ResourceUpdateFailureEventArgs)e;
            if (ne.RetryCount >= ne.TotalRetryCount)
            {
                Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
                return;
            }
            else
            {
                Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
            }
        }

        // 资源更新全部完成事件。
        private void OnResourceUpdateAllComplete()
        {
            Log.Info("Public resources update complete.");

            UpdateResource(1, GameEntry.Download.CurrentSpeed);

            SetUpdateType(UpdateType.CheackCourseware, () =>
            {
                ServerCourseware.Clear();
                for (int i = 0; i < CoursewareManager.Instance.SeasonResourceDownloadName.Length; i++)
                {
                    GFResource.CoursewareUpdate item = GameEntry.Resource.CoursewareUpdate.Find(
                        (x) =>
                        {
                            return x.ResourceName.Name.Contains(CoursewareManager.Instance.SeasonResourceDownloadName[i]);
                        }
                    );
                    if (item != null) ServerCourseware.Add(item);
                }

                Log.Info("---------------- 2 -------------------");
                foreach (var item in ServerCourseware) Log.Warning(item.ResourceName.FullName);

                m_UpdateCount = ServerCourseware.Count;
                m_UpdateSuccessCount = 0;

                SetUpdateType(UpdateType.UpdateCourseware, () =>
                {
                    GameEntry.Resource.UpdateCoursewareResources(ServerCourseware.ToArray(), OnCoursewareUpdateAllComplete);
                });
            });
        }

        private void OnCoursewareUpdateAllComplete()
        {
            Log.Info("Courseware update complete.");
            SetUpdateType(UpdateType.UpdateCoursewareFinish, () => { m_UpdateAllComplete = true; });
        }

        private void OpenUpdateUIForm()
        {
            if (GameEntry.BuiltinData.m_UpdateGameForm)
                GameEntry.BuiltinData.m_UpdateGameForm.gameObject.SetActive(true);
            if (GameEntry.BuiltinData.m_UpdateGameViceForm)
                GameEntry.BuiltinData.m_UpdateGameViceForm.gameObject.SetActive(true);
        }

        private void CloseUpdateUIForm()
        {
            GameEntry.BuiltinData.ClearResourceUI();
        }

        private void UpdateResource(float _progress, float _speed)
        {
            if (GameEntry.BuiltinData.m_UpdateGameForm)
            {
                GameEntry.BuiltinData.m_UpdateGameForm.SetUpdateValue(_progress, _speed);
            }
        }

        private void SetUpdateType(UpdateType updateType, GameFrameworkAction delayCall)
        {
            m_UpdateType = updateType;
            GameEntry.BuiltinData.m_UpdateGameForm.SetUpdateType(updateType, delayCall);
        }
    }
}
