using System;
using System.Collections;
using System.Threading.Tasks;
using Training;
using UnityEngine;
using Kmax;
using UnityEngine.Playables;

public class Training_Timeline : TimelinePlot
{
    public int id = -1;
    Animator animator;

    private void OnDisable()
    {
        id = -1;
    }
    protected override void Ini(Action onIniOver)
    {
        onIniOver.Invoke();
    }

    DRAnimation tableData;
    protected override IEnumerator MainLogic()
    {
        while (id == -1)
        {
            Debug.Log(PlotName + "等待id赋值");
            yield return null;
        }
        yield return StartCoroutine(SetByTableData());
        yield return StartCoroutine(base.MainLogic());
    }

    IEnumerator SetByTableData()
    {
        tableData = GameEntry.DataTable.GetDataTable<DRAnimation>().GetDataRow(id);
        if (tableData.OverPlot != "")
        {
            Task<Plot> plotTemp = TrainingFactory.GetPlotTempByTableStr(tableData.OverPlot);

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

        isSole = tableData.IsSole;
        prefabPath = tableData.PrefabPath;
        playableAssetPath = tableData.AnimationPath;
        wrapMode=(DirectorWrapMode)tableData.PlayMode;


        //Task<Plot> getOverPlot = TrainingFactory.GetPlotTempByTableStr(tableData.OverPlot);
        //while (getOverPlot.IsCompleted==false)
        //{
        //    yield return null;
        //}
        //overPlot = new ChildPlotInformation(true,getOverPlot.Result);
    }



    
    protected override void SetTrans()
    {
        GameObject temp = PlotPoolManager.PlotPool.loadedPrefabs[prefabPath];

        if (tableData.InitPos != "")
        {
            playPosition = TrainingFactory.ParseCoordinates(tableData.InitPos);
        }
        else
        {
            playPosition = temp.transform.position;
        }
        if (tableData.InitAngle != "")
        {
            playEuler = TrainingFactory.ParseCoordinates(tableData.InitAngle);
        }
        else
        {
            playEuler = temp.transform.eulerAngles;
        }
        if (tableData.InitScale != "")
        {
            playScale = TrainingFactory.ParseCoordinates(tableData.InitScale);
        }
        else
        {
            playScale = temp.transform.localScale;
        }

        base.SetTrans();
    }

   

    protected override IEnumerator StartEndPlot()
    {
        yield return StartNewPlot(overPlot, TrainingFactory.GetPlotInstActionByTableStr(tableData.OverPlot));
    }

}
