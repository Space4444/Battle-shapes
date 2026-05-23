using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Diagnostics;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public float width;
    public bool right;
    public Transform control;
    private Canvas canvas;
    private float angle, value, width1;
    private Vector2 tapPos, halfScreenSize;
    [Serializable]
    public class MethodContainer : UnityEngine.Events.UnityEvent<float, float> { };
    [Header("First is angle, second is value from 0 to 1")]
    public MethodContainer Drag;

    private void Start()
    {
        #if !UNITY_ANDROID
            gameObject.SetActive(false);
        #endif
        halfScreenSize = new Vector2(Screen.width, Screen.height) * 0.5f;
        width1 = 1f / width;
        canvas = ((RectTransform)transform).GetComponentInParent<Canvas>();
    }

    public virtual void OnPointerDown(PointerEventData data)
    {
        if (right)
            StaticVars.player.StartFire();
        else
            StaticVars.player.StartMove();
        OnDrag(data);
    }

    public virtual void OnDrag(PointerEventData data)
    {
        tapPos = data.position;
        Vector2 d = tapPos - halfScreenSize - (Vector2)transform.localPosition * canvas.scaleFactor;
        float c = d.magnitude;
        value = c > width ? width : c;
        angle = Mathf.Atan2(d.y, d.x);
        control.localPosition = value * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Drag.Invoke(angle, value * width1);
    }

    public virtual void OnPointerUp(PointerEventData data)
    {
        if (right)
            StaticVars.player.StopFire();
        else
            StaticVars.player.StopMove();
        control.localPosition = Vector3.zero;
    }
}