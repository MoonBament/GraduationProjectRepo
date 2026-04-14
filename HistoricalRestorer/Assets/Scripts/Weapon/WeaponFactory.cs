using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory 
{
    private DataBase weaponDB;

    public WeaponFactory(DataBase _weaponDB)
    {
        weaponDB = _weaponDB;
    }
    public GameObject CreateWeapon(string weaponName, Vector3 pos, Quaternion rot)
    {
        GameObject prefab = (GameObject)Resources.Load(weaponName);
        GameObject obj= GameObject.Instantiate(prefab, pos, rot);

        WeaponData wdata = obj.AddComponent<WeaponData>();
        wdata.ATK = weaponDB.weaponDataBase[weaponName]["ATK"].floatValue;
        return obj;
    }
    public Collider CreateWeapon(string weaponName,string side,WeaponManager wm)
    {
        WeaponController wc;
        if (side=="L")
        {
            wc = wm.wcL;
        }
        else if(side=="R")
        {
            wc = wm.wcR;
        }
        else
        {
            return null;
        }

        GameObject prefab = (GameObject)Resources.Load(weaponName);
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = wc.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        WeaponData wdata = obj.AddComponent<WeaponData>();
        wdata.ATK = weaponDB.weaponDataBase[weaponName]["ATK"].floatValue;
        wc.wdata = wdata;
        return obj.GetComponent<Collider>();
    }
}
