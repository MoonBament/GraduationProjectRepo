using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//玩家动画及相关状态控制（打算抽象出来做接口，因为角色多了之后不好分了
public class ActorController : MonoBehaviour
{
    public GameObject model;
    public List<GameObject> models;//角色小队
    private int index = 0;//角色序号：战士0 法师1 弓箭手2
    public CameraController camcon;
    public IUserInput pi;
    public ActorManager am;
    public MyBagNow mbn;
    public float walkspeed; //解决位置移动与动画进行状态不匹配问题
    public float runMultiplier;
    public float jumpVelocity;//角色在跳跃时所加带的位移
    public float rollVelocity;//翻滚位移

    private Animator anim;
    public List<Animator> anims;//角色小队的动画控制器
    private Rigidbody rigid;
    public  Vector3 planarMoving;//角色平面xy移动位置movingVec
    private Vector3 thrusVec;//冲量向量中间存量，比如适配动画，从而增加在y轴上的波动等
    private Vector3 deltaPos;//动画带来的移动量
    private bool lockPlanar=false;//锁死角色在空中平面刚体移动速度
    private bool trackDirection = false;//追踪方向
    private bool canAttack;
    //public float lerpTarget;//用于动画权重线性内插
    private CapsuleCollider col;
    
    public bool leftIsShield=true;
    //特效列表
    public List<GameObject> efs = new List<GameObject>();

    [Space(10)]
    [Header("===== Friction Setings =====")]   //解决物理材质摩擦力问题
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    //交互动作事件
    public delegate void OnActionDelegete();
    public event OnActionDelegete OnAction;

    //关于在小地图上生成敌人 （到时候记得改
    private MiniMap miniMap;
    private bool isDisplay;
    public List<GameObject> enemysNow=new List<GameObject>();
    private Dictionary<GameObject, Image> minimapEs = new Dictionary<GameObject, Image>();
    private List<Image> minimapImageNow = new List<Image>();
    // Start is called before the first frame update

