using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PolygonCollider2D))]
public class Botao : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    public CaixaRuim cr;
    public TMP_Text text;
    public bool is2016;
    //[SerializeField] private float fadeDuration;
    private SpriteRenderer image;
    private PolygonCollider2D colisor;
    public UnityEvent _onRelease;
    public UnityEvent _onPress;
    public UnityEvent _onEnter;
    public UnityEvent _onExit;

    public bool inside;
    public void OnPointerDown()
    {
        _onPress.Invoke();
        image.color = pressedColor;
        if (is2016)
        {
            cr.clicked2016 = true;
        }
        else
        {
            cr.clicked2016 = false;
        }
        cr.Refresh();
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
        colisor = GetComponent<PolygonCollider2D>();
        image.color = normalColor;
    }

    private void Update()
    {

    }
}