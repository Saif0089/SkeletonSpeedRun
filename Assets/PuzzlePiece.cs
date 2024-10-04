using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string MyName;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Canvas canvas;
    public RectTransform MyTarget;
    bool isConnected;
    public GameManger gameManger;

    [ContextMenu("Fix")]
    public void FixArabic()
    {
        gameObject.name = ArabicFixer.Fix(gameObject.name);
        GetComponent<Text>().text = gameObject.name;
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
        originalPosition = rectTransform.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();
    }

    // Called when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optional: Implement any logic needed when dragging starts
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
        if (other.gameObject == MyTarget.gameObject)
        {
            collided = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == MyTarget.gameObject.name)
        {
            collided = false;
        }
    }

    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        // Calculate the distance between the dragged object and its target
        float distance = Vector3.Distance(rectTransform.transform.position, MyTarget.transform.position);
        Debug.Log($"Distance: {distance}");

        // If the dragged object collided with the target
        if (collided)
        {
            DoAttached();
        }
        //else
        //{
        //    // If it did not collide, reset the position
        //    ResetPosition();
        //}
    }


    [ContextMenu("Attach")]
    public void DoAttached()
    {
        // Snap the object to the target's position
        rectTransform.anchoredPosition = MyTarget.anchoredPosition;

        MyTarget.GetComponent<Image>().color = Color.white;
        isConnected = true;
        GameManger.Instance.ConnectedObjects += 1;

        // Disable BoxColliders on the connected object and the target object
        if (TryGetComponent(out BoxCollider2D myCollider))
        {
            myCollider.enabled = false;
        }
        if (MyTarget.TryGetComponent(out BoxCollider2D targetCollider))
        {
            targetCollider.enabled = false;
        }

        // Log the snap event for debugging
        Debug.Log($"{gameObject.name} snapped to {MyTarget.gameObject.name} at position {MyTarget.anchoredPosition}");

        // Check for completion conditions
        if (GameManger.Instance.ConnectedObjects == GameManger.Instance.boneManager.Bones.Count && GameManger.Instance.StageNumber == 1)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.ConnectedObjects == GameManger.Instance.organManager.Organs.Count && GameManger.Instance.StageNumber == 2)
        {
            GameManger.Instance.FinishGame();
        }
        else if (GameManger.Instance.ConnectedObjects == GameManger.Instance.muscleManager.Muscles.Count && GameManger.Instance.StageNumber == 3)
        {
            GameManger.Instance.FinishGame();
        }
    }

    // Optional: Reset the element back to its original position
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        isConnected = false;  // Ensure isConnected is false to allow further dragging
        collided = false;     // Reset collided status
    }
    public GameObject sample;

    [ContextMenu("Do the thing")]
    public void ChangeToBullet()
    {
        GameObject go = Instantiate(sample, transform);
        go.GetComponent<Text>().text = "•";
        go.name = gameObject.name;
        GetComponent<Text>().text = GetComponent<Text>().text.Replace("•", " ");
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