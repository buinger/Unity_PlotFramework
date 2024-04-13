using System.Collections.Generic;
using UnityEngine;
public class TransformHelper
{/// <summary>
 /// 在层级未知的情况下查找子物体
 /// </summary>
 /// <param name="parentTF">父物体变换组件</param>
 /// <param name="children">子物体名称</param>
    public static Transform GetChild(Transform parentTF, string childName)
    {
        //在子物体中查找
        Transform childTF = parentTF.Find(childName);
        if (childTF != null) return childTF;
        //将问题交给子物体
        for (int i = 0; i < parentTF.childCount; i++)
        {
            childTF = GetChild(parentTF.GetChild(i), childName);
            if (childTF != null) return childTF;
        }
        return null;
    }


    public static List<Transform> GetImmediateChildList(Transform parentTransform)
    {
        List<Transform> immediateChildren = new List<Transform>();

        // 遍历父物体的子物体
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            immediateChildren.Add(child);
        }

        return immediateChildren;
    }
}