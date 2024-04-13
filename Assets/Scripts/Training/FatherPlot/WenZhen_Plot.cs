using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenZhen_Plot : Plot
{

    protected override void Ini(Action onIniOver)
    {
        onIniOver.Invoke();
    }

    protected override IEnumerator MainLogic()
    {
        while (true)
        {
            Debug.Log("问诊执行中，点击鼠标左键退出");
            yield return null;
            if (Input.GetMouseButtonDown(0)) break;
        }
    }

    protected override void ResetPlot()
    {

    }
}
