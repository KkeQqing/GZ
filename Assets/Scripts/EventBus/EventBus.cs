using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType {

    Time_Rewind_Start,// 回溯开始
    Time_Rewind_End,
    Time_Slow_Start, // 减速开始
    Time_Slow_End,
    Time_FastForward_Start, // 快进开始
    Time_FastForward_End

}
public class EventBus 
{
    private static Dictionary<EventType, Action> eventDictionary = new Dictionary<EventType, Action>();

    public static void registerEvent(EventType type, Action action) {
        if (!eventDictionary.ContainsKey(type)) {
            eventDictionary[type] = null;
        }
        eventDictionary[type] += action;
    }

    public static void disRegisterEvent(EventType type, Action action)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type] -= action;
        }
    }

    public static void publish(EventType type)
    {
        if (eventDictionary.ContainsKey(type)) {
            eventDictionary[type]?.Invoke();
        }
    }
}
