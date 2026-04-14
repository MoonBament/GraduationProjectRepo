using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystrickInput : IUserInput
{
    [Header("===== Joystrick Settings =====")]
    public string axisX="axisX";
    public string axisY="axisY";

    public string axisJright = "axis5";
    public string axisJup = "axis4";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnX = "btn2";
    public string btnY = "btn3";
    public string btnLB = "LB";//左手防御以及攻击按钮
    public string btnLT = "LT";//轴
    public string btnRT = "RT";//轴
    public string btnRB = "RB";//右手攻击按钮  
    public string btnJstick = "btn9";//手柄右边的蘑菇头按键

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonRT = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonJstick = new MyButton();

    public float ltNum;
    public float rtNum;
    public bool isLt;
    public bool isRt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ltNum = Input.GetAxis(btnLT);
        rtNum = Input.GetAxis(btnRT);

        if (ltNum > 0)
        {
            isLt = true;
        }
        else
        {
            isLt = false;
        }
        if (rtNum > 0)
        {
            isRt = true;
        }
        else
        {
            isRt = false;
        }

        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnX));
        buttonD.Tick(Input.GetButton(btnY));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonLT.Tick(isLt);
        buttonRT.Tick(isRt);
        buttonRB.Tick(Input.GetButton(btnRB));     
        buttonJstick.Tick(Input.GetButton(btnJstick));

        //print(buttonJstick.OnPressed);
        //print(Jup);

        //Jup和Jright为镜头控制
        Jup = -1*Input.GetAxis(axisJright);
        Jright = Input.GetAxis(axisJup);

        targetDup = Input.GetAxis(axisY);//跟踪目标前进后退值
        targetDright = Input.GetAxis(axisX);//跟踪目标左右值

        if (inputEnable == false)
        {
            targetDup = 0;
            targetDright = 0;
        }

        //用SmoothDamp，平滑缓冲至目标值
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));//角色在轴上行走的单位距离
        Dvec = Dup2 * transform.forward + Dright2 * transform.right;//角色在轴上的方向

        //isrun = Input.GetButton(btnA);//奔跑状态
        isrun = buttonA.IsPressing && !buttonA.IsDelaying || buttonA.IsExtending;
        //defense = Input.GetButton(btnLB);//防御状态
        defense = buttonLB.IsPressing;
        //跳跃
        #region        
        //bool tempjump;
        //tempjump = Input.GetButton(btnB);
        //if (tempjump != lastjump && tempjump == true)
        //{
        //    jump = true;
        //}
        //else
        //{
        //    jump = false;
        //}
        #endregion        //lastjump = tempjump;
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.OnPressed && buttonA.IsDelaying;
        action = buttonC.OnPressed;
        //攻击
        #region
        //bool tempattack;
        //tempattack = Input.GetButton(btnX);
        //if (tempattack != lastattack && tempattack == true)
        //{
        //    attack = true;
        //}
        //else
        //{
        //    attack = false;
        //}
        #endregion       //lastattack = tempattack;
        //attack = buttonC.OnPressed;
        rb = buttonRB.OnPressed;
        lb = buttonLB.OnPressed;
        rt = buttonRT.OnPressed;
        lt = buttonLT.OnPressed;
        lockon = buttonJstick.OnPressed;//锁定敌人
        if (rt)
        {
            Debug.Log("LT!!!");
        }
    }

    //private Vector2 SquareToCircle(Vector2 input)
    //{
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}
}
