using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;//正在被按压 _|------|_
    public bool OnPressed = false;//刚刚被按住(按住的开头一瞬间)_|-|______
    public bool OnReleased = false;//刚刚被释放 _______|-|_
    public bool IsExtending = false;//在extending内判断是否doubleTrigger
    public bool IsDelaying = false;//判断在按下的瞬间后的一小段时间内是否再次按压

    public float extendingDuration = 0.15f;//按下松手后的一小段时间
    public float delayingDuration = 0.15f;//在按下的瞬间之后一小短时间内

    private bool curState = false;//当前状态
    private bool lastState = false;//上一次状态

    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)//按键是否被按下
    {
        //判断状态
        extTimer.Tick();
        delayTimer.Tick();

        curState = input;
        IsPressing = curState;

        OnPressed = false;
        OnReleased = false;
        IsExtending = false;
        IsDelaying = false;

        if (curState!=lastState)
        {
            if (curState==true)//按压下时
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);//开始倒计时
            }
            else//按压松手后
            {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);//开始倒计时
            }
        }
        lastState = curState;

        if (extTimer.state==MyTimer.STATE.RUN)
        {
            IsExtending = true;
        }
        if (delayTimer.state==MyTimer.STATE.RUN)
        {
            IsDelaying = true;
        }
    }

    private void StartTimer(MyTimer timer,float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
