using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject bagPanel;
    public GameObject deadPanel;
    public GameObject enemyPanel;
    bool isOpen=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OpenMyBug();
    }
    void OpenMyBug()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            InventoryManager.instance.RefreshBagUI();
        }
    }
    public void DisplayDeadPanel()
    {
        deadPanel.SetActive(true);
    }

    public void DisplayEnemyPanel(bool isDisplay)
    {
        enemyPanel.SetActive(isDisplay);
    }
}
