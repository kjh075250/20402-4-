using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    public GameObject monsterSpawner = null;

    public List<GameObject> monsters = new List<GameObject>();

    public int spawnMaxCount = 50;

    private float rndPos = 100f;
    void Spawn()
    {
        if(monsters.Count > spawnMaxCount)
        {
            return;
        }
        Vector3 vecSpawn = new Vector3(Random.Range(-rndPos, rndPos), 1000f, Random.Range(-rndPos, rndPos));
        Ray ray = new Ray(vecSpawn, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity)== true)
        {
            vecSpawn.y = raycastHit.point.y;
            
        }
        GameObject newMonster = Instantiate(monsterSpawner, vecSpawn, Quaternion.identity);
        monsters.Add(newMonster);
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 3f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
