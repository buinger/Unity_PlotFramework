using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Kmax;
using Training;

public class JieZhen_LinearPlot : LinearPlot<JieZhen_Selection>
{
    public static GameObject gameObjModel;


    public IDataTable<DRCommonJieZhen> table;

    Dictionary<int, int> plotsId = new Dictionary<int, int>();


    protected async override void Ini(Action onIniOver)
    {
        table = GameEntry.DataTable.GetDataTable<DRCommonJieZhen>();

        if (gameObjModel == null)
        {
            Task<GameObject> modelTask = PlotPoolManager.PlotPool.LoadGameObjectAsync("Assets/HotResources/Plot/TrainingPlot/Common/SelectTalk.prefab");
            await modelTask;
            gameObjModel = modelTask.Result;
        }

        //通过表plotModel赋值
        plotModel.Clear();
        DRCommonJieZhen[] dates = table.GetAllDataRows();
        JieZhen_Selection temp = gameObjModel.GetComponent<JieZhen_Selection>();

        for (int i = 0; i < dates.Length; i++)
        {
            DRCommonJieZhen item = dates[i];
            plotModel.Add(temp);
            plotsId.Add(i, item.Id);
        }

        base.Ini(onIniOver);
    }



    protected override IEnumerator MainLogic()
    {
        string plotStr = "2*1";
        Task<PlotActionPair> getPlotInfo = TrainingFactory.GetPlotActionPair(plotStr);
        while (getPlotInfo.IsCompleted == false) yield return null;

        Debug.Log(getPlotInfo.Result.plotInfo.plotModel.name+"----------------");
        yield return StartCoroutine(StartNewPlot(getPlotInfo.Result.plotInfo, getPlotInfo.Result.onPlotInst));

        for (int i = 0; i < childPlotModels.Count; i++)
        {
            int index = i;

            yield return StartNewPlot(childPlotModels[index], (target) =>
            {
                JieZhen_Selection plot = target as JieZhen_Selection;
                plot.jieZhenSelectionID = plotsId[index];
            });
        }
    }
}
