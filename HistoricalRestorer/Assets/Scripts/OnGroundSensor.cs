using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour         //落地侦测器
{
    public CapsuleCollider capsule;    //得到该物体的Collier
    public float offset = 0.1f;

    private Vector3 point1;//capsule下面球的球心
    private Vector3 point2;//capsule上面球的球心
    private float radius;


    // Start is called before the first frame update
    void Awake()
    {
        radius = capsule.radius-0.05f;   //得到capsule的半径

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        point1 = transform.position + transform.up * (radius-offset);
        point2 = transform.position + transform.up * (capsule.height-offset) - transform.up * radius;

        Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Environment"));//用Collider数组存
        if (outputCols.Length != 0)//撞到东西就不等于零
        {
            SendMessageUpwards("isGround");//调用母物体的“isGround”方法
        }
        else 
        {
            SendMessageUpwards("isNotGround");
        }
        
    }
}
