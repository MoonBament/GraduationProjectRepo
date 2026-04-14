
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag;//角色在轴上行走的单位距离
    public Vector3 Dvec;//角色在轴上的方向
    public float Jup;
    public float Jright;

    //1.pressing signal
    public bool isrun;
    public bool defense;
    public bool isChangeMod;
    //2.trigger once signal
    public bool jump;
    public bool action;
    protected bool lastjump;
    //public bool attack;
    protected bool lastattack;
    public bool roll;//翻滚
    public bool lockon;//是否锁定目标
    public bool lb;//左手防御以及攻击
    public bool lt;//左手重攻击
    public bool rb;//右手攻击
    public bool rt;//右手重攻击
    public bool skill1;
    public bool skill2;
    public bool skill3;
    //3.double trigger

    [Header("===== Others =====")]

    public bool inputEnable = true;//控制角色是否行动
    protected float targetDup;
    protected float targetDright;
    //缓冲减速值（默认赋值为零即可）
    protected float velocityDup;
    protected float velocityDright;

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    public void UpdateDmagDvec(float Dup2,float Dright2)
    {
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));//角色在轴上行走的单位距离
        Dvec = Dup2 * transform.forward + Dright2 * transform.right;//角色在轴上的方向
    }
}
