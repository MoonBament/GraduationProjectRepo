using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionControl : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 用OnAnimatorMove()方法就不用自己去调曲线，也可以更精确地执行原本需要的位移量
    /// </summary>
    private void OnAnimatorMove()
    {
        SendMessageUpwards("OnUpdateRM", (object)anim.deltaPosition);//向上传送调用OnUpdateRM方法的信息，并传值
    }
}
