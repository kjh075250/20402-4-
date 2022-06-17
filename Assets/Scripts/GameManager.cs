using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject monsterSpawner = null;
    public GameObject itemObject = null;

    public GameObject UICanvas = null;
    public GameObject UICtrlObject = null;
    public GameObject GameOverCanvas = null;
    public GameObject WorldLight = null;
    public GameObject EnemyLight = null;
    public Image HpGageImage = null;

    public List<GameObject> monsters = new List<GameObject>();
    private float endTime = 0f;

    public int spawnMaxCount = 1;
    public int score = 0;
    public int playerHp = 200;
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
        Time.timeScale = 1;
        Instance = this;
        InvokeRepeating("Spawn", 3f, 1f);
        GameOverCanvas.SetActive(false);
            RenderSettings.fog = false;

    }

    // Update is called once per frame
    void Update()
    {
        CheckDie();
    }
    public void SetHp()
    {
        playerHp -= 10;
        HpGageImage.rectTransform.localScale = new Vector3(playerHp / 200f, 1f, 1f);
        UICtrlObject.transform.DOShakePosition(0.5f, 4f, 15, 90f,false,true);
        //if(score > 100)
        //{
        //    RenderSettings.fog = true;
        //}
    }
    public void DropItem(Transform itemtrans)
    {
        score += 100;
        int randomItem = Random.RandomRange(-2, 2);
        if(randomItem > 0)
        {
            Instantiate(itemObject, itemtrans.position, Quaternion.identity);
        }
    }

    void OnGUI()
    {
        var labelstyle = new GUIStyle();
        labelstyle.fontSize = 40;
        labelstyle.normal.textColor = Color.white;
        GUILayout.Label("점수 : " + score, labelstyle);
        GUILayout.Label("현재 몬스터 수 : " + monsters.Count, labelstyle);
        GUILayout.Label("현재 플레이어 체력 : " + playerHp, labelstyle);
    }
    void CheckDie()
    {
        endTime -= Time.deltaTime;
        if (playerHp <= 0)
        {
            if(endTime <= 0)
            {
                endTime = 1.5f;
            }
                if(endTime > 0)
                {
                    Time.timeScale = 0.2f;
                }
                else
                {
                    Time.timeScale = 0f;
                }
            UICanvas.SetActive(false);
            GameOverCanvas.SetActive(true);
            Camera.main.fieldOfView = 30f;
            Debug.Log(endTime);
        }
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
