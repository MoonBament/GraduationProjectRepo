using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : IActorManagerInterface
{
    public static DialogManager s_Instance;
    public TextAsset dialogDataFile;  //对话文本文件，UTF.csv格式(弄成数组，因为到时候有很多对话的说，
    //public SpriteRenderer spriteLeft;
    //public SpriteRenderer spriteRight;
    public GameObject dialogPan;
    public Text nameText;
    public Text dialogText;
    //public List<Sprite> sprites = new List<Sprite>(); //角色图片列表
    //private Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();//角色名字对应图片的字典
    public int dialogIndex;        //准备显示的对话索引值
    private string[] dialogRows;   //将文件里按行存储
    public Button nextButton;      //对话继续按钮
    public Text nextButtonText;
    public GameObject optionButton;//选项按钮预制体
    public Transform buttonGroup;  //选项按钮父节点，用于自动排列
    public List<Person> people = new List<Person>();
    private KeyboardInput kpi;
    private void Awake()
    {
        s_Instance = this;
        //imageDic["艾雅法拉"] = sprites[0];
        //imageDic["博士"] = sprites[1];
        Person person = new Person();
        person.name = "艾雅法拉";
        people.Add(person);
        Person doctor = new Person();
        doctor.name = "博士";
        people.Add(doctor);
    }
    // Start is called before the first frame update
    void Start()
    {
        ReadText(dialogDataFile);
        //到时候写在交互上
        //ShowDialogRow();
        kpi = (KeyboardInput)am.ac.pi;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReadText(TextAsset textAsset)
    {
        //一行一行的读取文件中的数据
        dialogRows = textAsset.text.Split('\n');
    }
    public void UpdateText(string name,string text)
    {
        nameText.text = name;
        dialogText.text = text;
    }
    public void UpdateImage(string name,string position)
    {
        if (position =="left")
        {
            //spriteLeft.sprite = imageDic[name];
        }
        else if(position =="right")
        {
            //spriteRight.sprite = imageDic[name];
        }
    }
    /// <summary>
    /// 玩家屏幕显示具体对话
    /// </summary>
    public void ShowDialogRow()
    {
        DialogPanelDisplay(true);
        //对话期间禁止玩家对角色进行控制
        kpi.inputEnable = false;        
        kpi.mouuseEnable = false;
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            //当#：对话，，从第二行开始读取
            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                //读取人物名及对话内容
                UpdateText(cells[2], cells[4]);
                //UpdateImage(cells[2], cells[3]);
                nextButtonText.text = "继续";
                nextButton.gameObject.SetActive(true);
                //跳转id:dialogIndex
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            //如果是玩家可选择的选项&
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            //到END，剧情结束关闭对话框
            else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {
                //nextButtonText.text = "关闭";
                DialogPanelDisplay(false);
                //剧情结束时，所需做的相关操作
                //对话结束，玩家可继续控制角色
                kpi.inputEnable = true;
                kpi.mouuseEnable = true;
                Debug.Log("剧情结束");
            }
        } 
    }
    /// <summary>
    /// 对话继续按钮被点击
    /// </summary>
    public void OnClickNext()
    {
        ShowDialogRow();
    }
    /// <summary>
    /// 生成选项按钮
    /// </summary>
    /// <param name="index">当前行</param>
    public void GenerateOption(int index)
    {
        string[] cells = dialogRows[index].Split(',');
        //如果选项就生成对应数量的选项按钮
        if (cells[0]=="&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            //按钮UI显示对应选项文本
            button.GetComponentInChildren<Text>().text = cells[4];
            //绑定按钮点击事件
            button.GetComponent<Button>().onClick.AddListener
                (
                    delegate
                    {
                        //委托调用
                        OnOptionClick(int.Parse(cells[5]));
                        //如果效果栏不为空
                        if (cells[6]!="")
                        {
                            //效果@参数
                            string[] effect = cells[6].Split('@');
                            //因为csv文件最后一列会带有奇怪的符号，手动删除这些符号，或者表格里最后一列全写空，不去读最后一列即可
                            cells[7] = Regex.Replace(cells[7], @"[\r\n]", "");

                            OptionEffect(effect[0], int.Parse(effect[1]), cells[7]);
                        }
                    }
                );
            //判断下一行是否还是选项
            GenerateOption(index + 1);
        }
    }
    /// <summary>
    /// 选项按钮被点击
    /// </summary>
    /// <param name="id">即将跳转到文件里第几行的对话</param>
    public void OnOptionClick(int id)
    {
        dialogIndex = id;
        ShowDialogRow();
        //可优化，把按钮做成对象池
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// 选项按钮被点击后产生的效果
    /// </summary>
    /// <param name="effect">效果</param>
    /// <param name="parm">值</param>
    /// <param name="target">效果目标</param>
    public void OptionEffect(string effect,int parm,string target)
    {
        if (effect=="好感度加")
        {
            foreach (var person in people)
            {
                if (person.name==target)
                {
                    person.likeValue += parm;
                }
            }
        }
        else if (effect == "体力值加")
        {
            foreach (var person in people)
            {
                if (person.name == target)
                {
                    person.strengValue += parm;
                }
            }
        }
    }

    public void DialogPanelDisplay(bool isDiaplay)
    {
        dialogPan.SetActive(isDiaplay);
    }
}
