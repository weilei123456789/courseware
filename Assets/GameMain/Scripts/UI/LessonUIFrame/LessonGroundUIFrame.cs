using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using GameFramework;
using System;
using GameFramework.Sound;

namespace Penny
{

    public class LessonGroundUIFrame : LessonBaseUIFrame
    {
        ///// <summary>
        ///// 对应课地址
        ///// </summary>
        //protected string m_LessonAssetPath = null;
        ///// <summary>
        ///// 对应季地址
        ///// </summary>
        //protected string m_SeasonAssetPath = null;
        /// <summary>
        /// 对应lesson表内数据
        /// </summary>
        protected DRLesson drlesson;
        /// <summary>
        /// 游戏回合数 开始时计数
        /// </summary>
        protected int GameTurn;
        /// <summary>
        /// 游戏回合数 结束时计数
        /// </summary>
        protected int EndTurn;
        /// <summary>
        /// 游戏人数
        /// </summary>
        protected int HumanNumber = 1;

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (GameEntry.GameManager.IsMouseDebug)
            {
                RayHitByPC();
            }
        }
        /// <summary>
        /// 界面打开时常用时间
        /// </summary>

        protected virtual void UIOpenEvent(object userData)
        {
            drlesson = (DRLesson)userData;
            m_LessonAssetPath = drlesson.LessonPath;
            m_SeasonAssetPath = drlesson.SeasonPath;
            GameTurn = 0;
            EndTurn = 0;

            HumanNumber = GlobalData.HumanNumber;
        }



        /// <summary>
        /// 常用UI事件订阅
        /// </summary>
        protected void UIEventSubscribe()
        {

            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            GameEntry.Event.Subscribe(PenPublicEventArgs.EventId, OnPenPublicChange);

            GameEntry.Windows.SubscribeUIGroundEvent(OnRayHitByLeida);

            //GameEntry.Event.Subscribe(LeiDaGameObjectEventArgs.EventId, OnRayHitByLeida);
            GameEntry.Event.Subscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);

        }

        /// <summary>
        /// 常用UI事件退订
        /// </summary>
        protected void UIEventUnsubscribe()
        {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            GameEntry.Event.Unsubscribe(PenPublicEventArgs.EventId, OnPenPublicChange);
            GameEntry.Windows.UnSubscribeUIGroundEvent(OnRayHitByLeida);
          
            //GameEntry.Event.Unsubscribe(LeiDaGameObjectEventArgs.EventId, OnRayHitByLeida);
            GameEntry.Event.Unsubscribe(NormalDifficultyEventArgs.EventId, OnDiffcultyChange);

           
        }

        ///// <summary>
        ///// 播放游戏语音
        ///// </summary>
        ///// <param name="FileName">游戏语音文件名称</param>
        //protected int PlayGameVoice(string FileName)
        //{
        //    string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, FileName);
        //    return  GameEntry.Sound.PlaySound(path, "Sound");
        //}
        //protected int PlayGameVoice(string FileName, PlaySoundParams Soundparams )
        //{
        //    string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, FileName);
        //    return GameEntry.Sound.PlaySound(path, "Sound",0, Soundparams);
        //}

        ///// <summary>
        ///// 生成墙屏实体——有资源路径
        ///// </summary>
        ///// <param name="logicType">墙屏实体类型</param>
        ///// <param name="data">墙屏实体类型数据</param>
        //protected void ShowWallEntity(Type logicType, EntityData data)
        //{
        //    GameEntry.Entity.ShowWallModel(logicType, m_SeasonAssetPath, m_LessonAssetPath, data);
        //}

        ///// <summary>
        ///// 生成地屏实体——有资源路径
        ///// </summary>
        ///// <param name="logicType">地屏实体类型</param>
        ///// <param name="data"地屏实体数据></param>
        //protected void ShowGroundEntity(Type logicType, EntityData data)
        //{
        //    GameEntry.Entity.ShowGroundModel(logicType, m_SeasonAssetPath, m_LessonAssetPath, data);
        //}

        ///// <summary>
        ///// 生成特效实体——有资源路径
        ///// </summary>
        ///// <param name="logicType">墙屏实体类型</param>
        ///// <param name="data">墙屏实体类型数据</param>
        //protected void ShowEffectEntity(EffectData data)
        //{
        //    GameEntry.Entity.ShowEffect(m_SeasonAssetPath, m_LessonAssetPath, data);
        //}
        ///// <summary>
        ///// 生成特效实体——无资源路径
        ///// </summary>
        ///// <param name="data"></param>
        //protected void ShowUsualEffectEntity(EffectData data)
        //{
        //    GameEntry.Entity.ShowEffect(data);
        //}

        ///// <summary>
        ///// 生成墙屏实体 公用资源——无资源路径
        ///// </summary>
        ///// <param name="logicType"></param>
        ///// <param name="data"></param>
        //protected void ShowUsualWallEntity(Type logicType, EntityData data)
        //{
        //    GameEntry.Entity.ShowWallModel(logicType, data);
        //}
        ///// <summary>
        ///// 生成地屏实体 公用资源——无资源路径
        ///// </summary>
        ///// <param name="logicType"></param>
        ///// <param name="data"></param>
        //protected void ShowUsualGroundEntity(Type logicType, EntityData data)
        //{
        //    GameEntry.Entity.ShowGroundModel(logicType, data);
        //}


        /// <summary>
        /// 游戏计数器 分别统计起点和终点 取最小值
        /// </summary>
        protected virtual int GameCount() {
            if (GameTurn > EndTurn)
            {
                return EndTurn;
            }
            else{
                return GameTurn;
            }
        }


        /// <summary>
        /// 地屏生成射线
        /// </summary>
        protected void RayHitByPC()
        {
            if (!GameEntry.GameManager.IsNowCam)
                return;

            GameEntry.Windows.GroundUICameraRay(Input.mousePosition);
            GameEntry.Windows.GroundUICameraRay(Input.mousePosition + Vector3.up);
            GameEntry.Windows.GroundUICameraRay(Input.mousePosition + Vector3.down);
            GameEntry.Windows.GroundUICameraRay(Input.mousePosition + Vector3.right);
            GameEntry.Windows.GroundUICameraRay(Input.mousePosition + Vector3.left);
        }

        /// <summary>
        /// 重置游戏
        /// </summary>
        protected virtual void ResetGame() {

        }
        /// <summary>
        /// 跳过游戏
        /// </summary>
        protected virtual void SkipGame()
        {

        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }


        protected virtual void OnRayHitByLeida(GameObject go, Vector3 vc)
        {
            if (go == null)
                return;

            if (!GameEntry.GameManager.IsInGame)
                return;

        }

        protected virtual void OnDiffcultyChange(object sender, GameEventArgs e)
        {
            

        }

        protected virtual void OnPenPublicChange(object sender, GameEventArgs e)
        {
            PenPublicEventArgs ne = e as PenPublicEventArgs;
            PenPublicEventParams parms = ne.PenParams;

            switch (parms.BtnClick)
            {
                case PenPublicBtnClick.OnClickReset:
                    ResetGame();
                    break;
                case PenPublicBtnClick.OnClickTwiceSkipOnce:
                    SkipGame();
                    break;

            }
        }

    }
}