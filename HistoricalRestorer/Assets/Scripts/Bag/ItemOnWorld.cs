using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在场景里存在的Item身上
public class ItemOnWorld : MonoBehaviour
{
    //public Item thisItem;
    //public Inventory playerInv;
    public int id;
    public BagItem bagItem;
    //public BagItems bagItems;
    public MyBagNow bagNow;
    void Start()
    {
        bagItem = GameManager.instance.itemInfos[id];
        bagNow = InventoryManager.instance.bagNow;
    }
    //碰到就自动捡起
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AddNewItem();
            //实时更新Json数据
            //......
            Destroy(gameObject,0.4f);
        }
    }

    //public void AddNewItem()
    //{
    //    //如果背包列表里有该种类物品，该物品数量＋1
    //    if (playerInv.itemList.Contains(thisItem))
    //    {
    //        thisItem.itemHeld += 1;
    //    }
    //    //否则就将该物品新添至背包里
    //    else
    //    {
    //        playerInv.itemList.Add(thisItem);
    //        //InventoryManager.CreateNewItem(thisItem);
    //    }
    //    InventoryManager.RefreshItem();
    //}
    public void AddNewItem()
    {
        //如果背包列表里有该种类物品，该物品数量＋1
        if (bagNow.info.Contains(bagItem))
        {
            bagItem.itemHeld += 1;
        }
        //否则就将该物品新添至背包里
        else
        {
            bagNow.info.Add(bagItem);
            //InventoryManager.CreateNewItem(thisItem);
        }
        InventoryManager.RefreshItem();
    }

}
