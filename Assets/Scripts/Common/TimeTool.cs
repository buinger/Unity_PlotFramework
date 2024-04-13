using UnityEngine;
using System;

class TimeTool
{
    public static double DiffSeconds(DateTime startTime, DateTime endTime)
    {
        TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return secondSpan.TotalSeconds;
    }
    public static double DiffMinutes(DateTime startTime, DateTime endTime)
    {
        TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return minuteSpan.TotalMinutes;
    }
    public static double DiffHours(DateTime startTime, DateTime endTime)
    {
        TimeSpan hoursSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return hoursSpan.TotalHours;
    }
    public static double DiffDays(DateTime startTime, DateTime endTime)
    {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan.TotalDays;
    }
    public static int DiffDaysInt(DateTime startTime, DateTime endTime)
    {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan.Days;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        //DateTime.Now获取的是电脑上的当前时间
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

}

[System.Serializable]
public class ColdTimeMachine
{
    public float cdTime = 10;
    private float passTime = 0;

    public bool PassingAuto()
    {
        passTime += Time.deltaTime;
        if (passTime >= cdTime)
        {
            Reset();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PassingManual()
    {
        passTime += Time.deltaTime;
        if (passTime >= cdTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ColdTimeMachine(float _cdTime)
    {
        cdTime = _cdTime;
    }

    public void ChangeCdTime(float _cdTime)
    {
        cdTime = _cdTime;
    }

    public void Reset()
    {
        passTime = 0;
    }

}
[System.Serializable]
public class RandomTime
{
    public float nowTime = 2;
    public float minTime = 1;
    public float maxTime = 5;

    public string GetExcelValueStr()
    {
        return nowTime.ToString() + "*" + minTime.ToString() + "*" + maxTime.ToString();
    }

    public RandomTime(float _min, float _max)
    {
        minTime = _min;
        maxTime = _max;
        SetNewTime();
    }

    public void Ini()
    {
        if (nowTime == 0)
        {
            nowTime = 6;
        }
    }

    public void SetNewTime()
    {
        nowTime = UnityEngine.Random.Range(minTime, maxTime);
    }

}
