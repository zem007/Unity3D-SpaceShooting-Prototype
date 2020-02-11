using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy2: ")]
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;
    //sin波形对速度的影响程度因子
    public float sinEccentricity = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[2];

        Vector3 cbMin = Utils.camBounds.min;
        Vector3 cbMax = Utils.camBounds.max;
        Vector3 v = Vector3.zero;
        v.x = cbMin.x - Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[0] = v;

        v = Vector3.zero;
        v.x = cbMax.x + Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[1] = v;

        //有50%的概率换边
        if(Random.value < 0.5f) {
            points[0].x *= -1;
            points[1].x *= -1;
        }

        birthTime = Time.time;
    }

    public override void Move() {
        //插值
        float u = (Time.time - birthTime) / lifeTime;
        if(u > 1) {
            Destroy(this.gameObject);
            return;
        }
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        pos = (1-u)*points[0] + u*points[1];
    }
}
