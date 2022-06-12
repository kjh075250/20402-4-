using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    public GameObject monsterSpawner = null;
    public GameObject itemObject = null;

    public static List<GameObject> monsters = new List<GameObject>();

    public int spawnMaxCount = 50;
    public static int score = 0;

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
        InvokeRepeating("Spawn", 3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DropItem(Transform itemtrans)
    {
        score += 100;
        Debug.Log(score);
        int randomItem = Random.RandomRange(-2, 2);
        if(randomItem > 0)
        {
            Instantiate(itemObject, itemtrans.position, Quaternion.identity);
            Debug.Log("Item Drop");
        }
    }
    void OnGUI()
    {
        var labelstyle = new GUIStyle();
        labelstyle.fontSize = 50;
        labelstyle.normal.textColor = Color.white;
        GUILayout.Label("Á¡¼ö : " + score, labelstyle);
    }
}
