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

    public GameObject spawnPos = null;
    public GameObject SecSpawnPos = null;

    public GameObject UICanvas = null;
    public GameObject UICtrlObject = null;
    public GameObject GameOverCanvas = null;
    public GameObject WorldLight = null;
    public GameObject EnemyLight = null;
    public GameObject moneyText = null;
    public Transform moneyTextTrans = null;
    public Image HpGageImage = null;

    public List<GameObject> monsters = new List<GameObject>();
    private float endTime = 0f;

    public int spawnMaxCount = 1;
    public int score = 0;
    public int playerHp = 200;
    public float rndPosX;
    public float rndPosZ;
    public int money = 0;
    private float spawnrate = 2f;
    public bool a = false;
    public bool IsEscapeActivate = false;

    public float probeEndTime = 180f;
    public Text timeText = null;
    public Text HPText = null;
    public GameObject compass = null;
    public GameObject EscapeRot = null;
    public GameObject EscapePos = null;
    void Spawn()
    {
        rndPosX = spawnPos.transform.position.x;
        rndPosZ = spawnPos.transform.position.z;
        if (monsters.Count > spawnMaxCount)
        {
            return;
        }
        Vector3 vecSpawn = new Vector3(Random.RandomRange(rndPosX - 100f,rndPosX + 100f), 1000f ,Random.RandomRange(rndPosZ - 100f,rndPosZ + 100f));
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
        moneyText.SetActive(false);
        SecSpawnPos.SetActive(false);
        Instance = this;
        InvokeRepeating("Spawn", 3f, spawnrate);
        GameOverCanvas.SetActive(false);
        HPText.text = "조사기기 체력";
        compass.SetActive(false);
        RenderSettings.fogDensity = 0;
        RenderSettings.fog = false;
    }
    // Update is called once per frame
    void Update()
    {
        CheckDie();
        CheckScore();
        InGameUI();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            probeEndTime = 10f;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            money += 100;
        }
    }
    public void SetHp(int i)
    {
        playerHp -= i;
        HpGageImage.rectTransform.localScale = new Vector3(playerHp / 200f, 1f, 1f);
        UICtrlObject.transform.DOShakePosition(0.5f, 4f, 15, 90f,false,true);

    }
    public void DropItem(Transform itemtrans)
    {
        score += 100;
        int randomItem = Random.RandomRange(-2, 3);
        if(randomItem > 0)
        {
            Instantiate(itemObject, itemtrans.position, Quaternion.identity);
        }
    }
    public void MoneyTextMove()
    {
        moneyText.SetActive(true);
        moneyText.GetComponent<Text>().DOFade(1, 0.3f).OnComplete(() => moneyText.GetComponent<Text>().DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() =>
        { moneyText.SetActive(false); }));
        
       
    }
    public void OnGUI()
    {
        var labelstyle = new GUIStyle();
        labelstyle.fontSize = 35;
        labelstyle.normal.textColor = Color.white;
        GUILayout.Label("점수 : " + score, labelstyle);
        GUILayout.Label("현재 몬스터 수 : " + monsters.Count, labelstyle);
        GUILayout.Label("현재 플레이어 체력 : " + playerHp, labelstyle);
        GUILayout.Label("현재 돈 : " + money, labelstyle);
    }
    public void InGameUI()
    {
        if (probeEndTime >= 0)
        {
            probeEndTime -= Time.deltaTime;
            timeText.text = "조사 완료까지 남은 시간\n" + probeEndTime.ToString("0");
        }
        if (probeEndTime <= 0)
        {
            RenderSettings.fog = true;
            HPText.text = "플레이어 체력";
            timeText.text = "탈출 지점까지 이동하세요";
            if(IsEscapeActivate == false)
            {
                SecSpawnPos.SetActive(true);
                EscapeRot.transform.Rotate(new Vector3(0f, Random.RandomRange(0f, 359f), 0f));
                SecSpawnPos.transform.position = EscapePos.transform.position;
                spawnPos = SecSpawnPos;
                SecSpawnPos.transform.DOMoveY(2, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                IsEscapeActivate = true;
            }
            compass.SetActive(true);
            compass.transform.LookAt(SecSpawnPos.transform.position);
            if (RenderSettings.fogDensity <= 0.06f)
            {
                RenderSettings.fogDensity += 0.07f * Time.deltaTime;
            }
        }
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
        }
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene("SampleScene");
    }
    private void CheckScore()
    {
        if(score >= 1000)
        {
            spawnrate = 1f; 
        }
    }
}
