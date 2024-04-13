using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQiTrans : MonoBehaviour
{
    MoveTransition xx;
    private void Start()
    {
        xx = GetComponent<MoveTransition>();
        xx.toEvents.onTransitionStart.AddListener(() => { Debug.Log("toS"); });
        xx.toEvents.onTransitionEnd.AddListener(() => { Debug.Log("toE"); });
        xx.outEvents.onTransitionStart.AddListener(() => { Debug.Log("outS"); });
        xx.outEvents.onTransitionEnd.AddListener(() => { Debug.Log("outE"); });
    }

    [ContextMenu("�����뿪")]
    private void PlayOut()
    {
        xx.ManualRePlayOut();    
    }

    [ContextMenu("����ȥ")]
    private void PlayTo()
    {
        xx.ManualRePlayTo();
    }

}
