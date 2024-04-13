using Kmax;
using System.Collections;
using Training;

[System.Serializable]
public class XuanJiao : SelectionPart
{

    protected override string GetName()
    {
        return "宣教";
    }
    protected override void SetCommonRows()
    {
        IDataTable<DRCommonXuanJiao> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCommonXuanJiao>();
        commonRows = dRCommonEntrys.GetAllDataRows();
    }
    protected override void SetCourseRows()
    {
        IDataTable<DRCourseXuanJiao> dRCommonEntrys = GameEntry.DataTable.GetDataTable<DRCourseXuanJiao>();
        courseRows = dRCommonEntrys.GetAllDataRows();
    }

    protected override OperationInfo GetOperationInfo(int index)
    {
        DRCommonXuanJiao rowTemp = commonRows[index] as DRCommonXuanJiao;
        OperationInfo temp = new OperationInfo(rowTemp.Id, rowTemp.PlotName, rowTemp.ScoreWeight);
        return temp;
    }

    protected override int GetTargetOperationId(int index)
    {
        DRCourseXuanJiao rowTemp = courseRows[index] as DRCourseXuanJiao;
        return rowTemp.Id;
    }

    protected override float GetIndependentPlotScoreWeight(float operationScoreWeight, string operationName)
    {
        return 0;
    }

   
}
