using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private RectTransform rect;
    private Transform player;
    private static Image item;
    private Image playerImage;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        item = Resources.Load<Image>("PlayerI");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player!=null)
        {
            playerImage = Instantiate(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlayer();
    }
    public void ShowPlayer()
    {
        //玩家图标在地图上的大小
        playerImage.rectTransform.sizeDelta = new Vector2(30, 30);
        //玩家图标始终位于地图中点
        playerImage.rectTransform.anchoredPosition = new Vector2(0, 0);
        playerImage.sprite = Resources.Load<Sprite>("_GFX/playerIcon");
        //玩家图标角度跟玩家旋转方向同步
        playerImage.rectTransform.eulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
        playerImage.transform.SetParent(transform, false);
    }
    public void ShowEnemy(Image image,float disX,float disY)
    {
        image.rectTransform.sizeDelta = new Vector2(30, 30);
        image.rectTransform.anchoredPosition = new Vector2(disX * 150, disY * 150);
        image.sprite = Resources.Load<Sprite>("_GFX/enemyI");
        image.transform.SetParent(transform, false);
    }
    public static Image CreateImage()
    {
        return Instantiate(item);
    }
}
