using Training;
using UnityEngine;
using Kmax;
using System.Threading.Tasks;
using System.Collections;
using System;

public class XuanJiao_Selection : SelectableWithOverButtonPlot
{
    IDataTable<DRCourseXuanJiao> table;
    protected async override void Ini(Action onIniOver)
    {
        selection.choices.Clear();
        table = GameEntry.DataTable.GetDataTable<DRCourseXuanJiao>();

        DRCourseXuanJiao[] data = table.GetAllDataRows();

        foreach (var item in data)
        {
            Choice choiceTemp = null;
            if (item.PlotAfterClick != "")
            {
                Task<GameObject> task = PlotPoolManager.PlotPool.LoadGameObjectAsync(item.PlotAfterClick);
                await task;
                ChildPlotInformation childPlotInfo = new ChildPlotInformation(true, task.Result.GetComponent<Plot>());
                choiceTemp = new Choice(item.PlotName, childPlotInfo);
            }
            else
            {
                choiceTemp = new Choice(item.PlotName, new ChildPlotInformation());
            }
            selection.choices.Add(choiceTemp);
        }
        base.Ini(onIniOver);
    }


    protected override string DoContentClickEvent(GameObject item)
    {
        string buttonStr = base.DoContentClickEvent(item);

        DRCourseXuanJiao[] dataTemp = table.GetDataRows(p => p.PlotName == buttonStr);
        if (dataTemp.Length == 0)
        {
            Debug.LogError($"表中不存在plot：{buttonStr}");
            return buttonStr;
        }
        else if (dataTemp.Length > 1)
        {
            Debug.LogError($"表中存在重名plot：{buttonStr}");
        }
        GameEntry.Course.GetTrainingFactory.UpdateScore(TrainingPartType.XuanJiao, dataTemp[0].Id,true);
        return buttonStr;
    }

}
