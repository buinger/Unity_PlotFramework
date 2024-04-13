using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kmax;
using Training;

public class ShowXueYaInfo_ShowUi : ShowTextDataPlot
{
    protected override string GetInfoText()
    {
        DRGuestInfomation data = GameEntry.DataTable.GetDataTable<DRGuestInfomation>()[GameEntry.Course.GetTrainingFactory.targetGuestInfoId];

        string targetStr = data.BloodPressure;
        if (targetStr != "")
        {
            string[] temps = targetStr.Split('/');
            if (temps.Length == 3)
            {
                return temps[0] + "/" + temps[1];
            }
            else
            {
                Debug.LogError("���ݳ��Ȳ���:" + targetStr);
                return "120/70";
            }
        }
        Debug.Log("������û���ݣ�����Ĭ��Ѫѹ");
        return "120/70";
    }
}
