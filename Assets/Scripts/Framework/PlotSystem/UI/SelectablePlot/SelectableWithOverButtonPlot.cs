using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SelectableWithOverButtonPlot : SelectablePlot
{
    protected GameObject overButton;
    protected GameObject SeletionUI;

    protected override void Ini(Action onIniOver)
    {
        SeletionUI = TransformHelper.GetChild(transform, "SeletionUI").gameObject;
        overButton = TransformHelper.GetChild(SeletionUI.transform, "OverButton").gameObject;
        SeletionUI.SetActive(false);
        base.Ini(onIniOver);
    }

    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(CreateAllUi());
        SeletionUI.SetActive(true);
        SeletionUI.transform.SetAsLastSibling();
        yield return StartCoroutine(SelectRoutine());
        ResetPlot();
    }

    protected override void ResetPlot()
    {
        base.ResetPlot();
        SeletionUI.SetActive(false);
        overButton.SetActive(true);
    }
    protected override IEnumerator SelectRoutine()
    {
    
        GameObject clickedButton = null;
        Button overBut = overButton.GetComponent<Button>();
        overBut.onClick.RemoveAllListeners();
        overBut.onClick.AddListener(() =>
        {
            clickedButton = overButton;
        });
        foreach (var item in selectionTemp)
        {
            Button button = item.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                clickedButton = item;
            });
        }

        while (true)
        {
            if (clickedButton != null)
            {
                if (clickedButton == overButton)
                {
                    break;
                }
                else
                {
                    //不等待子Plot走完再走点击事件                                        
                    DoContentClickEvent(clickedButton);
                    //--------------------------
                    int index = selectionTemp.IndexOf(clickedButton);
                   
                    yield return StartCoroutine(StartPlotBySelectionIndex(index));

                    if (overAfterSelect)
                    {
                        break;
                    }
                    clickedButton = null;
                }

            }
            yield return null;
        }

    }

    protected override IEnumerator StartPlotBySelectionIndex(int index)
    {
        if (selection.choices[index].plotAfterChoose.plotModel != null)
        {
            overButton.SetActive(false);
            targetPanel.SetActive(false);
            yield return StartNewPlot(selection.choices[index].plotAfterChoose);
            overButton.SetActive(true);
            targetPanel.SetActive(true);
        }
    }

}
