using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GeneralUnityActions : MonoBehaviour
{


    public UnityEvent OnEnableAction;
    public UnityEvent OnDisableAction;
    public UnityEvent OnDistroyAction;
    public UnityEvent WhenNoGraphAction;

    public UnityEvent WhenScrolledToTop;
    public GameObject LoaderPrefab;
    [HideInInspector] public GameObject loadedLoader;
    Vector2 initialPositionoOfContent;

    [ContextMenu("Fix Arabic")]
    public void FixArabic()
    {
        GetComponent<Text>().text = ArabicFixer.Fix(GetComponent<Text>().text);
    }
    private void OnEnable()
    {

        OnEnableAction?.Invoke();

    }

    private void OnDisable()
    {
        OnDisableAction.Invoke();
        
    
    }

    private void OnDestroy()
    {
        OnDistroyAction.Invoke();
        
    }
    public void DetectScrollOnTop(ScrollRect scrollRect)
    {
        if (initialPositionoOfContent == Vector2.zero)
        {
            initialPositionoOfContent = scrollRect.content.anchoredPosition;
        }
        float difference = initialPositionoOfContent.y - scrollRect.content.anchoredPosition.y;
        // Debug.Log(difference);
        if (difference >= 10 && loadedLoader == null)
        {
            loadedLoader = Instantiate(LoaderPrefab, scrollRect.content.transform.parent);
            loadedLoader.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
            loadedLoader.transform.SetAsFirstSibling();
        }
        if (difference >= 10f && Input.GetMouseButtonUp(0))
        {
            WhenScrolledToTop?.Invoke();
            initialPositionoOfContent = Vector2.zero;
        }
        if (difference <= 10f && loadedLoader != null && !Input.GetMouseButton(0))
        {
            Destroy(loadedLoader);
            loadedLoader = null;
            initialPositionoOfContent = Vector2.zero;
        }
    }
}
