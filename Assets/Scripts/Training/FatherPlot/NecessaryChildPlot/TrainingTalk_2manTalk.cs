using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kmax;
using Training;
using System.Threading.Tasks;

public class TrainingTalk_2manTalk : TwoManTalkPlot
{
    Dictionary<int, string> scentenceChildPlotInfo_TableString = new Dictionary<int, string>();
    DRTalkData tableData;
    
    public int id = -1;
    protected override void Ini(Action onIniOver)
    {
        onIniOver.Invoke();
    }

    protected override IEnumerator MainLogic()
    {
        while (id == -1)
        {
            Debug.Log(PlotName + "µÈ´ýid¸³Öµ");
            yield return null;
        }

        allSentence.Clear();
        scentenceChildPlotInfo_TableString.Clear();
        sentenceChildPlotInfo.Clear();
        sentencePlotDic.Clear();

        tableData = GameEntry.DataTable.GetDataTable<DRTalkData>().GetDataRow(id);
        if (tableData.TalkContent != "")
        {
            string[] sentenceStrs = tableData.TalkContent.Split('|');

            foreach (var item in sentenceStrs)
            {
                string target = item.Trim();
                allSentence.Add(target);
            }
            mainPersonName = tableData.FirstPersonName;
        }

        if (tableData.SentencePlots != null)
        {
            string[] childPlotStrs = tableData.SentencePlots.Split('|');
            foreach (var item in childPlotStrs)
            {
                if (item != "")
                {
                    string[] tempStrs = item.Split('-');
                    int index = int.Parse(tempStrs[0]);
                    if (!scentenceChildPlotInfo_TableString.ContainsKey(index))
                    {
                        scentenceChildPlotInfo_TableString.Add(index, tempStrs[1]);
                    }
                }
            }
            foreach (var item in scentenceChildPlotInfo_TableString)
            {
                Task<Plot> getModelPlot = TrainingFactory.GetPlotTempByTableStr(item.Value);
                while (getModelPlot.IsCompleted == false) yield return null;

                SentencePlot sentencePlot = new SentencePlot(item.Key, new ChildPlotInformation(), new ChildPlotInformation(true, getModelPlot.Result));
                sentenceChildPlotInfo.Add(sentencePlot);
            }

        }

        if (tableData.OverPlot != "")
        {
            Task<Plot> getEndPlotTemp = TrainingFactory.GetPlotTempByTableStr(tableData.OverPlot);
            while (getEndPlotTemp.IsCompleted == false)
            {
                yield return null;
            }
            overPlot = new ChildPlotInformation(true, getEndPlotTemp.Result);
        }
        else
        {
            overPlot = new ChildPlotInformation();
        }



        for (int i = 0; i < sentenceChildPlotInfo.Count; i++)
        {
            SentencePlot item = sentenceChildPlotInfo[i];
            if (item.sentenceIndex <= allSentence.Count - 1 && item.sentenceIndex >= 0)
            {
                if (!sentencePlotDic.ContainsKey(item.sentenceIndex))
                {
                    sentencePlotDic.Add(item.sentenceIndex, item);
                }
            }
        }
        CheckUi();

        yield return StartCoroutine(base.MainLogic());
    }


    protected override IEnumerator StartSentenceOverPlot(int index)
    {
        Action<Plot> onPlotInstTemp = TrainingFactory.GetPlotInstActionByTableStr(scentenceChildPlotInfo_TableString[index]);

        yield return StartCoroutine(StartNewPlot(sentencePlotDic[index].childPlotModel_onDisappear, onPlotInstTemp));
    }

    protected override IEnumerator StartEndPlot()
    {
        yield return StartNewPlot(overPlot, TrainingFactory.GetPlotInstActionByTableStr(tableData.OverPlot));
    }

    private void OnDisable()
    {
        id = -1;
    }
}
