using Kmax;
using System.Collections;
using Training;

[System.Serializable]
public class TiGeJianCha : SelectionPart
{
    protected override string GetName()
    {
        return "体格检查";
    }

    protected override void SetCommonRows()
    {
        IDataTable<DRCommonTiGeJianCha> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonTiGeJianCha>();
        commonRows = dRCommonEntrys.GetAllDataRows();
    }

    protected override void SetCourseRows()
    {
        courseRows = GameEntry.DataTable.GetDataTable<DRCourseTiGeJianCha>().GetAllDataRows();
    }

    protected override OperationInfo GetOperationInfo(int index)
    {
        DRCommonTiGeJianCha rowTemp = commonRows[index] as DRCommonTiGeJianCha;
        OperationInfo temp = new OperationInfo(rowTemp.Id, rowTemp.PlotName, rowTemp.ScoreWeight);
        return temp;
    }

    protected override int GetTargetOperationId(int index)
    {
        DRCourseTiGeJianCha rowTemp = courseRows[index] as DRCourseTiGeJianCha;
        return rowTemp.Id;
    }

    protected override float GetIndependentPlotScoreWeight(float operationScoreWeight, string operationName)
    {
        //这里要通过调用静态函数方式获取得分
        switch (operationName)
        {
            case "检查床":
                break;        
        }

        //暂时没有重点plot
        return 0;
    }

   
}
