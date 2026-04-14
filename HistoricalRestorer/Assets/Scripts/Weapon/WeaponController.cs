using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponManager wm;
    public WeaponData wdata;
    //public DynamicTextData textData;
    private void Awake()
    {
        wdata = GetComponentInChildren<WeaponData>();
    }

    /// <summary>
    /// 返回武器伤害+人物基础伤害
    /// </summary>
    /// <returns></returns>
    public float GetATK()
    {
        return wdata.ATK + wm.am.sm.ATK; ;
    }
}
