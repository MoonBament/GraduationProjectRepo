using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BagItem
{
    public int id;
    public string name;
    public int icon;
    public int type;//0:ÎäÆ÷ 1£º¶ÜÅÆ 2£ºÀúÊ·ËéÆ¬ 3£ºÒ©Ë®
    public string tips;
    public int itemHeld;
    public int equip;
}
