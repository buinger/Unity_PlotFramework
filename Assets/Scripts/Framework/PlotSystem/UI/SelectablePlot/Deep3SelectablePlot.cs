using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deep3SelectablePlot : BaseSelectablePlot
{
    [Header("选项配置")]
    [SerializeField]
    List<Selection3> selections = new List<Selection3>();
    Dictionary<string, Choice> choicesDic = new Dictionary<string, Choice>();
    Dictionary<int, Dictionary<string, SelectionPanel>> allSelectionPanel = new Dictionary<int, Dictionary<string, SelectionPanel>>();


    Transform pageFather;
    Button returnButton;

    

    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        yield return StartCoroutine(SetAllButtonEvent());
        yield return StartCoroutine(SelectRoutine());
        ResetPlot();
    }



    void ReturnButtonSet(bool active, Action onClick)
    {
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => { onClick.Invoke(); });
        returnButton.gameObject.SetActive(active);
    }

    IEnumerator SetAllButtonEvent()
    {
        Book book1 = gameObject.AddComponent<Book>();
        Book book2 = gameObject.AddComponent<Book>();

        book1.pages.Add(allSelectionPanel[1][PlotName].container);

        List<Transform> tempElement = allSelectionPanel[1][PlotName].allElement;
        foreach (var item in tempElement)
        {
            string pageName = item.GetComponentInChildren<TMP_Text>().text;
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                book2.CloseAllPages();
                book1.ChangePageByGameobject(allSelectionPanel[2][pageName].container);

                ReturnButtonSet(true, () =>
                {
                    book1.ChangePageTo(1);
                    book2.CloseAllPages();
                    returnButton.gameObject.SetActive(false);
                });
            });
        }

        foreach (var item in allSelectionPanel[2])
        {
            book1.pages.Add(item.Value.container);
            tempElement = item.Value.allElement;
            foreach (var item2 in tempElement)
            {
                string pageName = item2.GetComponentInChildren<TMP_Text>().text;
                Button button = item2.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    book1.CloseAllPages();
                    book2.ChangePageByGameobject(allSelectionPanel[3][pageName].container);

                    ReturnButtonSet(true, () =>
                    {
                        book1.ChangePageByGameobject(allSelectionPanel[2][item.Value.name].container);
                        book2.CloseAllPages();

                        ReturnButtonSet(true, () =>
                        {
                            book1.ChangePageTo(1);
                            book2.CloseAllPages();
                            returnButton.gameObject.SetActive(false);
                        });
                    });
                });
            }
        }

        foreach (var item in allSelectionPanel[3])
        {
            book2.pages.Add(item.Value.container);
        }
        allSelectionPanel[1][PlotName].container.SetActive(true);
        yield return null;
    }

    protected override void ResetPlot()
    {
        allSelectionPanel.Clear();
        Book[] books = gameObject.GetComponents<Book>();
        if (books.Length != 0)
        {
            foreach (var item in books)
            {
                Destroy(item);
            }
        }
        returnButton.gameObject.SetActive(false);
        returnButton.onClick.RemoveAllListeners();

        selectionTemp.Clear();

        
        List<Transform> recycleTrans = TransformHelper.GetImmediateChildList(pageFather);

        for (int i = 0; i < recycleTrans.Count; i++)
        {
            Transform tempChild = recycleTrans[i];
            List<Transform> recycleTrans2 = TransformHelper.GetImmediateChildList(tempChild);
            for (int j = 0; j < recycleTrans2.Count; j++)
            {
                GameEntry.PlotPool.PutInPool(recycleTrans2[j].gameObject);
            }

            GameEntry.PlotPool.PutInPool(tempChild.gameObject);
        }
    }

    protected override void Ini(Action onIniOver)
    {
        pageFather = transform.GetChild(0);
        returnButton = transform.GetComponentInChildren<Button>();
        returnButton.gameObject.SetActive(false);
        foreach (var item in selections)
        {
            foreach (var item2 in item.Level2Selections)
            {
                foreach (var item3 in item2.choices)
                {
                    Choice aimChoice = item3;
                    if (!choicesDic.ContainsKey(aimChoice.word))
                    {
                        choicesDic.Add(aimChoice.word, aimChoice);
                    }
                    else
                    {
                        Debug.Log("有相同子项：" + aimChoice.word);
                    }
                    //预加载按钮
                    //GameObjectPoolManager.LoadPrefabToPoolAsync(buttonTemplate.path);
                }
            }
        }
        onIniOver.Invoke();
    }


    async Task<GameObject> SetOnePanel(string name, List<string> selectWords, bool isEventWords, int level)
    {

        //IEnumerator getPanel = PrepareSelectPanel(selectWords, isEventWords, pageFather, false);
        //yield return StartCoroutine(getPanel);
        //GameObject mainPagePanel = getPanel.Current as GameObject;

        Task<GameObject> makepanel = PrepareSelectPanel(selectWords, isEventWords, false);
        await makepanel;
        GameObject mainPagePanel = makepanel.Result;

        List<Transform> mainElement = TransformHelper.GetImmediateChildList(mainPagePanel.transform);

        SelectionPanel buttonPanel = new SelectionPanel(level, name, mainPagePanel, mainElement);

        if (!allSelectionPanel.ContainsKey(level))
        {
            allSelectionPanel.Add(level, new Dictionary<string, SelectionPanel>());
        }
        if (!allSelectionPanel[level].ContainsKey(name))
        {
            allSelectionPanel[level].Add(name, buttonPanel);
        }

         return mainPagePanel;
    }


    protected override IEnumerator CreateAllUi()
    {
        //先生成0号层级
        List<string> selectWords = new List<string>();
        foreach (var item in selections)
        {
            selectWords.Add(item.name);
        }

        //IEnumerator createMainPage = SetOneButtonPanel(plotName, selectWords, false, 1);
        //yield return StartCoroutine(createMainPage);
        //GameObject page0 = createMainPage.Current as GameObject;

        Task<GameObject> createMainPage=SetOnePanel(PlotName, selectWords, false, 1);
        while (createMainPage.IsCompleted==false)
        {
            yield return null;
        }
        GameObject page0 = createMainPage.Result;


        foreach (var item in selections)
        {
            selectWords.Clear();
            foreach (var item2 in item.Level2Selections)
            {
                selectWords.Add(item2.name);
                //IEnumerator getPanel3 = SetOneButtonPanel(item2.name, item2.strList, true, 3);
                //yield return StartCoroutine(getPanel3);
                Task<GameObject> getPanel3 = SetOnePanel(item2.name, item2.strList, true, 3);
                while (getPanel3.IsCompleted == false)
                {
                    yield return null;
                }
       
            }
            //IEnumerator getPanel2 = SetOneButtonPanel(item.name, selectWords, false, 2);
            //yield return StartCoroutine(getPanel2);

            Task<GameObject> getPanel2 = SetOnePanel(item.name, selectWords, false, 2);
            while (getPanel2.IsCompleted == false)
            {
                yield return null;
            }
        }

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
