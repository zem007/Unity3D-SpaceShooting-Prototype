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
    public Weapon[] weapons;  //用来显示头端的各个炮筒
    public bool _______________________;
    public Bounds bounds;
    //声明一个新的委托类型
    public delegate void WeaponFireDelegate();
    //创建一个委托类型的字段
    public WeaponFireDelegate fireDelegate;

    void Awake() 
    {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //重置武器, 然战机从装备一个高爆武器开始
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
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

        //使用fireDelegate委托发射武器
        //首先，确认玩家按下了Jump键，然后确认fireDelegate不为零
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }

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
            } else if(go.tag == "PowerUp") {
                AbsorbPowerUp(go);
            } else {
                print("触发碰撞事件： " + go.name);
            }
        } else {
            print("触发碰撞事件： " + other.gameObject.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>(); //获取PowerUp的脚本!
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel ++;
                break;
            default:    //如果是任意一种武器的升级道具
                //检查当前的武器类型
                if(pu.type == weapons[0].type) {
                    Weapon w = GetEmptyWeaponSlot();
                    if(w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject); //调用PowerUp脚本中的AbsorbedBy函数，来销毁道具
    }
    //找到一个空白的武器位置
    Weapon GetEmptyWeaponSlot() {
        for(int i=0; i<weapons.Length; i++) {
            if(weapons[i].type == WeaponType.none) {
                return(weapons[i]);
            }
        }
        return(null);
    }
    
    void ClearWeapons() {
        foreach(Weapon w in weapons) {
            w.SetType(WeaponType.none);
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
