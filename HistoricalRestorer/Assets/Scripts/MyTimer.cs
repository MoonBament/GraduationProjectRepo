using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer //倒数计时用
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }
    public STATE state;
    public float duration = 1.0f;//所需倒计时时长（根据测试后调用方法时，自行给定
    private float elapsedTime = 0;//计时的时长

    public void Tick()
    {
        if (state==STATE.IDLE)
        {

        }
        else if (state==STATE.RUN)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime>=duration)
            {
                state = STATE.FINISHED;
            }
        }
        else if (state==STATE.FINISHED)
        {

        }
        else
        {
            Debug.LogError("");
        }
    }

    public void Go()
    {
        elapsedTime = 0;
        state = STATE.RUN;
    }
}
