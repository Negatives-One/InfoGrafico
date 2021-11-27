using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
[ExecuteInEditMode]
public class GameButton : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    //[SerializeField] private float fadeDuration;
    private SpriteRenderer image;
    private BoxCollider2D colisor;
    public UnityEvent _onRelease;
    public UnityEvent _onPress;
    public UnityEvent _onEnter;
    public UnityEvent _onExit;

    public bool inside;
    public void OnPointerDown()
    {
        _onPress.Invoke();
        image.color = pressedColor;
        Debug.Log("Press");
    }

    public void OnPointerUp()
    {
        _onRelease.Invoke();
        image.color = highlightedColor;
        Debug.Log("Release");
    }

    public void OnPointerEnter()
    {
        inside = true;
        image.color = highlightedColor;
        Debug.Log("Enter");
    }

    public void OnPointerExit()
    {
        inside = false;
        image.color = normalColor;
        Debug.Log("Exit");
    }
    private void Awake()
    {
        if (_onRelease == null) { _onRelease = new UnityEvent(); }
        if (_onPress == null) { _onPress = new UnityEvent(); }
        if (_onExit == null) { _onExit = new UnityEvent(); }
        if (_onEnter == null) { _onEnter = new UnityEvent(); }
    }

    private void Start()
    {
        image = GetComponent<SpriteRenderer>();
        colisor = GetComponent<BoxCollider2D>();
        image.color = normalColor;
    }

    private void Update()
    {
        colisor.size = image.size;
    }
}
