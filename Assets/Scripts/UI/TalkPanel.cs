using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkPanel : MonoBehaviour
{
    public TMP_Text personName;
    public TMP_Text words;


    public void SetText(string _name, string _words, bool showName = true)
    {
        personName.text = _name;
        words.text = _words;
        if (showName)
        {
            personName.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            personName.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ClearContent()
    {
        personName.text = "";
        words.text = "";
    }
}
