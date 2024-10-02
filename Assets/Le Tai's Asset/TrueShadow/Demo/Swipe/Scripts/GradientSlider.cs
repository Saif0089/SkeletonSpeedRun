using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow.Demo
{
[RequireComponent(typeof(Image))]
public class GradientSlider : MonoBehaviour
{
    public Gradient gradient;

    Image image;

    void Start()
    {
        image = GetComponent<Image>();
            Set();
    }
    
    [ContextMenu("test")]
    public void toset(){
            Set();
    }
    public void Set(float value = 1)
    {
        if (!image) return;

        image.fillAmount = value;
        image.color      = gradient.Evaluate(value);
    }
}
}
