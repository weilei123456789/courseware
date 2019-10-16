//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2019-09-29 01:23:52.773
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    /// <summary>
    /// 课程配置表。
    /// </summary>
    public class DRLesson : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取课程编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取服务器ID。
        /// </summary>
        public int SeverID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否动画环节。
        /// </summary>
        public int IsAni
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取拥有后续。
        /// </summary>
        public int HasFollow
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙面UIID。
        /// </summary>
        public int WallID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙面场景背景ID。
        /// </summary>
        public string WallBG
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地面UIID。
        /// </summary>
        public int GroundID
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地面场景背景ID。
        /// </summary>
        public string GroundBG
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取动画名称。
        /// </summary>
        public string AniName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取课件本地路径。
        /// </summary>
        public string LessonPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取分属学季路径。
        /// </summary>
        public string SeasonPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否墙屏是3D场景（弃用）。
        /// </summary>
        public int IsThreeDWallArea
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否地屏是3D场景（弃用）。
        /// </summary>
        public int IsThreeDGroundArea
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取3D场景名称（弃用）。
        /// </summary>
        public string ThreeDArea
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏父物体坐标（弃用）。
        /// </summary>
        public string WallParentPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏父物体旋转（弃用）。
        /// </summary>
        public string WallParentRotation
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏父物体坐标（弃用）。
        /// </summary>
        public string GroundParentPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏父物体旋转（弃用）。
        /// </summary>
        public string GroundParentRotation
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏摄像机坐标（弃用）。
        /// </summary>
        public string WallPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏摄像机旋转（弃用）。
        /// </summary>
        public string WallRotation
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏摄像机坐标（弃用）。
        /// </summary>
        public string GroundPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏摄像机旋转（弃用）。
        /// </summary>
        public string GroundRotation
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏平面背景坐标（弃用）。
        /// </summary>
        public string WallBGPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取墙屏平面背景大小（弃用）。
        /// </summary>
        public string WallBGScale
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏平面背景坐标（弃用）。
        /// </summary>
        public string GroundBGPostion
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取地屏平面背景大小（弃用）。
        /// </summary>
        public string GroundBGScale
        {
            get;
            private set;
        }

        public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
        {
            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
            string[] columnTexts = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnTexts.Length; i++)
            {
                columnTexts[i] = columnTexts[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnTexts[index++]);
            index++;
            SeverID = int.Parse(columnTexts[index++]);
            IsAni = int.Parse(columnTexts[index++]);
            HasFollow = int.Parse(columnTexts[index++]);
            WallID = int.Parse(columnTexts[index++]);
            WallBG = columnTexts[index++];
            GroundID = int.Parse(columnTexts[index++]);
            GroundBG = columnTexts[index++];
            AniName = columnTexts[index++];
            LessonPath = columnTexts[index++];
            SeasonPath = columnTexts[index++];
            IsThreeDWallArea = int.Parse(columnTexts[index++]);
            IsThreeDGroundArea = int.Parse(columnTexts[index++]);
            ThreeDArea = columnTexts[index++];
            WallParentPostion = columnTexts[index++];
            WallParentRotation = columnTexts[index++];
            GroundParentPostion = columnTexts[index++];
            GroundParentRotation = columnTexts[index++];
            WallPostion = columnTexts[index++];
            WallRotation = columnTexts[index++];
            GroundPostion = columnTexts[index++];
            GroundRotation = columnTexts[index++];
            WallBGPostion = columnTexts[index++];
            WallBGScale = columnTexts[index++];
            GroundBGPostion = columnTexts[index++];
            GroundBGScale = columnTexts[index++];

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
        {
            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, dataRowSegment.Offset, dataRowSegment.Length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.ReadInt32();
                    SeverID = binaryReader.ReadInt32();
                    IsAni = binaryReader.ReadInt32();
                    HasFollow = binaryReader.ReadInt32();
                    WallID = binaryReader.ReadInt32();
                    WallBG = binaryReader.ReadString();
                    GroundID = binaryReader.ReadInt32();
                    GroundBG = binaryReader.ReadString();
                    AniName = binaryReader.ReadString();
                    LessonPath = binaryReader.ReadString();
                    SeasonPath = binaryReader.ReadString();
                    IsThreeDWallArea = binaryReader.ReadInt32();
                    IsThreeDGroundArea = binaryReader.ReadInt32();
                    ThreeDArea = binaryReader.ReadString();
                    WallParentPostion = binaryReader.ReadString();
                    WallParentRotation = binaryReader.ReadString();
                    GroundParentPostion = binaryReader.ReadString();
                    GroundParentRotation = binaryReader.ReadString();
                    WallPostion = binaryReader.ReadString();
                    WallRotation = binaryReader.ReadString();
                    GroundPostion = binaryReader.ReadString();
                    GroundRotation = binaryReader.ReadString();
                    WallBGPostion = binaryReader.ReadString();
                    WallBGScale = binaryReader.ReadString();
                    GroundBGPostion = binaryReader.ReadString();
                    GroundBGScale = binaryReader.ReadString();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
        {
            Log.Warning("Not implemented ParseDataRow(GameFrameworkSegment<Stream>)");
            return false;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
