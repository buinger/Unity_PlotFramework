using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Kmax;
using System.Threading.Tasks;
using System;

public abstract class TalkPlot : UiPlot
{

    //UI资源路径
    [Header("ui名字")]
    [SerializeField]
    protected PrefabInfo uiTemplate;

    [Header("Ui显示位置，相对于此脚本所在位置")]
    public Vector3 uiLocalPos = Vector3.zero;

    //---------------------EDITOR---------------
    [ContextMenu("设置localPosition位置为当前位置")]
    void SetUiLocalPostion()
    {
        uiLocalPos = transform.localPosition;
    }

    protected void CheckUi()
    {
        if (uiTemplate == null)
        {
            Debug.LogError("ui都没设置啊");
            Destroy(this);
        }
    }
    protected override void Ini(Action onIniOver)
    {
        CheckUi();
        //transform.localPosition = Vector3.zero;
        //RectTransform rectTrans= transform.GetComponent<RectTransform>();
        //rectTrans.sizeDelta = new Vector2(1920,1080);
        onIniOver.Invoke();
    }

  


    protected virtual Vector3 GetAimPostion(string speakerName = "")
    {
        return uiLocalPos;
    }


    protected override void ResetPlot()
    {

        List<Transform> alltrans = TransformHelper.GetImmediateChildList(transform);

        for (int i = 0; i < alltrans.Count; i++)
        {
            GameObject temp = alltrans[i].gameObject;

            TalkPanel talkPanel = temp.GetComponent<TalkPanel>();
            talkPanel.ClearContent();

            GameEntry.PlotPool.PutInPool(temp);
        }
    }
}


