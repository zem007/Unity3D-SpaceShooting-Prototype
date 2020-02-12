using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part {
    public string name;
    public float health;
    public string[] protectedBy;
    
    public GameObject go;
    public Material mat;
}

public class Enemy_4 : Enemy
{
    [Header("Enemy_4-the Boss: ")]
    public Vector3[] points;
    public float timeStart;
    public float duration = 4; //每个循环运动的时间
    public Part[] parts; //储存敌机boss的各部分的数组

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[2];
        points[0] = pos;
        points[1] = pos;
        InitMovement();
        
        //在parts中缓存每个组件的对象和材质
        Transform t;
        foreach(Part prt in parts) {
            t = transform.Find(prt.name);
            if(t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    //这里重写Enemy中的OnCollisionEnter函数
    void OnCollisionEnter(Collision coll) 
    {
        GameObject other = coll.gameObject;
        switch(other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>(); //调用Projectile脚本
                //在进入屏幕之前，敌机不会受到伤害
                bounds.center = transform.position + boundsCenterOffset;
                if(bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
                    Destroy(other);
                    break;
                }
                //给敌机造成伤害，找到被击中的游戏对象。coll中包含了一个由碰撞点组成的数组contacts[]。
                //由于碰撞，其中至少存在一个元素contacts[0]。碰撞点包含了对碰撞器thisCollider的引用,该碰撞器为北击中组件的碰撞器
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null) {
                    //说明thisCollider不是敌机的组件,而是ProjectileHero
                    //这时需要查看参与碰撞的另一个碰撞器otherCollider
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //检查被碰撞的组件是否收到了其他组件的保护
                if(prtHit.protectedBy != null) {
                    foreach(string s in prtHit.protectedBy) {
                        if(!Destroyed(s)) {
                            Destroy(other);
                            return;
                        }
                    }
                }
                //如果未受到其他组件的保护
                prtHit.health -= Main.W_DEFS[p.type].damageOnHit;
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0) {
                    prtHit.go.SetActive(false);  //使该组件失效
                }
                bool allDestroyed = true;
                foreach(Part prt in parts) {
                    if(!Destroyed(prt)) {
                        allDestroyed = false;
                        break;
                    }
                }
                if(allDestroyed) {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }

    Part FindPart(string n) {
        foreach(Part prt in parts) {
            if(prt.name == n) {
                return(prt);
            }
        }
        return(null);
    }

    Part FindPart(GameObject go) {
        foreach(Part prt in parts) {
            if(prt.go == go) {
                return(prt);
            }
        }
        return(null);
    }

    bool Destroyed(GameObject go) {
        return(Destroyed(FindPart(go)));
    }

    bool Destroyed(string n) {
        return(Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt) {
        if(prt == null) {
            return(true);
        }
        return(prt.health <= 0);
    }

    void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        remainingDamageFrames = showDamageForFrames;
    }

    void InitMovement() 
    {
        Vector3 p1 = Vector3.zero;
        float esp = Main.S.enemySpawnPadding;
        Bounds cBounds = Utils.camBounds;
        p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
        p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

        points[0] = points[1];
        points[1] = p1;

        //重置时间
        timeStart = Time.time;
    }

    public override void Move() 
    {
        float u = (Time.time - timeStart) / duration;
        if(u > 1) {
            //选择一个新的终点p1并初始化运动
            InitMovement();
            u = 0;
        }
        //使u值使用慢速平滑过渡
        u = 1 - Mathf.Pow(1-u, 2);

        pos = (1-u)*points[0] + u * points[1];
    }
}
