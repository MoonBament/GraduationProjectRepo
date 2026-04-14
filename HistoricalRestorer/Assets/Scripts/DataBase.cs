using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public class DataBase 
{
    public string weaponDatabaseFileName = "weaponData";
    public readonly JSONObject weaponDataBase;
    public DataBase()
    {
        TextAsset weaponContent = (TextAsset)Resources.Load(weaponDatabaseFileName);
        weaponDataBase = new JSONObject(weaponContent.text);
        //print(weaponDataBase["Falchion"]["ATK"].floatValue);
    }


}
