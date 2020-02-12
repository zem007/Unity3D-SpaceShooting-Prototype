using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//定义了所有武器的一个枚举类型
public enum WeaponType {
    none, 
    blaster, //高爆
    spread, //发射两发
    phaser, //沿波形前进
    missile, //追踪导弹
    laser, //激光
    shield, //提高护盾等级
}

//WeaponDefinition类可以在检视面板中设定特定武器属性
//Main脚本中有一个WeaponDefinition的数组，可以在其中进行设置
//[System.Serializable]通知Unity在检视面板中查看WeaponDefinition
[System.Serializable]
public class WeaponDefinition {
    public WeaponType type = WeaponType.none;
    public string letter;     //升级道具显示的字母
    public Color color = Color.white;    //Colar和升级道具的颜色
    public GameObject  projectilePrefab; //炮弹的预设
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;  //造成的伤害点数
    public float continuousDamage = 0;  //每秒造成的点数(Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20;   //炮弹的速度
} //武器预设，颜色将在Main类中设置

public class Weapon : MonoBehaviour
{
    public static Transform PROJECTILE_ANCHOR;
    public bool ____________________________;
    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot;

    void Awake() {
        collar = transform.Find("Collar").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        //调用SetType()，正确设置默认武器的类型_type
        SetType(_type);
        if(PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //查找父对象的fireDelegate
        GameObject parentGO = transform.parent.gameObject;
        if(parentGO.tag == "Hero") {
            Hero.S.fireDelegate += Fire;   //将Fire函数嵌入委托函数中
        }
    }

    public WeaponType type {
        get {
            return(_type);
        }
        set {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt) {
        _type = wt;
        if(type == WeaponType.none) {
            this.gameObject.SetActive(false);
            return;
        } else {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collar.GetComponent<Renderer>().material.color = def.color;
        lastShot = 0;
    }

    public void Fire() {
        if(!this.gameObject.activeInHierarchy) return;
        if(Time.time - lastShot < def.delayBetweenShots) return;

        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if(transform.up.y < 0) {
            vel.y = -vel.y;
        }
        switch (type) {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.GetComponent<Rigidbody>().velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.GetComponent<Rigidbody>().velocity = p.transform.rotation * vel;
                break;
        }
    }

    //发射炮弹, 战机或敌机
    public Projectile MakeProjectile() {
        GameObject go = Instantiate(def.projectilePrefab) as GameObject;
        if(transform.parent.gameObject.tag == "Hero") {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        } else {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return(p);
    }
}
