using System;
using System.Collections;
using UnityEngine.EventSystems;

public static class UIEvents
{
    public static void CreateEventTriggerInstance(EventTrigger trigger, EventTriggerType triggerType, Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener((eventData) => action());

        trigger.triggers.Add(entry);
    }

    public static IEnumerator RepeateAction(float startDelay, float repeateTerm, Action repeateAction)
    {
        yield return CoroutineTime.GetWaitForSecondsTime(startDelay);

        while (true)
        {
            repeateAction();
            yield return CoroutineTime.GetWaitForSecondsTime(repeateTerm);
        }
    }
}
