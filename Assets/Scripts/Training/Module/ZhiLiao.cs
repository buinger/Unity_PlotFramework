using Kmax;
using Training;
using UnityEngine;

[System.Serializable]
public class ZhiLiao : SelectionPart
{
    protected override string GetName()
    {
        return "治疗";
    }

    protected override void SetCommonRows()
    {
        IDataTable<DRCommonZhiLiao> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonZhiLiao>();
        commonRows = dRCommonEntrys.GetAllDataRows();
    }
    protected override void SetCourseRows()
    {
        courseRows = GameEntry.DataTable.GetDataTable<DRCourseZhiLiao>().GetAllDataRows();
    }

    protected override OperationInfo GetOperationInfo(int index)
    {
        DRCommonZhiLiao rowTemp = commonRows[index] as DRCommonZhiLiao;
        OperationInfo temp = new OperationInfo(rowTemp.Id, rowTemp.PlotName, rowTemp.ScoreWeight);
        return temp;
    }

    protected override int GetTargetOperationId(int index)
    {
        DRCourseZhiLiao rowTemp = courseRows[index] as DRCourseZhiLiao;
        return rowTemp.Id;
    }
    protected override float GetIndependentPlotScoreWeight(float operationScoreWeight, string operationName)
    {
        //这里要通过调用静态函数方式获取得分
        switch (operationName)
        {
            case "拔罐法":

                break;
            case "灸法":

                break;
        }
        return 0;
    }


    
}
