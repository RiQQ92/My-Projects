using UnityEngine;
using System.Collections;
using System;

public class ToolManager : GetInput
{
    public enum Tools
    {
        Select = 1,
        Create = 2,
        Push = 3
    };

    public Transform CreateMarker;
    public Transform PushMarker;
    private static Tools selectedTool = Tools.Create;
    private CreateTool createTool = new CreateTool();
    public PushTool pushTool = new PushTool();

    private bool leftMouseFirstPress;
    private bool rightMouseFirstPress;
    private bool hasMouseMoved;

    private float doubleClickInterval;
    private float lastClickTime;
    
    private Vector3 mouseClickStartPos;

    // Use this for initialization
    void Start ()
    {
        createTool.Initialize(CreateMarker);
        pushTool.Initialize(PushMarker);
        leftMouseFirstPress = false;
        rightMouseFirstPress = false;
        hasMouseMoved = false;
        useGUILayout = false;
        
        SetTool((int)selectedTool);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!GameManager.menuOpen)
            useTool();
    }

    private void useTool()
    {
        switch(selectedTool)
        {
            case Tools.Create:
                createTool.ManualUpdate();
                break;

            case Tools.Push:
                pushTool.ManualUpdate();
                break;

            default:
                break;
        }
    }
    
    public void SetTool(int ToolToUse)
    {
        selectedTool = (Tools)ToolToUse;

        createTool.Disable();
        pushTool.Disable();

        switch ((Tools)ToolToUse)
        {
            case Tools.Create:
                createTool.Enable();
                break;

            case Tools.Push:
                pushTool.Enable();
                break;

            default:
                break;
        }
    }

    private void selectTroops()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

        }

        // mouse left button Actions
        if (leftMousePressed && !leftMouseFirstPress)
        {

        }
        else if(leftMousePressed && leftMouseFirstPress)
        {

        }
        else
        {

        }

        // mouse right button Actions
        if(rightMousePressed && !rightMouseFirstPress)
        {

        }
        else if(!rightMousePressed && rightMouseFirstPress)
        {

        }
    }

    private bool checkMousePos(float moveError = 5.0f)
    {
        bool mouseMoved = false;
        Vector3 mousePos = Input.mousePosition;

        float xDiff = Mathf.Abs(mousePos.x - mouseClickStartPos.x);
        float yDiff = Mathf.Abs(mousePos.y - mouseClickStartPos.y);

        if (moveError < xDiff || moveError < yDiff)
            mouseMoved = true;

        return mouseMoved;
    }
}