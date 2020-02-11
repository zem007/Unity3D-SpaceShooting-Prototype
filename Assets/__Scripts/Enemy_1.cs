using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Inspect after Enemy")]
    public float waveFrequency = 2;   //完成一个完整正弦所需时间
    public float waveWidth = 4;
    public float waveRotY = 45;
    private float x0;
    private float birthTime;

    // Start is called before the first frame update
    void Start()
    {
        //Start之前的Main.SpawnEnemy()已经完成了对初始位置的设定
        x0 = pos.x;
        birthTime = Time.time;
    }

    //重写Enemy类中的Move虚函数
    public override void Move() {
        Vector3 tempPos = pos;
        //基于时间调整三角函数的值
        float age = Time.time - birthTime; //已经过去的时间
        float theta = Mathf.PI * 2 * age/waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;
        //让Enemy_1绕y轴微微旋转
        Vector3 rot = new Vector3(0, waveRotY * sin, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        //在Y轴的运动仍由父类中的Move函数处理
        base.Move();
    }
}
