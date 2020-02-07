using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float rotationsPerSecond = 0.1f;
    public bool ________________________;
    public int levelShown = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //读取当前飞机的护盾等级
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        if(currLevel != levelShown) {
            levelShown = currLevel;
            Material mat = this.GetComponent<Renderer>().material;
            //调整纹理偏移量，呈现正确的护盾等级
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        //每秒将护盾旋转一定的角度
        float rZ = (rotationsPerSecond * Time.time*360) % 360f;
        transform.rotation = Quaternion.Euler(0,0,rZ);
    }
}
