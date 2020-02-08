using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public static Hero S;
    public float gameRestartDelay = 2f;
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    [SerializeField]
    private float _shieldLevel = 1;
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
        //使飞机在摄像头屏幕内
        bounds.center = transform.position;
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
        if(off != Vector3.zero) {
            pos -= off;
            transform.position = pos;
        }
        //飞机移动时，旋转一个角度，更有动感
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);
    }

    public GameObject lastTriggerGo = null;

    void OnTriggerEnter(Collider other) 
    {
        GameObject go = Utils.FindTaggedParent(other.gameObject);
        if(go != null) {
            if(go == lastTriggerGo) {
                //当同一架敌机的两个子对象(在同一帧中)碰撞到护盾时发生，则忽略此事件
                return;
            }
            lastTriggerGo = go;
            if(go.tag == "Enemy") {
                shieldLevel --;
                Destroy(go);
            } else {
                print("触发碰撞事件： " + go.name);
            }
        } else {
            print("触发碰撞事件： " + other.gameObject.name);
        }
    }
    //shieldLevel属性将监视_shieldLevel字段
    public float shieldLevel {
        get {
            return(_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            if(value < 0) {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
