using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventTriggerTool : MonoBehaviour
{

    public static void SetTriggerEvent(Graphic triggerComponent, Action mouseFun, EventTriggerType triggerType)
    {
        EventTrigger eventTrigger = triggerComponent.GetComponent<EventTrigger>();
        if (eventTrigger==null)
        {
            eventTrigger = triggerComponent.gameObject.AddComponent<EventTrigger>();
        }
        SetEvent(eventTrigger,mouseFun,triggerType);
    }

    static void SetEvent(EventTrigger et, Action mouseFun, EventTriggerType triggerType)
    {
        EventTrigger.Entry ete = new EventTrigger.Entry();
        ete.eventID = triggerType;
        ete.callback.AddListener((baseEventData) =>
        {
            mouseFun();
        });
        et.triggers.Add(ete);
    }
}
