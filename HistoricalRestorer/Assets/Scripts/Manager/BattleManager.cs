using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//避免出现忘记加Collider的情况
[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    //扇形角度
    [SerializeField] private float angle = 80f;
    //扇形半径
    [SerializeField] private float radius = 3.5f;
    public List<Transform> b =new List<Transform>();               //大范围检测到的敌人（从外部添加
    public List<Transform> enemysInScope=new List<Transform>();
    public List<Transform> enemysInScope2 = new List<Transform>();
    public bool flag2;
    public bool flag3;

    //public ActorManager am;
    private CapsuleCollider decol;

    private void Start()
    {
        //对CapsuleCollider做初始化数值设定
        decol = GetComponent<CapsuleCollider>();
        decol.center = Vector3.up * 1.0f;
        decol.height = 2.0f;
        decol.radius = 0.5f;
        decol.isTrigger = true;
    }

    private void Update()
    {
        flag2 = IsInRange(angle, radius, transform, b);
        flag3 = IsInRange(2, transform, b);
    }


    private void OnTriggerEnter(Collider col)
    {
        //拿到对方的WeaponController
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        if (targetWc == null)
        {
            return;
        }
        //Debug.Log(col.name);
        //攻击方
        GameObject attacker = targetWc.wm.am.gameObject;
        //受击方
        GameObject receiver = am.ac.model;
        //攻击方恢复技能值
        targetWc.wm.am.sm.skillValue += 2;
        //如果被武器打到，就做相应的事，例如判断扣血之类的
        if (col.CompareTag("Weapon") && attacker.layer != receiver.layer)
        {
            //Debug.Log(targetWc.name);
            //                        是否可攻击                          是否可盾反
            am.TryDoDamage(targetWc, CheckAngelTarget(receiver, attacker, 70), CheckAnglePlayer(receiver, attacker, 30));
            return;
        }
        if (targetWc != null && receiver.layer == 9)
        {
            //Debug.Log(targetWc.name);
            am.TryDoDamage(targetWc, CheckAngelTarget(receiver, attacker, 70), CheckAnglePlayer(receiver, attacker, 30));
        }

    }

    //从玩家出发点来查核角度是否能进行互动
    public static bool CheckAnglePlayer(GameObject player,GameObject target,float playerAngelLimit)
    {
        Vector3 counterDir = target.transform.position - player.transform.position;

        //算出敌方是否在自己的盾反范围内
        float counterAngle1 = Vector3.Angle(player.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward);//should be closed to 180 degree

        //判断是否合理
        bool counterValid = (counterAngle1 < playerAngelLimit && Mathf.Abs(counterAngle2 - 180) < playerAngelLimit);

        return counterValid;
    }
    //从target出发点来查核角度是否能进行互动
    public static bool CheckAngelTarget(GameObject player,GameObject target,float targetAngelLimit)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position;

        //算出自己与敌方正前方的夹角（判断是否在攻击范围内
        float attackingAngle1 = Vector3.Angle(target.transform.forward, attackingDir);

        //判断是否合理
        bool attackValid = (attackingAngle1 < targetAngelLimit);

        return attackValid;
    }

    /// <summary>
    /// 判断target是否在区域内
    /// </summary>
    /// <param name="sectorAngle">角度</param>
    /// <param name="sectorRadius">半径</param>
    /// <param name="attacker">攻击者的transform信息</param>
    /// <param name="targets">目标</param>
    /// <returns>目标target在扇形区域内返回true 否则返回false</returns>
    public bool IsInRange(float sectorAngle, float sectorRadius, Transform attacker, List<Transform> targets)
    {
        if (targets.Count==0)
        {
            return false;
        }
        foreach (Transform target in targets)
        {
            //攻击者位置指向目标位置的向量
            Vector3 direction = target.position - attacker.position;
            //点乘积结果
            float dot = Vector3.Dot(direction.normalized, transform.forward);
            //反余弦计算角度
            float offsetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (offsetAngle < sectorAngle * .5f && direction.magnitude < sectorRadius)
            {
                if (!enemysInScope.Contains(target))
                {
                    enemysInScope.Add(target);
                }          
            }
            else
            {
                if (enemysInScope.Contains(target))
                {
                    enemysInScope.Remove(target);
                }
            }
        }
        return enemysInScope.Count == 0 ? false : true;
        
    }

    public bool IsInRange(float sectorRadius,Transform attacker, List<Transform> targets)
    {
        if (targets.Count == 0)
        {
            return false;
        }
        foreach (Transform target in targets)
        {
            //攻击者位置指向目标位置的向量
            Vector3 direction = target.position - attacker.position;
            //点乘积结果
            float dot = Vector3.Dot(direction.normalized, transform.forward);
            //反余弦计算角度
            float offsetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (offsetAngle < 360 * .5f && direction.magnitude < sectorRadius)
            {
                if (!enemysInScope2.Contains(target))
                {
                    enemysInScope2.Add(target);
                }
            }
            else
            {
                if (enemysInScope2.Contains(target))
                {
                    enemysInScope2.Remove(target);
                }
            }
        }
        return enemysInScope2.Count == 0 ? false : true;

    }

    /// <summary>
    /// 范围伤害
    /// </summary>
    public void ScopeHit(float value,int no)
    {
        switch (no)
        {
            case 2:
                if (enemysInScope2 != null)
                {
                    foreach (Transform item in enemysInScope2)
                    {
                        //造成自身攻击力百分之value的伤害
                        item.gameObject.GetComponent<StateManager>().HP -= am.sm.ATK * value;
                        item.gameObject.GetComponent<ActorManager>().Hit();
                    }
                }
                break;
            case 3:
                if (enemysInScope != null)
                {
                    foreach (Transform item in enemysInScope)
                    {
                        //造成自身攻击力百分之value的伤害
                        item.gameObject.GetComponent<StateManager>().HP -= am.sm.ATK * value;
                        item.gameObject.GetComponent<ActorManager>().Hit();
                    }
                }
                break;
            default:
                Debug.LogError("无当前匹配的范围伤害");
                break;
        }
        
    }
    private void OnDrawGizmos()
    {
        Handles.color = flag2 ? Color.cyan : Color.red;

        float x = radius * Mathf.Sin(angle / 2f * Mathf.Deg2Rad);
        float y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2f));
        Vector3 a = new Vector3(transform.position.x - x, transform.parent.position.y, transform.position.z + y);
        Vector3 b = new Vector3(transform.position.x + x, transform.parent.position.y, transform.position.z + y);

        Handles.DrawLine(transform.position, a);
        Handles.DrawLine(transform.position, b);

        float half = angle / 2;
        for (int i = 0; i < half; i++)
        {
            x = radius * Mathf.Sin((half - i) * Mathf.Deg2Rad);
            y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2f));
            a = new Vector3(transform.position.x - x, transform.parent.position.y, transform.position.z + y);
            x = radius * Mathf.Sin((half - i - 1) * Mathf.Deg2Rad);
            y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2f));
            b = new Vector3(transform.position.x - x, transform.parent.position.y, transform.position.z + y);

            Handles.DrawLine(a, b);
        }
        for (int i = 0; i < half; i++)
        {
            x = radius * Mathf.Sin((half - i) * Mathf.Deg2Rad);
            y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2f));
            a = new Vector3(transform.position.x + x, transform.parent.position.y, transform.position.z + y);
            x = radius * Mathf.Sin((half - i - 1) * Mathf.Deg2Rad);
            y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2f));
            b = new Vector3(transform.position.x + x, transform.parent.position.y, transform.position.z + y);

            Handles.DrawLine(a, b);
        }
    }

    //战士技能特效
    public void SoldierFirstSkillE(GameObject obj,Vector3 vector3)
    {      
        //特效
        GameObject ef = Instantiate(obj);
        ef.transform.position = this.transform.position + vector3;
        ////倒计时10秒
        //float time = 10f;
        //StartCoroutine(ChangeTime(time, atk));
        Destroy(ef, 10);
    }
    public void SoldierFirstSkillE(GameObject obj, Vector3 vector3,Transform parent)
    {
        //特效
        GameObject ef = Instantiate(obj);      
        ef.transform.forward = parent.forward;
        ef.transform.position = parent.position + vector3;
        Destroy(ef, 10);
    }
    //private IEnumerator ChangeTime(float time,float atk)
    //{
    //    while (time > 0)
    //    {
    //        //Debug.Log(time);
    //        yield return new WaitForSeconds(1);
    //        time--;           
    //    }
    //    Debug.Log(time);
    //    RestorePlayerATK(atk);
    //}
    //private void RestorePlayerATK(float atk)
    //{
    //    am.sm.ATK = atk;
    //}
}
