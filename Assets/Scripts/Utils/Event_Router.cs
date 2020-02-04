using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event_Router : MonoBehaviour
{

    public UnityEvent custom_event1;
    public UnityEvent custom_event2;
    public UnityEvent custom_event3;

    public void TriggerEvent1() { if (custom_event1 != null) custom_event1.Invoke(); }
    public void TriggerEvent2() { if (custom_event2 != null) custom_event2.Invoke(); }
    public void TriggerEvent3() { if (custom_event3 != null) custom_event3.Invoke(); }

}
