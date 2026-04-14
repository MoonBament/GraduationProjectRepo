using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateController : MonoBehaviour
{
    public Slider bloodSlider;      //人物显示血条
    public Slider enduranceSlider;  //人物显示耐力条
    public Text skillOutputValue;   //人物技能值
    public StateManager sm;
    private void Awake()
    {
        sm = GetComponent<StateManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //进行切换角色后的数值替换赋予操作模块
    //
    public void ChangeRole()
    {
        //把角色做成枚举，然后通过判断，在角色身上应该带点东西（加个脚本，用来存储角色自身的属性值（捏麻我还是书看完再回来写，不然写出来的都是什么狗屎东西
    }
}
