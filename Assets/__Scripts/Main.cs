using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static Main S;
    public static Dictionary<WeaponType, WeaponDefinition> W_DEFS;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemySpawnPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public bool ________________________;
    public WeaponType[] activeWeaponTypes;
    public float enemySpawnRate;

    void Awake()
    {
        S = this;
        Utils.SetCameraBounds(this.GetComponent<Camera>());
        enemySpawnRate = 1f/enemySpawnPerSecond;
        Invoke("SpawnEnemy", enemySpawnRate);
        W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            W_DEFS[def.type] = def;
        }
    }

    public static WeaponDefinition GetWeaponDefinition(WeaponType wt) 
    {
        //先检查W_DEFS中是否存在wt的key，如果不存在可能会抛出异常
        if(W_DEFS.ContainsKey(wt)) {
            return(W_DEFS[wt]);
        }
        //如果wt不存在，则返回一个新的WeaponDefinition，其中的WeaponTye.type = none;
        return(new WeaponDefinition());
    }

    void Start() 
    {
        //根据weaponDefinition传入的type，得到相应的武器种类，组成一个数组
        activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
        for(int i=0; i < weaponDefinitions.Length; i++) {
            activeWeaponTypes[i] = weaponDefinitions[i].type;
        }
    }

    public void SpawnEnemy() 
    {
        //随机取一个敌机并实例化
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;
        //放置于屏幕上方，x坐标随机
        Vector3 pos = Vector3.zero;
        float xMin = Utils.camBounds.min.x + enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - enemySpawnPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = Utils.camBounds.max.y + enemySpawnPadding;
        go.transform.position = pos;
        Invoke("SpawnEnemy", enemySpawnRate);
    }

    public void DelayedRestart(float delay) 
    {
        Invoke("Restart", delay);
    }

    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
