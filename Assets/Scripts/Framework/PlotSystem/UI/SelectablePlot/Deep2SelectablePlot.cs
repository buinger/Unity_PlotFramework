using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deep2SelectablePlot : BaseSelectablePlot
{
    [Header("选项配置")]
    [SerializeField]
    List<Selection2> selections = new List<Selection2>();
    Dictionary<string, Choice> choicesDic = new Dictionary<string, Choice>();


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        yield return StartCoroutine(SelectRoutine());
        ResetPlot();
    }



    //显示按扭项目
    IEnumerator CreateOtherPages(Book book, List<Button> mainButtons)
    {
        if (mainButtons.Count == selections.Count)
        {
            for (int i = 0; i < mainButtons.Count; i++)
            {
                int aimPage = i + 2;
                mainButtons[i].onClick.AddListener(() =>
                {
                    book.ChangePageTo(aimPage);
                    returnButton.gameObject.SetActive(true);
                });

                //IEnumerator createChildPanel = PrepareSelectPanel(selections[i].strList, true, pageFather, false);

                //yield return StartCoroutine(createChildPanel);

                //GameObject newPanel = createChildPanel.Current as GameObject;

                Task<GameObject> makepanel = PrepareSelectPanel(selections[i].strList, true, false);
                while (makepanel.IsCompleted == false)
                {
                    yield return null;
                }
                GameObject newPanel  = makepanel.Result;


                book.pages.Add(newPanel);
            }
        }


    }




    protected override void ResetPlot()
    {
        returnButton.gameObject.SetActive(false);
        returnButton.onClick.RemoveAllListeners();

        selectionTemp.Clear();
        //Debug.Log(pageFather.childCount);
        List<Transform> recycleTrans = TransformHelper.GetImmediateChildList(pageFather);

        for (int i = 0; i < recycleTrans.Count; i++)
        {
            Transform tempChild = recycleTrans[i];
            List<Transform> recycleTrans2=TransformHelper.GetImmediateChildList(tempChild);
            for (int j = 0; j < recycleTrans2.Count; j++)
            {
                GameEntry.PlotPool.PutInPool(recycleTrans2[j].gameObject);
            }

            GameEntry.PlotPool.PutInPool(tempChild.gameObject);
        }
    }

    Transform pageFather;
    Button returnButton;



    protected override void Ini(Action onIniOver)
    {
        pageFather = transform.GetChild(0);
        returnButton = transform.GetComponentInChildren<Button>();
        returnButton.gameObject.SetActive(false);

        foreach (var item in selections)
        {
            foreach (var item2 in item.choices)
            {
                Choice aimChoice = item2;
                if (!choicesDic.ContainsKey(aimChoice.word))
                {
                    choicesDic.Add(aimChoice.word, aimChoice);
                }
                else
                {
                    Debug.LogError("有相同子项：" + aimChoice.word);
                }
            }
        }
        onIniOver.Invoke();
    }

   

    protected override IEnumerator CreateAllUi()
    {
        List<string> selectWords = new List<string>();
        foreach (var item in selections)
        {
            selectWords.Add(item.name);
        }
        //IEnumerator getPanel = PrepareSelectPanel(selectWords, false, pageFather, false);
        //yield return StartCoroutine(getPanel);
        //GameObject mainPagePanel = getPanel.Current as GameObject;

        Task<GameObject> makepanel = PrepareSelectPanel(selectWords, false, false);
        while (makepanel.IsCompleted == false)
        {
            yield return null;
        }
        GameObject mainPagePanel = makepanel.Result;


        Book myBook = gameObject.GetComponent<Book>();
        if (myBook == null)
        {
            myBook = gameObject.AddComponent<Book>();
        }
        myBook.pages.Clear();
        myBook.pages.Add(mainPagePanel);


        returnButton.onClick.AddListener(() =>
        {
            myBook.ChangePageTo(1);
            returnButton.gameObject.SetActive(false);
        });


        returnButton.gameObject.SetActive(false);

        List<Button> mainButtons = mainPagePanel.GetComponentsInChildren<Button>().ToList();

        yield return StartCoroutine(CreateOtherPages(myBook, mainButtons));


        mainPagePanel.SetActive(true);
    }

    protected override IEnumerator StartPlotBySelectionIndex(int index)
    {
        string key = selectionTemp[index].GetComponentInChildren<TMP_Text>().text;
        if (choicesDic[key].plotAfterChoose.plotModel != null)
        {
            selectionTemp[index].transform.parent.gameObject.SetActive(false);
            yield return StartNewPlot(choicesDic[key].plotAfterChoose);
        }
    }
}
