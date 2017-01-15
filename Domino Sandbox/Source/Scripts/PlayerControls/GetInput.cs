using UnityEngine;
using System.Collections;

public class GetInput : MonoBehaviour
{
    public static bool leftMousePressed;
    public static bool rightMousePressed;
    public static bool middleMousePressed;
    public static bool shiftKeyPressed;
    public static bool ctrlKeyPressed;

    public static bool cursorOverHud;
    public static bool cursorOverColorpicker;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!GameManager.menuOpen)
        {
            // left MB
            if (Input.GetKey(KeyCode.Mouse0) && !leftMousePressed)
            {
                leftMousePressed = true;
            }
            else if (!Input.GetKey(KeyCode.Mouse0) && leftMousePressed)
            {
                leftMousePressed = false;
            }

            // right MB
            if (Input.GetKey(KeyCode.Mouse1) && !rightMousePressed)
            {
                rightMousePressed = true;
            }
            else if (!Input.GetKey(KeyCode.Mouse1) && rightMousePressed)
            {
                rightMousePressed = false;
            }

            // middle MB
            if (Input.GetKey(KeyCode.Mouse2) && !middleMousePressed)
            {
                middleMousePressed = true;
            }
            else if (!Input.GetKey(KeyCode.Mouse2) && middleMousePressed)
            {
                middleMousePressed = false;
            }

            // shiftKeyPressed
            if (Input.GetKey(KeyCode.LeftShift) && !shiftKeyPressed ||
                Input.GetKey(KeyCode.RightShift) && !shiftKeyPressed)
            {
                shiftKeyPressed = true;
            }
            else if (!Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.LeftShift) && shiftKeyPressed)
            {
                shiftKeyPressed = false;
            }

            // ControlKeyPressed
            if (Input.GetKey(KeyCode.LeftControl) && !ctrlKeyPressed ||
                Input.GetKey(KeyCode.RightControl) && !ctrlKeyPressed)
            {
                ctrlKeyPressed = true;
            }
            else if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl) && ctrlKeyPressed)
            {
                ctrlKeyPressed = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                MyEvents.TriggerEvent(MyEventTypes.LEFT_MB_DOWN);
                if (!cursorOverColorpicker)
                    if (HSVUtil.colorPickerActive)
                        MyEvents.TriggerEvent(MyEventTypes.COLORPICKER_CLOSE);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                MyEvents.TriggerEvent(MyEventTypes.RIGHT_MB_DOWN);
                if (HSVUtil.colorPickerActive)
                    MyEvents.TriggerEvent(MyEventTypes.COLORPICKER_CLOSE);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                MyEvents.TriggerEvent(MyEventTypes.MIDDLE_MB_DOWN);
                if (HSVUtil.colorPickerActive)
                    MyEvents.TriggerEvent(MyEventTypes.COLORPICKER_CLOSE);
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    MyEvents.TriggerEvent(MyEventTypes.MOUSE_SCROLL_UP);
                else
                    MyEvents.TriggerEvent(MyEventTypes.MOUSE_SCROLL_DOWN);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
                MyEvents.TriggerEvent(MyEventTypes.LEFT_MB_UP);

            if (Input.GetKeyDown(KeyCode.Space))
                MyEvents.TriggerEvent(MyEventTypes.GAME_TOGGLE_PAUSE_EVENT);

            if (Input.GetKeyDown(KeyCode.Backspace))
                MyEvents.TriggerEvent(MyEventTypes.RESET_SCENE_EVENT);
        }
    }
}
