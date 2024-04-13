using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kmax;
using UnityEngine.UI;

public class ShowSheTouImage_ShowUi : ShowUiPlot
{    
    Button bkgButton;
    public Sprite[] allSprite;
    Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

    protected override void Ini(Action onIniOver)
    {
        GetContainer();
        bkgButton = uiContainer.GetComponent<Button>();       
        overButton.image.sprite = null;
        foreach (var item in allSprite)
        {
            if (!spriteDic.ContainsKey(item.name))
            {
                spriteDic.Add(item.name, item);
            }
        }
        DRGuestInfomation aimInfo = GameEntry.DataTable.GetDataTable<DRGuestInfomation>().GetDataRow(GameEntry.Course.GetTrainingFactory.targetGuestInfoId);
        if (aimInfo.SheZhenPic != null)
        {
            if (spriteDic.ContainsKey(aimInfo.SheZhenPic))
            {
                overButton.image.sprite = spriteDic[aimInfo.SheZhenPic];
            }
            else
            {
                Debug.LogError("²»´æÔÚ´ËÍ¼:" + aimInfo.SheZhenPic);
            }
        }
        //RectTransform imageRect= overButton.transform.GetComponent<RectTransform>();
        //imageRect.sizeDelta = new Vector2(sheZhenUi.image.sprite.rect.width, sheZhenUi.image.sprite.rect.height);
        uiContainer.gameObject.SetActive(false);
        onIniOver.Invoke();
    }

    protected override void AddOverEventToButton(Action setOverFlag)
    {
        base.AddOverEventToButton(setOverFlag);
        bkgButton.onClick.AddListener(()=> { setOverFlag.Invoke(); });
    }


    protected override void ResetPlot()
    {
        base.ResetPlot();
        bkgButton.onClick.RemoveAllListeners();
    }
}
