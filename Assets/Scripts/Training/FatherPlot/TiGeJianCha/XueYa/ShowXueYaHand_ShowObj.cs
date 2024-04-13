using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Kmax;

public class ShowXueYaHand_ShowObj : ShowAndCloseObjPlot
{

    

    public GameObject screen;
    public GameObject hand;

    public TextMesh gaoyaText;
    public TextMesh diyaText;
    public TextMesh xinlvText;

    int gaoyaValue;
    int diyaValue;
    int xinlvValue;

    protected override void Ini(Action onIniOver)
    {
        int targetInfoId = GameEntry.Course.GetTrainingFactory.targetGuestInfoId;
        DRGuestInfomation data = GameEntry.DataTable.GetDataTable<DRGuestInfomation>().GetDataRow(targetInfoId);

        gaoyaValue = 120;
        diyaValue = 70;
        xinlvValue = 70;

        string targetStr = data.BloodPressure;
        if (targetStr != "")
        {
            string[] temps = targetStr.Split('/');
            if (temps.Length == 3)
            {
                gaoyaValue = int.Parse(temps[0]);
                diyaValue = int.Parse(temps[1]);
                xinlvValue = int.Parse(temps[2]);
            }
            else
            {
                Debug.LogError("数据长度不对:" + targetStr);
            }
        }

        ResetPlot();
        base.Ini(onIniOver);

    }

    protected override void ResetPlot()
    {
        gaoyaText.text = "";
        diyaText.text = "";
        xinlvText.text = "";
        screen.SetActive(false);
        hand.SetActive(false);
        base.ResetPlot();
    }

    void SetAimValue()
    {
        gaoyaText.text = gaoyaValue.ToString();
        diyaText.text = diyaValue.ToString();
        xinlvText.text = xinlvValue.ToString();
    }
    protected override IEnumerator LogicBeforeClose()
    {
        hand.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        screen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SetAimValue();
        float cd = 2;
        float passTime = 0;
        while (passTime < cd)
        {
            passTime += Time.deltaTime;
            int minValue = Mathf.Clamp(diyaValue - 10, 0, diyaValue);
            diyaText.text = UnityEngine.Random.Range(minValue, diyaValue + 10).ToString();
            yield return null;
        }
        SetAimValue();
        yield return new WaitForSeconds(0.5f);
        hand.SetActive(false);     
    }
}
