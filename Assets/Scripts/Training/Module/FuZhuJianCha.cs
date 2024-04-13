using Kmax;
using System.Collections;
using System.Collections.Generic;
using Training;
using UnityEngine;

[System.Serializable]
public class FuZhuJianCha : SelectionPart
{
    protected override string GetName()
    {
        return "辅助检查";
    }

    protected override void SetCommonRows()
    {
        IDataTable<DRCommonFuZhuJianCha> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonFuZhuJianCha>();
        commonRows = dRCommonEntrys.GetAllDataRows();
    }

    protected override void SetCourseRows()
    {
        IDataTable<DRCourseFuZhuJianCha> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCourseFuZhuJianCha>();
        courseRows = dRCommonEntrys.GetAllDataRows();
    }

    protected override float GetIndependentPlotScoreWeight(float operationScoreWeight, string operationName)
    {
        //辅助检查没有独立Plot
        return 0;
    }


    protected override OperationInfo GetOperationInfo(int index)
    {
        DRCommonFuZhuJianCha rowTemp = commonRows[index] as DRCommonFuZhuJianCha;
        OperationInfo temp = new OperationInfo(rowTemp.Id, rowTemp.LevelName3, rowTemp.ScoreWeight);
        return temp;
    }

    protected override int GetTargetOperationId(int index)
    {
        DRCourseFuZhuJianCha rowTemp = courseRows[index] as DRCourseFuZhuJianCha;
        return rowTemp.Id;
    }
}
