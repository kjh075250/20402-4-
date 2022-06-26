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
    public GameObject BarriCtrlObject = null;
    public GameObject GameOverCanvas = null;
    public GameObject WorldLight = null;
    public GameObject EnemyLight = null;
    public GameObject TextMove = null;
    public Transform TextMoveTran = null;
    public Image HpGageImage = null;
    public Image BarriGageImage = null;
    public GameObject barri = null;
    public GameObject HHP = null;
    public GameObject Cam1 = null;
    public GameObject Campos = null;
    public float shaketime = 0f;

    public List<GameObject> monsters = new List<GameObject>();
    private float endTime = 0f;

    public int spawnMaxCount = 1;
    public int score = 0;
    public int playerHp = 200;
    public int barriHp = 200;
    public float rndPosX;
    public float rndPosZ;
    public int money = 0;
    private float spawnrate = 2f;
    public bool a = false;
    public bool IsEscapeActivate = false;
    public bool IsBarriActivate = false;
    public bool IsUIActivate = false;

    public float probeEndTime = 180f;
    private float min = 0f;
    public Text timeText = null;
    public Text HPText = null;
    public Text moneyText = null;
    public GameObject compass = null;
    public GameObject EscapeRot = null;
    public GameObject EscapePos = null;
    public GameObject EscapeCanvas = null;


    // Start is called before the first frame update
    void Start()
    {
        Cam1.SetActive(false);
        barri.SetActive(false);
        BarriCtrlObject.SetActive(false);
        SecSpawnPos.SetActive(false);
        Instance = this;
        InvokeRepeating("Spawn", 3f, spawnrate);
        GameOverCanvas.SetActive(false);
        EscapeCanvas.SetActive(false);
        HPText.text = "������ ü��";
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
            min = 0f;
            probeEndTime = 5f;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            money += 100;
        }
        if(shaketime >= 0)
        {
            shaketime -= Time.deltaTime;

        }
        else
        {
            Cam1.SetActive(false);
        }
    }
    void Spawn()
    {

        rndPosX = spawnPos.transform.position.x;
        rndPosZ = spawnPos.transform.position.z;
        if (monsters.Count > spawnMaxCount)
        {
            return;
        }
        Vector3 vecSpawn = new Vector3(Random.RandomRange(rndPosX - 100f, rndPosX + 100f), 1000f, Random.RandomRange(rndPosZ - 100f, rndPosZ + 100f));
        Ray ray = new Ray(vecSpawn, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            vecSpawn.y = raycastHit.point.y;
        }
        GameObject newMonster = Instantiate(monsterSpawner, vecSpawn, Quaternion.identity);
        monsters.Add(newMonster);
    }
    public void SetHp(int i)
    {
        if(IsBarriActivate)
        {
            barriHp -= 50;
            BarriGageImage.rectTransform.localScale = new Vector3(barriHp / 200f, 1f, 1f);
            BarriCtrlObject.transform.DOShakePosition(0.5f, 4f, 15, 90f, false, true);
        }
        else
        {
            playerHp -= i;
            HpGageImage.rectTransform.localScale = new Vector3(playerHp / 200f, 1f, 1f);
            UICtrlObject.transform.DOShakePosition(0.5f, 4f, 15, 90f, false, true);
        }
    }
    public void SetScore(int i, string s)
    {
        score += i;
        GameObject c = Instantiate(TextMove, TextMoveTran);
        c.GetComponentInChildren<Text>().text = s;
    }
    public void DropItem(Transform itemtrans)
    {
        SetScore(100, "���� +100(�� óġ)");
        int randomItem = Random.RandomRange(-2, 3);
        if(randomItem > 0)
        {
            Instantiate(itemObject, itemtrans.position, Quaternion.identity);
        }
    }
    public void OnGUI()
    {
        if (IsUIActivate)
        {
            return;
        }
        else
        {
            var labelstyle = new GUIStyle();
            labelstyle.fontSize = 35;
            labelstyle.normal.textColor = Color.white;
            GUILayout.Label("���� : " + score, labelstyle);
            GUILayout.Label("���� ���� �� : " + monsters.Count, labelstyle);
            GUILayout.Label("���� �÷��̾� ü�� : " + playerHp, labelstyle);
            GUILayout.Label("���� �� : " + money, labelstyle);
        }


    }
    public void InGameUI()
    {
        if (probeEndTime >= 0 && min >= 0)
        {
            probeEndTime -= Time.deltaTime;
            if(probeEndTime > 59)
            {
                min++;
                probeEndTime -= 60;
            }
            if(probeEndTime <= 0)
            {
                if(min != 0)
                {
                    probeEndTime = 59;
                    min--;
                }
            }
            timeText.text = "���� �Ϸ���� ���� �ð�\n" + string.Format("{0} : {1}", min, (int)probeEndTime);
        }
        if (probeEndTime <= 0 && min <= 0)
        {
            RenderSettings.fog = true;
            HPText.text = "�÷��̾� ü��";
            timeText.text = "Ż�� �������� �̵��ϼ���";
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
        moneyText.text = "���� ��: " + money;
    }
    void CheckDie()
    {
        if(barriHp <= 0)
        {
            IsBarriActivate = false;
            BarriCtrlObject.SetActive(false);
            barri.SetActive(false);
        }
        if (playerHp <= 0)
        {
            if(GameOverCanvas.activeInHierarchy == false)
            {
                Time.timeScale = 0.2f;
                GameOverCanvas.GetComponentInChildren<Text>().text += "\n����� ���� :" + score;
                UICanvas.SetActive(false);
                GameOverCanvas.SetActive(true);
                Camera.main.fieldOfView = 30f;
            }
        }
    }
    public void CheckEscape()
    {
        Time.timeScale = 0f;
        EscapeCanvas.GetComponentInChildren<Text>().text += "\n����� ���� :" + score;
        UICanvas.SetActive(false);
        EscapeCanvas.SetActive(true);
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
    public void ShakeCamera()
    {
        shaketime = 0.5f;

        Cam1.SetActive(true);

    }
}
