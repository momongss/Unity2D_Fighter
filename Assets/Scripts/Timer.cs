using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private long lastTime;
    private long waitTime;

    private System.Diagnostics.Stopwatch stopwatch;

    public Timer(long waitTime)
    {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Reset();
        stopwatch.Start();
        lastTime = stopwatch.ElapsedMilliseconds - waitTime;
        this.waitTime = waitTime;
    }

    public void setTime()
    {
        lastTime = stopwatch.ElapsedMilliseconds;
    }

    public void stop()
    {
        stopwatch.Stop();
    }

    public void start()
    {
        stopwatch.Start();
    }

    public long getTime()
    {
        return stopwatch.ElapsedMilliseconds - lastTime;
    }

    public long getRemainderTime()
    {
        return waitTime - (stopwatch.ElapsedMilliseconds - lastTime);
    }

    public bool isDone()
    {
        if (stopwatch.ElapsedMilliseconds - lastTime >= waitTime)
            return true;
        else
            return false;
    }

    public long getWaitTime()
    {
        return waitTime;
    }

    public void setWaitTime(long waitTime)
    {
        this.waitTime = waitTime;
    }
}
