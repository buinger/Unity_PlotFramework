using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Training;
using Kmax;
using System;

public class FuZhuJianCha_3Toggle : Deep3TogglePlot
{
    public GameObject mainContent;
    public GameObject selectModule;
    public Dictionary<string, Toggle> selectModule_Toggles = new Dictionary<string, Toggle>();
    Button selectModule_ConfirmButton;
    Transform selectModule_Container;
    Dictionary<string, GameObject> level1Buttons = new Dictionary<string, GameObject>();
    IDataTable<DRCommonFuZhuJianCha> table;


    protected override void ResetPlot()
    {
        base.ResetPlot();
        selectModule.SetActive(false);
        level1Buttons.Clear();
        selectModule.SetActive(false);
        mainContent.SetActive(false);
    }


    //获取所有一级Button，并全部隐藏
    void Get1LevelButtons()
    {
        List<Transform> level1ButtonsTrans = allSelectionPanel[1][PlotName].allElement;
        foreach (var item in level1ButtonsTrans)
        {
            string name = item.GetComponentInChildren<TMP_Text>().text;
            level1Buttons.Add(name, item.gameObject);
            item.gameObject.SetActive(false);
        }
    }

    //初始化选择模块
    IEnumerator SetSelectModule()
    {
        if (selectModule_Toggles.Count == 0)
        {
            foreach (var item in level1Buttons)
            {
                Task<GameObject> getToggle = GetUiEntityAsync(true, toggleTemplate.path, Vector3.zero, selectModule_Container);
                while (getToggle.IsCompleted == false)
                {
                    yield return null;
                }
                getToggle.Result.GetComponentInChildren<TMP_Text>().text = item.Key;
                selectModule_Toggles.Add(item.Key, getToggle.Result.GetComponent<Toggle>());
            }
        }
        selectModule.SetActive(true);

    }

    //根据勾选模块，刷新一级按钮的显示
    void Update1LevelButtons()
    {
        foreach (var item in selectModule_Toggles)
        {
            if (item.Value.isOn == true)
            {
                if (level1Buttons.ContainsKey(item.Key))
                {
                    level1Buttons[item.Key].SetActive(true);
                }
            }
        }
    }


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        //ResetPlot();
        //yield return StartCoroutine(CreateAllUi());
        Get1LevelButtons();
        yield return StartCoroutine(SetSelectModule());
        yield return StartCoroutine(SetAllButtonEvent());
        //阻塞-监听选择页面关闭
        selectModule.SetActive(true);
        while (selectModule.activeSelf == true)
        {
            yield return null;
        }

        //Debug.Log("3级数量:" + allSelectionPanel[3].Count);
        //Debug.Log("2级数量:" + allSelectionPanel[2].Count);
        //Debug.Log("1级数量:" + allSelectionPanel[1].Count);
        //foreach (var item in allSelectionPanel[2])
        //{
        //    Debug.Log(item.Key);
        //}


        overButton.gameObject.SetActive(true);
        allSelectionPanel[1][PlotName].container.SetActive(true);
        yield return StartCoroutine(SelectRoutine());

        ResetPlot();
    }



    protected override void SetHistory()
    {
        List<string> selectedKey = new List<string>();
        foreach (var item in childToggles)
        {
            Toggle aimToggle = item.Key.GetComponent<Toggle>();
            bool targetData = aimToggle.isOn;
            string keyName = item.Value;

            if (targetData)
            {
                string[] titleName = keyName.Split('*');
                if (selectModule_Toggles.ContainsKey(titleName[0]))
                {
                    selectedKey.Add(keyName);
                }
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


    }

    protected override void OnSetHistory(List<string> historyString)
    {
        base.OnSetHistory(historyString);
        List<int> allId = new List<int>();
        foreach (var item in historyString)
        {
            string[] strs = item.Split('*');
            if (strs.Length != 3)
            {
                Debug.LogError($"字符串不正确：{item}");
                return;
            }
            DRCommonFuZhuJianCha[] dataTemp = table.GetDataRows(p => p.LevelName1 == strs[0] && p.LevelName2 == strs[1] && p.LevelName3 == strs[2]);
            if (dataTemp.Length <= 0)
            {
                Debug.LogError($"表内不存在此元素：{item}");
            }
            else
            {
                allId.Add(dataTemp[0].Id);
            }
        }
        //GameEntry.Course.GetTrainingFactory.fuZhuJianCha.CheckSelection(allId);
    }


    void SetTable()
    {
        table = GameEntry.DataTable.GetDataTable<DRCommonFuZhuJianCha>();
        DRCommonFuZhuJianCha[] data = table.GetAllDataRows();

        selections.Clear();
        List<string> str1 = new List<string>();
        foreach (var item in data)
        {
            if (!str1.Contains(item.LevelName1))
            {
                str1.Add(item.LevelName1);
                List<Selection2> s2tempList = new List<Selection2>();
                DRCommonFuZhuJianCha[] data2 = table.GetDataRows(p => p.LevelName1 == item.LevelName1);
                List<string> str2 = new List<string>();

                foreach (var item2 in data2)
                {
                    if (!str2.Contains(item2.LevelName2))
                    {
                        Selection2 s2Temp = new Selection2();
                        s2Temp.name = item2.LevelName2;
                        List<Choice> choiceTempList = new List<Choice>();
                        //choiceTempList.Add

                        DRCommonFuZhuJianCha[] data3 = table.GetDataRows(p => p.LevelName1 == item.LevelName1 && p.LevelName2 == item2.LevelName2);
                        foreach (var item3 in data3)
                        {
                            Choice choiceTemp = new Choice(item3.LevelName3, new ChildPlotInformation());
                            choiceTempList.Add(choiceTemp);
                        }

                        s2Temp.choices = choiceTempList;

                        s2tempList.Add(s2Temp);

                        str2.Add(item2.LevelName2);
                    }
                }

                Selection3 s3Temp = new Selection3(item.LevelName1, s2tempList);
                selections.Add(s3Temp);
            }
        }
    }

    protected override void Ini(Action onIniOver)
    {
        CustomIni();
        SetTable();
        //初始化容器和按钮
        selectModule_Container = TransformHelper.GetChild(selectModule.transform, "一级勾选容器");
        selectModule_ConfirmButton = TransformHelper.GetChild(selectModule.transform, "确定按钮").GetComponent<Button>();

        selectModule_ConfirmButton.onClick.AddListener(() =>
        {
            Update1LevelButtons();
            selectModule.SetActive(false);
            mainContent.SetActive(true);
        });
        selectModule.SetActive(false);
        mainContent.SetActive(false);

        onIniOver();
    }

}
