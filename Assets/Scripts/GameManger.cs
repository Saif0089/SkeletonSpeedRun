using System;
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



[System.Serializable]
public class  PlayerData
{
    public float score;
    public string name;
    public int correct;
    public int incorrect;
    
}

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
    public List<PlayerData> PlayerScoress = new List<PlayerData>();
    public GameObject ScorePrefab;
    public GameObject Leaderboard;
    public Button StartGameButton;
    public TextMeshProUGUI StartGameButtonText;
    public int StageNumber =1;
    public Image LoadingScreen;
    public GameObject GameOverPanel;
    public GameObject GameWinPanel;
    public GameObject GameWinScore;
    public GameObject submitButton;

    public GameObject TakeNamePanel;
    public GameObject InputnameError;
    public TMP_InputField PlayerNameText;

    public GameObject demo;
    public float DemoHideTime = 15;
    float demotime = 0;
    bool canplaydemo = true;
    string playername;
    bool nameAssigned=false;
    public TextMeshProUGUI correctLostAnswersText;
    public TextMeshProUGUI IncorrectLostAnswersText;
    public TextMeshProUGUI correctWinAnswersText;
    public TextMeshProUGUI IncorrectWinAnswersText;
    public int CorrectAnsers=0;
    public List<ObjectHandler> objectHandlers = new List<ObjectHandler>();
    private void Awake()
    {
        DOTween.Init();
        
        if (Instance == null)
        {
            Instance = this;
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ScoreJson")))
        {
            PlayerScoress = JsonConvert.DeserializeObject<List<PlayerData>>(PlayerPrefs.GetString("ScoreJson"));
            PlayerScoress.Sort((p1, p2) => p1.score.CompareTo(p2.score).CompareTo(p1.correct) );
            // PlayerScoress.Sort();
        }
    }

    public void CheckCompletion()
    {
        GameManger.Instance.FinishGame();
        //bool isfinished = false;

        //if ((GameManger.Instance.ConnectedObjects >= GameManger.Instance.boneManager.Bones.Count) && GameManger.Instance.StageNumber == 1)
        //{
        //    isfinished = true;
        //    GameManger.Instance.FinishGame();
        //}
        //else if (GameManger.Instance.ConnectedObjects >= GameManger.Instance.organManager.Organs.Count && GameManger.Instance.StageNumber == 2)
        //{
        //    isfinished = true;
        //    GameManger.Instance.FinishGame();
        //}
        //else if (GameManger.Instance.ConnectedObjects >= GameManger.Instance.muscleManager.Muscles.Count && GameManger.Instance.StageNumber >= 3)
        //{
        //    isfinished = true;
        //    GameManger.Instance.FinishGame();
        //}


        //if(isfinished==false)
        //CheckAllin();
    }
    public void CheckAllin()
    {
        if ((GameManger.Instance.objectHandlers.Count >= GameManger.Instance.boneManager.Bones.Count) && GameManger.Instance.StageNumber == 1)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.objectHandlers.Count >= (GameManger.Instance.boneManager.Bones.Count + GameManger.Instance.organManager.Organs.Count)
            && GameManger.Instance.StageNumber == 2)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.objectHandlers.Count >= (GameManger.Instance.boneManager.Bones.Count + GameManger.Instance.organManager.Organs.Count + GameManger.Instance.muscleManager.Muscles.Count) && GameManger.Instance.StageNumber >= 3)
        {
            GameManger.Instance.FinishGame();
        }
    }
 
    public void AddName()
    {

        playername = PlayerNameText.text;

        if (playername.Length > 0 && playername != string.Empty)
        {


            TakeNamePanel.SetActive(false);
            StartGameText.SetActive(false);
            bonesSection.SetActive(true);
            submitButton.SetActive(true);
            GameTimeText.gameObject.SetActive(true);
            nameAssigned = true;
            Started = true;
            StartGameButton.gameObject.SetActive(true);
            StartGameButtonText.text = "Stop - ﻒﻗ";
            StartGameButton.targetGraphic.color = Color.red;
            StartGameButton.onClick.RemoveAllListeners();
            StartGameButton.onClick.AddListener(() => StopGame());
            InputnameError.SetActive(false);
        }
        else
        {
            InputnameError.SetActive(true);
        }
    }
    public void CloseNamePanel()
    {
        StopGame();

        TakeNamePanel.SetActive(false);
     //   StartGameText.SetActive(true);
        bonesSection.SetActive(false);
        Leaderboard.SetActive(false);
        StartGameButton.gameObject.SetActive(true);
        Started = false;
        canplaydemo = true;
        demotime = 0;
    }

    public void StartGame()
    {
        CorrectAnsers = 0;
        if (nameAssigned)
        {
            TakeNamePanel.SetActive(false);
          //  StartGameText.SetActive(false);
           
            if(StageNumber==1)
            bonesSection.SetActive(true);
         
            Leaderboard.SetActive(false);
            
            Started = true;
            StartGameButtonText.text = "Stop - ﻒﻗ";
            StartGameButton.targetGraphic.color = Color.red;
            StartGameButton.onClick.RemoveAllListeners();
            StartGameButton.onClick.AddListener(() => StopGame());
        }
        else
        {
            PlayerNameText.text=$"Player{(int)UnityEngine.Random.Range(1000,100000)}";
            InputnameError.SetActive(false);
             StartGameButton.gameObject.SetActive(false);
            TakeNamePanel.SetActive(true);
           // StartGameText.SetActive(false);
            bonesSection.SetActive(false);
            Leaderboard.SetActive(false);
            Started = false;
            canplaydemo = false;
        }

    }
    public void StopGame()
    {
        Started = false;
        Debug.Log("stop game now");
        StartGameButtonText.text = "Start - أﺪﺒﻳ";
        StartGameButton.targetGraphic.color = new Color(0.07197545f, 0.8301887f, 0, 1);
        StartGameButton.onClick.RemoveAllListeners();
        StartGameButton.onClick.AddListener(() => StartGame());
    }
    private void Update()
    {



        if(Started){
            gametime += Time.deltaTime;
            var ts = TimeSpan.FromSeconds(gametime);
            GameTimeText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds) + " sec";
            if (gametime>=120){

       

            
                Started = false;
                GameOver();
            }
      
       
        }
        else
        {
            if(canplaydemo)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    demotime = 0;
                    if (demo.activeSelf)
                        demo.SetActive(false);
                }
                else
                {


                    demotime += Time.deltaTime;

                    if (demotime >= DemoHideTime)
                    {
                        if (!demo.activeSelf)
                            demo.SetActive(true);
                    }
                }
            }
            
            if(canplaydemo==false)
            {
                demotime = 0;
                if (demo.activeSelf)
                    demo.SetActive(false);
            }

            
        }
    }
    public void GameOver()
    {
        StopGame();
        correctLostAnswersText.text =$"{CorrectAnsers} / 16"; //objectHandlers.FindAll(x=>x.IsConnected==false).Count
        IncorrectLostAnswersText.text = $" {16 - CorrectAnsers} / 16"; //objectHandlers.FindAll(x=>x.IsConnected==false).Count
        submitButton.SetActive(false);
        GameOverPanel.SetActive(true);
        Invoke(nameof(RestartGame), 5);
    }
    public async UniTask FinishGame()
    {
        Debug.Log("GameFinish");
        Started = false;
        if (StageNumber != 3)
        {
            if (StageNumber == 1)
            {
           
                Started = false;
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
                GameTimeText.gameObject.SetActive(true);
                Started = true;
            } 
            if(StageNumber == 2)
            {
                Started = false;

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
                GameTimeText.gameObject.SetActive(true);
                Started = true;
            }
            StageNumber += 1;

        }
        else
        {
            Started = false;
            muscleManager.OrganImages.ForEach(x => x.color = new Color(1, 1, 1, 1));
 
            ConnectedObjects = 0;
                //LoadingScreen.gameObject.SetActive(true);
                //await UniTask.Delay(1000, ignoreTimeScale: false);
                //LoadingScreen.DOFade(1, 1f);
                //await UniTask.Delay(2000, ignoreTimeScale: false);
                //organSection.SetActive(false);
                //muscleSection.SetActive(false);
                //LoadingScreen.DOFade(0, 1f);
                //await UniTask.Delay(1000, ignoreTimeScale: false);
                LoadingScreen.gameObject.SetActive(false);
          
            PlayerData data = new PlayerData();
            data.name = playername;
            data.score = gametime;

            data.correct = CorrectAnsers;
            data.incorrect = 16-CorrectAnsers;
            PlayerScoress.Add(data);
            PlayerScoress.Sort((p1, p2) => p1.score.CompareTo(p2.score).CompareTo(p1.correct));

 
            string json = JsonConvert.SerializeObject(PlayerScoress);
            PlayerPrefs.SetString("ScoreJson", json);
            GameWinScore.transform.GetChild(0).GetComponent<TMP_Text>().text = playername.ToString();

            correctWinAnswersText.text = $"{CorrectAnsers} / 16"; //objectHandlers.FindAll(x=>x.IsConnected==false).Count
            IncorrectWinAnswersText.text = $" {16 - CorrectAnsers} / 16"; //objectHandlers.FindAll(x=>x.IsConnected==false).Count

            GameWinScore.transform.GetChild(1).GetComponent<TMP_Text>().text = gametime.ToString("0.0")+ " sec";
            GameWinPanel.gameObject.SetActive(true);
            submitButton.SetActive(false);
            Invoke(nameof(RestartGame),5);
            // ShowLeaderBoard();
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
                go.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.gray;
                go.transform.GetChild(3).GetComponent<TMP_Text>().color = Color.gray;
            }
            go.transform.GetChild(0).GetComponent<TMP_Text>().text = PlayerScoress[i].name.ToString();
            go.transform.GetChild(1).GetComponent<TMP_Text>().text ="Correct : "+ PlayerScoress[i].correct.ToString();
            go.transform.GetChild(2).GetComponent<TMP_Text>().text = "Incorrect : " + PlayerScoress[i].incorrect.ToString();
            go.transform.GetChild(3).GetComponent<TMP_Text>().text = PlayerScoress[i].score.ToString("0.0") + " sec";
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
