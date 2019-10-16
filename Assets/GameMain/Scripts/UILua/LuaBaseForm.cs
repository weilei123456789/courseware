using XLua;
using GameFramework;
using UnityEngine;

namespace Penny
{
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    [LuaCallCSharp]
    public class LuaBaseForm : UGuiForm
    {
        //Lua脚本名称，从xlua组件中获取byte[]
        public string m_LuaFileName;
        public Injection[] injections;

        //Lua回调
        private GameFrameworkAction<object> m_LuaOnInit;
        private GameFrameworkAction<object> m_LuaOnOpen;
        private GameFrameworkAction<object> m_LuaOnClose;
        private GameFrameworkAction<float, float> m_LuaOnUpdate;
        private GameFrameworkAction m_LuaOnPause;
        private GameFrameworkAction m_LuaOnResume;
        private GameFrameworkAction m_LuaOnCover;
        private GameFrameworkAction m_LuaOnReveal;
        private GameFrameworkAction<object> m_LuaOnRefocus;
        private GameFrameworkAction<int, int> m_LuaOnDepthChanged;

        //Lua表
        private LuaTable m_ScriptEnv;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_ScriptEnv = GameEntry.XLua.LuaEnvironment.NewTable();

            LuaTable _meta = GameEntry.XLua.LuaEnvironment.NewTable();
            _meta.Set("__index", GameEntry.XLua.LuaEnvironment.Global);
            m_ScriptEnv.SetMetaTable(_meta);
            _meta.Dispose();

            m_ScriptEnv.Set("self", this);
            foreach (var injection in injections)
            {
                m_ScriptEnv.Set(injection.name, injection.value);
            }
            //加载lua
            byte[] bytes = GameEntry.XLua.m_CachedLuaScripts[m_LuaFileName];
            //GameEntry.XLua.AddLoader((ref string filepath) => { return bytes; });
            GameEntry.XLua.DoString(Utility.Converter.GetString(bytes), "LuaBaseForm", m_ScriptEnv);
            //GameEntry.XLua.DoString(string.Format("require '{0}'", m_LuaFileName));

            //lua注册
            m_ScriptEnv.Get("OnInit", out m_LuaOnInit);
            m_ScriptEnv.Get("OnOpen", out m_LuaOnOpen);
            m_ScriptEnv.Get("OnClose", out m_LuaOnClose);
            m_ScriptEnv.Get("OnUpdate", out m_LuaOnUpdate);
            m_ScriptEnv.Get("OnPause", out m_LuaOnPause);
            m_ScriptEnv.Get("OnResume", out m_LuaOnResume);
            m_ScriptEnv.Get("OnCover", out m_LuaOnCover);
            m_ScriptEnv.Get("OnReveal", out m_LuaOnReveal);
            m_ScriptEnv.Get("OnRefocus", out m_LuaOnRefocus);
            m_ScriptEnv.Get("OnDepthChanged", out m_LuaOnDepthChanged);

            if (m_LuaOnInit != null)
                m_LuaOnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            if (m_LuaOnOpen != null)
                m_LuaOnOpen(userData);
        }

        protected override void OnClose(object userData)
        {
            if (m_LuaOnClose != null)
                m_LuaOnClose(userData);
            m_LuaOnInit = null;
            m_LuaOnOpen = null;
            m_LuaOnClose = null;
            m_LuaOnUpdate = null;
            m_LuaOnPause = null;
            m_LuaOnResume = null;
            m_LuaOnCover = null;
            m_LuaOnReveal = null;
            m_LuaOnRefocus = null;
            m_LuaOnDepthChanged = null;
            m_ScriptEnv.Dispose();
            base.OnClose(userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (m_LuaOnPause != null)
                m_LuaOnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (m_LuaOnResume != null)
                m_LuaOnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            if (m_LuaOnCover != null)
                m_LuaOnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            if (m_LuaOnReveal != null)
                m_LuaOnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            if (m_LuaOnRefocus != null)
                m_LuaOnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_LuaOnUpdate != null)
                m_LuaOnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            if (m_LuaOnDepthChanged != null)
                m_LuaOnDepthChanged(uiGroupDepth, depthInUIGroup);
        }
    }
}