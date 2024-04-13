using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowUiPlot : UiPlot
{
    protected Transform uiContainer;
    public Button overButton;

    protected override void Ini(Action onIniOver)
    {
        GetContainer();
        onIniOver.Invoke();
    }

    protected void GetContainer()
    {
        uiContainer = transform.GetChild(0);
        uiContainer.gameObject.SetActive(false);
    }

    protected override IEnumerator MainLogic()
    {
        uiContainer.gameObject.SetActive(true);
        bool clickedOverButton = false;
        AddOverEventToButton(() => { clickedOverButton = true; });
        yield return new WaitUntil(() => clickedOverButton == true);
        uiContainer.gameObject.SetActive(false);
    }
    protected virtual void AddOverEventToButton(Action setOverFlag)
    {
        overButton.onClick.AddListener(() =>
        {
            setOverFlag.Invoke();
        });
    }

    protected override void ResetPlot()
    {
        uiContainer.gameObject.SetActive(false);
        overButton.onClick.RemoveAllListeners();
    }
}


public abstract class ShowTextDataPlot : ShowUiPlot
{
    public TMP_Text infoText;

    protected override void Ini(Action onIniOver)
    {
        GetContainer();      
        infoText.text = GetInfoText();
        onIniOver.Invoke();
    }


    protected abstract string GetInfoText();

}