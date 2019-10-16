using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Penny
{
    public class ProcedureTeacherLogin : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }
        private TeacherFaceLoginForm m_FaceLoginForm = null;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            IsEnterNextProduce = false;
            IsGrabLoginTeacher = false;
            IsBackInitProceduce = false;
            GlobalData.GameStateType = GameStateType.Unknown;

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            GameEntry.UI.OpenUIForm(UIFormId.TeacherFaceLoginForm, this);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (IsBackInitProceduce)
            {
                ChangeState<ProcedureInit>(procedureOwner);
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
                ChangeState<ProcedureSelCourseware>(procedureOwner);
            }

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_FaceLoginForm != null)
            {
                m_FaceLoginForm.Close(true);
                m_FaceLoginForm = null;
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            m_FaceLoginForm = (TeacherFaceLoginForm)ne.UIForm.Logic;
        }
    }
}
