using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LeTai;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHandlerAssigner : MonoBehaviour
{
    public List<ObjectHandler> ObjectHandlers= new List<ObjectHandler>();  

    [ContextMenu("Assign Handlers")]
    public void AssignHandlers()
    {
        ObjectHandlers = transform.GetComponentsInChildren<ObjectHandler>().ToList();
    }
    [ContextMenu("Delete Themselves")]
    public void DeleteThemSelves()
    {
        foreach(var item in ObjectHandlers)
        {
            string itemText = item.GetComponent<Text>().text;
            string parentText = item.transform.parent.GetComponent<Text>().text;
            float itemwidth = item.GetComponent<RectTransform>().sizeDelta.x;
            float parentwidth = item.transform.parent.GetComponent<RectTransform>().sizeDelta.x;

            float itemposition = item.GetComponent<RectTransform>().anchoredPosition.x;
            float parentPosition = item.transform.parent.GetComponent<RectTransform>().anchoredPosition.x;

            item.GetComponent<Text>().text = parentText;
            item.transform.parent.GetComponent<Text>().text = itemText;
            
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(parentwidth, item.GetComponent<RectTransform>().sizeDelta.y);
            item.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemwidth,item.transform.parent.GetComponent<RectTransform>().sizeDelta.y);

            item.GetComponent<RectTransform>().anchoredPosition =  new Vector2(parentPosition, item.GetComponent<RectTransform>().anchoredPosition.y);
            item.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(itemposition, item.transform.parent.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
    [ContextMenu("Delete Unnecessary")]
    public void RemoveRightOnes()
    {
        List<ObjectHandler> temphandlers = new List<ObjectHandler>();
        foreach(var item in ObjectHandlers)
        {
            if(item.name.Contains("Right")){
                temphandlers.Add(item);
            }
        }
        temphandlers.ForEach(x => { ObjectHandlers.Remove(x); DestroyImmediate(x.gameObject); });
    }

    [ContextMenu("ReassignBones")]
    public void ReAssignBones()
    {
        BoneManager boneManager = FindObjectOfType<BoneManager>(true);
        boneManager.Bones.Clear();
        foreach(var item in ObjectHandlers)
        {
            boneManager.Bones.Add(item.MyTarget.gameObject);
        }
    }

    [ContextMenu("ReassignMuscles")]
    public void ReAssignMuscles()
    {
        MuscleManager boneManager = FindObjectOfType<MuscleManager>(true);
        boneManager.Muscles.Clear();
        foreach(var item in ObjectHandlers)
        {
            boneManager.Muscles.Add(item.MyTarget.gameObject);
        }
    }

    [ContextMenu("ReassignOrgans")]
    public void ReAssignOrgans()
    {
        OrganManager boneManager = FindObjectOfType<OrganManager>(true);
        boneManager.Organs.Clear();
        foreach(var item in ObjectHandlers)
        {
            boneManager.Organs.Add(item.MyTarget.gameObject);
        }
    }

    [ContextMenu("AssignToParent")]
    public void AssignToParent()
    {
        foreach(var item in ObjectHandlers)
        {
            item.transform.parent.AddComponent<ObjectHandler>();
        }
        ObjectHandlers.ForEach(x => DestroyImmediate(x));
        AssignHandlers();
    }

    [ContextMenu("Remove target")]
    public void RemoveThem(){
        foreach(var item in ObjectHandlers)
        {
            item.RemoveIt();
        }
    }

    [ContextMenu("CHANGE TEXT")]
    public void ChangeText()
    {
        foreach (var item in ObjectHandlers)
        {
            item.GetComponent<Text>().fontSize = 16;
            item.transform.GetChild(0).GetComponent<Text>().fontSize = 16;
            item.transform.GetChild(0).GetComponent<Text>().text = item.transform.GetChild(0).GetComponent<Text>().text.Replace("•", " ");
            item.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;   
            item.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(item.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, item.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
    [ContextMenu("AddColls")]
    public void AddColls(){
        transform.GetComponentsInChildren<BoxCollider2D>().ToList().ForEach(x => DestroyImmediate(x));
        transform.GetComponentsInChildren<Rigidbody2D>().ToList().ForEach(x => DestroyImmediate(x));
        foreach(var item in ObjectHandlers)
        {
            // item.RemoveColls();
            item.AddColliders();
        }
    }
}
