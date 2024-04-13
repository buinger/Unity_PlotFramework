using Training;
using UnityEngine;
using Kmax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections;

public class JieZhen_Selection : SelectablePlot
{
    [Header("接诊选项id")]
    public int jieZhenSelectionID = -1;
    DRCommonJieZhen tableData;
    DRCourseJieZhen tableDataCourse;

    protected override string DoContentClickEvent(GameObject item)
    {
        string clickedContent = base.DoContentClickEvent(item);
        GameEntry.Course.GetTrainingFactory.UpdateScore(TrainingPartType.JieZhen, jieZhenSelectionID, true, clickedContent);
        return clickedContent;
    }

    protected override IEnumerator MainLogic()
    {
        if (jieZhenSelectionID <= 0)
        {
            Debug.Log(PlotName + ":等待id设置");
            yield return null;
        }
        //初始化选项
        selection = new Selection();

        tableData = GameEntry.DataTable.GetDataTable<DRCommonJieZhen>().GetDataRow(jieZhenSelectionID);
        tableDataCourse = GameEntry.DataTable.GetDataTable<DRCourseJieZhen>().GetDataRow(jieZhenSelectionID);

        //这里以公表的选择为准
        List<string> choiceContents = tableData.AllChoice.Split('|').ToList();
        foreach (var item in choiceContents)
        {
            selection.choices.Add(new Choice(item, new ChildPlotInformation()));
        }


        if (tableDataCourse.OverPlot != "")
        {
            Task<Plot> plotTemp = TrainingFactory.GetPlotTempByTableStr(tableDataCourse.OverPlot);

            while (plotTemp.IsCompleted == false)
            {
                yield return null;
            }

            overPlot = new ChildPlotInformation(true, plotTemp.Result);
        }
        else
        {
            overPlot = new ChildPlotInformation();
        }

        yield return StartCoroutine(base.MainLogic());
    }


    protected override IEnumerator StartPlotBySelectionIndex(int index)
    {
        if (selection.choices[index].plotAfterChoose.plotModel != null)
        {
            targetPanel.SetActive(false);
            yield return StartNewPlot(selection.choices[index].plotAfterChoose);
        }
    }

    protected override IEnumerator StartEndPlot()
    {
        yield return StartNewPlot(overPlot, TrainingFactory.GetPlotInstActionByTableStr(tableDataCourse.OverPlot));
    }
    private void OnDisable()
    {
        jieZhenSelectionID = -1;
    }
}
