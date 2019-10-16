using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Penny
{
    /// <summary>
    /// 进入选择课件流程
    /// </summary>
    public class ProcedureSelCourseware : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }
        private SelCoursewareForm m_SelCoursewareForm = null;

        private int m_LoadFormSerieid = -1;

        private int m_GameLessonLocalId = -1;
        public int m_GameLessonServerId = -1;
        private bool m_IsEnterKinectGame = false;

        private int m_SeasonMenuUIID = -1;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            IsEnterNextProduce = false;
            IsGrabLoginTeacher = false;
            IsBackInitProceduce = false;
            m_IsEnterKinectGame = false;
            m_GameLessonLocalId = -1;
            m_GameLessonServerId = -1;

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.UI.OpenUIForm(UIFormId.SelCoursewareForm, this);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (IsBackInitProceduce)
            {
                ChangeState<ProcedureInit>(procedureOwner);
                return;
            }

            if (m_IsEnterKinectGame)
            {
                ChangeState<ProcedureKinectGame>(procedureOwner);
                return;
            }

            if (IsGrabLoginTeacher)
            {
                ChangeState<ProcedureTeacherLogin>(procedureOwner);
                return;
            }

            if (IsEnterNextProduce)
            {
                //跳转游戏流程
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameLessonLocalId, m_GameLessonLocalId);
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameLessonServerId, m_GameLessonServerId);
                ChangeState<ProcedureGame>(procedureOwner);
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                if (!GameEntry.UI.HasUIForm(m_SeasonMenuUIID)&&!GameEntry.UI.HasUIForm(m_LoadFormSerieid))
                    OpenControlPenUIForm();
            }


        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_SelCoursewareForm != null)
            {
                m_SelCoursewareForm.Close(true);
                m_SelCoursewareForm = null;
            }
            if (m_LoadFormSerieid != -1) {
                GameEntry.UI.CloseUIForm(m_LoadFormSerieid);
                m_LoadFormSerieid = -1;
            }

        }

        private void OpenControlPenUIForm() {
            if (m_SelCoursewareForm != null)
            {
                m_SelCoursewareForm.Close(true);
                m_SelCoursewareForm = null;
            }
           
            m_SeasonMenuUIID = (int)GameEntry.UI.OpenUIForm(UIFormId.RemoteControlPenForm, this);
        }


        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            if (ne.UIForm.Logic.GetType() == typeof(SelCoursewareForm))
                m_SelCoursewareForm = (SelCoursewareForm)ne.UIForm.Logic;
        }

        //设置课件的ID
        public void GameLessonId(int localId, int serverId)
        {
            m_SeasonMenuUIID = -1;
            m_GameLessonLocalId = localId;
            m_GameLessonServerId = serverId;
            NextProduce();
        }


        public void LoadLessonRes(int LocalID ,int ServerID) {
            m_SeasonMenuUIID = -1;
            m_GameLessonLocalId = LocalID;
            m_GameLessonServerId = ServerID;

            if (m_SelCoursewareForm != null)
            {
                m_SelCoursewareForm.Close(true);
                m_SelCoursewareForm = null;
            }
            m_LoadFormSerieid = (int)GameEntry.UI.OpenUIForm(UIFormId.LoadForm, this);
        }

        public void EnterKinectGame()
        {
            m_IsEnterKinectGame = true;
        }

    }
}
