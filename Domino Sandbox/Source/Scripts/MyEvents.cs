using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public static class MyEventTypes
{
    public static string DEFAULT_EVENT = "Default Event";

    public static string COLORPICKER_SET_TARGET = "Colorpicker Set Target Event";
    public static string COLORPICKER_CLOSE = "Colorpicker Close Event";

    public static string GAME_TOGGLE_PAUSE_EVENT = "Game Toggle Pause Event";
    public static string GAME_PAUSE_EVENT = "Game Pause Event";
    public static string GAME_PLAY_EVENT = "Game Play Event";

    public static string RESET_SCENE_EVENT = "Reset Scene Event";
    public static string CLEAR_SCENE_EVENT = "Clear Scene Event";
    public static string CLEAR_DYNAMIC_EVENT = "Clear Dynamic Objects Event";
    public static string CLEAR_STATIC_EVENT = "Clear Static Objects Event";

    public static string LEFT_MB_DOWN = "Left Mouse Down Event";
    public static string RIGHT_MB_DOWN = "Right Mouse Down Event";
    public static string MIDDLE_MB_DOWN = "Middle Mouse Down Event";

    public static string LEFT_MB_UP = "Left Mouse Up Event";
    public static string RIGHT_MB_UP = "Right Mouse Up Event";
    public static string MIDDLE_MB_UP = "Middle Mouse Up Event";
    
    public static string MOUSE_SCROLL_DOWN = "Mouse Scroll Down Event";
    public static string MOUSE_SCROLL_UP = "Mouse Scroll Up Event";
}

public static class MyEvents
{
    private class EventCall
    {
        public UnityAction Event;
        public string EventType;

        public EventCall(UnityAction EVENT, string EVENT_TYPE)
        {
            Event = EVENT;
            EventType = EVENT_TYPE;
        }
    }

    private static List<EventCall> EventCalls = new List<EventCall>();

    public static void TriggerEvent(string EventType)
    {
        foreach(EventCall Call in EventCalls.FindAll(delegate (EventCall ec) { return ec.EventType == EventType; }))
        {
            if(Call.Event != null)
                Call.Event();
        }
    }

    public static void AddEventListener(UnityAction FunctionToCall, string EventType)
    {
        EventCalls.Add(new EventCall(FunctionToCall, EventType));
    }

    public static void RemoveEventListener(UnityAction FunctionToCall, string EventType)
    {
        EventCalls.RemoveAll(delegate (EventCall ec) { return ec.EventType == EventType && ec.Event == FunctionToCall; });
    }

    public static void RemoveAllListenersOfType(string EventType)
    {
        EventCalls.RemoveAll(delegate (EventCall ec) { return ec.EventType == EventType; });
    }

    public static void RemoveAllListeners()
    {
        EventCalls = null;
        EventCalls = new List<EventCall>();
    }
}
