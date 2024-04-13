using Training;
using UnityEngine;
using Kmax;
using System.Collections;
using System.Threading.Tasks;
using System;

public class TiGeJianCha_Selection : SelectableWithOverButtonPlot
{
    IDataTable<DRCommonTiGeJianCha> table;

    protected async override void Ini(Action onIniOver)
    {
        selection.choices.Clear();
        table = GameEntry.DataTable.GetDataTable<DRCommonTiGeJianCha>();
        DRCommonTiGeJianCha[] data = table.GetAllDataRows();      
        foreach (var item in data)
        {         
            Choice choiceTemp = null;
            if (item.PlotAfterClick != "")
            {
                Task<Plot> plotModelTask = TrainingFactory.GetPlotTempByTableStr(item.PlotAfterClick);
                await plotModelTask;
                Plot plotModel = plotModelTask.Result;

                ChildPlotInformation childPlotInfo = new ChildPlotInformation(true, plotModel);
                choiceTemp = new Choice(item.PlotName, childPlotInfo);
            }
            else
            {
                choiceTemp = new Choice(item.PlotName, new ChildPlotInformation());
            }
            selection.choices.Add(choiceTemp);
        }
        Debug.Log("vvvvvvvvv加载与之完成");
        base.Ini(onIniOver);
        
    }

    [ContextMenu("归零")]
    void SetZero()
    {
        transform.localPosition = Vector3.zero;
    }

    protected override string DoContentClickEvent(GameObject item)
    {
        string buttonStr = base.DoContentClickEvent(item);

        DRCommonTiGeJianCha[] dataTemp = table.GetDataRows(p => p.PlotName == buttonStr);
        if (dataTemp.Length == 0)
        {
            Debug.LogError($"表中不存在plot：{buttonStr}");
            return buttonStr;
        }
        else if (dataTemp.Length > 1)
        {
            Debug.LogError($"表中存在重名plot：{buttonStr}");
        }
        GameEntry.Course.GetTrainingFactory.UpdateScore(TrainingPartType.TiGeJianCha, dataTemp[0].Id, true);
        return buttonStr;
    }

}


//abstract class TiGeJianCha