using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private WeaponType _type;

    public WeaponType type {
        get {
            return(_type);
        }
        set {
            SetType(value);
        }
    }

    void Awake() 
    {
        //每隔2s检测一次，查看对象是否除了屏幕
        InvokeRepeating("CheckOffscreen", 2f, 2f);
    }

    public void SetType(WeaponType eType) {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        GetComponent<Renderer>().material.color = def.projectileColor;
    }

    void CheckOffscreen() {
        if(Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
            Destroy(this.gameObject);
        }
    }
}
