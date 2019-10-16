using GameFramework;
using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class FaceComponent : GameFrameworkComponent
    {
        [SerializeField]
        private string m_Api_Key = "YMlT_OVnZw1CLIVEruiGYiYYIUv3jM0E";
        [SerializeField]
        private string m_Api_Secret = "2A3KNb-BHe_jaiE4ZJ11zSi0yqszyTRQ";
        [SerializeField]
        private string m_GetFaceSetsURL = "https://api-cn.faceplusplus.com/facepp/v3/faceset/getfacesets";
        [SerializeField]
        private string m_SearchFaceURL = "https://api-cn.faceplusplus.com/facepp/v3/search";
        [SerializeField]
        private string m_GetDetailURL = "https://api-cn.faceplusplus.com/facepp/v3/faceset/getdetail";

        [SerializeField]
        private List<FaceSet.FaceSetData> m_FaceSetDatas = new List<FaceSet.FaceSetData>();

        public List<FaceSet.FaceSetData> FaceSets
        {
            get { return m_FaceSetDatas; }
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            m_FaceSetDatas.Clear();
            m_FaceSetDatas = null;
        }

        public string GetFacesetTokenByOuterId(string outer_id)
        {
            foreach (var item in FaceSets)
            {
                if (string.Equals(outer_id, item.outer_id))
                {
                    return item.faceset_token;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取facesets
        /// </summary>
        /// <param name="start"></param>
        /// <param name="uerData"></param>
        public void GetFaceSets(int start, object uerData)
        {
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("api_key", WebUtility.EscapeString(m_Api_Key));
            wwwForm.AddField("api_secret", WebUtility.EscapeString(m_Api_Secret));
            //wwwForm.AddField("tags", WebUtility.EscapeString(string.Empty));
            if (start != 0)
                wwwForm.AddField("start", WebUtility.EscapeString(start.ToString()));
            GameEntry.WebRequest.AddWebRequest(m_GetFaceSetsURL, wwwForm, uerData);
        }

        /// <summary>
        /// 搜寻face
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="uerData"></param>
        public void SearchFace(byte[] base64, string faceset_token, object uerData)
        {
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("api_key", WebUtility.EscapeString(m_Api_Key));
            wwwForm.AddField("api_secret", WebUtility.EscapeString(m_Api_Secret));
            wwwForm.AddField("image_base64", Convert.ToBase64String(base64));
            wwwForm.AddField("faceset_token", WebUtility.EscapeString(faceset_token));
            GameEntry.WebRequest.AddWebRequest(m_SearchFaceURL, wwwForm, uerData);
        }

        /// <summary>
        /// 获取faceset下面的所有facetoken
        /// </summary>
        /// <param name="faceset_token"></param>
        /// <param name="uerData"></param>
        public void GetDatil(string faceset_token, object uerData)
        {
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("api_key", WebUtility.EscapeString(m_Api_Key));
            wwwForm.AddField("api_secret", WebUtility.EscapeString(m_Api_Secret));
            wwwForm.AddField("faceset_token", WebUtility.EscapeString(faceset_token));
            GameEntry.WebRequest.AddWebRequest(m_SearchFaceURL, wwwForm, uerData);
        }

        public void AddFaceSetDatas(List<FaceSet.FaceSetData> facesets)
        {
            m_FaceSetDatas.AddRange(facesets);
        }


        public SearchFace.Results MaxConfidence(List<SearchFace.Results> resoure)
        {
            SearchFace.Results temp = null;
            for (int i = 0; i < resoure.Count - 1; i++)
            {
                for (int j = i + 1; j < resoure.Count; j++)
                {
                    if (resoure[i].confidence > resoure[j].confidence)
                    {
                        temp = resoure[i];
                        resoure[i] = resoure[j];
                        resoure[j] = temp;
                    }
                }
            }
            foreach (var item in resoure)
            {
                if (item.confidence > 85f)
                {
                    return item;
                }
            }
            return null;
        }

        public List<SearchFace.Results> ScreeningOver(List<SearchFace.Results> resoure, int defaultConfidence = 85)
        {
            List<SearchFace.Results> temp = new List<SearchFace.Results>();
            for (int i = 0; i < resoure.Count; i++)
            {
                if (resoure[i].confidence > defaultConfidence)
                {
                    temp.Add(resoure[i]);
                }
            }
            return temp;
        }

        public string[] ExtractFaceToken(List<SearchFace.Results> resoure)
        {
            string[] faceTokens = new string[resoure.Count];
            for (int i = 0; i < resoure.Count; i++)
            {
                faceTokens[i] = resoure[i].face_token;
            }
            return faceTokens;
        }
    }

    public class FaceSet
    {
        public class FaceSetData
        {
            public string faceset_token { set; get; }
            public string outer_id { set; get; }
            public string display_name { set; get; }
            public string tags { set; get; }
        }
        /// <summary>
        /// 耗时/ms
        /// </summary>
        public int time_used { set; get; }
        public List<FaceSetData> facesets { set; get; }
        public string request_id { set; get; }
        public string next { set; get; }
    }

    public class SearchFace
    {
        public class Face
        {
            public Rectangle image_id { set; get; }
            public string face_token { set; get; }
        }
        public class Rectangle
        {
            public int width { set; get; }
            public int top { set; get; }
            public int left { set; get; }
            public int height { set; get; }
        }
        public class Results
        {
            public double confidence { set; get; }
            public string user_id { set; get; }
            public string face_token { set; get; }
        }

        public string image_id { set; get; }
        public List<Face> faces { set; get; }
        public int time_used { set; get; }
        public string request_id { set; get; }
        public List<Results> results { set; get; }
    }


}