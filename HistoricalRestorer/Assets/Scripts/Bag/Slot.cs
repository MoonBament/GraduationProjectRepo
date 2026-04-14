using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//背包物品格
public class Slot : MonoBehaviour
{
    //public Item slotItem;  //
    //public Image slotImage;//物体图片
    //public Text slotNum;   //物体的数量

    public BagItem slotbagItem;
    public Image slotbagImage;
    public Text slotbagNum;
    public void ItemOnClicked()
    {
        //InventoryManager.UpdataItemInfo(slotItem.itemInfo);
        //InventoryManager.DisplayPanel(slotItem.equip);

        InventoryManager.UpdataItemInfo(slotbagItem.tips);
        InventoryManager.UpdataItemName(slotbagItem.name);
        InventoryManager.DisplayPanel(slotbagItem.equip);
    }
   
}
