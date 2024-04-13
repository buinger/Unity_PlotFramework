using Kmax;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMaiZhenResult_ShowUi : ShowTextDataPlot
{
    public MaiZhenType handType = MaiZhenType.zuo;


    public enum MaiZhenType
    {
        zuo,
        you
    }


    protected override string GetInfoText()
    {
        DRGuestInfomation aimInfo = GameEntry.DataTable.GetDataTable<DRGuestInfomation>().GetDataRow(GameEntry.Course.GetTrainingFactory.targetGuestInfoId);
        switch (handType)
        {
            case MaiZhenType.zuo:
                return aimInfo.ZuoMaiZhen;
            case MaiZhenType.you:
                return aimInfo.YouMaiZhen;
            default:
                Debug.LogError("未配置数据");
                return "未配置数据";
        }
    }

}