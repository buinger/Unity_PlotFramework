using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowMaiZhenHand_ShowObj : ShowAndCloseObjPlot
{
    public Image xueWei;
    public Image xueWeiHighLight;

    public GameObject colliderObj;
    protected override void Ini(Action onIniOver)
    {
        ResetPlot();
        base.Ini(onIniOver);
    }

    protected override IEnumerator LogicBeforeClose()
    {
        while (true)
        {
            // ������������
            if (Input.GetMouseButtonDown(0))
            {
                // ��������
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                // ���������ײ
                if (Physics.Raycast(ray, out hitInfo))
                {
                    // �ڿ���̨�����ײ����������Ϣ
                    Debug.Log("�������" + hitInfo.collider.gameObject.name);

                    if (colliderObj == hitInfo.collider.gameObject)
                    {
                        xueWeiHighLight.gameObject.SetActive(true);
                        break;

                    }

                }
            }
            yield return null;
        }
        //yield return new WaitForSeconds(0.5f);

    }
    protected override void ResetPlot()
    {
        xueWeiHighLight.gameObject.SetActive(false);
        base.ResetPlot();
    }
}
