using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyIUserInput : IUserInput {
    // Start is called before the first frame update
    public bool isFindPlayer=false;
    public Collider[] cols;
    public bool notEnemy;
    public StateManager sm;
    //IEnumerator Start()
    //{
    //    while (true)
    //    {
    //        rb = true;//一直攻击
    //        yield return 0;
    //    }
    //    //Dup = 1.0f;
    //    //Dright = 0;

    //}

    private void Awake()
    {
        sm = GetComponent<StateManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (notEnemy!=true)
        {
            CheckPlayer();
        }
        
    }

    public void CheckPlayer()
    {
        Vector3 modelOrigin1 = transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);//往模型身高上1单位向量
        Vector3 boxCenter = modelOrigin2 + transform.forward;//往前探半径5,即10单位向量范围
        cols = Physics.OverlapBox(boxCenter, new Vector3(5f, 0.5f, 5f), transform.rotation, LayerMask.GetMask("Player"));

        if (cols.Length != 0)
        {
            isFindPlayer = true;
            
        }
        else
        {
            isFindPlayer = false;
            //foreach (var col in cols)
            //{

            //}
        }
        if (sm.HP > 0)
        {
            AttackPlayer(isFindPlayer);
        }
        
    }

    //角色面向玩家
    public void AttackPlayer(bool isFind)
    {
        if (isFind || sm.HP > 0)
        {
            transform.LookAt(cols[0].transform,Vector3.up);
            if (tag.Contains("Enemy"))
            {
                if (Vector3.Distance(cols[0].transform.position, this.transform.position) < 3.5f)
                {
                    rb = true;
                    Dup = 0.05f;
                }
                UpdateDmagDvec(Dup, Dright);
            }          
        }
        else
        {
            UpdateDmagDvec(0, 0);
            rb = false;
            Dup = 0f;
            //Dup = 1.1f;
        }
    }
}
