using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//数值有关信息操作
public class StateManager : IActorManagerInterface
{
    public StateController sc;
    //public ActorManager am;
    public float HP = 20.0f;
    public float HPMax = 100.0f;     
    public float endurance=50.0f;    //人物耐力值
    public float enduranceMax = 100.0f;
    public float skillValue;        //角色技能值
    public float skillValueMax = 100.0f;
    public float ATK ;       //角色基础的伤害值
    //public Slider bloodSlider;      //人物显示血条
    //public Slider enduranceSlider;  //人物显示耐力条
    public float coolingTime;
    public float atkFirstValue=10;  //游戏刚刚开始时玩家自身的攻击力
    [Header("1st order state flags")]
    public bool isGround;
    public bool isRoll;
    public bool isJump;
    public bool isFall;
    public bool isJab;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    public bool isBlocked;
    public bool isDefense;
    public bool isCounterBack;//整个盾反动画
    public bool isCounterBackEnable;//能盾反敌人的时间间隔

    [Header("2nd order state flags")]
    public bool isAllowDefense;  //是否允许进行防御
    public bool isImmortal;     //是否处在无敌状态
    public bool isCounterBackSuccess;//盾反时的无敌帧时间
    public bool isCounterBackFailure;//盾反整个动作的非无敌帧时间


    private void Awake()
    {
        sc = GetComponent<StateController>();
        HP = HPMax;
        endurance = enduranceMax;
        skillValue = skillValueMax;
        ATK = atkFirstValue;
    }

    private void Start()
    {
        //一开始就修整HP值，跟耐力值
       
        if (gameObject.layer==9)//gameObject.layer==11|| 敌人血条暂时还未制作，之后再加进去
        {            
            sc.bloodSlider.maxValue = HPMax;
            //sc.bloodSlider.value = HP;
            sc.enduranceSlider.maxValue = enduranceMax;
            sc.enduranceSlider.value = endurance;
            sc.skillOutputValue.text = skillValue.ToString();
        }
        
    }

    private void Update()
    {
        isGround = am.ac.CheckState("ground");
        isRoll = am.ac.CheckState("roll");
        isJump = am.ac.CheckState("jump");
        isFall = am.ac.CheckState("fall");
        isJab = am.ac.CheckState("jab");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isBlocked = am.ac.CheckState("blocked");
        isCounterBack = am.ac.CheckState("counterBack");
        isCounterBackSuccess = isCounterBackEnable;
        isCounterBackFailure = isCounterBack && !isCounterBackEnable;
        //isDefense = am.ac.CheckState("defense1h","defense");
        isAllowDefense = isGround || isBlocked;
        isDefense = isAllowDefense && am.ac.CheckState("defense1h","defense");
        isImmortal = isRoll || isJab;
        if (gameObject.layer == 9)// || gameObject.layer == 23)
        {
            HPUpdata();
            EnduranceControl();
            RecoverPlayerSkill();
        }              
    }
    public void RecoverPlayerSkill()
    {
        if (skillValue>skillValueMax)
        {
            skillValue = skillValueMax;
        }
        sc.skillOutputValue.text = skillValue.ToString();
    }

    public void RecoverPlayerHP(float value)
    {
        HP += HP * value;
        if (HP>HPMax)
        {
            HP = HPMax;
        }
    }
    public void PlayerATKControl(float value)
    {
        ATK += ATK * value;
        //开始倒计时，7.5秒后恢复原来的ATK
        StartCoroutine(ChangeTime(7.5f));      
    }
    private IEnumerator ChangeTime(float time)
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);// 每次 自减1，等待 1 秒
            time--;      
        }
        ATK = atkFirstValue;
    }

    /// <summary>
    /// 扣除玩家技能值
    /// </summary>
    /// <param name="value">所消耗的技能值</param>
    public void PlayerSkillValue(float value)
    {
        skillValue -= value;
        if (skillValue < 0)
        {
            skillValue = 0;
        }
    }

    public void AddHP(float value)
    {
        HP += value;
        //为什么这句会出问题呢？因为我铸币把第一个参数写成value啦！！
        HP = Mathf.Clamp(HP, 0, HPMax);

    }

    public void HPUpdata()
    {
        if (gameObject.layer==25)
        {
            return;
        }
        sc.bloodSlider.value = HP;
    }

    public void EnduranceControl()
    {
        if (am.ac.Animator().GetFloat("forward")>1.0f || isFall ||isRoll)
        {
            if (isRoll)
            {
                sc.enduranceSlider.value -= 0.2f;
            }
            else
            {
                sc.enduranceSlider.value -= 0.4f;
            }
            
            if (sc.enduranceSlider.value==0)
            {
                //角色切换为行走动画
                am.ac.Animator().SetFloat("forward", am.ac.pi.Dmag * Mathf.Lerp(am.ac.Animator().GetFloat("forward"), 0.5f , 0.5f));
                //角色解除奔跑状态(是为FALSE啦。但是怎么ac里不是一直FALSE呢？
                am.ac.pi.isrun = false;
                //麻了，我特么直接改movingVec！（但是效果不太好，要做速度缓冲,就这样吧......累了
                am.ac.planarMoving = am.ac.pi.Dmag * am.ac.model.transform.forward * am.ac.walkspeed; //* ((pi.isrun) ? runMultiplier : 1.0f);//角色在地面移动
            }
        }
        else 
        {
            if (isGround&&gameObject.layer==9)
            {
                sc.enduranceSlider.value += 0.5f;
            }
        }
    }


}
