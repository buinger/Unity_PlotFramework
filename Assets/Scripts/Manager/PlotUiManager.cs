using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kmax;
using UnityEngine.SceneManagement;

public class PlotUiManager : MonoBehaviour, IBaseComponent
{

    public static PlotUiManager instance;
    public static Canvas plotCanvas = null;
    protected Dictionary<string, Transform> allUiContainers = new Dictionary<string, Transform>();


    public Transform MainUI
    {
        get { return allUiContainers[nameof(MainUI)]; }
    }
    public Transform PlotUI
    {
        get { return allUiContainers[nameof(PlotUI)]; }
    }
    public Transform PublicUI
    {
        get { return allUiContainers[nameof(PublicUI)]; }
    }
    public Transform CoverUI
    {
        get { return allUiContainers[nameof(CoverUI)]; }
    }



    //private static GraphicRaycaster graphicRaycaster;
    //private static EventSystem eventSystem;
    //private static PointerEventData eventData;



    public IEnumerator Init()
    {

       
        GameEntry.UI.ShowUI(UIFormId.PlotUI, (canvas) =>
        {
            plotCanvas = canvas.GetComponent<Canvas>();
        });

        while (plotCanvas == null)
        {
            yield return null;
        }

        for (int i = 0; i < plotCanvas.transform.childCount; i++)
        {
            Transform temp = plotCanvas.transform.GetChild(i);
            allUiContainers.Add(temp.name, temp);
        }

        //graphicRaycaster = plotCanvas.transform.GetComponent<GraphicRaycaster>();
        //eventSystem = EventSystem.current;
        //eventData = new PointerEventData(eventSystem);

        GameEntry.UI.CloseCurUI(UIFormId.PlotUI);

        //SceneManager.sceneLoaded += ((scene, mode) =>
        //{
        //    if (scene.name == "Training")
        //    {
        //        GameEntry.UI.ShowUI(UIFormId.PlotUI);
        //    }
        //});

        //SceneManager.sceneUnloaded += ((scene) =>
        //{

        //    GameEntry.UI.CloseCurUI(UIFormId.PlotUI);
        //    ClearAllUi();
        //});

        instance = this;
    }

    public void ClearAllUi()
    {
        if (plotCanvas != null)
        {
            foreach (var item in allUiContainers)
            {
                for (int i = 0; i < item.Value.childCount; i++)
                {
                    Destroy(item.Value.GetChild(i).gameObject);
                }
            }
        }
    }





    ///// <summary>
    ///// 是否在UI上
    ///// </summary>
    ///// <returns></returns>
    //public static bool IsOnUi()
    //{
    //    if (graphicRaycaster == null)
    //    {
    //        Debug.Log("无GraphicRaycaster,有问题");
    //        return false;
    //    }
    //    eventData.pressPosition = Input.mousePosition;
    //    eventData.position = Input.mousePosition;
    //    List<RaycastResult> list = new List<RaycastResult>();
    //    graphicRaycaster.Raycast(eventData, list);
    //    foreach (var temp in list)
    //    {
    //        if (temp.gameObject.layer.Equals(5))
    //        {
    //            return true;

    //        }
    //    }
    //    return false;
    //}

    //public static GameObject GetUi()
    //{
    //    if (graphicRaycaster == null)
    //    {
    //        Debug.Log("无GraphicRaycaster,有问题");
    //        return null;
    //    }
    //    eventData.pressPosition = Input.mousePosition;
    //    eventData.position = Input.mousePosition;
    //    List<RaycastResult> list = new List<RaycastResult>();
    //    graphicRaycaster.Raycast(eventData, list);
    //    foreach (var temp in list)
    //    {
    //        if (temp.gameObject.layer.Equals(5))
    //        {
    //            return temp.gameObject;
    //        }
    //    }
    //    return null;
    //}








}