    private void Awake()
    {   
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled==true)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        am = GetComponent<ActorManager>();
        mbn = GetComponent<MyBagNow>();
    }
    void Start()
    {
        //游戏一开始默认角色
        model = models[index];
        //Debug.Log(models[0].name);
        miniMap = GameObject.Find("MiniMap").GetComponent<MiniMap>();
        isDisplay = false;
    }

    /// <summary>
    /// 将敌人显示到小地图上
    /// </summary>
    private void CheckEnemyDisplayOnMiniMap()
    {
        for (int i = 0; i < enemysNow.Count; i++)
        {         
            if (!enemysNow[i].GetComponent<StateManager>().isDie)
            {
                float dis = Vector3.Distance(transform.position, enemysNow[i].transform.position);
                CheckDis(dis, enemysNow[i]);                         
            }
            else
            {
                minimapEs[enemysNow[i]].enabled = false;
            }
            
        }
    }
    public void UnDisplayMiniMap(GameObject obj)
    {
        int index = 0;
        for (int i = 0; i < enemysNow.Count; i++)
        {
            if (enemysNow[i]==obj)
            {
                index = i;
                break;
            }
        }
        //会报越界错误，今天想不出来了，明天再说，先做别的
        //订正
        if (enemysNow.Count != 0)
        {
            enemysNow.Remove(enemysNow[index]);
            minimapEs.Remove(enemysNow[index]);
        }
        
    }
    public void CheckDis(float dis, GameObject enemy)
    {
        float x = (enemy.transform.position.x - transform.position.x) / 10;
        float y = (enemy.transform.position.z - transform.position.z) / 10;
        if (dis <= 9)
        {          
            if (!minimapEs.ContainsKey(enemy))
            {
                Image image = MiniMap.CreateImage();
                minimapEs.Add(enemy, image);
                //minimapImageNow.Add(image);
            }
            minimapEs[enemy].enabled = true;
            miniMap.ShowEnemy(minimapEs[enemy], x, y);
        }
        if (dis > 9)
        {
            if (minimapEs.ContainsKey(enemy))
            {
                minimapEs[enemy].enabled = false;
            }
        }
        
    }

    public Animator Animator()
    {
        return anim;
    }

    // Update is called once per frame
    void Update()
    {  
        if (gameObject.tag.Contains("Player"))
        {
            if (enemysNow!=null)
            {
                CheckEnemyDisplayOnMiniMap();
            }        
            am.sensor.transform.rotation = model.transform.rotation;
        }
        
        //如果锁定敌人时，镜头锁定等一系列
        if (pi.lockon)
        {
            camcon.LockUnlock();
        }
        if (camcon.lockState==false)
        {
            //播放动画状态值
            // (函数名称， 函数新值  )                                      //判断是否处于跑步状态
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.isrun) ? 2.0f : 1.0f, 0.5f));
            anim.SetFloat("right", 0);
            
        }
        else
        {
            Vector3 localDVec = transform.InverseTransformVector(pi.Dvec);
            anim.SetFloat("forward", localDVec.z * ((pi.isrun) ? 2.0f:1.0f));
            anim.SetFloat("right", localDVec.x * ((pi.isrun) ? 2.0f : 1.0f));
        }

        ////lerp：线性插值  Slerp：球形线性插值
        //anim.SetBool("defense", pi.defense);

        if (pi.roll || rigid.velocity.magnitude > 5.0f)
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.jump)
        {
            //播放jump动画
            anim.SetTrigger("jump");
            canAttack = false;
        }

        //攻击状态
        if ((pi.rb || pi.lb ) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)
        {
            //右手攻击状态
            if (pi.rb)
            {
                anim.SetBool("R0L1", false);
                anim.SetTrigger("attack");
            }
            //左手攻击状态
            else if(pi.lb && !leftIsShield)
            {
                anim.SetBool("R0L1", true);
                anim.SetTrigger("attack");
            }            
        }

        if ((pi.rt || pi.lt) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)
        {
            //右手重攻击状态
            if (pi.rt)
            {
                //do right heavy attack
            }
            //左手重攻击状态
            else 
            {
                if (!leftIsShield)
                {
                    //do left heavy attack
                }
                //左手是盾，则做盾反
                else
                {
                    if (GameManager.instance.testwm.wcL.wdata.name == "Shield(Clone)")
                    {
                        anim.SetTrigger("counterBack");
                    }
                    else
                    {

                    }
                                   
                }
            }
            
        }
        
        //左手是否是防御状态
        if (leftIsShield)
        {
            if (CheckState("ground")||CheckState("blocked"))
            {
                anim.SetBool("defense", pi.defense);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 1);
            }
            else
            {
                anim.SetBool("defense", false);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
            }
        }
        else
        {
            anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
        }

        //镜头未锁定
        if (camcon.lockState==false)
        {
            if (pi.inputEnable==true)
            {
                //玩家模型去追角度
                if (pi.Dmag > 0.1f)
                {
                    //Vector3 targetForward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);//用Slerp来对物理量之间的转化做渐变
                    //解决在斜向上速度不均匀的问题
                    model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);//对角色旋转值算出前进方向的向量

                }
            }          
            if (lockPlanar == false)//平面没有被锁死，说明角色不处于空中
            {
                //角色在地面移动
                planarMoving = pi.Dmag * model.transform.forward * walkspeed * ((pi.isrun) ? runMultiplier : 1.0f);
            }
            
        }
        else
        {
            if (trackDirection == false)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planarMoving.normalized;//normalized：实现规范化，让向量保持相同的方向，但长度为1.0
            }

            if (lockPlanar==false)
            {
                planarMoving = pi.Dvec * walkspeed * ((pi.isrun) ? runMultiplier : 1.0f);
            }
        }
        //如果按下交互键，就激活方法
        if (pi.action)
        {
            OnAction.Invoke();
        }
        //玩家按下切换角色键
        if (pi.isChangeMod && gameObject.layer==9)
        {
            ChangeModel();
        }

        if (am.sm.isGround)
        {
            //玩家使用技能
            if (pi.skill1 && am.sm.skillValue > 20)
            {
                anim.SetBool("inSkill", true);
                anim.SetTrigger("skill1");
                //扣除对应技能值
                am.sm.PlayerSkillValue(20f);
                //写了一大堆if，有空再改吧
                if (model==models[0])
                {
                    //恢复30%的血量
                    am.sm.RecoverPlayerHP(0.3f);
                    //增加15%的普攻
                    am.sm.PlayerATKControl(0.15f);
                    //特效及提示文字等视觉效果，及声音
                }
                else if (model==models[1])
                {

                }
                else
                {

                }

            }
            else if (pi.skill2 && am.sm.skillValue > 30)
            {
                anim.SetBool("inSkill", true);
                anim.SetTrigger("skill2");
                am.sm.PlayerSkillValue(30f);
        
                if (model == models[0])
                {
                    //范围攻击
                    StartCoroutine(TakeDamage(0.6f, 4));
                }
                else if (model == models[1])
                {

                }
                else
                {

                }
            }
            else if (pi.skill3 && am.sm.skillValue > 40)
            {
                anim.SetBool("inSkill", true);
                anim.SetTrigger("skill3");
                am.sm.PlayerSkillValue(40f);

                if (model == models[0])
                {
                    //强化一刀斩(方法写在动画帧事件上，因需根据动画播放状态进行触发的
                }
                else if (model == models[1])
                {

                }
                else
                {

                }
            }
        }       
    }

    /// <summary>
    /// 持续伤害效果
    /// </summary>
    /// <param name="timeOff">时间间隔</param>
    /// <param name="count">伤害次数</param>
    /// <returns></returns>
    IEnumerator TakeDamage(float timeOff, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(timeOff);
            am.bm.ScopeHit(1.1f,2);
        }
    }

    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        // rigid.position += planarMoving * Time.fixedDeltaTime;
        //刚体的位置
        rigid.velocity = new Vector3(planarMoving.x, rigid.velocity.y, planarMoving.z) + thrusVec;
        //重置冲量
        thrusVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    //切换角色
    public void ChangeModel()
    {        
        if (index == 2)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        model = models[index];
        anim = anims[index];
        //其他角色隐藏
        foreach (var mod in models)
        {
            if (mod == model)
            {
                mod.SetActive(true);
            }
            else
            {
                mod.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 检查动画现处在哪个动画层中
    /// </summary>
    public bool CheckState(string stateName,string layerName="Base Layer")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;
    }
    /// <summary>
    /// 检查具体动画播放
    /// </summary>
    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tagName);
        return result;
    }


    //在FSM中被使用
    //接收到跳跃信号
    public void onJumpEnter()
    {
        //跳跃时角色操作控制失效
        pi.inputEnable = false;
        lockPlanar = true;
        thrusVec = new Vector3(0, jumpVelocity, 0);
        trackDirection = true;
    }
    //战士3技能
    public void OnJumpAttackEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        trackDirection = true;
        am.bm.SoldierFirstSkillE(efs[2], new Vector3(0, 0.45f,0), am.sensor.transform);
    }
    public void OnJumpAttackExit()
    {       
        //角色落地开始造成伤害
        am.bm.ScopeHit(1.3f,3);
    }
    //战士2技能
    public void OnRotateAttackEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        trackDirection = true;
        am.bm.SoldierFirstSkillE(efs[1],new Vector3(0,0.1f,0));
    }
    public void OnRotateAttackExit()
    {
        
        
    }
    //战士1技能
    public void OnPowerUpEnter()
    {
        pi.inputEnable = false;
        trackDirection = true;
        am.bm.SoldierFirstSkillE(efs[0], new Vector3(0, 0.1f, 0));
    }
    public void OnPowerUpExit()
    {

    }
    //接收到翻滚信号
    public void onRollEnter()
    {
        thrusVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnable = false;
        lockPlanar = true;
        trackDirection = true;
    }
    //在地面时（这两在OnGroundSensor里调用
    public void isGround()  
    {
        anim.SetBool("isGround", true);
    }
    //不在地面时
    public void isNotGround()   
    {
        //跳跃时角色不能自行操作
        pi.inputEnable = false;
        lockPlanar = true;
        anim.SetBool("isGround", false);
    }

    //接收进入地面信号
    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
        anim.SetBool("inSkill", false);
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    // 后跳模块
    public void onJabEnter() {
        //跳跃时角色操作控制失效
        pi.inputEnable = false;
        lockPlanar = true;       
    }
    public void onJabUpdate() {
        //用曲线接收
        thrusVec = model.transform.forward * anim .GetFloat("jabVelocity")*3.0f;
    }

    // 攻击模块
    public void OnAttack1hAEnter()//接收到攻击信号
    {
        //攻击时不允许玩家随意控制角色
        pi.inputEnable = false;
        //lockPlanar = true;
        //lerpTarget = 1.0f;
        
    }
    public void OnAttack1hAUpdate()
    {
        thrusVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");

        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));//当前的权重
        //新的权重用lerp缓冲至目标值
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.5f);
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }
    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }
    #region
    //public void OnAttackIdleEnter()//接收到待机信号
    //{
    //    pi.inputEnable = true;
    //    //lockPlanar = false;
    //    //lerpTarget = 0;
    //    //anim.SetLayerWeight(anim.GetLayerIndex("attack"), 0);//攻击完后权重变为零
    //}

    ////攻击完成后回到Idle状态
    //public void OnAttackIdleUpdate()
    //{
    //    //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));//当前的权重
    //    ////新的权重用lerp缓冲至目标值
    //    //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.5f);
    //    //anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    //}
    #endregion

    //_deltaPos是经由OnAnimatorMove传过来的
    public void OnUpdateRM(object _deltaPos)
    {
        //检查：如果是第三段动画，就将动画带来的位移加进去
        if (CheckState("attack1hC"))
        {
            deltaPos = (Vector3)_deltaPos;
        }
    }

    //进入被打状态
    public void OnHitEnter()
    {
        pi.inputEnable = false;
        //被打时速度清零
        planarMoving = Vector3.zero;
        model.SendMessage("WeaponDisable");

    }
    //进入死亡状态
    public void OnDieEnter()
    {
        pi.inputEnable = false;
        planarMoving = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    //进入格挡状态
    public void OnBlockedEnter()
    {
        pi.inputEnable = false;
    }

    //进入被盾反状态
    public void OnStunnedEnter()
    {
        pi.inputEnable = false;
        planarMoving = Vector3.zero;
    }
    //进入盾反攻击状态
    public void OnCounterBackEnter()
    {
        pi.inputEnable = false;
        planarMoving = Vector3.zero;
    }
    public void OnCounterBackExit()
    {
        model.SendMessage("CounterBackDisable");
    }

    public void OnLockEnter()
    {
        pi.inputEnable = false;
        planarMoving = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    //动画Trigger触发方法
    public void IssueTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
    public void MySetBool(string boolName,bool value)
    {
        anim.SetBool(boolName, value);
    }
}

