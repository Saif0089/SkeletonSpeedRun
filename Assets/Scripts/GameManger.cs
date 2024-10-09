using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;
    public bool Started;
    public int ConnectedObjects;
    public BoneManager boneManager;
    public OrganManager organManager;
    public MuscleManager muscleManager;
    public GameObject StartGameText;
    public GameObject bonesSection;
    public GameObject organSection;
    public GameObject muscleSection;
    public float gametime;
    public TMP_Text GameTimeText;
    public List<float> PlayerScoress = new List<float>();
    public GameObject ScorePrefab;
    public GameObject Leaderboard;
    public Button StartGameButton;
    public Text StartGameButtonText;
    public int StageNumber =1;
    public Image LoadingScreen;
    public GameObject GameOverPanel;
    private void Awake()
    {
        DOTween.Init();
        
        if (Instance == null)
        {
            Instance = this;
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ScoreJson")))
        {
            PlayerScoress = JsonConvert.DeserializeObject<List<float>>(PlayerPrefs.GetString("ScoreJson"));
            PlayerScoress.Sort();
        }
    }
    public void StartGame()
    {
        StartGameText.SetActive(false);
        bonesSection.SetActive(true);
        Started = true;
        StartGameButtonText.text = "Stop - ﻒﻗ";
        StartGameButton.targetGraphic.color = Color.red;
        StartGameButton.onClick.RemoveAllListeners();
        StartGameButton.onClick.AddListener(() => StopGame());
    }
    public void StopGame()
    {
        Started = false;
        StartGameButtonText.text = "Start - أﺪﺒﻳ";
        StartGameButton.targetGraphic.color = new Color(0.07197545f, 0.8301887f, 0, 1);
        StartGameButton.onClick.RemoveAllListeners();
        StartGameButton.onClick.AddListener(() => StartGame());
    }
    private void Update()
    {
        if(Started){
            gametime += Time.deltaTime;
            GameTimeText.text = gametime.ToString("0.0");
            if(gametime>=120){

            }
        }
    }
    public void GameOver()
    {
        StopGame();
        GameOverPanel.SetActive(true);
    }
    public async UniTask FinishGame()
    {
        Debug.Log("GameFinish");
        Started = false;
        if (StageNumber != 3)
        {
            if (StageNumber == 1)
            {
                boneManager.SkeletonImages.ForEach(x => x.color = new Color(1, 1, 1, 1));
                ConnectedObjects = 0;
                LoadingScreen.gameObject.SetActive(true);
                 await UniTask.Delay(1000, ignoreTimeScale: false);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.DOFade(1, 1f);
                await UniTask.Delay(2000, ignoreTimeScale: false);
                bonesSection.SetActive(false);
                organSection.SetActive(true);
                LoadingScreen.DOFade(0, 1f);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.gameObject.SetActive(false);
                Started = true;
            }
            if(StageNumber == 2)
            {
                organManager.OrganImages.ForEach(x => x.color = new Color(1, 1, 1, 1));
                ConnectedObjects = 0;
                LoadingScreen.gameObject.SetActive(true);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.DOFade(1, 1f);
                await UniTask.Delay(2000, ignoreTimeScale: false);
                organSection.SetActive(false);
                muscleSection.SetActive(true);
                LoadingScreen.DOFade(0, 1f);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.gameObject.SetActive(false);
                Started = true;
            }
            StageNumber += 1;

        }
        else
        {
                muscleManager.OrganImages.ForEach(x => x.color = new Color(1, 1, 1, 1));
                ConnectedObjects = 0;
                LoadingScreen.gameObject.SetActive(true);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.DOFade(1, 1f);
                await UniTask.Delay(2000, ignoreTimeScale: false);
                organSection.SetActive(false);
                muscleSection.SetActive(false);
                LoadingScreen.DOFade(0, 1f);
                await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.gameObject.SetActive(false);
            PlayerScoress.Add(gametime);
            PlayerScoress.Sort();
            string json = JsonConvert.SerializeObject(PlayerScoress);
            PlayerPrefs.SetString("ScoreJson", json);
            ShowLeaderBoard();
        }
        
    }
    List<GameObject> InstantiatedScores = new List<GameObject>();
    public void ShowLeaderBoard()
    {
        InstantiatedScores.ForEach(x => Destroy(x));
        for(int i = 0;i<10;i++)
        {
            if(i > PlayerScoress.Count-1){
                continue;
            }
            GameObject go = Instantiate(ScorePrefab, ScorePrefab.transform.parent);
            go.SetActive(true);
            if(i % 2 == 0){
                go.GetComponent<Image>().color = Color.white;
                go.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.gray;
                go.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.gray;
            }
            go.transform.GetChild(1).GetComponent<TMP_Text>().text = PlayerScoress[i].ToString("0.0");
            InstantiatedScores.Add(go);
        }
        Leaderboard.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void ResetLeaderBoard()
    {
        PlayerScoress.Clear();
        PlayerPrefs.DeleteAll();
        InstantiatedScores.ForEach(x => Destroy(x));
        InstantiatedScores.Clear();
    }
}
