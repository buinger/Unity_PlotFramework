using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnePanelTalkPlot : TalkPlot
{
    //事件字典
   protected  Dictionary<int, SentencePlot> sentencePlotDic = new Dictionary<int, SentencePlot>();
    [Header("自动播放显示时间")]
    public float stayTime = 1f;
    [Header("自动空白间隔时间")]
    public float breakTime = 0;
    [Header("对话交互模式")]
    [SerializeField]
    private TalkMode talkMode = TalkMode.AutoAndManual;

    [Header("是否显示名字")]
    [SerializeField]
    private bool showName = true;

    [Header("对话配置")]
    public List<string> allSentence = new List<string>();
    [Header("事件绑定")]
    [SerializeField]
    protected List<SentencePlot> sentenceChildPlotInfo = new List<SentencePlot>();


    protected override void Ini(Action onIniOver)
    {
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
        onIniOver.Invoke();
    }

    protected override IEnumerator MainLogic()
    {
        if (allSentence.Count != 0)
        {
            for (int i = 0; i < allSentence.Count; i++)
            {
                GameObject sentenceUi = null;


                string item = allSentence[i];
                string[] aimText = item.Split('*');
                string sentenceOwnerName = "陌生人";
                if (aimText.Length == 2)
                {
                    sentenceOwnerName = aimText[0];
                }
                IEnumerator ie_GetUi = GetUiEntity(false, uiTemplate.path, GetAimPostion(sentenceOwnerName), transform);
                yield return StartCoroutine(ie_GetUi);
                sentenceUi = ie_GetUi.Current as GameObject;
                sentenceUi.transform.localScale = Vector3.one;



                TalkPanel talkPanel = sentenceUi.GetComponent<TalkPanel>();
                if (aimText.Length == 2)
                {
                    talkPanel.SetText(aimText[0], aimText[1], showName);
                }
                else if (aimText.Length == 1)
                {
                    talkPanel.SetText("陌生人", aimText[0], showName);
                }
                sentenceUi.SetActive(true);

                bool hasEvent = sentencePlotDic.ContainsKey(i);

                if (hasEvent && sentencePlotDic[i].childPlotModel_onAppear.plotModel != null)
                {
                    yield return StartCoroutine(StartSentenceBeforePlot(i));
                }

                bool ifManual = false;

                switch (talkMode)
                {
                    case TalkMode.Manual:
                        ifManual = true;
                        Button targetButton = sentenceUi.GetComponent<Button>();
                        bool waitClick = true;
                        targetButton.onClick.AddListener(()=> {
                            waitClick = false;
                        });

                        while (waitClick)
                        {
                            yield return null;
                        }

                        //while (true)
                        //{
                        //    yield return null;
                        //    if (Input.GetMouseButtonDown(0))
                        //    {
                        //        if (PlotUiManager.IsOnUi() == false || PlotUiManager.GetUi().gameObject == sentenceUi)
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        break;
                    case TalkMode.Auto:
                        yield return new WaitForSeconds(stayTime);
                        break;
                    case TalkMode.AutoAndManual:
                        float passTime = 0;

                        Button targetButton2 = sentenceUi.GetComponent<Button>();
                        bool waitClick2 = true;
                        targetButton2.onClick.AddListener(() => {
                            waitClick = false;
                            Debug.Log("触发了点击换句子");
                        });
                        while (waitClick2)
                        {
                            passTime += Time.deltaTime;
                            if (passTime >= stayTime)
                            {
                                break;
                            }
                            yield return null;
                        }


                        //while (true)
                        //{
                        //    passTime += Time.deltaTime;
                        //    yield return null;
                        //    if (passTime >= stayTime)
                        //    {
                        //        break;
                        //    }
                        //    else if (Input.GetMouseButtonDown(0))
                        //    {
                        //        if (PlotUiManager.IsOnUi() == false || PlotUiManager.GetUi().gameObject == sentenceUi.gameObject)
                        //        {
                        //            ifManual = true;
                        //            break;
                        //        }
                        //    }
                        //}
                        break;
                }

                if (hasEvent && sentencePlotDic[i].childPlotModel_onDisappear.plotModel != null)
                {
                    yield return StartCoroutine(StartSentenceOverPlot(i));
                }

                talkPanel.ClearContent();
                GameEntry.PlotPool.PutInPool(sentenceUi);

                if (ifManual == false)
                {
                    if (i != allSentence.Count - 1)
                    {
                        yield return new WaitForSeconds(breakTime);
                    }
                }
            }
        }
        yield return null;
    }


    protected virtual IEnumerator StartSentenceBeforePlot(int index) {

        yield return StartCoroutine(StartNewPlot(sentencePlotDic[index].childPlotModel_onAppear));

    }
    protected virtual IEnumerator StartSentenceOverPlot(int index)
    {

        yield return StartCoroutine(StartNewPlot(sentencePlotDic[index].childPlotModel_onDisappear));

    }


    private enum TalkMode
    {
        Manual,
        Auto,
        AutoAndManual,
    }



    [System.Serializable]
    protected class SentencePlot
    {
        public int sentenceIndex;
        public ChildPlotInformation childPlotModel_onAppear;
        public ChildPlotInformation childPlotModel_onDisappear;

        // 构造函数
        public SentencePlot(int index, ChildPlotInformation onAppear, ChildPlotInformation onDisappear)
        {
            sentenceIndex = index;
            childPlotModel_onAppear = onAppear;
            childPlotModel_onDisappear = onDisappear;
        }
    }
}
