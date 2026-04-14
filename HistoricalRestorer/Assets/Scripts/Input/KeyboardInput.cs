using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput
{
    [Header("===== Key setting =====")]
    //获取玩家左右前进后退移动输入
    public string keyup;
    public string keydown;
    public string keyleft;
    public string keyright;


    public string keyA;//加速
    public string keyB;//跳跃
    public string keyC;//攻击（右手
    public string keyE;//攻击（左手
    public string keyD;//暂且用作防御
    public string keyF;//锁定敌人
    public string keyLT;//左手重攻击
    public string keyRT;//右手重攻击
    public string keyZ;//切换角色：Q键
    public string keyP;//与场景道具交互，e键
    public string keyR;//翻滚
    public string key1;//技能1
    public string key2;
    public string key3;

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonE = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonF = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonRT = new MyButton();
    public MyButton buttonZ = new MyButton();
    public MyButton buttonP = new MyButton();
    public MyButton buttonR = new MyButton();
    public MyButton button1 = new MyButton();
    public MyButton button2 = new MyButton();
    public MyButton button3 = new MyButton();

    //镜头控制
    public string keyJUp;
    public string keyJDown;
    public string keyJLeft;
    public string keyJRight;

    [Header("===== Mouse settings =====")]
    public bool mouuseEnable = false;//是否用鼠标代替控制
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    void Update()
    {
        buttonA.Tick(Input.GetKey(keyA));//加速
        buttonB.Tick(Input.GetKey(keyB));//跳跃
        buttonC.Tick(Input.GetKey(keyC));//右手攻击
        buttonE.Tick(Input.GetKey(keyE));//左手攻击
        buttonD.Tick(Input.GetKey(keyD));//暂做防御
        buttonF.Tick(Input.GetKey(keyF));//锁定敌人
        buttonLT.Tick(Input.GetKey(keyLT));//左手重攻击
        buttonRT.Tick(Input.GetKey(keyRT));//右手重攻击
        buttonZ.Tick(Input.GetKey(keyZ));//切换角色：Q键
        buttonP.Tick(Input.GetKey(keyP));//与场景道具交互，e键
        buttonR.Tick(Input.GetKey(keyR));//翻滚，左Ctrl
        button1.Tick(Input.GetKey(key1));//技能
        button2.Tick(Input.GetKey(key2));
        button3.Tick(Input.GetKey(key3));
        //Jup和Jright为镜头控制
        if (mouuseEnable==true)
        {
            Jup = Input.GetAxis("Mouse Y")*1.0f* mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X")*1.5f* mouseSensitivityX;
        }
        else
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        }
        

        targetDup = (Input.GetKey(keyup)? 1.0f:0) - (Input.GetKey(keydown)? 1.0f:0);//跟踪目标前进后退值
        targetDright = (Input.GetKey(keyright) ? 1.0f : 0) - (Input.GetKey(keyleft) ? 1.0f : 0);//跟踪目标左右值

        if (inputEnable==false)
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
        Dvec = Dup2 * transform.forward+Dright2 * transform.right;//角色在轴上的方向

        //奔跑状态
        isrun = buttonA.IsPressing && !buttonA.IsDelaying || buttonA.IsExtending;
        //防御状态
        defense = buttonD.IsPressing;
        // 切换角色
        isChangeMod = buttonZ.OnPressed;
        //跳跃
        #region
        //bool tempjump;
        //tempjump = Input.GetKey(keyB);
        //if (tempjump != lastjump && tempjump ==true)
        //{
        //    jump = true;
        //}
        //else 
        //{
        //    jump = false;
        //}
        #endregion//lastjump = tempjump;
        jump = buttonB.OnPressed && buttonB.IsDelaying;
        roll = buttonR.OnPressed && buttonR.IsDelaying;//翻滚
        action = buttonP.OnPressed;
        //攻击
        #region
        //bool tempattack;
        //tempattack = Input.GetKey(keyC);
        //if (tempattack != lastattack && tempattack == true)
        //{
        //    attack  = true;
        //}
        //else
        //{
        //    attack= false;
        //}
        #endregion//lastattack = tempattack;
        //attack = buttonC.OnPressed; 键盘设置后再做左右手攻击修改 C:右手 E：左手
        rb = buttonC.OnPressed;
        lb = buttonE.OnPressed;
        lt = buttonLT.OnPressed;
        rt = buttonRT.OnPressed;
        //技能部分
        skill1 = button1.OnPressed;
        skill2 = button2.OnPressed;
        skill3 = button3.OnPressed;
        //锁定敌人
        lockon = buttonF.OnPressed;

    }

    //private Vector2 SquareToCircle(Vector2 input)
    //{
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}

}
