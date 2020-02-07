using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Bounds BoundsUnion(Bounds b0, Bounds b1) 
    {
        //如果其中一个size=0，则忽略
        if(b0.size == Vector3.zero && b1.size != Vector3.zero) {
            return(b1);
        } else if(b0.size != Vector3.zero && b1.size == Vector3.zero) {
            return(b0);
        } else if(b0.size == Vector3.zero && b1.size == Vector3.zero) {
            return(b0);
        }
        //扩展b0，使其可以包含b1
        b0.Encapsulate(b1.min);
        b0.Encapsulate(b1.max);
        return(b0);
    }
    
    public static Bounds CombineBoundsOfChildren(GameObject go) 
    {
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);
        if(go.GetComponent<Renderer>() != null) {
            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
        }
        if(go.GetComponent<Renderer>() != null) {
            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
        }

        //递归遍历游戏对象Transform组件的每个子对象
        foreach(Transform t in go.transform) {
            b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
        }
        return(b);
    }

    //这是一个局部静态字段，在camBounds属性定义中使用
    private static Bounds _camBounds;

    //camBounds为静态只读的全局属性
    public static Bounds camBounds 
    {
        get {
            //如果未设置_camBounds变量
            if(_camBounds.size == Vector3.zero) {
                SetCameraBounds();
            }
            return (_camBounds);
        }
    }

    //本函数用于camBounds属性，可设置_camBounds变量值，也可以直接调用
    public static void SetCameraBounds(Camera cam=null)
    {
        if (cam == null) cam = Camera.main;
        //假设摄像机为正投影，假设摄像机的旋转为[0,0,0]
        Vector3 topLeft = new Vector3(0,0,0);
        Vector3 bottomRight = new Vector3(Screen.width, Screen.height, 0);
        //转化为世界坐标
        Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
        Vector3 boundBRF = cam.ScreenToWorldPoint(bottomRight);
        //将两个向量的z值分别设置为摄像机远剪切平面和近剪切平面的z值
        boundTLN.z += cam.nearClipPlane;
        boundBRF.z += cam.farClipPlane;
        //查找边界框的中心
        Vector3 center = (boundTLN + boundBRF) / 2f;
        _camBounds = new Bounds(center, Vector3.zero);
        //扩展_camBounds, 使其具有size
        _camBounds.Encapsulate(boundTLN);
        _camBounds.Encapsulate(boundBRF);
    }
}
