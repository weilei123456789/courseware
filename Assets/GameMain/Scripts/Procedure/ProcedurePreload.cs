//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Penny
{
    public class ProcedurePreload : ProcedureBase
    {
        public static readonly string[] DataTableNames = new string[]
        {
            "Entity",
            "Music",
            "Scene",
            "Sound",
            "UIForm",
            "UISound",
            "Lesson",
            "Game",
        };

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        //private string[] m_LoadLuaNames = null;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
            m_LoadedFlag.Clear();
            PreloadResources();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //if (m_LoadLuaNames != null)
            //{
            //    foreach (var item in m_LoadLuaNames)
            //    {
            //        LoadLua(item);
            //    }
            //    m_LoadLuaNames = null;
            //}

            IEnumerator<bool> iter = m_LoadedFlag.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (!iter.Current)
                {
                    return;
                }
            }

            ChangeState<ProcedureInit>(procedureOwner);
        }

        private void PreloadResources()
        {
            // Preload configs
            LoadConfig("DefaultConfig");

            // Preload data tables
            foreach (string dataTableName in DataTableNames)
            {
                LoadDataTable(dataTableName);
            }

            // Preload dictionaries
            LoadDictionary("Default");

            // Preload fonts
            LoadFont("MainFont");

            // Preload Lua
            LoadLuaList("LuaScriptList");
            //LoadLua("TestBase");
        }

        private void LoadConfig(string configName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Config.{0}", configName), false);
            GameEntry.Config.LoadConfig(configName, LoadType.Bytes, this);
        }

        private void LoadDataTable(string dataTableName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
            GameEntry.DataTable.LoadDataTable(dataTableName, LoadType.Bytes, this);
        }

        private void LoadDictionary(string dictionaryName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Dictionary.{0}", dictionaryName), false);
            GameEntry.Localization.LoadDictionary(dictionaryName, LoadType.Text, this);
        }

        private void LoadFont(string fontName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Font.{0}", fontName), false);
            GameEntry.Resource.LoadAsset(AssetUtility.GetFontAsset(fontName), Constant.AssetPriority.FontAsset, new LoadAssetCallbacks(
                (assetName, asset, duration, userData) =>
                {
                    m_LoadedFlag[Utility.Text.Format("Font.{0}", fontName)] = true;
                    UGuiForm.SetMainFont((Font)asset);
                    Log.Info("<color=lime>Load font '{0}' OK.</color>", fontName);
                },

                (assetName, status, errorMessage, userData) =>
                {
                    Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName, errorMessage);
                }));
        }

        private void LoadLuaList(string listFileName)
        {
            m_LoadedFlag.Add(string.Format("LuaList.{0}", listFileName), false);
            GameEntry.Resource.LoadAsset(AssetUtility.GetLuaListAsset(listFileName), Constant.AssetPriority.FontAsset, new LoadAssetCallbacks(
               (assetName, asset, duration, userData) =>
               {
                   m_LoadedFlag[string.Format("LuaList.{0}", listFileName)] = true;
                   TextAsset textAsset = (TextAsset)asset;
                   Log.Info("<color=lime>Load LuaList '{0}' OK.</color>", listFileName);

                   string[] m_LoadLuaNames = textAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                   for (int i = 0; i < m_LoadLuaNames.Length; i++)
                   {
                       if (!string.IsNullOrEmpty(m_LoadLuaNames[i]))
                           LoadLua(m_LoadLuaNames[i].ToString());
                   }
               },

               (assetName, status, errorMessage, userData) =>
               {
                   Log.Error("Can not load lua list file '{0}' from '{1}' with error message '{2}'.", listFileName, assetName, errorMessage);
               }));
        }

        private void LoadLua(string luaName, bool isHotfix = false)
        {
            m_LoadedFlag.Add(string.Format("Lua.{0}", luaName), false);
            GameEntry.XLua.LoadFile(AssetUtility.GetLuaAsset(luaName), luaName,
              (fileName, filePath) =>
              {
                  m_LoadedFlag[string.Format("Lua.{0}", fileName)] = true;
                  Log.Info("<color=lime>Load Lua '{0}' OK.</color>", fileName);
                  byte[] bytes = GameEntry.XLua.m_CachedLuaScripts[fileName];
                  GameEntry.XLua.AddLoader((ref string filepath) =>
                  {
                      filepath = filePath;
                      return bytes;
                  });
                  if (isHotfix)
                  {
                      GameEntry.XLua.DoString(string.Format("require '{0}'", fileName));
                  }
              },
              (fileName, status, errorMessage) =>
              {
                  string error = string.Format("Name:{0}  status:{1} error:{2}", fileName, status, errorMessage);
                  Log.Error("Failure: " + error);
              }
            );
        }

        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[Utility.Text.Format("Config.{0}", ne.ConfigName)] = true;
            Log.Info("<color=lime>Load config '{0}' OK.</color>", ne.ConfigName);
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigName, ne.ConfigAssetName, ne.ErrorMessage);
        }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            m_LoadedFlag[Utility.Text.Format("DataTable.{0}", ne.DataTableName)] = true;
            Log.Info("<color=lime>Load data table '{0}' OK.</color>", ne.DataTableName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            LoadDataTableFailureEventArgs ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableName, ne.DataTableAssetName, ne.ErrorMessage);
        }

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[Utility.Text.Format("Dictionary.{0}", ne.DictionaryName)] = true;
            Log.Info("<color=lime>Load dictionary '{0}' OK.</color>", ne.DictionaryName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryName, ne.DictionaryAssetName, ne.ErrorMessage);
        }


    }
}
