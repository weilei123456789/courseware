using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadNetResTools : MonoBehaviour
{
    public const string RecvFileSuffix = "*.png";
    private const string MSM_Website = "http://47.101.136.112:8080/test/upload/";

    private static LoadNetResTools m_Instance;
    public static LoadNetResTools Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType(typeof(LoadNetResTools)) as LoadNetResTools;
            return m_Instance;
        }
    }


    public void Awake()
    {
        m_Instance = this;
    }

    private GameFrameworkAction<Sprite> m_FinishAction = null;
    private List<string> m_PersistentAllName = new List<string>();

    public void NCFileRecv()
    {
        m_PersistentAllName.Clear();
        FileInfo[] completePath = GetFolderChildsPath(FolderPath);
        for (int i = 0; i < completePath.Length; i++)
        {
            //Debug.LogWarning("图片完整路径： " + completePath[i].Name);
            m_PersistentAllName.Add(completePath[i].Name);
        }
    }

    public void LoadSprite(string headInfo, Image img)
    {

        StartCoroutine(LoadSprites(headInfo, img));
    }

    public IEnumerator LoadSprites(string headInfo, Image img)
    {
        //Debug.LogWarning("自定义头像路径: " + headInfo);

        string reduceWebsite = ReplaceName(headInfo, MSM_Website, string.Empty);
        string fileName = ReplaceName(reduceWebsite, "/", "_");
        //本地存在这个名字
        if (FindName(fileName))
        {
            //Debug.LogWarning("本地存在： " + string.Format("{0}/{1}", FolderPath, fileName));
            //TODO: 这路径在ios下好像会出错，没测过
            WWW www = new WWW(string.Format("file:///{0}/{1}", FolderPath, fileName));
            yield return www;
            while (!www.isDone)
                yield return new WaitForEndOfFrame();
            if (img != null)
                img.sprite = (Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f)));
            www.Dispose();
            www = null;
        }
        //下载保存本地
        else
        {
            if (string.IsNullOrEmpty(headInfo))
            {
                if (img != null)
                    img = null;
                yield break;
            }
            WWW www = new WWW(headInfo);
            yield return www;
            while (!www.isDone)
                yield return new WaitForEndOfFrame();
            if (img != null)
            {
                if (www.texture != null)
                {
                    img.sprite = (Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f)));
                    CreatePNG(string.Format("{0}/{1}", FolderPath, fileName), www.bytes, fileName);
                }
                else
                {
                    img = null;
                }
            }

            www.Dispose();
            www = null;
        }

    }



    public IEnumerator LoadSprites(string headInfo, GameFrameworkAction<Sprite> finishAction)
    {
        Debug.LogWarning("自定义头像路径: " + headInfo);

        string reduceWebsite = ReplaceName(headInfo, MSM_Website, string.Empty);
        string fileName = ReplaceName(reduceWebsite, "/", "_");
        //本地存在这个名字
        if (FindName(fileName))
        {
            //Debug.LogWarning("本地存在： " + string.Format("{0}/{1}", FolderPath, fileName));
            //TODO: 这路径在ios下好像会出错，没测过
            WWW www = new WWW(string.Format("file:///{0}/{1}", FolderPath, fileName));
            yield return www;
            while (!www.isDone)
                yield return new WaitForEndOfFrame();
            if (finishAction != null)
                finishAction(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f)));
            www.Dispose();
            www = null;
        }
        //下载保存本地
        else
        {
            if (string.IsNullOrEmpty(headInfo))
            {
                if (finishAction != null)
                    finishAction(null);
                yield break;
            }
            WWW www = new WWW(headInfo);
            yield return www;
            while (!www.isDone)
                yield return new WaitForEndOfFrame();
            if (finishAction != null)
            {
                if (www.texture != null)
                {
                    finishAction(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f)));
                    CreatePNG(string.Format("{0}/{1}", FolderPath, fileName), www.bytes, fileName);
                }
                else
                {
                    finishAction(null);
                }
            }

            www.Dispose();
            www = null;
        }

    }

    private bool FindName(string name)
    {
        for (int i = 0; i < m_PersistentAllName.Count; i++)
        {
            if (m_PersistentAllName[i].Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    #region 文件增删改查
    private string FolderPath
    {
        get
        {
            return string.Format("{0}/{1}", Application.persistentDataPath, "NCFile");
        }
    }

    private FileInfo[] GetFolderChildsPath(string folderPath)
    {
        //Debug.LogWarning("图片存放路径： " + folderPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        DirectoryInfo all = new DirectoryInfo(folderPath);
        return all.GetFiles(RecvFileSuffix);
    }

    void CreatePNG(string path, byte[] bytes, string name)
    {
        File.WriteAllBytes(path, bytes);
        m_PersistentAllName.Add(name);
    }

    public string ReplaceName(string strs, string oldValue, string newValue)
    {
        string str = strs.Replace(oldValue, newValue);
        return str;
    }

    #endregion
}
