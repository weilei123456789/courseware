using GameFramework;
using GameFramework.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Penny
{

    public class LessonBaseUIFrame : UGuiForm
    {
        /// <summary>
        /// 常用流程状态
        /// </summary>
        protected enum Produceing
        {
            None,
            //延时
            Delaying,
            //延时结束
            DelayEnd,
            //播放音频
            PlayingVoice,
            //播放音频
            PlayingVoiceEnd,
            //播放动画
            PlayingAni,
            //播放音频
            PlayingAniEnd,
            //播放音频和动画
            PlayingAniVoice,
            //播放音频和动画
            PlayingAniVoiceEnd,
            //游戏结束
            EndGame,
        }

        /// <summary>
        /// 辅助延迟状态
        /// </summary>
        protected enum ViceProduceing
        {
            None,
            //播放动画
            Playing,
            //动画播放完毕
            PlayingEnd,
        }

        [Header("组件层级父级")]
        [SerializeField]
        protected Transform BgPart;
        [SerializeField]
        protected Transform MidPart;
        [SerializeField]
        protected Transform FrontPart;

        [Header("有限状态机")]
        [SerializeField]
        protected Produceing m_ProduceingState = Produceing.None;

        [SerializeField]
        protected ViceProduceing m_ViceProduceingState = ViceProduceing.None;

        /// <summary>
        /// 当前播放语音索引
        /// </summary>
        protected int m_VoiceIndex = 0;

        /// <summary>
        /// 当前播放语音ID
        /// </summary>
        [SerializeField]
        protected int m_VoiceTrack = 0;
        /// <summary>
        /// 语音播放计时器
        /// </summary>
        protected float m_CurrentVoiceTime = 0;
        /// <summary>
        /// 最大语音时间长度
        /// </summary>
        protected float m_CurrentVoiceTimeLength = 0;

        /// <summary>
        /// 当前播放动画ID
        /// </summary>
        protected int m_ViceTrack = -1;
        /// <summary>
        /// 动画播放计时器
        /// </summary>
        protected float m_CurrentViceTime = 0;
        /// <summary>
        /// 最大动画时间长度
        /// </summary>
        protected float m_CurrentViceTimeLength = 0;

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            switch (m_ProduceingState)
            {
                case Produceing.PlayingVoice:
                    {
                        m_CurrentVoiceTime += elapseSeconds;
                        if (m_CurrentVoiceTime > m_CurrentVoiceTimeLength)
                        {
                            m_CurrentVoiceTime = 0;
                            m_ProduceingState = Produceing.PlayingVoiceEnd;
                        }
                    }
                    break;
                case Produceing.PlayingVoiceEnd:
                    {
                        VoiceOnComplete();
                    }
                    break;
                case Produceing.Delaying:
                    {
                        m_CurrentVoiceTime += elapseSeconds;
                        if (m_CurrentVoiceTime > m_CurrentVoiceTimeLength)
                        {
                            m_CurrentVoiceTime = 0;
                            m_ProduceingState = Produceing.DelayEnd;
                        }
                    }
                    break;
                case Produceing.DelayEnd:
                    {
                        m_ProduceingState = Produceing.None;
                        VoiceOnComplete();
                    }
                    break;
                case Produceing.EndGame:
                    {
                        GameEntry.GameManager.IsInGame = false;
                    }
                    break;
                default:
                    break;
            }

            switch (m_ViceProduceingState)
            {
                case ViceProduceing.None:
                    break;
                case ViceProduceing.Playing:
                    {
                        m_CurrentViceTime += elapseSeconds;
                        if (m_CurrentViceTime > m_CurrentViceTimeLength)
                        {
                            m_CurrentViceTime = 0;
                            m_ViceProduceingState = ViceProduceing.PlayingEnd;
                        }
                    }
                    break;
                case ViceProduceing.PlayingEnd:
                    {
                        ViceOnComplete();
                        m_ViceProduceingState = ViceProduceing.None;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 语音播放完成事件
        /// </summary>
        protected virtual void VoiceOnComplete()
        {
            switch (m_VoiceTrack)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// 延时完成事件 计时部分和语音通用 ID分配 5000—10000
        /// </summary>
        protected virtual void DelayOnComplete()
        {
            switch (m_VoiceTrack)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// 动画播放完成事件
        /// </summary>
        protected virtual void ViceOnComplete()
        {
            switch (m_ViceTrack)
            {
                default:
                    break;
            }
        }


        /// <summary>
        /// 对应课地址
        /// </summary>
        protected string m_LessonAssetPath = null;
        /// <summary>
        /// 对应季地址
        /// </summary>
        protected string m_SeasonAssetPath = null;


        private int DefaultPriority = 0;
        /// <summary>
        /// 播放游戏语音
        /// </summary>
        /// <param name="FileName">游戏语音文件名称</param>
        protected int PlayGameVoice(string FileName)
        {
            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, FileName);
            return GameEntry.Sound.PlaySound(path, "Sound");
        }

        protected int PlayGameVoice(string FileName, PlaySoundParams Soundparams)
        {
            
            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, FileName);
            return GameEntry.Sound.PlaySound(path, "Sound", DefaultPriority, Soundparams);
        }

        protected int PlayGameUIVoice(string FileName, PlaySoundParams Soundparams)
        {
            string path = AssetUtility.GetTTSSoundAsset(m_SeasonAssetPath, m_LessonAssetPath, FileName);
            return GameEntry.Sound.PlaySound(path, "UISound", DefaultPriority, Soundparams);
        }


        protected int PlayGameVoice(string FileName, int PlayPriority)
        {
            return PlayGameVoice(FileName, new PlaySoundParams { Priority = PlayPriority, });
        }

        protected int PlayGameUIVoice(string FileName, int PlayPriority)
        {
            return PlayGameUIVoice(FileName, new PlaySoundParams { Priority = PlayPriority, });
        }

        protected int PlayGameVoice(string FileName, SoundLevel soundLevel)
        {
            if (soundLevel == SoundLevel.Talk)
            {
                if(m_VoiceIndex>0)
                GameEntry.Sound.StopSound(m_VoiceIndex);
                
                return m_VoiceIndex= PlayGameVoice(FileName, (int)soundLevel);
            }
            else
            {
                return PlayGameUIVoice(FileName, (int)soundLevel);
            }

        }




        private int? s_NowBGM = null;
        /// <summary>
        /// 播放游戏背景音乐
        /// </summary>
        /// <param name="BGMName">文件名</param>
        protected int? PlayBGM(string BGMName)
        {

            StopLessonMusic();
            string path = AssetUtility.GetLessonMusicAsset(m_SeasonAssetPath, m_LessonAssetPath, BGMName);
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 64,
                Loop = true,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = 1f,
                SpatialBlend = 0f,
            };
            s_NowBGM = GameEntry.Sound.PlaySound(path, "Music", Constant.AssetPriority.MusicAsset, playSoundParams);
            return s_NowBGM;
        }

        /// <summary>
        /// 停止播放游戏BGM
        /// </summary>
        protected void StopLessonMusic()
        {
            if (!s_NowBGM.HasValue)
            {
                return;
            }

            GameEntry.Sound.StopSound(s_NowBGM.Value, 1f);
            s_NowBGM = null;
        }


        private float m_TempTime;
        /// <summary>
        /// 有时间间隔的播放语音
        /// </summary>
        /// <param name="IntervalTime">间断时间</param>
        /// <param name="IsPlayTerm">播放条件</param>
        /// <param name="FileName">文件名称</param>     
        protected int PlayIntervalVoice(float IntervalTime, bool IsPlayTerm, string FileName)
        {
            if (IsPlayTerm)
            {
                m_TempTime -= Time.deltaTime;
                if (m_TempTime < 0)
                {
                    m_TempTime = IntervalTime;
                    return PlayGameVoice(FileName);
                }
            }

            return -1;
        }


        protected int PlayIntervalVoice(float IntervalTime, bool IsPlayTerm, string FileName, SoundLevel soundLevel)
        {
            if (IsPlayTerm)
            {
                m_TempTime -= Time.deltaTime;
                if (m_TempTime < 0)
                {
                    m_TempTime = IntervalTime;
                    return PlayGameVoice(FileName, soundLevel);
                }
            }

            return -1;
        }

        /// <summary>
        /// 生成墙屏实体——有资源路径
        /// </summary>
        /// <param name="logicType">墙屏实体类型</param>
        /// <param name="data">墙屏实体类型数据</param>
        protected void ShowWallEntity(Type logicType, EntityData data)
        {
            GameEntry.Entity.ShowWallModel(logicType, m_SeasonAssetPath, m_LessonAssetPath, data);
        }
        /// <summary>
        /// 生成地屏实体——有资源路径
        /// </summary>
        /// <param name="logicType">地屏实体类型</param>
        /// <param name="data">地屏实体数据</param>
        protected void ShowGroundEntity(Type logicType, EntityData data)
        {
            GameEntry.Entity.ShowGroundModel(logicType, m_SeasonAssetPath, m_LessonAssetPath, data);
        }
        /// <summary>
        /// 生成特效实体——有资源路径
        /// </summary>
        /// <param name="logicType">墙屏实体类型</param>
        /// <param name="data">墙屏实体类型数据</param>
        protected void ShowEffectEntity(EffectData data)
        {
            GameEntry.Entity.ShowEffect(m_SeasonAssetPath, m_LessonAssetPath, data);
        }
        /// <summary>
        /// 生成特效实体——无资源路径
        /// </summary>
        /// <param name="data"></param>
        protected void ShowUsualEffectEntity(EffectData data)
        {
            GameEntry.Entity.ShowEffect(data);
        }
        /// <summary>
        /// 生成墙屏实体 公用资源——无资源路径
        /// </summary>
        /// <param name="logicType"></param>
        /// <param name="data"></param>
        protected void ShowUsualWallEntity(Type logicType, EntityData data)
        {
            GameEntry.Entity.ShowWallModel(logicType, data);
        }
        /// <summary>
        /// 生成地屏实体 公用资源——无资源路径
        /// </summary>
        /// <param name="logicType"></param>
        /// <param name="data"></param>
        protected void ShowUsualGroundEntity(Type logicType, EntityData data)
        {
            GameEntry.Entity.ShowGroundModel(logicType, data);
        }
    }
}