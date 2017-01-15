using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class CreateTool
{
    private bool createNewMaterial; // optimization to minimize batching amount
    private bool cursorLocked;

    /*  //////
    GUI ITEMS
    /////// */
    // Scale
    private InputField widthInput;
    private InputField heightInput;
    private InputField thicknessInput;

    // Pacing
    private Slider gapSlider;
    private Text gapSliderValue;

    // Tilt
    private Slider xAxis;
    private Slider yAxis;
    private Text xAxisValue;
    private Text yAxisValue;

    // Visual
    private Button primaryColor;
    private Button secondaryColor;
    /****************/

    Rigidbody rb;
    
    private float bendFactor = 5f;
    private float groupBending = 0f;
    private float MaxBending = 359.5f;

    private Vector3 objectSpace;
    private Vector3 objectSize;

    private GameObject toolSettingsPanel;
    private Transform test, groupHolder, pointerLocation, dominoHolder;
    private List<Transform> Placeholders = new List<Transform>();
    private Transform selectedObjPlaceholderTransform;
    
    public void Initialize (Transform objToCreate)
    {
        toolSettingsPanel = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings");
        fetchGUIElements();
        addGuiElementListeners();

        groupHolder = new GameObject("Creator Group").transform;
        dominoHolder = new GameObject("Dominoes").transform;
        pointerLocation = new GameObject("Mouse Pointer").transform;
        groupHolder.parent = pointerLocation;

        test = objToCreate;
        selectedObjPlaceholderTransform = UnityEngine.Object.Instantiate(test);
        selectedObjPlaceholderTransform.GetComponent<Rigidbody>().isKinematic = true;
        
        MeshFilter t = selectedObjPlaceholderTransform.GetComponent<MeshFilter>();
        Vector3 tMax = selectedObjPlaceholderTransform.TransformPoint(t.mesh.bounds.max);
        Vector3 tMin = selectedObjPlaceholderTransform.TransformPoint(t.mesh.bounds.min);
        objectSpace = Vector3.zero;
        objectSize = new Vector3(tMax.x - tMin.x, tMax.y - tMin.y, tMax.z - tMin.z);

        foreach (Collider col in selectedObjPlaceholderTransform.GetComponents<Collider>())
            col.isTrigger = true;

        selectedObjPlaceholderTransform.gameObject.layer = 2;
        selectedObjPlaceholderTransform.parent = groupHolder;

        Placeholders.Add(selectedObjPlaceholderTransform);

        primaryColor.image.color = Placeholders[0].GetComponent<Renderer>().materials[0].color;
        secondaryColor.image.color = Placeholders[0].GetComponent<Renderer>().materials[1].color;
        
        calculateBoundaries();
    }

    /// <summary>
    /// Update function for non-monobehavior functions. Should be called from Update() in script which inherits from MonoBehavior
    /// </summary>
    public void ManualUpdate ()
    {
        if (GetInput.middleMousePressed && GetInput.shiftKeyPressed)
        {
            if (!cursorLocked)
            {
                cursorLocked = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }

            float bendAmount = Input.GetAxisRaw("Mouse X");

            groupBending -= bendAmount * bendFactor;

            groupBending = Mathf.Clamp(groupBending, -MaxBending, MaxBending);

            calculateObjectPositions();
        }
        else
        {
            if (cursorLocked)
            {
                cursorLocked = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                float temp = objectSpace.x / 2 - objectSize.x/2;
                if (selectedObjPlaceholderTransform.localRotation.eulerAngles.z < 91)
                    temp *= -1; // when dominoes are tilted forward, this keeps dominos foot always at cursors point

                // calculate cursors point on world and then apply domino group there correctly on ground based on their scale and rotation
                pointerLocation.position = hit.point + new Vector3(0, objectSpace.y / 2, 0);
                groupHolder.localPosition = new Vector3(temp, 0, 0);
            }
            else
            {
                pointerLocation.position = new Vector3(0, -100, 0);
            }
        }
    }

    public void Enable()
    {
        MyEvents.AddEventListener(CreateObject, MyEventTypes.LEFT_MB_DOWN);
        MyEvents.AddEventListener(ScrollUp, MyEventTypes.MOUSE_SCROLL_UP);
        MyEvents.AddEventListener(ScrollDown, MyEventTypes.MOUSE_SCROLL_DOWN);
        MyEvents.AddEventListener(SetTargetColor, MyEventTypes.COLORPICKER_SET_TARGET);
        toolSettingsPanel.SetActive(true);
        groupHolder.gameObject.SetActive(true);
    }

    public void Disable()
    {
        MyEvents.RemoveEventListener(CreateObject, MyEventTypes.LEFT_MB_DOWN);
        MyEvents.RemoveEventListener(ScrollUp, MyEventTypes.MOUSE_SCROLL_UP);
        MyEvents.RemoveEventListener(ScrollDown, MyEventTypes.MOUSE_SCROLL_DOWN);
        MyEvents.RemoveEventListener(SetTargetColor, MyEventTypes.COLORPICKER_SET_TARGET);
        toolSettingsPanel.SetActive(false);
        groupHolder.gameObject.SetActive(false);
    }

    private void fetchGUIElements()
    {
        // Scale
        widthInput = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Dimensions/ScaleZ").GetComponent<InputField>();
        heightInput = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Dimensions/ScaleY").GetComponent<InputField>();
        thicknessInput = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Dimensions/ScaleX").GetComponent<InputField>();

        // Pacing
        gapSlider = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Pacing/GroupGapSize").GetComponent<Slider>();
        gapSliderValue = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Pacing/GroupGapSize/ValueBox/Text").GetComponent<Text>();

        // Tilt
        xAxis = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Rotations/TiltLeftRight").GetComponent<Slider>();
        yAxis = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Rotations/TiltForwardBackward").GetComponent<Slider>();
        xAxisValue = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Rotations/TiltLeftRight/ValueBox/Text").GetComponent<Text>();
        yAxisValue = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Rotations/TiltForwardBackward/ValueBox/Text").GetComponent<Text>();

        // Visual
        primaryColor = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Visuals/ButtonPrimaryColor").GetComponent<Button>();
        secondaryColor = GameObject.Find("/HUD/LeftHandPanel/CreateToolSettings/Visuals/ButtonSecondaryColor").GetComponent<Button>();
    }
    
    private void addGuiElementListeners()
    {
        widthInput.onValueChange.AddListener(delegate { this.calculateObjectPositions(); });
        heightInput.onValueChange.AddListener(delegate { this.calculateObjectPositions(); });
        thicknessInput.onValueChange.AddListener(delegate { this.calculateObjectPositions(); });

        // Pacing
        gapSlider.onValueChanged.AddListener(delegate { this.calculateObjectPositions(); });

        // Tilt
        xAxis.onValueChanged.AddListener(delegate { this.calculateObjectPositions(); });
        yAxis.onValueChanged.AddListener(delegate { this.calculateObjectPositions(); });

        // Visual
        /*
        primaryColor;
        secondaryColor;
        */
    }

    /// <summary>
    /// Vector calculator to calculate the x/y change in coordinates, when distance is d and angle is a
    /// </summary>
    /// <param name="dir">Direction in Angles</param>
    /// <param name="distance">Length to calculate in given Direction</param>
    /// <returns>Returns a two-dimensional vector from given values</returns>
    private Vector3 setVector(float dir, float distance)
	{
		float xspeed = Mathf.Cos(dir* Mathf.PI/180)* distance;
		float yspeed = Mathf.Sin(dir* Mathf.PI/180)* distance;
		Vector3 retPoint = new Vector3(xspeed, 0f, yspeed);
		return retPoint;
	}

    private void calculateBoundaries()
    {
        double yAngle = selectedObjPlaceholderTransform.localRotation.eulerAngles.z +90;
        double xAngle = yAngle;

        if (yAngle >= 360)
            yAngle -= 360;

        if (xAngle >= 360)
            xAngle -= 360;

        xAngle *= Mathf.Deg2Rad;
        yAngle *= Mathf.Deg2Rad;

        double a = Math.Abs(Math.Sin(xAngle) * objectSize.x) * selectedObjPlaceholderTransform.localScale.x;    // X Height
        double b = Math.Abs(Math.Cos(xAngle) * objectSize.x) * selectedObjPlaceholderTransform.localScale.x;    // X Width
        double c = Math.Abs(Math.Sin(yAngle) * objectSize.y) * selectedObjPlaceholderTransform.localScale.y;    // Y Width
        double d = Math.Abs(Math.Cos(yAngle) * objectSize.y) * selectedObjPlaceholderTransform.localScale.y;    // Y Height

        // Debug.Log("Sin: "+ Math.Sin(yAngle) + " Cos: "+ Math.Cos(yAngle) + " for Angle: "+Angle);
        
        double xSize = a+d;
        double ySize = b+c;
        // Debug.Log(ySize);
        // Debug.Log(objectSize.y);

        objectSpace.x = (float)xSize;
        objectSpace.y = (float)ySize;
    }

    private void calculateObjectPositions()
    {
        gapSliderValue.text = gapSlider.value.ToString();
        xAxisValue.text = xAxis.value.ToString();
        yAxisValue.text = yAxis.value.ToString();

        float rotPerPiece;
        if (Placeholders.Count > 2)
            rotPerPiece = groupBending / Placeholders.Count;
        else
            rotPerPiece = 0;

        Vector3 lastPos = Vector3.zero;
        Vector3 newScale = new Vector3(float.Parse(thicknessInput.text), float.Parse(widthInput.text), float.Parse(heightInput.text));

        for (int i = 0; i < Placeholders.Count; i++)
        {
            if (i != 0)
                Placeholders[i].localPosition = lastPos + setVector(rotPerPiece * i, ((objectSize.x * newScale.x) + gapSlider.value / 100));
            
            lastPos = Placeholders[i].localPosition;

            Placeholders[i].localScale = newScale;
            Placeholders[i].localRotation = test.localRotation;

            Placeholders[i].Rotate(new Vector3(0, -rotPerPiece * i, 0));
            Placeholders[i].Rotate(new Vector3(0f, xAxis.value, yAxis.value));
        }

        calculateBoundaries();
    }

    private void ScrollControl(int scrollDir)
    {
        if(GetInput.shiftKeyPressed)
        {
            pointerLocation.Rotate(new Vector3(0, 22.5f, 0)*scrollDir);
        }
        else if(GetInput.ctrlKeyPressed)
        {
            if (scrollDir > 0)
                AddToGroup();
            else
                RemoveFromGroup();
        }
    }

    private void ScrollUp()
    {
        ScrollControl(1);
    }

    private void ScrollDown()
    {
        ScrollControl(-1);
    }

    private void SetTargetColor()
    {
        HSVUtil.colorPicker.CurrentColor = Placeholders[0].GetComponent<Renderer>().sharedMaterials[HSVUtil.NthColor - 1].color;
        HSVUtil.colorPicker.onValueChanged.AddListener(color =>
        {
            foreach (Transform ph in Placeholders)
                ph.GetComponent<Renderer>().sharedMaterials[HSVUtil.NthColor - 1].color = color;
        });
    }

    private void AddToGroup()
    {
        selectedObjPlaceholderTransform = UnityEngine.Object.Instantiate(Placeholders[Placeholders.Count-1]);
        selectedObjPlaceholderTransform.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Collider col in selectedObjPlaceholderTransform.GetComponents<Collider>())
            col.isTrigger = true;

        selectedObjPlaceholderTransform.gameObject.layer = 2;
        selectedObjPlaceholderTransform.parent = groupHolder;

        Placeholders.Add(selectedObjPlaceholderTransform);

        calculateObjectPositions();
    }

    private void RemoveFromGroup()
    {
        if(Placeholders.Count > 1)
        {
            selectedObjPlaceholderTransform = Placeholders[Placeholders.Count - 2];
            GameObject.Destroy(Placeholders[Placeholders.Count - 1].gameObject);
            Placeholders.RemoveAt(Placeholders.Count-1);
            calculateObjectPositions();
        }
    }

    private void CreateObject()
    {
        if (!GetInput.cursorOverHud)
        {
            foreach (Transform t in Placeholders)
            {
                Transform o = UnityEngine.Object.Instantiate(t, t.position, t.rotation) as Transform;

                foreach (Collider col in o.GetComponents<Collider>())
                    col.isTrigger = false;

                o.gameObject.layer = 9;
                o.GetComponent<Rigidbody>().isKinematic = false;
                o.GetComponent<DynamicObject>().Set();
                Renderer oRend = o.GetComponent<Renderer>();
                Renderer tRend = t.GetComponent<Renderer>();
                Material[] tempMats = new Material[oRend.sharedMaterials.Length];

                for (int i = 0; i < oRend.sharedMaterials.Length; i++)
                    tempMats[i] = PerformanceMaterialColors.GetMaterial(tRend, tRend.sharedMaterials[i].color, i);

                oRend.sharedMaterials = tempMats;
                o.parent = dominoHolder;
            }
        }
    }
}
