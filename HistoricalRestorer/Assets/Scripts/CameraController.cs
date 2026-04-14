using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //public PlayerInput pi;
    public IUserInput pi;
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private GameObject camera1;

    public bool lockState;//锁定状态旗标
    public Image lockDot;//屏幕上的点点
    public float horizontalspeed;
    public float verticalspeed;

    private float tempEulerX;
    private Vector3 cameraDampVelocity;
    public  LockTarget lockTarget;
    public bool isAI = false;
    private ActorController ac;

    // Start is called before the first frame update
    void Start()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 7;

        ac = playerHandle.GetComponent<ActorController>();
        //人物模型
        model = ac.model;
        //model = playerHandle.GetComponent<ActorController>().model;
        IUserInput[] inputs = playerHandle.GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = input;
                break;
            }
        }
        if (!isAI)
        {
            camera1 = Camera.main.gameObject;
            lockDot.enabled = false;
            //Cursor.lockState = CursorLockMode.Locked;//鼠标在游戏运行时隐藏
        }
        lockState = false;       
    }
   
    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockTarget==null)
        {
            //存储人物模型角度，保持其在镜头旋转时不变
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalspeed * Time.fixedDeltaTime);//镜头左右

            tempEulerX -= pi.Jup * verticalspeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(    //镜头上下
                tempEulerX, 0, 0);
            model.transform.eulerAngles = tempModelEuler;
        }
        else //锁定敌人状态
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;//使胶囊体的正前方变成一直朝向锁定物体的方向
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }

        if (!isAI)
        {
            //镜头追踪角色                                    相机位置                      追踪目标的位置   中间变量（不用特地给定数值  速度
            camera1.transform.position = Vector3.SmoothDamp(camera1.transform.position, transform.position, ref cameraDampVelocity, 0.12f);//用SmoothDamp做镜头缓冲
            camera1.transform.eulerAngles = transform.eulerAngles;
        }
        
    }

    private void Update()
    {
        //人物模型,实时更新
        model = ac.model;
        if (lockTarget!=null)
        {
            //当锁定目标时，点点在被锁定方的半高上
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            //超过距离锁定就失效
            if (Vector3.Distance(model.transform.position,lockTarget.obj.transform.position)>10.0f)
            {
                LockProcessA(null, false, false, isAI);
            }
            //敌人死亡后取消锁定
            //判断对方身上是否有ActorManager
            if (lockTarget.am!=null && lockTarget.am.sm.isDie)
            {
                LockProcessA(null, false, false, isAI);
            }
        }
    }

    private void LockProcessA(LockTarget _lockTarget,bool _lockDotEnable,bool _lockState,bool _isAI)
    {
        lockTarget = _lockTarget;
        if (!_isAI)
        {
            lockDot.enabled = _lockDotEnable;
        }
        lockState = _lockState;
    }

    /// <summary>
    /// <para>是否锁定敌人</para>
    /// <param name="cols">锁定对象的数组</param>
    /// </summary>
    public void LockUnlock()
    {
        //if (lockTarget==null)
        //{
            
        //}
        //try to lock
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);//往模型身高上1单位向量
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;//往前探半径5,即10单位向量范围
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask(isAI?"Player":"Enemy"));
        Debug.Log(cols.Length);
        if (cols.Length == 0)
        {
            LockProcessA(null, false, false, isAI);
        }
        else
        {
            foreach (var col in cols)
            {
                //是否是敌人本身
                if (col.CompareTag("Enemy"))
                {
                    if (lockTarget != null && lockTarget.obj == col.gameObject)//再次选中该物体，则取消锁定
                    {
                        LockProcessA(null, false, false, isAI);
                        break;
                    }
                    //bounds.extents：xyz的半长
                    LockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                    break;
                }               
            }
        }
    }

    public class LockTarget
    {
        public GameObject obj;
        public float halfHeight;//身高的半高
        public ActorManager am;

        public LockTarget(GameObject _obj, float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
            am = _obj.GetComponent<ActorManager>();
        }
    }

}
