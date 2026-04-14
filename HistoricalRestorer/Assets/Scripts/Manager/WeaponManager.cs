using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    [SerializeField]
    private Collider weaponColL;
    [SerializeField]
    private Collider weaponColR;
    //public ActorManager am;

    public GameObject whL;
    public GameObject whR;

    public WeaponController wcL;
    public WeaponController wcR;
    private void Start()
    {
        try
        {
            whL = transform.DeepFind("weaponHandleL").gameObject;
            wcL = BindWeaponController(whL);
            weaponColL = whL.GetComponentInChildren<Collider>();
        }
        catch (System.Exception)
        {
            //If there is no "weaponHandleL" or related object.
        }

        try
        {
            whR = transform.DeepFind("weaponHandleR").gameObject;
            wcR = BindWeaponController(whR);
            weaponColR = whR.GetComponentInChildren<Collider>();
        }
        catch (System.Exception)
        {
            //If there is no "weaponHandleR" or related object.
        }

        //weaponColL = whL.GetComponent<Collider>();
        //weaponColR = whR.GetComponent<Collider>();

    }

    public void UpdateWeaponCollider(string side,Collider col)
    {
        if (side=="L")
        {
            weaponColL = col;
        }
        else if (side=="R")
        {
            weaponColR = col;
        }
    }

    public void UnloadWeapon(string side)
    {
        if (side=="L")
        {
            foreach (Transform tran in whL.transform)
            {
                weaponColL = null;
                wcL.wdata = null;
                Destroy(tran.gameObject);
            }
        }
        else if (side=="R")
        {
            foreach (Transform tran in whR.transform)
            {
                weaponColR = null;
                wcR.wdata = null;
                Destroy(tran.gameObject);
            }
            
        }
    }

    public WeaponController BindWeaponController(GameObject targetObj)
    {
        WeaponController tempWc;
        tempWc = targetObj.GetComponent<WeaponController>();
        if (tempWc==null)
        {
            tempWc=targetObj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }

    //在AC里model.SendMessage("")调用,以及动画事件帧调用
    //控制武器开关
    public void WeaponEnable()
    {
        if (am.ac.CheckStateTag("attackL"))
        {
            weaponColL.enabled = true;
        }
        else
        {
            weaponColR.enabled = true;
        }
        
    }
    public void WeaponDisable()
    {
        weaponColR.enabled = false;
        if (weaponColL==null)
        {
            return;
        }
        weaponColL.enabled = false;
        
    }
    //动画事件 
    public void CounterBackEnable()
    {
        am.SetIsCounterBack(true);
    }
    public void CounterBackDisable()
    {
        am.SetIsCounterBack(false);
    }
    public void ChangeDualHands(bool dualOn)
    {
        am.ChangeDualHands(dualOn);
    }
}
