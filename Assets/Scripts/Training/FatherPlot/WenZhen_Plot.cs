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
            Debug.Log("����ִ���У�����������˳�");
            yield return null;
            if (Input.GetMouseButtonDown(0)) break;
        }
    }

    protected override void ResetPlot()
    {

    }
}
