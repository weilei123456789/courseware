using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Penny
{


    public class RemoteControlPenForm : UGuiForm
    {
        [SerializeField]
        private GameObject BtnItem = null;

        [SerializeField]
        private Transform PartTF = null;

        [SerializeField]
        private GameObject ScollItem = null;

        [SerializeField]
        private GameObject SeasonPart = null;
        private Transform SeasonParent = null;


        private Transform NowSeason = null;

        //public Image test;

        //private List<GameObject> SeasonList = new List<GameObject>();
        private Dictionary<int, GameObject> SeasonList = new Dictionary<int, GameObject>();


        private CourseWareMap[] m_courseWareMap ;

        private ProcedureSelCourseware m_ProcedureSelCourseware = null;

        private int LessonIndex = 0;
        private int T_lessonIndex = 0;
        private int SeasonIndex = 0;
        private int T_seasonIndex = 0;

        private bool IsSeason;

        private int m_RemoteControlPenViceForm = -1;



        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            LoadNetResTools.Instance.NCFileRecv();

        }



        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureSelCourseware = (ProcedureSelCourseware)userData;

            m_RemoteControlPenViceForm = (int)GameEntry.UI.OpenUIForm(UIFormId.RemoteControlPenViceForm, this);

            //GetCoursewareInServer m_GetCoursewareServer = new GetCoursewareInServer(1, OnGetCoursewareSucess, OnGetCoursewareFail);
            //m_GetCoursewareServer.SendMsg();
            m_courseWareMap = CoursewareManager.Instance.CourseWareMap;

            SeasonParent = SeasonPart.transform.GetChild(0);


            //LoadNetResTools.Instance.LoadSprite("http://47.101.136.112:8080/test/upload/lesson2.png", test);

            Log.Info("1___ " + m_courseWareMap.Length);

            InitScroll();
            OpenSeasonMenu();
        }

    
    


        protected override void OnClose(object userData)
        {
            if (GameEntry.UI.HasUIForm(m_RemoteControlPenViceForm))
                GameEntry.UI.CloseUIForm(m_RemoteControlPenViceForm);

            base.OnClose(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (IsSeason)
                {
                    ConfrimSeasonMenu();
                }
                else
                {
                    ConfrimChoess();
                }
             
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {

                OpenSeasonMenu();
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (IsSeason)
                {
                    SeasonIndex--;
                }
                else {
                    LessonIndex--;
                }
                
                ConrtrolPenArrow();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {

                if (IsSeason)
                {
                    SeasonIndex++;
                }
                else
                {
                    LessonIndex++;
                }

                ConrtrolPenArrow();
            }
        }


        private void InitScroll()
        {
            if (m_courseWareMap == null) {
                Log.Warning("课程信息为空！！~");
                return;
            }
           
            if (m_courseWareMap.Length > 0)
            {
                //账号创建部分
                for (int i = 0; i < m_courseWareMap.Length; i++)
                {

                    int lenth = m_courseWareMap[i].courseWareDetailMap.courseWareDetailMap.Count;

                    //学季选择部分
                    GameObject SeasonBtn;

                    if (i < SeasonParent.childCount)
                    {
                        SeasonBtn = SeasonParent.GetChild(i).gameObject;
                    }
                    else {
                        SeasonBtn = Instantiate(BtnItem);
                    }



                    SeasonBtn.transform.SetParent(SeasonParent);
                    SeasonBtn.transform.localScale = Vector3.one;
                    SeasonBtn.transform.GetChild(0).gameObject.SetActive(false);
                    SeasonBtn.name = m_courseWareMap[i].id.ToString();
                    SeasonBtn.transform.GetChild(1).GetComponent<Text>().text = m_courseWareMap[i].name;
                    LoadNetResTools.Instance.LoadSprite(m_courseWareMap[i].imgurl, SeasonBtn.GetComponent<Image>());

                    SeasonBtn.SetActive(true);

                    //课列表创建部分
                    GameObject Scogo;
                    if (i < PartTF.childCount)
                    {
                        Scogo = PartTF.GetChild(i).gameObject;
                    }
                    else
                    {
                        Scogo = Instantiate(ScollItem);
                    }
                    Scogo.transform.SetParent(PartTF);
                    Scogo.name = m_courseWareMap[i].id.ToString();
                    Scogo.transform.localScale = Vector3.one;
                    Scogo.SetActive(false);

                    ///确认对应id是否已有对应课表
                    if (SeasonList.ContainsKey(m_courseWareMap[i].id))
                    {
                        SeasonList[m_courseWareMap[i].id] = Scogo;
                    }
                    else
                    {
                        SeasonList.Add(m_courseWareMap[i].id, Scogo);
                    }

                    //季内部课表创建部分
                    int track = 0;
                    foreach (string  key in m_courseWareMap[i].courseWareDetailMap.courseWareDetailMap.Keys) {
                        GameObject go;
                        if (track < Scogo.transform.childCount)
                        {
                            go = Scogo.transform.GetChild(track).gameObject;
                        }
                        else
                        {
                            go = Instantiate(BtnItem);
                        }
                        go.transform.SetParent(Scogo.transform);
                        go.name = m_courseWareMap[i].courseWareDetailMap.courseWareDetailMap[key].cdid.ToString();
                        go.transform.localScale = Vector3.one;
                        go.SetActive(true);
                        go.transform.GetChild(0).gameObject.SetActive(false);
                        LoadNetResTools.Instance.LoadSprite(m_courseWareMap[i].courseWareDetailMap.courseWareDetailMap[key].imgurl, go.GetComponent<Image>());
                        go.transform.GetChild(1).GetComponent<Text>().text = m_courseWareMap[i].courseWareDetailMap.courseWareDetailMap[key].name;
                        track++;
                    }
                   
                }

                NowSeason = SeasonList[1].transform;
            }

           

            LessonIndex = 0;
            T_lessonIndex = 0;
            SeasonIndex = 0;
            T_seasonIndex = 0;
         
            SeasonParent.GetChild(SeasonIndex).GetChild(0).gameObject.SetActive(true);

            //SeasonList[1].SetActive(true);
            //SeasonList[1].transform.GetChild(LessonIndex).GetChild(0).gameObject.SetActive(true);
        }

        private void ConrtrolPenArrow()
        {
            if (IsSeason)
            {

                if (SeasonIndex >= SeasonParent.childCount)
                {
                    SeasonIndex = 0;
                }

                if (SeasonIndex < 0)
                {
                    SeasonIndex = SeasonParent.childCount - 1;
                }
                SeasonParent.GetChild(T_seasonIndex).GetChild(0).gameObject.SetActive(false);

                SeasonParent.GetChild(SeasonIndex).GetChild(0).gameObject.SetActive(true);

                T_seasonIndex = SeasonIndex;


            }
            else
            {

                if (LessonIndex >= NowSeason.childCount)
                {
                    LessonIndex = 0;
                }

                if (LessonIndex < 0)
                {
                    LessonIndex = NowSeason.childCount - 1;
                }

                NowSeason.GetChild(T_lessonIndex).GetChild(0).gameObject.SetActive(false);

                NowSeason.GetChild(LessonIndex).GetChild(0).gameObject.SetActive(true);

                T_lessonIndex = LessonIndex;
            }
        }

        private void ConfrimChoess()
        {
            int id = int.Parse(NowSeason.GetChild(LessonIndex).gameObject.name);

            m_ProcedureSelCourseware.LoadLessonRes(id, id);
            //资源预加载
            GameEntry.GameManager.PreloadDrLessonBGRes(id);

            CloseLessonMenu();



            Close();
        }

        private void CloseLessonMenu() {

            NowSeason.GetChild(LessonIndex).GetChild(0).gameObject.SetActive(false);
            foreach (GameObject tf in SeasonList.Values)
            {
                tf.SetActive(false);
            }
        }


        //从季的课列表退回到季列表
        private void OpenSeasonMenu() {
            IsSeason = true;

            SeasonPart.SetActive(true);
            PartTF.gameObject.SetActive(false);
            CloseLessonMenu();
        }


        //确认所选季列表
        private void ConfrimSeasonMenu() {
            IsSeason = false;
            SeasonPart.SetActive(false);
            PartTF.gameObject.SetActive(true);
            int id = int.Parse( SeasonParent.GetChild(SeasonIndex).gameObject.name);
            NowSeason = SeasonList[id].transform;
            NowSeason.gameObject.SetActive(true);
            NowSeason.GetChild(LessonIndex).GetChild(0).gameObject.SetActive(true);
        }


   
    }
}