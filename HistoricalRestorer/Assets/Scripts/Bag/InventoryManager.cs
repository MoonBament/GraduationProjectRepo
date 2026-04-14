using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Inventory myBag;
    //public BagItems bagItems;
    public MyBagNow bagNow;
    public GameObject slotGrid;
    public Slot slotPrefab;
    public Text itemInformation;
    public GameObject useButPan;

    public List<Sprite> sprites = new List<Sprite>();
    public string name;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(this);
    }

    private void OnEnable()
    {
        RefreshBagUI();
    }
    public void RefreshBagUI()
    {
        RefreshItem();
        instance.itemInformation.text = "";
    }
    /// <summary>
    /// 在背包物品详细栏实时更新物体的信息
    /// </summary>
    /// <param name="itemDescription"></param>
    public static void UpdataItemInfo(string itemDescription)
    {
        instance.itemInformation.text = itemDescription;
        
    }
    public static void UpdataItemName(string item_name)
    {
        instance.name = item_name;
    }
    //将物品添加显示到背包内
    //public static void CreateNewItem(Item item)
    //{
    //    Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
    //    newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
    //    newItem.slotItem = item;
    //    newItem.slotImage.sprite = item.itemImage;
    //    newItem.slotNum.text = item.itemHeld.ToString();
    //}
    public static void CreateNewItem(BagItem item)
    {
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
        newItem.slotbagItem = item;
        newItem.slotbagImage.sprite = instance.sprites[item.icon];
        newItem.slotbagNum.text = item.itemHeld.ToString();
    }
    /// <summary>
    /// 关闭游戏再次打开背包时，背包内部物品显示
    /// </summary>
    //public static void RefreshItem()
    //{
    //    //删除背包下的所有东西
    //    for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
    //    {
    //        if (instance.slotGrid.transform.childCount==0)
    //        {
    //            break;
    //        }
    //        Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
    //    }
    //    //遍历背包列表，将背包列表里的东西生成
    //    for (int i = 0; i < instance.myBag.itemList.Count; i++)
    //    {
    //        CreateNewItem(instance.myBag.itemList[i]);
    //    }
    //}
    public static void RefreshItem()
    {
        //删除背包下的所有东西
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.childCount == 0)
            {
                break;
            }
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
        }
        //遍历背包列表，将背包列表里的东西生成
        for (int i = 0; i < instance.bagNow.info.Count; i++)
        {
            CreateNewItem(instance.bagNow.info[i]);
        }

    }
        /// <summary>
        /// 点击物品是否显示使用/装备按钮
        /// </summary>
        /// <param name="isEuip"></param>
        public static void DisplayPanel(bool isEuip)
    {
        instance.useButPan.SetActive(isEuip);
    }
    public static void DisplayPanel(int isEuip)
    {
        if (isEuip==0)
        {
            instance.useButPan.SetActive(false);
        }
        else if(isEuip==1)
        {
            instance.useButPan.SetActive(true);
        }
        
    }
}
