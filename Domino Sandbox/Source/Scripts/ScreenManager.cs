using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour
{
    public Text PlayBtnTxt;
    public Transform ColorPicker;
    GraphicRaycaster gr;

    void Start()
    {
        HSVUtil.colorPicker = ColorPicker.GetComponent<ColorPicker>();

        gr = this.GetComponent<GraphicRaycaster>();
        MyEvents.AddEventListener(pauseToggled, MyEventTypes.GAME_TOGGLE_PAUSE_EVENT);
        MyEvents.AddEventListener(openPicker, MyEventTypes.COLORPICKER_SET_TARGET);
        MyEvents.AddEventListener(closePicker, MyEventTypes.COLORPICKER_CLOSE);
    }

    private void openPicker()
    {
        HSVUtil.colorPickerActive = true;
        ColorPicker.gameObject.SetActive(true);
    }

    private void closePicker()
    {
        HSVUtil.colorPicker.onValueChanged.RemoveAllListeners();
        HSVUtil.colorPickerActive = false;
        ColorPicker.gameObject.SetActive(false);
    }

    void Update()
    {
        UnityEngine.EventSystems.PointerEventData ped = new UnityEngine.EventSystems.PointerEventData(null);
        ped.position = Input.mousePosition;
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        gr.Raycast(ped, results);
        if(results.Count > 0)
            GetInput.cursorOverHud = true;
        else
            GetInput.cursorOverHud = false;

        GetInput.cursorOverColorpicker = false;

        foreach (UnityEngine.EventSystems.RaycastResult rr in results)
            if (rr.gameObject.tag == "ColorPicker")
            {
                GetInput.cursorOverColorpicker = true;
                break;
            }
    }

    public void TogglePause()
    {
        MyEvents.TriggerEvent(MyEventTypes.GAME_TOGGLE_PAUSE_EVENT);
    }

    private void pauseToggled()
    {
        if (PlayBtnTxt.text == "Play")
            PlayBtnTxt.text = "Pause";
        else
            PlayBtnTxt.text = "Play";
    }

    public void ResetScene()
    {
        MyEvents.TriggerEvent(MyEventTypes.RESET_SCENE_EVENT);
    }

    public void ClearScene()
    {
        MyEvents.TriggerEvent(MyEventTypes.CLEAR_SCENE_EVENT);
    }

    public void ClearDynamic()
    {
        MyEvents.TriggerEvent(MyEventTypes.CLEAR_DYNAMIC_EVENT);
    }

    public void ClearStatic()
    {
        MyEvents.TriggerEvent(MyEventTypes.CLEAR_STATIC_EVENT);
    }
}
