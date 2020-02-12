using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy_3是基于3点贝塞尔曲线插值，进行平滑移动的
public class Enemy_3 : Enemy
{
    [Header("Enemy_3: ")]
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[3];
        points[0] = pos;

        float xMin = Utils.camBounds.min.x + Main.S.enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - Main.S.enemySpawnPadding;
        Vector3 v;
        //在屏幕下部y<0处随机取一个中间节点
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = Random.Range(Utils.camBounds.min.y, 0);
        points[1] = v;
        //在屏幕上随机选取一个点作为终点
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        birthTime = Time.time;
    }

    public override void Move() 
    {
        float u = (Time.time - birthTime) / lifeTime;
        if(u > 1) {
            Destroy(this.gameObject);
            return;
        }
        //在3点贝塞尔曲线上插值
        Vector3 p01, p02;
        //可以选择对u插值让曲线在中部运动更平稳
        u = u - 0.2f * Mathf.Sin(u*Mathf.PI*2);
        p01 = (1-u)* points[0] + u*points[1];
        p02 = (1-u)* points[1] + u*points[2];
        pos = (1-u)* p01 + u*p02;
    }
}
