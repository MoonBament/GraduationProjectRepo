using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //清除attack信号
    //在角色动画添加事件点
    private void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }
}
