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
    private Canvas canvas;
    public RectTransform MyTarget;
    private bool isConnected;  // Tracks if the object is connected to a target
    public GameManger gameManger;
    private Image image;
    private bool collided; // Tracks collision state with the target

    // Store original size and position for resetting later
    private Vector2 originalSize;
    private Vector3 originalPosition;
    private Vector3 originalScale;

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
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        // Store original size and position on awake
        originalSize = rectTransform.sizeDelta;
        originalPosition = rectTransform.localPosition;
        originalScale = rectTransform.localScale;
    }

    // Called when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GameManger.Instance.Started) return;

        // Allow the UI element to be dragged by setting blockRaycasts to false
        canvasGroup.blocksRaycasts = false;
        collided = false; // Reset collided state on drag start

        // Change the size of the object to match the target's size while dragging
        rectTransform.sizeDelta = MyTarget.sizeDelta;
        rectTransform.localScale = MyTarget.localScale;
    }

    // Called every frame while dragging
    public void OnDrag(PointerEventData eventData)
    {
        if (!GameManger.Instance.Started) return;

        // Update the position of the UI element to follow the mouse
        rectTransform.position = Input.mousePosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == MyTarget.gameObject)
        {
            collided = true; // Set collided to true when entering the target
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == MyTarget.gameObject)
        {
            collided = false; // Reset collided when exiting the target
        }
    }

    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!GameManger.Instance.Started) return;

        // Restore the blockRaycasts so other UI elements can interact with it
        canvasGroup.blocksRaycasts = true;

        // Check if the dragged object has collided with the target
        if (collided)
        {
            SnapToTarget(); // Snap to target position if collided
            DoAttached();   // Handle attachment logic
        }
        else
        {
            // Reset the position, size, and scale if not connected to the target
            rectTransform.localPosition = originalPosition;
            rectTransform.sizeDelta = originalSize;
            rectTransform.localScale = originalScale;

            // Reset isConnected to allow dragging again
            isConnected = false;
        }
    }

    // Snap the position and size of the dragged object to the target position and size
    private void SnapToTarget()
    {
        // Snap the dragged object's position to the target's position and size
        rectTransform.position = MyTarget.position;
        rectTransform.sizeDelta = MyTarget.sizeDelta;
        rectTransform.localScale = MyTarget.localScale;

        canvasGroup.interactable = false;
        image.enabled = false;
    }

    [ContextMenu("Attach")]
    public void DoAttached()
    {
        MyTarget.GetComponent<Image>().color = Color.white; // Change the color to indicate connection
        isConnected = true; // Set isConnected to true to indicate attachment
        GameManger.Instance.ConnectedObjects += 1; // Increment the connected objects counter

        // Disable BoxCollider2D on both the attached object and the target
        DisableColliders();

        // Check for game completion conditions
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

    // Method to disable BoxCollider2D components
    private void DisableColliders()
    {
        // Disable BoxCollider2D on the attached object
        var myCollider = GetComponent<BoxCollider2D>();
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // Disable BoxCollider2D on the target object
        var targetCollider = MyTarget.GetComponent<BoxCollider2D>();
        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }
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
