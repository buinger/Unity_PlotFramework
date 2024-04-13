using Kmax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationPlot : Plot
{
    protected static Dictionary<string, Animator> soleEntities = new Dictionary<string, Animator>();

    public bool isSole = true;
    //动画物体路径
    public PrefabInfo prefabInfo;
    //播放起始位置
    public Vector3 playPosition = Vector3.zero;
    //播放起始缩放
    public Vector3 playScale = Vector3.one;
    //播放起始旋转
    public Vector3 playEuler = Vector3.one;
    //动画名
    public string animationName;


    //动画介绍
    //public string animationIntro;
    //动画播放器
    protected Animator targetAnimator;
    //播放模式
    //public WrapMode wrapMode = WrapMode.Default;
    //动画路径
    //public string animationClipPath;
    //动画资源
    //protected AnimationClip animationClip;


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(GetPlayableDirector());

        yield return StartCoroutine(PlayAnimation());
    }

    public static void ReleaseSoleEntities()
    {
        soleEntities.Clear();
    }




    IEnumerator GetPlayableDirector()
    {
        if (isSole)
        {
            if (!soleEntities.ContainsKey(prefabInfo.path))
            {
                Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabInfo.path, playPosition, null, false);
                while (getTarget.IsCompleted == false)
                {
                    yield return null;
                }
                GameObject animationObject = getTarget.Result;
                Animator temp = animationObject.GetComponent<Animator>();
                soleEntities.Add(prefabInfo.path, temp);
            }
            targetAnimator = soleEntities[prefabInfo.path];
        }
        else
        {
            Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabInfo.path, playPosition, null, false);
            while (getTarget.IsCompleted == false)
            {
                yield return null;
            }
            GameObject animationObject = getTarget.Result;
            targetAnimator = animationObject.GetComponent<Animator>();
        }
        SetTrans();
    }

    protected virtual void SetTrans()
    {
        targetAnimator.transform.position = playPosition;
        targetAnimator.transform.localScale = playScale;
        targetAnimator.transform.rotation = Quaternion.Euler(playEuler);
    }


    void PlayToEndOfAnimation(Animator animator)
    {
        // 获取当前动画状态的信息
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // 如果动画正在播放中，将 normalizedTime 设置为 1
        if (currentState.normalizedTime < 1.0f)
        {
            animator.Play(currentState.fullPathHash, 0, 1.0f);
        }
    }
    IEnumerator PlayAnimation()
    {
        targetAnimator.gameObject.SetActive(true);
        targetAnimator.Play(animationName);
        yield return null;
        bool isLoop = targetAnimator.GetCurrentAnimatorStateInfo(0).loop;

        if (!isLoop)
        {
            while (targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
               
                if (Input.GetMouseButtonDown(0))
                {
                    PlayToEndOfAnimation(targetAnimator);
                    break;
                }
                yield return null;
            }
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
            if (targetAnimator != null)
            {
                GameEntry.PlotPool.PutInPool(targetAnimator.gameObject);
            }
        }
        else
        {
            if (targetAnimator != null)
            {
                targetAnimator.gameObject.SetActive(false);
            }
        }
    }
}
