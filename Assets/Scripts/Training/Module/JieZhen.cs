using Kmax;
using System.Collections.Generic;
using Training;
using UnityEngine;

[System.Serializable]
public class JieZhen : SelectionWordsPart
{
    protected override string GetName()
    {
        return "接诊";
    }

    protected override void SetCommonRows()
    {
        IDataTable<DRCommonJieZhen> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonJieZhen>();
        commonRows = dRCommonEntrys.GetAllDataRows();
    }

    protected override void SetCourseRows()
    {
        IDataTable<DRCourseJieZhen> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCourseJieZhen>();
        courseRows = dRCommonEntrys.GetAllDataRows();
    }

    public override void Ini()
    {
        base.Ini();
        for (int i = 0; i < commonRows.Length; i++)
        {
            DRCommonJieZhen rowTemp = commonRows[i] as DRCommonJieZhen;
            string[] strs = rowTemp.AllChoice.Split('|');
            string[] weiths = rowTemp.ScoreWeights.Split('|');
            Dictionary<string, float> temp = new Dictionary<string, float>();
            if (strs.Length == weiths.Length)
            {

                for (int j = 0; j < strs.Length; j++)
                {
                    float targetWeight = float.Parse(weiths[j]);
                    temp.Add(strs[j], targetWeight);

                }

            }
            else
            {
                Debug.LogError("选项和对应权重数量不一");
            }
            if (!allOperation.ContainsKey(rowTemp.Id))
            {
                allOperation.Add(rowTemp.Id, temp);
            }
        }

        for (int i = 0; i < courseRows.Length; i++)
        {
            DRCourseJieZhen rowTemp = courseRows[i] as DRCourseJieZhen;

            if (allOperation.ContainsKey(rowTemp.Id))
            {
                if (!targetOperation.ContainsKey(rowTemp.Id))
                {
                    targetOperation.Add(rowTemp.Id, allOperation[rowTemp.Id]);
                    string[] strs = rowTemp.AllChoice.Split('|');
                    string[] weiths = rowTemp.ScoreWeights.Split('|');
                    if (strs.Length == weiths.Length)
                    {
                        float maxWeight = 0;
                        for (int j = 0; j < strs.Length; j++)
                        {
                            float targetWeight = float.Parse(weiths[j]);
                            if (targetWeight > maxWeight)
                            {
                                maxWeight = targetWeight;
                            }
                        }
                        fullWeight += maxWeight;
                    }
                    else
                    {
                        Debug.LogError("选项和对应权重数量不一");
                    }
                }
            }
            else
            {
                Debug.LogError("总表不存在此id:" + rowTemp.Id);
            }
        }

    }

    public override void UpdateScore(int key, bool isPlus, object other = null)
    {

        string content = other as string;
        if (allOperation.ContainsKey(key))
        {
            if (targetOperation.ContainsKey(key))
            {
                if (!selectionResult.ContainsKey(key))
                {
                    if (isPlus)
                    {
                        Debug.Log("项目得分:" + allOperation[key][content]);
                        selectionResult.Add(key, content);
                    }
                }
                else
                {
                    if (isPlus)
                    {
                        if (selectionResult[key] != content)
                        {
                            Debug.Log("项目得分:" + allOperation[key][content]);
                            selectionResult[key] = content;
                        }
                    }
                    else
                    {
                        Debug.Log("取消项目得分:" + allOperation[key][content]);
                        selectionResult.Remove(key);
                    }
                }
            }
            else
            {
                Debug.LogError($"不存在此选择：" + other);
            }
        }
    }

}