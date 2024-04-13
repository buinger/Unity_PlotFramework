using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Deep3TogglePlot : BaseSelectablePlot
{
    [Header("toogle预制件")]
    public PrefabInfo toggleTemplate;

    [Header("选项配置")]
    [SerializeField]
    protected List<Selection3> selections = new List<Selection3>();

    //最终选项键值对
    Dictionary<string, Choice> choicesDic = new Dictionary<string, Choice>();

    //所有toggle和它的键对
    protected Dictionary<GameObject, string> childToggles = new Dictionary<GameObject, string>();
    //所有页面
    protected Dictionary<int, Dictionary<string, SelectionPanel>> allSelectionPanel = new Dictionary<int, Dictionary<string, SelectionPanel>>();

    protected Dictionary<string, bool> toggleHistory = new Dictionary<string, bool>();


    Transform pageFather;
    Button returnButton;
    protected Button overButton;




    //[ContextMenu("测试赋值")]
    //void Test()
    //{
    //    for (int i = 0; i < 6; i++)
    //    {
    //        Selection3 xx = new Selection3();
    //        xx.name = "一级选项" + i;
    //        xx.Level2Selections = new List<Selection2>();
    //        for (int j = 0; j < 9; j++)
    //        {
    //            Selection2 yy = new Selection2();
    //            yy.name = "二级选项" + j;
    //            yy.choices = new List<Choice>();
    //            for (int k = 0; k < 10; k++)
    //            {
    //                Choice zz = new Choice();
    //                zz.words = "三级选项" + k;
    //                yy.choices.Add(zz);
    //            }
    //            xx.Level2Selections.Add(yy);
    //        }

    //        selections.Add(xx);
    //    }

    //}

    protected override IEnumerator SelectRoutine()
    {
        //当结束按钮按下时，退出循环
        GameObject checkObj = null;
        overButton.onClick.RemoveAllListeners();
        overButton.onClick.AddListener(() =>
        {
            checkObj = overButton.gameObject;
        });

        while (checkObj == null)
        {
            yield return null;
        }
        Debug.Log("点中");
        SetHistory();
    }
    //protected UnityAction<List<float>> onConfirm;
    protected virtual void SetHistory()
    {
        List<string> selectedKey = new List<string>();
        foreach (var item in childToggles)
        {
            Toggle aimToggle = item.Key.GetComponent<Toggle>();
            bool targetData = aimToggle.isOn;
            string keyName = item.Value;
            if (targetData)
            {
                selectedKey.Add(keyName);
            }
            if (toggleHistory.ContainsKey(keyName))
            {
                toggleHistory[keyName] = targetData;
            }
            else
            {
                toggleHistory.Add(keyName, targetData);
            }
        }

        //toggleHistory
    }

    protected virtual void OnSetHistory(List<string> historyString)
    {
        Debug.Log($"勾选了{historyString.Count}个元素");
    }


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        yield return StartCoroutine(SetAllButtonEvent());
        overButton.gameObject.SetActive(true);
        allSelectionPanel[1][PlotName].container.SetActive(true);
        yield return StartCoroutine(SelectRoutine());
        ResetPlot();
    }

    void ReturnButtonSet(bool active, Action onClick)
    {
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => { onClick.Invoke(); });
        returnButton.gameObject.SetActive(active);
    }

    protected virtual IEnumerator SetAllButtonEvent()
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
                    Debug.Log(pageName);
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

        yield return null;
    }


    protected override void ResetPlot()
    {
        allSelectionPanel.Clear();
        childToggles.Clear();
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

        overButton.gameObject.SetActive(false);
        overButton.onClick.RemoveAllListeners();

        selectionTemp.Clear();

        //for (int i = 0; i < pageFather.childCount; i++)
        //{
        //    Transform tempChild = pageFather.GetChild(i);
        //    for (int j = 0; j < tempChild.childCount; j++)
        //    {
        //        GameEntry.PrefabPool.PutInPool(tempChild.GetChild(j).gameObject);
        //    }
        //    GameEntry.PrefabPool.PutInPool(tempChild.gameObject);
        //}

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


    protected void CustomIni()
    {
        Transform firstFather = transform.GetChild(0);
        pageFather = firstFather.GetChild(0);
        returnButton = firstFather.GetChild(1).GetComponent<Button>();
        returnButton.gameObject.SetActive(false);

        overButton = firstFather.GetChild(2).GetComponent<Button>();
        overButton.gameObject.SetActive(false);

        foreach (var item in selections)
        {
            foreach (var item2 in item.Level2Selections)
            {
                foreach (var item3 in item2.choices)
                {
                    Choice aimChoice = item3;
                    string key = item.name + "*" + item2.name + "*" + aimChoice.word;
                    if (!choicesDic.ContainsKey(key))
                    {
                        choicesDic.Add(key, aimChoice);
                    }
                    else
                    {
                        Debug.LogError(PlotName + "有相同项：" + key);
                    }
                }

            }
        }
    }
    protected override void Ini(Action onIniOver)
    {
        CustomIni();
        onIniOver.Invoke();
    }




    protected override async Task<GameObject> PrepareSelectPanel(List<string> selectWords, bool isEventWords, bool activeAfterLoaded)
    {
        //IEnumerator ie_getUi = GetUiEntity(false, containerTemplate.path, uiLocalPos, containerFather);
        //yield return StartCoroutine(ie_getUi);
        //GameObject container = ie_getUi.Current as GameObject;

        Task<GameObject> getUi = GetUiEntityAsync(false, containerTemplate.path, Vector3.zero, ContainerFather);
        await getUi;
        GameObject container = getUi.Result;

        container.transform.localScale = Vector3.one;
        //制作按钮
        foreach (var item in selectWords)
        {

            GameObject element = null;
            //IEnumerator ie_getUi2 = GetUiEntity(true, buttonTemplate.path, uiLocalPos, container.transform);
            //yield return StartCoroutine(ie_getUi2);
            //button = ie_getUi2.Current as GameObject;
            Task<GameObject> getUi2;

            string keyName = thirdElementHead + item;
            if (isEventWords)
            {
                getUi2 = GetUiEntityAsync(true, toggleTemplate.path, Vector3.zero, container.transform);
                await getUi2;
                element = getUi2.Result;
                Toggle toggle = element.GetComponent<Toggle>();
                if (toggleHistory.ContainsKey(keyName))
                {
                    toggle.isOn = toggleHistory[keyName];
                }
                else
                {
                    toggle.isOn = false;
                }
            }
            else
            {
                getUi2 = GetUiEntityAsync(true, buttonTemplate.path, Vector3.zero, container.transform);
                await getUi2;
                element = getUi2.Result;
            }



            if (isEventWords)
            {
                TMP_Text targetText = element.GetComponentInChildren<TMP_Text>();
                targetText.text = item;
                selectionTemp.Add(element);
                childToggles.Add(element, keyName);
            }
            else
            {
                TMP_Text targetText = element.GetComponentInChildren<TMP_Text>();
                targetText.text = item;
            }
        }
        //
        container.SetActive(activeAfterLoaded);
        return container;
    }

    async Task<GameObject> SetOnePanel(string name, List<string> selectWords, int level)
    {

        //IEnumerator getPanel = PrepareSelectPanel(selectWords, isEventWords, pageFather, false);
        //yield return StartCoroutine(getPanel);
        //GameObject mainPagePanel = getPanel.Current as GameObject;

        Task<GameObject> makepanel = PrepareSelectPanel(selectWords, level == 3, false);
        await makepanel;
        GameObject mainPagePanel = makepanel.Result;

        List<Transform> mainElement = TransformHelper.GetImmediateChildList(mainPagePanel.transform);

        SelectionPanel buttonPanel = new SelectionPanel(level, name, mainPagePanel, mainElement);

        //if (name == PlotName)
        //{
        //    mainPagePanel.name = "!!!!!";
        //    foreach (var item in selectWords)
        //    {

        //    Debug.Log("---------------"+ item);

        //    }
        //    //allSelectionPanel[1][PlotName]
        //}


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

    string thirdElementHead = "";
    protected override IEnumerator CreateAllUi()
    {
        //先生成0号层级
        List<string> selectWords = new List<string>();
        foreach (var item in selections)
        {
            selectWords.Add(item.name);
        }

        Task<GameObject> createMainPage = SetOnePanel(PlotName, selectWords, 1);
        while (createMainPage.IsCompleted == false)
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
                thirdElementHead = item.name + "*" + item2.name + "*";

                Task<GameObject> getPanel3 = SetOnePanel(item2.name, item2.strList, 3);
                while (getPanel3.IsCompleted == false)
                {
                    yield return null;
                }

                thirdElementHead = "";
            }
            Task<GameObject> getPanel2 = SetOnePanel(item.name, selectWords, 2);
            while (getPanel2.IsCompleted == false)
            {
                yield return null;
            }
        }
    }


    protected override IEnumerator StartPlotBySelectionIndex(int index)
    {
        //由于时toggle不执行点击事件
        yield return null;
    }

}
