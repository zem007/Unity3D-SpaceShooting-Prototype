using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public static Hero S;
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float shieldLevel = 1;
    public bool _______________________;
    public Bounds bounds;

    void Awake() 
    {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //从用户输入(Input)类中获取信息
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        //基于用户的输入，每帧移动飞机的位置
        Vector3 pos = transform.position;
        pos.x += xAxis*speed*Time.deltaTime;
        pos.y += yAxis*speed*Time.deltaTime;
        transform.position = pos;
        //飞机移动时，旋转一个角度，更有动感
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);
    }
}
