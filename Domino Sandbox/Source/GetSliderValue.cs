using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    private Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
    }

    public void ToText(Slider slider)
    {
        txt.text = slider.value.ToString();
    }
}
