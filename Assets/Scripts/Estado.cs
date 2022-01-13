using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Shapes;

[RequireComponent(typeof(PolygonCollider2D))]
//[ExecuteInEditMode]
public class Estado : ImmediateModeShapeDrawer
{
    private Manager manager;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    //[SerializeField] private float fadeDuration;
    private SpriteRenderer image;
    private PolygonCollider2D colisor;
    public UnityEvent _onRelease;
    public UnityEvent _onPress;
    public UnityEvent _onEnter;
    public UnityEvent _onExit;

    [Header("Tamanhos")]
    [SerializeField] private Vector3 normalSize = Vector3.one;
    [SerializeField] private Vector3 highlightSize = Vector3.one * 1.5f;

    [SerializeField] public string nome;
    [SerializeField] private List<Transform> sprites = new List<Transform>();

    public bool inside;

    private Vector3 linha1;
    private Vector3 linha2;
    private Vector3 linha3;
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
        manager.ChangeSelected(this);
        ScaleAll(highlightSize, 0.5f);
        HighPriority();
        manager.DeactiveExcept(nome);
        Debug.Log("Enter");
    }

    public void OnPointerExit()
    {
        inside = false;
        image.color = normalColor;
        ScaleAll(normalSize, 0.5f);
        LowPriority();
        manager.ActiveExcept(nome);
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
        highlightSize = Vector3.one * 1.2f;
        manager = GameObject.Find("MouseManager").GetComponent<Manager>();
        image = GetComponent<SpriteRenderer>();
        colisor = GetComponent<PolygonCollider2D>();
        normalColor = Color.white;
        highlightedColor = Color.white;
        pressedColor = Color.white;
        image.color = normalColor;

        nome = transform.parent.gameObject.name;
        for(int i = 0; i < transform.parent.childCount; i++)
        {
            sprites.Add(transform.parent.GetChild(i));
        }
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.Line(transform.position, transform.position + Vector3.right * 5f, 0.1f, Color.white, Color.white); // Drawing happens here
        }
    }

    private void ScaleAll(Vector3 tamanho, float tempo)
    {
        foreach (Transform t in sprites)
        {
            t.DOScale(tamanho, tempo);
        }
    }

    private void HighPriority()
    {
        foreach (Transform t in sprites)
        {
            if (t.gameObject.name == "3D")
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 9;
            }
            else
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
        }
    }
    private void LowPriority()
    {
        foreach (Transform t in sprites)
        {
            if(t.gameObject.name == "3D")
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
            else
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    public void Resetar()
    {
        ScaleAll(normalSize, .5f);
        LowPriority();
    }
}
