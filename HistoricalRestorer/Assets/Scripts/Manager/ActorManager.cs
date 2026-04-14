using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;
    public PanelController pc;
    public GameObject sensor = null;
    [Header("=== Auto Generate if Null ===")]
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    public DirectorManager dm;
    public InteractionManager im;

    [Header("=== Override Animators ===")]
    public AnimatorOverrideController oneHandAnim;
    public AnimatorOverrideController twoHandAnim;
    // Start is called before the first frame update
    void Awake()
    {
        ac = GetComponent<ActorController>();
        GameObject model = ac.model;
        try
        {
            sensor = transform.Find("Sensor").gameObject;
        }
        catch (System.Exception)
        {
            //If there is no "Sensor" object.
        }
        

        //将ActorManager跟BattleManager、WeaponManager绑定起来，有AM就一定有BM和WM
        //没有BattleManager就自动加上
        bm = Bind<BattleManager>(sensor);        

        //没有WeaponManager就自动加上
        wm = Bind<WeaponManager>(model);

        sm = Bind<StateManager>(gameObject);
        dm = Bind<DirectorManager>(gameObject);
        im = Bind<InteractionManager>(sensor);

        //ac里交互 动画（Timeline）按钮事件委托方法
        ac.OnAction += DoAction;
    }

    public void DoAction()
    {
        if (im.overlapEcasterms.Count!=0)
        {
            if (im.overlapEcasterms[0].active==true && !dm.IsPlaying())
            {
                if (im.overlapEcasterms[0].eventName == "frontStab")
                {
                    dm.PlayFrontStab("frontStab", this, im.overlapEcasterms[0].am);
                    im.overlapEcasterms.Remove(im.overlapEcasterms[0]);//因为OnTriggerExit()有时候不触发真的很无语
                }
                else if (im.overlapEcasterms[0].eventName == "openBox")
                {
                    if (BattleManager.CheckAnglePlayer(ac.model,im.overlapEcasterms[0].am.gameObject,45))
                    {
                        im.overlapEcasterms[0].active = false;
                        //开箱时玩家的位移修整（TransformVector将相对坐标改为世界坐标，因为offset是在子物体身上
                        transform.position = im.overlapEcasterms[0].am.transform.position + im.overlapEcasterms[0].am.transform.TransformVector(im.overlapEcasterms[0].offset);
                        ac.model.transform.LookAt(im.overlapEcasterms[0].am.transform, Vector3.up);
                        dm.PlayFrontStab("openBox", this, im.overlapEcasterms[0].am);
                        im.overlapEcasterms.Remove(im.overlapEcasterms[0]);
                    }                   
                }
                else if (im.overlapEcasterms[0].eventName == "leverUp")
                {
                    if (BattleManager.CheckAnglePlayer(ac.model, im.overlapEcasterms[0].am.gameObject, 45))
                    {                      
                        im.overlapEcasterms[0].active = false;
                        //玩家的位移修整（TransformVector将相对坐标改为世界坐标，因为offset是在子物体身上
                        transform.position = im.overlapEcasterms[0].am.transform.position + im.overlapEcasterms[0].am.transform.TransformVector(im.overlapEcasterms[0].offset);
                        ac.model.transform.LookAt(im.overlapEcasterms[0].am.transform, Vector3.up);
                        dm.PlayFrontStab("leverUp", this, im.overlapEcasterms[0].am);
                        im.overlapEcasterms.Remove(im.overlapEcasterms[0]);
                        
                    }
                }

                else if (im.overlapEcasterms[0].eventName == "talkToGita")
                {
                    dm.PlayFrontStab("talkToGita", this, im.overlapEcasterms[0].am);
                    DialogManager.s_Instance.ShowDialogRow();
                }
            }
            
        }
    }


    //利用泛型
    private T Bind<T>(GameObject go) where T:IActorManagerInterface
    {
        T tempInstance;
        if (go==null)
        {
            return null;
        }
        tempInstance = go.GetComponent<T>();
        if (tempInstance==null)
        {
            tempInstance = go.AddComponent<T>();
        }
        tempInstance.am = this;
        return tempInstance;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //用来控制是否能盾反
    public void SetIsCounterBack(bool value)
    {
        sm.isCounterBackEnable = value;
    }


    //被武器打到时，需处理的事件
    public void TryDoDamage(WeaponController targetWc, bool attackValid, bool counterValid)
    {
        //举盾，是否是盾反成功状态
        if (sm.isCounterBackSuccess)
        {
            if (counterValid)
            {
                //对方播放被盾反动画
                targetWc.wm.am.Stunned();
            }         
        }
        //没成功就受击，但不播放受击动画
        else if (sm.isCounterBackFailure)
        {
            if (attackValid)
            {
                HitOrDie(targetWc,false);
            }          
        }
        //是否是无敌状态
        else if (sm.isImmortal)
        {
            //do nothing
        }
        //举盾，是否是防御状态
        else if (sm.isDefense)
        {
            //Attack should be blocked
            Blocked();
        }
        else
        {
            if (attackValid)
            {
                HitOrDie(targetWc,true);
                //if (dm.pd.playableAsset.name == "stab_timeline(Clone)")
                //{
                //    sm.AddHP(-2 * targetWc.GetATK());
                //}
            }          
        }
    }
    //格挡
    public void Blocked()
    {
        ac.IssueTrigger("blocked");
    }
    //被盾反
    public void Stunned()
    {
        ac.IssueTrigger("stunned");
    }

    public void LockUnlockActorController(bool value)
    {
        ac.MySetBool("lock", value);
    }

    //受击或是死亡
    public void HitOrDie(WeaponController targetWc,bool doHitAnimation)
    {
        if (sm.HP <= 0)
        {
            //Already dead
        }
        else
        {
            //受击
            sm.AddHP(-1*targetWc.GetATK());
            if (sm.HP>0)
            {
                //播放受击动画
                if (doHitAnimation)
                {
                    Hit();
                }
                //do some VFX
            }
            else
            {
                Die();
            }
        }
    }
    public void Hit()
    {
        ac.IssueTrigger("hit");
    }
    public void Die()
    {
        ac.IssueTrigger("die");
        //死亡后不允许玩家操作（耦合度太高，后续修改  
        if (gameObject.CompareTag("Player"))
        {
            pc.DisplayDeadPanel();
            ac.pi.inputEnable = false;
            //如果还处于锁定敌人状态，死亡后则解除锁定
            if (ac.camcon.lockState == true)
            {
                ac.camcon.LockUnlock();
            }
            //死亡后镜头锁定
            ac.camcon.enabled = false;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            pc.DisplayEnemyPanel(false);
            //ac.enemys = null;
            //GameManager.instance.playerAc.enemys = null;
            //ac.UnDisplayMiniMap(this.gameObject);
            //Destroy(gameObject,4.0f);
            Invoke("UnDisplayReadyToRestart", 3.6f);
            
        }
        
        
    }

    //敌人死亡后续操作
    private void UnDisplayReadyToRestart()
    {
        gameObject.SetActive(false);
        //加入对象池
        //......
        //掉落道具
        int index = Random.Range(0, 3);
        switch (index)
        {
            case 0:
                GameObject prop0 = (GameObject)Instantiate(Resources.Load("HPProp"), this.transform.position + Vector3.up, Quaternion.identity);
                break;
            case 1:
                GameObject prop1 = (GameObject)Instantiate(Resources.Load("ATKProp"), this.transform.position + Vector3.up, Quaternion.identity);
                break;
            case 2:
                GameObject prop2 = (GameObject)Instantiate(Resources.Load("SkillProp"), this.transform.position + Vector3.up, Quaternion.identity);
                break;
            default:
                break;
        }
        
    }
    public void ChangeDualHands(bool dualOn)
    {
        if (dualOn==false)
        {
            ac.Animator().runtimeAnimatorController = oneHandAnim;           
        }
        else
        {
            ac.Animator().runtimeAnimatorController = twoHandAnim;
        }
    }
}
