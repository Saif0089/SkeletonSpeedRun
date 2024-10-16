using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string MyName;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Canvas canvas;
  
    public RectTransform MyTarget;
    public List<RectTransform> othertargets=new List<RectTransform>();
    bool isConnected;

    public bool IsConnected => isConnected;
    public GameManger gameManger;
    bool inbody = false;

    [ContextMenu("Fix")]
    public void FixArabic()
    {

        gameObject.name = ArabicFixer.Fix(gameObject.name);
        GetComponent<Text>().text = gameObject.name;
        // gameManger = FindObjectOfType<GameManger>();
        // MyName = gameObject.name;
        // gameObject.name = MyName;
    }
    [ContextMenu("AssignMuscle")]
    public void AssignMuscle()
    {
        MyTarget = gameManger.muscleManager.Muscles.FirstOrDefault(x => x.name.Equals(MyName)).GetComponent<RectTransform>();
    }

    [ContextMenu("AssignBone")]
    public void AssignBone()
    {
        MyTarget = gameManger.boneManager.Bones.FirstOrDefault(x => x.name.Equals(MyName)).GetComponent<RectTransform>();
    }

    [ContextMenu("AssignOrgan")]
    public void AssignOrgan()
    {
        MyTarget = gameManger.organManager.Organs.FirstOrDefault(x => x.name.Equals(MyName)).GetComponent<RectTransform>();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();

    }

    // Called when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Allow the UI element to be dragged by setting blockRaycasts to false
        canvasGroup.blocksRaycasts = false;
    }

    // Called every frame while dragging
    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected || !GameManger.Instance.Started)
        {
            return;
        }
        // Update the position of the UI element to follow the mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }
    bool collided;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject == MyTarget.gameObject || othertargets.FirstOrDefault(x=>x.gameObject==other.gameObject)!=null)
        {
            collided = true;
            MyTarget = other.GetComponent<RectTransform>();
        }else if(other.gameObject.tag.Equals("body"))
        {
            inbody = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == MyTarget.gameObject || othertargets.FirstOrDefault(x => x.gameObject == other.gameObject) != null)
        {
            collided = false;
            MyTarget = other.GetComponent<RectTransform>();
        }
        else if (other.gameObject.tag.Equals("body"))
        {
            inbody = false;
        }
    }
    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore the blockRaycasts so other UI elements can interact with it
        canvasGroup.blocksRaycasts = true;
        float distance = Vector3.Distance(rectTransform.transform.position, MyTarget.transform.position);
        Debug.Log(distance);


        if (inbody)
        {
            if (!gameManger.objectHandlers.Contains(this))
            {
                gameManger.objectHandlers.Add(this);
            }
        }
        else
        {
            if (gameManger.objectHandlers.Contains(this))
            {
                gameManger.objectHandlers.Remove(this);
            }
        }

        if (collided && set == false)
        {
            DoAttached();
            set = true;
        }
        else
        {


            CheckAllin();
         

        }
        
    }
   
    public void CheckAllin()
    {
        if ((GameManger.Instance.objectHandlers.Count == GameManger.Instance.boneManager.Bones.Count) && GameManger.Instance.StageNumber == 1)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.objectHandlers.Count == (GameManger.Instance.boneManager.Bones.Count + GameManger.Instance.organManager.Organs.Count)
            && GameManger.Instance.StageNumber == 2)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.objectHandlers.Count >= (GameManger.Instance.boneManager.Bones.Count + GameManger.Instance.organManager.Organs.Count + GameManger.Instance.muscleManager.Muscles.Count) && GameManger.Instance.StageNumber >= 3)
        {
            GameManger.Instance.FinishGame();
        }
    }
    bool set = false;
    [ContextMenu("Attach")]
    public void DoAttached()
    {
        
        MyTarget.GetComponent<Image>().color = Color.white;
        isConnected = true;
        GameManger.Instance.ConnectedObjects += 1;

        GameManger.Instance.CorrectAnsers += 1;

        bool wonset = false;
        // transform.parent = transform.parent.transform;
        if ((GameManger.Instance.ConnectedObjects == GameManger.Instance.boneManager.Bones.Count) && GameManger.Instance.StageNumber == 1)
        {
            wonset = true;
            GameManger.Instance.FinishGame();
           
        }
        else if (GameManger.Instance.ConnectedObjects == GameManger.Instance.organManager.Organs.Count && GameManger.Instance.StageNumber == 2)
        {
            wonset = true;
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.ConnectedObjects >= GameManger.Instance.muscleManager.Muscles.Count && GameManger.Instance.StageNumber >= 3)
        {
            wonset = true;
            GameManger.Instance.FinishGame();
        }

        if(wonset==false)
        CheckAllin();
    }
    // Optional: Reset the element back to its original position
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
    public GameObject sample;

    [ContextMenu("Do the thing")]
    public void ChangeToBullet()
    {
        GameObject go = Instantiate(sample, transform);
        go.GetComponent<Text>().text = "•";
        go.name = gameObject.name;
        GetComponent<Text>().text.Replace("•", " ");
        DestroyImmediate(this);
    }

    public void RemoveIt()
    {
        transform.GetChild(0).GetComponent<Text>().raycastTarget = false;
    }
    public void AddColliders()
    {
        var mycoll = gameObject.AddComponent<BoxCollider2D>();
        mycoll.size = new Vector2(15, 30);
        var rigi = gameObject.AddComponent<Rigidbody2D>();
        rigi.isKinematic = true;

        var coll = MyTarget.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
        coll.size = MyTarget.sizeDelta;
    }
    public void RemoveColls()
    {
        DestroyImmediate(GetComponent<BoxCollider2D>());
        DestroyImmediate(GetComponent<Rigidbody2D>());

        DestroyImmediate(MyTarget.GetComponent<BoxCollider2D>());
    }
}
