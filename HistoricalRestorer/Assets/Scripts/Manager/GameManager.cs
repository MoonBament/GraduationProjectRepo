using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WeaponManager testwm;
    public static GameManager instance;
    public ActorController playerAc;
    private DataBase weaponDB;
    private WeaponFactory weaponFact;
    public Dictionary<int, BagItem> itemInfos = new Dictionary<int, BagItem>();
    public string myBagJsonPath;
    public MyBagNowItems myBagNowItems;//要存入的对象
    private void Awake()
    {
        InitJson();
        CheckGameObject();
        CheckSingle();
        playerAc = testwm.GetComponentInParent<ActorController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitWeaponDB();
        InitWeaponFactory();

        Collider col= weaponFact.CreateWeapon("Sword", "R", testwm);
        testwm.UpdateWeaponCollider("R", col);
        testwm.ChangeDualHands(false);
    }

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 150, 30), "wear R Sword"))
    //    {
    //        testwm.UnloadWeapon("R");
    //        Collider col = weaponFact.CreateWeapon("Sword", "R", testwm);
    //        testwm.UpdateWeaponCollider("R", col);
    //        testwm.ChangeDualHands(false);
    //    }
    //    if (GUI.Button(new Rect(0, 40, 150, 30), "wear R Falchion"))
    //    {
    //        testwm.UnloadWeapon("R");
    //        Collider col = weaponFact.CreateWeapon("Falchion", "R", testwm);
    //        testwm.UpdateWeaponCollider("R", col);
    //        testwm.ChangeDualHands(true);
    //    }
    //    if (GUI.Button(new Rect(0, 80, 150, 30), "wear R Mace"))
    //    {
    //        testwm.UnloadWeapon("R");
    //        Collider col = weaponFact.CreateWeapon("Mace", "R", testwm);
    //        testwm.UpdateWeaponCollider("R", col);
    //        testwm.ChangeDualHands(false);
    //    }
    //    if (GUI.Button(new Rect(0, 120, 150, 30), "Clear All Weapon"))
    //    {
    //        testwm.UnloadWeapon("R");
    //        testwm.ChangeDualHands(false);
    //    }
    //}


    public void InitWeaponDB()
    {
        weaponDB = new DataBase();
    }
    public void InitWeaponFactory()
    {
        weaponFact = new WeaponFactory(weaponDB);
    }
    private void CheckGameObject()
    {
        if (tag=="GM")
        {
            return;
        }
        Destroy(this);
    }
    public void CheckSingle()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(this);
        
    }


    //这下面这块东西应该放在WeaponManager或者WeaponController里，testwm就是挂在角色身上的WeaponManeger
    /// <summary>
    /// 通过名称装备背包里的物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void EquipWeapon(string itemName)
    {
        //是否是双手
        bool dualHands=false;

        if (itemName=="Sword")
        {
            dualHands = false;
            ChangeWeapon(itemName,"R", dualHands);         
        }
        else if (itemName=="Falchion")
        {
            dualHands = true;
            testwm.UnloadWeapon("L");
            ChangeWeapon(itemName,"R", dualHands);
        }
        else if (itemName=="Mace")
        {
            dualHands = false;
            ChangeWeapon(itemName, "R", dualHands);
        }
        else if (itemName=="Shield")
        {
            dualHands = false;
            ChangeWeapon(itemName, "L", dualHands);
            if (testwm.wcR.wdata.name=="Falchion(Clone)")
            {
                testwm.UnloadWeapon("R");
            }
        }
        else if (itemName =="HPPotion")
        {
            //血量上限加20
            playerAc.am.sm.HPMax += 20;
            //恢复当前血量的百分之20
            playerAc.am.sm.AddHP(playerAc.am.sm.HP * 0.2f);
            UpdateBagItemInfo(itemName);
            InventoryManager.instance.RefreshBagUI();
        }
        else if (itemName == "ATKPotion")
        {
            //自身攻击力加百分之10
            playerAc.am.sm.ATK += playerAc.am.sm.ATK*0.1f;
            UpdateBagItemInfo(itemName);
            InventoryManager.instance.RefreshBagUI();
        }
        else if (itemName == "SkillPotion")
        {
            //技能上限加20
            playerAc.am.sm.skillValueMax += 20;
            //恢复20点技能
            playerAc.am.sm.skillValue += 20;
            UpdateBagItemInfo(itemName);
            InventoryManager.instance.RefreshBagUI();
        }
    }

    private void UpdateBagItemInfo(string n)
    {
        //for (int i = 0; i < grid.childCount; i++)
        //{
        //    if (grid.GetChild(i).gameObject.GetComponent<Slot>().slotbagItem.name == n && grid.GetChild(i).gameObject.GetComponent<Slot>().slotbagItem.itemHeld==0)
        //    {
        //        Destroy(grid.GetChild(i).gameObject);
        //    }
            
        //}
        for (int i = 0; i < playerAc.mbn.info.Count; i++)
        {
            if (playerAc.mbn.info[i].name == n)
            {
                playerAc.mbn.info[i].itemHeld--;
                if (playerAc.mbn.info[i].itemHeld ==0)
                {
                    playerAc.mbn.info.RemoveAt(i);
                    return;
                }
                return;
            }
        }
    }

    public void ChangeWeapon(string weaponName,string side,bool dualHands)
    {
        testwm.UnloadWeapon(side);
        Collider col = weaponFact.CreateWeapon(weaponName, side, testwm);
        testwm.UpdateWeaponCollider(side, col);
        testwm.ChangeDualHands(dualHands);
    }

    /// <summary>
    /// 在背包里点击“使用”，装备武器
    /// </summary>
    public void UseOnCliked()
    {
        GameManager.instance.EquipWeapon(InventoryManager.instance.name);
    }

    /// <summary>
    /// 初始化道具数据
    /// </summary>
    public void InitJson()
    {
        //读取对应文件里的文本
        string info = Resources.Load<TextAsset>("Json/Inventory").text;
        //Debug.Log(info);
        //将文件转化为对应的数据结构
        BagItems items = JsonUtility.FromJson<BagItems>(info);
        //Debug.Log(items.info.Count);
        for (int i = 0; i < items.info.Count; i++)
        {
            itemInfos.Add(items.info[i].id, items.info[i]);
        }
        //存储目前背包里的本地路径
        myBagJsonPath = Application.persistentDataPath + "/MyBagNowJson.txt";
    }

    public void SaveJson()
    {
        if (!File.Exists(myBagJsonPath))
        {
            //本地没有该文件，就创建
            File.Create(myBagJsonPath);
        }
        //玩家里背包现有数据写入文档里
        string json = JsonUtility.ToJson(myBagNowItems, true);
        File.WriteAllText(myBagJsonPath, json);
    }
    public void ReadJson()
    {
        if (!File.Exists(myBagJsonPath))
        {
            Debug.LogError("读取的文件不存在！");
            return;
        }

        //string json = File.ReadAllText(JsonPath);
    }
}

