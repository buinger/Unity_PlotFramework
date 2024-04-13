using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kmax;
using Training;

public class ShowTemperature_ShowUi : ShowTextDataPlot
{
    public TMP_Text temperatureText;
    protected override string GetInfoText()
    {
        DRGuestInfomation data = GameEntry.DataTable.GetDataTable<DRGuestInfomation>()[GameEntry.Course.GetTrainingFactory.targetGuestInfoId];
        return data.Temperature.ToString() + "¡æ";
    }
}
