using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Training;
using System.Threading.Tasks;
using System;
using Kmax;

public class BianBingBianZheng_Ui : UiPlot
{
    GameObject tanChuang;
    List<GameObject> buttons = new List<GameObject>();


    protected override void Ini(Action onIniOver)
    {
        tanChuang = transform.GetChild(0).gameObject;
        Button[] temps = transform.GetComponentsInChildren<Button>();
        foreach (var item in temps)
        {
            buttons.Add(item.gameObject);
        }

        tanChuang.SetActive(false);
        onIniOver.Invoke();
    }


    //protected override void Ini()
    //{
    //    tanChuang = transform.GetChild(0).gameObject;
    //    Button[] temps = transform.GetComponentsInChildren<Button>();
    //    foreach (var item in temps)
    //    {
    //        buttons.Add(item.gameObject);
    //    }

    //    tanChuang.SetActive(false);
    //}

    protected override IEnumerator MainLogic()
    {
        tanChuang.SetActive(true);


        GameObject checkObj = null;

        foreach (var item in buttons)
        {
            Button tempButton = item.GetComponent<Button>();
            tempButton.onClick.RemoveAllListeners();
            tempButton.onClick.AddListener(() =>
            {
                checkObj = item;
            });

        }


        while (true)
        {
            if (checkObj != null)
            {
               
                int index = buttons.IndexOf(checkObj);

                if (index == 0)
                {
                    tanChuang.SetActive(false);
                    GameEntry.Course.GetTrainingFactory.SwitchBingLiDan(false);
                    if (GameEntry.Course.GetTrainingFactory == null)
                    {
                        Debug.LogError("病历单未初始化");
                    }
                    while (GameEntry.Course.GetTrainingFactory.bingLiDan.activeSelf == true)
                    {
                        yield return null;
                    }
                    tanChuang.SetActive(true);
                }
                else if (index == 1)
                {
                    break;
                }

                checkObj = null;
            }
            yield return null;
        }





    }

    protected override void ResetPlot()
    {
        tanChuang.SetActive(false);
    }
}
