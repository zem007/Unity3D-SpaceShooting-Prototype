using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//定义一种新的变量类型 BoundsTest， 该类型里面有三个值
public enum BoundsTest {
    center,
    onScreen,
    offScreen
}

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

    //=================================边界框函数===================================//

    //检查飞机边界框bnd是否位于镜头框camBounds之内
    public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center) {
        return(BoundsInBoundsCheck(camBounds, bnd, test));
    }

    //检查边界框lilB是否位于边界框bigB之内，根据所选的BoundsTest，此函数的行为也不同
    public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen) 
    {
        Vector3 pos = lilB.center;

        //初始化offset为0
        Vector3 off = Vector3.zero;
        //测定平移距离off的值
        switch(test) {
            case BoundsTest.center:
                if(bigB.Contains(pos)) {
                    return(Vector3.zero);
                }
                if(pos.x > bigB.max.x) {
                    off.x = pos.x - bigB.max.x;
                } else if(pos.x < bigB.min.x) {
                    off.x = pos.x - bigB.min.x;
                }
                if(pos.y > bigB.max.y) {
                    off.y = pos.y - bigB.max.y;
                } else if(pos.y < bigB.min.y) {
                    off.y = pos.y - bigB.min.y;
                }
                if(pos.z > bigB.max.z) {
                    off.z = pos.z - bigB.max.z;
                } else if(pos.z < bigB.min.z) {
                    off.z = pos.z - bigB.min.z;
                }
                return(off);

            case BoundsTest.onScreen:
                if(bigB.Contains(lilB.min) && bigB.Contains(lilB.max)) {
                    return(Vector3.zero);
                }
                if(lilB.max.x > bigB.max.x) {
                    off.x = lilB.max.x - bigB.max.x;
                } else if(lilB.min.x < bigB.min.x) {
                    off.x = lilB.min.x - bigB.min.x;
                }
                if(lilB.max.y > bigB.max.y) {
                    off.y = lilB.max.y - bigB.max.y;
                } else if(lilB.min.y < bigB.min.y) {
                    off.y = lilB.min.y - bigB.min.y;
                }
                if(lilB.max.z > bigB.max.z) {
                    off.z = lilB.max.z - bigB.max.z;
                } else if(lilB.min.z < bigB.min.z) {
                    off.z = lilB.min.z - bigB.min.z;
                }
                return(off);

            case BoundsTest.offScreen:
                if(bigB.Contains(lilB.min) || bigB.Contains(lilB.max)) {
                    return(Vector3.zero);
                }
                if(lilB.min.x > bigB.max.x) {
                    off.x = lilB.min.x - bigB.max.x;
                } else if(lilB.max.x < bigB.min.x) {
                    off.x = lilB.max.x - bigB.min.x;
                }
                if(lilB.min.y > bigB.max.y) {
                    off.y = lilB.min.y - bigB.max.y;
                } else if(lilB.max.y < bigB.min.y) {
                    off.y = lilB.max.y - bigB.min.y;
                }
                if(lilB.min.z > bigB.max.z) {
                    off.z = lilB.min.z - bigB.max.z;
                } else if(lilB.max.z < bigB.min.z) {
                    off.z = lilB.max.z - bigB.min.z;
                }
                return(off);
        }
        return(Vector3.zero);
    }

    //==================================变换函数====================================//

    //递归寻找transform.parent树
    //直到找到对象的tag != "Untagged" 或者没有父对象为止
    public static GameObject FindTaggedParent(GameObject go)
    {
        if(go.tag != "Untagged") {
            return(go);
        }
        if(go.transform.parent == null) {
            return(null);
        }
        return(FindTaggedParent(go.transform.parent.gameObject));
    }

    //这个版本的FindTaggedParent（）函数以transform为参数
    public static GameObject FindTaggedParent(Transform t) {
        return(FindTaggedParent(t.gameObject));
    }

    //=================================材质函数===================================//
    //用一个List返回游戏对象和其子对象的所有材质
    public static Material[] GetAllMaterials(GameObject go) {
        List<Material> mats = new List<Material>();
        if(go.GetComponent<Renderer>() != null) {
            mats.Add(go.GetComponent<Renderer>().material);
        }
        foreach(Transform t in go.transform) {
            mats.AddRange(GetAllMaterials(t.gameObject));
        }
        return(mats.ToArray());
    }

}
