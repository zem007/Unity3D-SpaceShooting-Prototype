using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject poi;  //Hero
    public GameObject[] panels;   //滚动的前背景
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f; //控制前背景画面对玩家运动的反馈程度
    public float panelHt;  //每个前景的高度
    public float depth; //pos.z

    // Start is called before the first frame update
    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;
        //设置两个前背景的初始位置
        panels[0].transform.position = new Vector3(0,0,depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    // Update is called once per frame
    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt*0.5f);
        if(poi != null) {
            tX = -poi.transform.position.x * motionMult;
        }
        //设置panels[0]的位置
        panels[0].transform.position = new Vector3(tX, tY, depth);
        //设置panels[1]的位置，使背景连续
        if(tY >= 0) {
            panels[1].transform.position = new Vector3(tX, tY-panelHt, depth);
        } else {
            panels[1].transform.position = new Vector3(tX, tY+panelHt, depth);
        }
    }
}
