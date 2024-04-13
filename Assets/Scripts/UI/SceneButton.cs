using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [Header("要切换的场景名字")]
    public string sceneName;


    // Start is called before the first frame update
    void Start()
    {
        Button target = transform.GetComponent<Button>();
        if (target!=null)
        {
            target.onClick.AddListener(()=> {
               // GameManager.instance.ChangeSceneTo(sceneName);
            
            });
        }

    }

    
}
