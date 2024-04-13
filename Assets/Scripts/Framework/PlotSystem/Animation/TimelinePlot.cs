using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlot : Plot
{
    protected static Dictionary<string, PlayableDirector> soleEntities = new Dictionary<string, PlayableDirector>();
    protected static Dictionary<GameObject, List<ObjectActivationState>> allEntityOringnalStates = new Dictionary<GameObject, List<ObjectActivationState>>();
    public bool isSole = true;
    //动画物体路径
    public string prefabPath;
    //播放起始位置
    public Vector3 playPosition = Vector3.zero;
    //播放起始缩放
    public Vector3 playScale = Vector3.one;
    //播放起始旋转
    public Vector3 playEuler = Vector3.one;
    //动画介绍
    //public string animationIntro;
    //timeline播放器
    protected PlayableDirector playableDirector;
    //播放模式
    public DirectorWrapMode wrapMode = DirectorWrapMode.None;
    //动画路径
    public string playableAssetPath;
    //动画资源
    protected PlayableAsset playableAsset;

    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(GetPlayableDirector());

        yield return StartCoroutine(PreparePlayableDirector());

        yield return StartCoroutine(PlayAnimation());
    }

    public static void ReleaseSoleEntities()
    {
        soleEntities.Clear();
        allEntityOringnalStates.Clear();
    }



    IEnumerator PreparePlayableDirector()
    {
        playableDirector.extrapolationMode = wrapMode;
        playableDirector.playOnAwake = false;

        GameEntry.Resource.HasAsset(playableAssetPath);
        Task<PlayableAsset> getAnimation = PlotPoolManager.PlotPool.LoadSomethingAsync<PlayableAsset>(playableAssetPath);
        while (getAnimation.IsCompleted == false)
        {
            yield return null;
        }
        playableDirector.playableAsset = getAnimation.Result;
    
    }


    IEnumerator GetPlayableDirector()
    {
        if (isSole)
        {
            if (!soleEntities.ContainsKey(prefabPath))
            {
                Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabPath, playPosition, null, false);
                while (getTarget.IsCompleted == false)
                {
                    yield return null;
                }
                GameObject animationObject = getTarget.Result;
                PlayableDirector temp = animationObject.GetComponent<PlayableDirector>();
                soleEntities.Add(prefabPath, temp);
            }
            playableDirector = soleEntities[prefabPath];
        }
        else
        {
            Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabPath, playPosition, null, false);
            while (getTarget.IsCompleted == false)
            {
                yield return null;
            }
            GameObject animationObject = getTarget.Result;
            playableDirector = animationObject.GetComponent<PlayableDirector>();
        }

        if (!allEntityOringnalStates.ContainsKey(playableDirector.gameObject))
        {
            allEntityOringnalStates.Add(playableDirector.gameObject, RecordAll(playableDirector.transform));
        }
        SetSonActive(playableDirector.gameObject);
        SetTrans();
    }

    
    void SetSonActive(GameObject keyObj)
    {
        if (allEntityOringnalStates.ContainsKey(keyObj) == true)
        {
            foreach (var item in allEntityOringnalStates[keyObj])
            {              
                item.Restore();
            }
        }
        else
        {
            Debug.Log("没有记录激活状态");
        }
    }

    protected virtual void SetTrans()
    {
        playableDirector.transform.position = playPosition;
        playableDirector.transform.localScale = playScale;
        playableDirector.transform.rotation = Quaternion.Euler(playEuler);
    }


    IEnumerator PlayAnimation()
    {
        playableDirector.gameObject.SetActive(true);
        playableDirector.Stop();
        playableDirector.time = 0;
        playableDirector.Play();
        switch (playableDirector.extrapolationMode)
        {
            case DirectorWrapMode.Hold:
                while (true)
                {
                    if (playableDirector.duration - playableDirector.time < 0.01f)
                    {
                        break;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        break;
                    }
                    yield return null;
                }
                break;
            case DirectorWrapMode.Loop:
                //overPlot = new ChildPlotInformation();
                break;
            case DirectorWrapMode.None:
                while (true)
                {
                    if (playableDirector.duration - playableDirector.time < 0.01f)
                    {
                        break;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        break;
                    }
                    yield return null;
                }
                break;
        }
    }

    protected override void Ini(Action onIniOver)
    {
        onIniOver.Invoke();
    }

    protected override void ResetPlot()
    {
        if (!isSole)
        {
            if (playableDirector != null)
            {
                GameEntry.PlotPool.PutInPool(playableDirector.gameObject);
            }
        }
        else
        {
            if (playableDirector != null)
            {
                playableDirector.gameObject.SetActive(false);
            }
        }
    }


    // 用于记录激活状态的结构体
    protected struct ObjectActivationState
    {
        public GameObject obj;
        public bool isActive;
        public void Restore()
        {
            obj.SetActive(isActive);
        }

    }


    List<ObjectActivationState> RecordAll(Transform father)
    {
        List<ObjectActivationState> result = new List<ObjectActivationState>();
        foreach (Transform child in father)
        {
            RecordObjectStates(child, result);
        }
        return result;
    }


    // 递归记录所有物体的激活状态
    void RecordObjectStates(Transform parent, List<ObjectActivationState> container)
    {
        container.Add(RecordActivationState(parent.gameObject));
        foreach (Transform child in parent)
        {
            RecordObjectStates(child, container);
        }
    }

    ObjectActivationState RecordActivationState(GameObject obj)
    {
        return (new ObjectActivationState
        {
            obj = obj,
            isActive = obj.activeSelf
        });
    }


}
