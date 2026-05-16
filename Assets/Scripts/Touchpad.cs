using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Touchpad : MonoBehaviour
{

    private bool touch;
    private Vector2 tapPos;
    [Serializable]
    public class MethodContainer : UnityEngine.Events.UnityEvent<Vector2> { };
    [Header("Coords of tap:")]
    public MethodContainer OnDrag;

    private void Start()
    {
        EventTrigger.Entry down = new EventTrigger.Entry(), drag = new EventTrigger.Entry(), up = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(OnPointerDown);
        gameObject.AddComponent<EventTrigger>().triggers.Add(down);
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener(OnPointerDrag);
        GetComponent<EventTrigger>().triggers.Add(drag);
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(OnPointerUp);
        GetComponent<EventTrigger>().triggers.Add(up);
    }

    public void OnPointerDown(BaseEventData data)
    {
        touch = true;
        tapPos = (data as PointerEventData).position;
        //StaticVars.game.SetJoystick(tapPos);//, (data as PointerEventData).pointerId);
    }

    public void OnPointerDrag(BaseEventData data)
    {
        tapPos = (data as PointerEventData).position;
    }

    public void OnPointerUp(BaseEventData data)
    {
        touch = false;
        //StaticVars.game.HideJoystick((data as PointerEventData).pointerId);
    }

    private void Update()
    {
        if (touch)
            OnDrag.Invoke(tapPos);
    }

}