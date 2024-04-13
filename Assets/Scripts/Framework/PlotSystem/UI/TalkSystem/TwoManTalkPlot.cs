using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoManTalkPlot : OnePanelTalkPlot
{
    [Header("Ui显示位置，相对于此脚本所在位置")]
    public Vector3 uiLocalPos_another = Vector3.zero;
    [Header("主要人物（自己）的名字")]
    public string mainPersonName;


    protected override Vector3 GetAimPostion(string speakerName)
    {
        if (mainPersonName == speakerName)
        {
            return base.GetAimPostion(speakerName);
        }
        else
        {
            return uiLocalPos_another;
        }
    }

    //---------------------EDITOR---------------
    [ContextMenu("设置uiLocalPos_another位置为当前位置")]
    void SetUiLocalPos_another()
    {
        uiLocalPos_another = transform.localPosition;
    }
}
