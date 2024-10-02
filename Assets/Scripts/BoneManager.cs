using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoneManager : MonoBehaviour
{
   public List<GameObject> Bones = new List<GameObject>();
   public List<Image> SkeletonImages = new List<Image>();
    public Color disabledColor;

    [ContextMenu("Set DisabledColor")]
    public void DisableAll(){
        Bones.ForEach(x => x.GetComponent<Image>().color = disabledColor);
    }
     private void Update()
    {
        if(Screen.width > Screen.height)
        {
            transform.localScale = new Vector3(0.31f, 0.31f, 0.31f);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 7);
        }
        else if(Screen.width < Screen.height)
        {
            transform.localScale = new Vector3(0.5011605f, 0.5011605f, 0.5011605f);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 48);
        }
    }
}
