using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChildSize : MonoBehaviour
{
    RectTransform selfRt;
    RectTransform targetChildRt;

    // Start is called before the first frame update
    void Start()
    {
        targetChildRt = transform.GetChild(0)?.GetComponent<RectTransform>();
        selfRt = transform.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (selfRt != null && targetChildRt != null)
        {
            if (targetChildRt.sizeDelta != selfRt.sizeDelta)
            {
                selfRt.sizeDelta = targetChildRt.sizeDelta;
            }

        }
    }


    //void OnGUI()
    //{
       
    //}
}
