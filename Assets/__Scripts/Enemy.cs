using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public float score = 100;
    public int showDamageForFrames = 2;
    public bool ______________________;
    public Color[] originalColors;
    public Material[] materials;
    public int remainingDamageFrames = 0;
    public Bounds bounds;
    public Vector3 boundsCenterOffset; //bounds.center 到position的距离

    void Awake()
    {
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i=0; i<materials.Length; i++) {
            originalColors[i] = materials[i].color;
        }
        InvokeRepeating("CheckOffScreen", 0f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if(remainingDamageFrames > 0) {
            remainingDamageFrames--;
            if(remainingDamageFrames == 0) {
                UnShowDamage();
            }
        }
    }

    public Vector3 pos {
        get {
            return(this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    public virtual void Move() 
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void CheckOffScreen() 
    {
        //如果边界框默认为初始值
        if(bounds.size == Vector3.zero) {
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);
            boundsCenterOffset = bounds.center - transform.position;
        }
        bounds.center = transform.position + boundsCenterOffset;
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        if(off != Vector3.zero) {
            if(off.y < 0) {
                Destroy(this.gameObject);
            }
        }
    }

    void OncollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch(other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //进入屏幕前，敌机不会受到伤害，这会避免玩家射击到屏幕之外的敌机
                bounds.center = transform.position + boundsCenterOffset;
                if(bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
                    Destroy(other);
                    break;
                }
                //表现受伤动画
                ShowDamage();
                //给予敌机伤害
                health -= Main.W_DEFS[p.type].damageOnHit;
                if(health <= 0) {
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
            default:
            print("Enemy hit by non-ProjectileHero: " + other.name);
            break;
        }
    }

    void ShowDamage() {
        foreach(Material m in materials) {
            m.color = Color.red;
        }
        remainingDamageFrames = showDamageForFrames;
    }

    void UnShowDamage() {
        for(int i=0; i<materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
    }
}
