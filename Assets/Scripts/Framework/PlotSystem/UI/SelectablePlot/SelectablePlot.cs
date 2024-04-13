using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePlot : BaseSelectablePlot
{

    [Header("选项配置")]
    [SerializeField]
    protected Selection selection = new Selection();
    protected GameObject targetPanel;


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        yield return StartCoroutine(SelectRoutine());
        ResetPlot();
    }

    protected override IEnumerator CreateAllUi()
    {
        //IEnumerator containerProduce = PrepareSelectPanel(selection.strList, true, transform, true);
        //yield return StartCoroutine(containerProduce);
        //targetPanel = containerProduce.Current as GameObject;

        Task<GameObject> makepanel = PrepareSelectPanel(selection.strList, true, true);
        while (makepanel.IsCompleted == false)
        {
            yield return null;
        }
        targetPanel = makepanel.Result;
    }

    protected override IEnumerator StartPlotBySelectionIndex(int index)
    {
        if (selection.choices[index].plotAfterChoose.plotModel != null)
        {
            targetPanel.SetActive(false);
            yield return StartNewPlot(selection.choices[index].plotAfterChoose);
            targetPanel.SetActive(true);
        }
    }

    protected override void ResetPlot()
    {
        selectionTemp.Clear();
        if (targetPanel != null)
        {
            List<Transform> allChild = TransformHelper.GetImmediateChildList(targetPanel.transform);
            for (int i = 0; i < allChild.Count; i++)
            {
                GameEntry.PlotPool.PutInPool(allChild[i].gameObject);
            }
            GameEntry.PlotPool.PutInPool(targetPanel);
            targetPanel = null;
        }
    }

   



}




public abstract class BaseSelectablePlot : UiPlot
{

    [Header("选择后是否消失")]
    public bool overAfterSelect = false;

    //[Header("容器显示位置，相对于此脚本所在位置")]
    //public Vector3 uiLocalPos = Vector3.zero;

    [Header("容器预制件")]
    [SerializeField]
    protected PrefabInfo containerTemplate;

    [Header("按钮预制件")]
    [SerializeField]
    protected PrefabInfo buttonTemplate;

    // public Action<float> onContentClick;



    protected List<GameObject> selectionTemp = new List<GameObject>();


  protected  Transform ContainerFather;
    protected override void Ini(Action onIniOver)
    {
        ContainerFather = TransformHelper.GetChild(transform, nameof(ContainerFather));
        // Debug.Log("---------------"+ContainerFather);
        if (ContainerFather == null)
        {
            ContainerFather = transform;
        }
        onIniOver.Invoke();
    }

    protected abstract IEnumerator CreateAllUi();
    protected abstract IEnumerator StartPlotBySelectionIndex(int index);

    /// <summary>
    /// 准备一个选择面板
    /// </summary>
    /// <param name="activeAfterLoaded"></param>
    /// <returns></returns>
    protected virtual async Task<GameObject> PrepareSelectPanel(List<string> selectWords, bool level, bool activeAfterLoaded)
    {
        //IEnumerator ie_getUi = GetUiEntity(false, containerTemplate.path, uiLocalPos, containerFather);
        //yield return StartCoroutine(ie_getUi);
        //GameObject container = ie_getUi.Current as GameObject;

        Debug.Log(ContainerFather);
        Task<GameObject> getUi = GetUiEntityAsync(false, containerTemplate.path, Vector3.zero, ContainerFather);
        await getUi;
        GameObject container = getUi.Result;



        container.transform.localScale = Vector3.one;
        container.transform.SetAsFirstSibling();
        //制作按钮
        foreach (var item in selectWords)
        {
            GameObject button = null;
            //IEnumerator ie_getUi2 = GetUiEntity(true, buttonTemplate.path, uiLocalPos, container.transform);
            //yield return StartCoroutine(ie_getUi2);
            //button = ie_getUi2.Current as GameObject;

            Task<GameObject> getUi2 = GetUiEntityAsync(true, buttonTemplate.path, Vector3.zero, container.transform);
            await getUi2;
            button = getUi2.Result;

            button.GetComponentInChildren<TMP_Text>().text = item;
            if (level)
            {
                selectionTemp.Add(button);
            }
        }
        //
        container.SetActive(activeAfterLoaded);
        return container;

    }

    protected virtual string DoContentClickEvent(GameObject item)
    {
        //TMP_Text text = item.GetComponentInChildren<TMP_Text>();
        //if (text==null)
        //{
        //    Debug.Log(item.name);
        //}
        string clickedContent = item.GetComponentInChildren<TMP_Text>().text;
        Debug.Log("点击了：" + clickedContent);
        return clickedContent;
        // onContentClick.Invoke(clickedContent);
    }

    protected virtual IEnumerator SelectRoutine()
    {
        GameObject checkObj = null;

        foreach (var item in selectionTemp)
        {
            Button tempButton = item.GetComponent<Button>();
            tempButton.onClick.RemoveAllListeners();
            tempButton.onClick.AddListener(()=> {
                checkObj = item;
            });

        }

       
        while (true)
        {
            if (checkObj != null)
            {
                int index = selectionTemp.IndexOf(checkObj);
                DoContentClickEvent(checkObj);
                yield return StartCoroutine(StartPlotBySelectionIndex(index));
                                        
                checkObj = null;

                if (overAfterSelect)
                {
                    break;
                }
                checkObj.transform.parent.gameObject.SetActive(true);
            }
            

            yield return null;
        }
    }



    protected class SelectionPanel
    {
        public int level;
        public string name;
        public GameObject container;
        public List<Transform> allElement = new List<Transform>();

        // 构造函数
        public SelectionPanel(int level, string name, GameObject container, List<Transform> allElement)
        {
            this.level = level;
            this.name = name;
            this.container = container;
            this.allElement = allElement;
        }
    }


    [System.Serializable]
    protected class Choice
    {
        public string word;
        public ChildPlotInformation plotAfterChoose;

        // 构造函数
        public Choice(string words, ChildPlotInformation plotAfterChoose)
        {
            this.word = words;
            this.plotAfterChoose = plotAfterChoose;
        }
    }

    [System.Serializable]
    protected class Selection
    {
        public List<Choice> choices = new List<Choice>();

        public List<string> strList
        {
            get
            {
                List<string> aimStrs = new List<string>();
                foreach (var item in choices)
                {
                    aimStrs.Add(item.word);
                }
                return aimStrs;
            }
        }

    }

    [System.Serializable]
    protected class Selection2
    {
        public string name;
        public List<Choice> choices = new List<Choice>();

        public List<string> strList
        {
            get
            {
                List<string> aimStrs = new List<string>();
                foreach (var item in choices)
                {
                    aimStrs.Add(item.word);
                }
                return aimStrs;
            }
        }
    }

    [System.Serializable]
    protected class Selection3
    {
        public string name;
        public List<Selection2> Level2Selections = new List<Selection2>();

        public Selection3(string name, List<Selection2> s2List)
        {
            this.name = name;
            Level2Selections = s2List;
        }
    }

}