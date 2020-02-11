using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 6;
    public float fadeTime = 4;
    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    void Awake() 
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        //设置一个随机速度
        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        GetComponent<Rigidbody>().velocity = vel;
        transform.rotation = Quaternion.identity;
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        //每2s检查一次，看是否在屏幕之外
        InvokeRepeating("CheckOffscreen", 2f, 2f);
    }

    void CheckOffscreen() {
        //如果升级道具完全飘出屏幕之外，销毁
        if(Utils.ScreenBoundsCheck(cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //每帧根据速度旋转cube对象, 基于时间旋转
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        //隔一段时间，让升级道具逐渐透明直到消失
        float u = (Time.time - birthTime - lifeTime) / fadeTime;
        if(u >= 1) {
            Destroy(this.gameObject);
            return;
        }
        if(u > 0) {
            Color c = cube.GetComponent<Renderer>().material.color;
            //大于lifeTime之后，cube的颜色开始变浅
            c.a = 1f - u;
            cube.GetComponent<Renderer>().material.color = c;
            //道具字母同理，但是变浅的程度低一点
            c = letter.color;
            c.a = 1f - (u*0.5f);
            letter.color = c;
        }
    }

    public void SetType(WeaponType wt) {
        //从Main中获取wt的值
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cube.GetComponent<Renderer>().material.color = def.color;
        letter.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target) {
        Destroy(this.gameObject);
    }
}
