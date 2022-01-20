using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Shapes;
using SimpleJSON;
using System.IO;

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

    private Polyline linha;
    private Rectangle rect;

    private Vector2 rectSize = new Vector2();
    private Vector2 textBoxSize = new Vector2(5f, 3.5f);

    private Vector3 distanceToDisplay = new Vector3();
    private float offSet = 0f;
    private float offSetMax = 0.3f;

    private float stepTime = 0.15f;

    private Sequence returnSequence;
    private Sequence goSequence;
    private Sequence runningSequence;

    public bool goEnded = false;
    public bool returnEnded = false;

    public bool isInside = false;

    private Color cor = new Color(1f, 1f, 1f);

    public void OnPointerDown()
    {
        _onPress.Invoke();
        //image.color = pressedColor;
        Debug.Log("Press");
    }

    public void OnPointerUp()
    {
        _onRelease.Invoke();
        //image.color = highlightedColor;
        Debug.Log("Release");
    }

    public void OnPointerEnter()
    {
        if (!manager.geralMapa)
        {
            inside = true;
            image.color = highlightedColor;
            manager.ChangeSelected(this);
            ScaleAll(highlightSize, 0.5f);
            HighPriority();
            manager.DeactiveExcept(nome);
            Debug.Log("Enter");

            Go();
        }
    }

    public void OnPointerExit()
    {
        if (!manager.geralMapa)
        {
            inside = false;
            image.color = normalColor;
            ScaleAll(normalSize, 0.5f);
            LowPriority();
            manager.ActiveExcept(nome);
            Debug.Log("Exit");

            Return();
        }
    }
    private void Awake()
    {
        if (_onRelease == null) { _onRelease = new UnityEvent(); }
        if (_onPress == null) { _onPress = new UnityEvent(); }
        if (_onExit == null) { _onExit = new UnityEvent(); }
        if (_onEnter == null) { _onEnter = new UnityEvent(); }
    }
    // Atributos do Estado 
    [SerializeField]
    public string estadoNome2020;
    [SerializeField]
    public int estadoHomicidiosFem2020;
    [SerializeField]
    public int estadoFeminicidios2020;
    [SerializeField]
    public string estadoNome2016;
    [SerializeField]
    public int estadoHomicidiosFem2016;
    [SerializeField]
    public int estadoFeminicidios2016;
    //Sistema de carregamento do Json.

    void Load()
    {
        string path = Application.streamingAssetsPath + "/estados2020.json";
        string jsonString = File.ReadAllText(path);
        JSONObject estadoJson = (JSONObject)JSON.Parse(jsonString);
        var estadoBase = (JSONObject)JSON.Parse(estadoJson[nome].ToString());
        estadoNome2020 = estadoBase["estadoNome"];
        estadoHomicidiosFem2020 = estadoBase["estadoHomicidiosFem"];
        estadoFeminicidios2020 = estadoBase["estadoFeminicidios"];

        string path2 = Application.streamingAssetsPath + "/estados2016.json";
        string jsonString2 = File.ReadAllText(path2);
        JSONObject estadoJson2 = (JSONObject)JSON.Parse(jsonString2);
        var estadoBase2 = (JSONObject)JSON.Parse(estadoJson2[nome].ToString());
        estadoNome2016 = estadoBase2["estadoNome"];
        estadoHomicidiosFem2016 = estadoBase2["estadoHomicidiosFem"];
        estadoFeminicidios2016 = estadoBase2["estadoFeminicidios"];

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
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            sprites.Add(transform.parent.GetChild(i));
        }

        linha = transform.parent.gameObject.GetComponent<Polyline>();
        linha.SortingOrder = -3;
        linha.Thickness = 0.1f;
        linha.Color = Color.white;
        for(int p = 0; p < linha.points.Count; p++)
        {
            linha.points[p] = new PolylinePoint(linha.points[p].point, Color.white, linha.points[p].thickness - 0.1f) ;
        }
        Reiniciar();
        Load();

        rect = gameObject.transform.parent.GetChild(3).gameObject.GetComponent<Rectangle>();
        rect.gameObject.transform.position += (manager.displayPos.position - transform.parent.position) + Vector3.right * offSetMax;
        rect.gameObject.transform.position += Vector3.up * 0.06f;
        rect.SortingOrder = 20;
        rect.Color = new Color(1f, 1f, 1f, 0f);
        rect.Type = Rectangle.RectangleType.RoundedSolid;
        rect.CornerRadiusMode = Rectangle.RectangleCornerRadiusMode.PerCorner;
        rect.CornerRadii = new Vector4(0f, 0.25f, 0.25f, 0.25f);
    }

    void Update()
    {
        //distanceToDisplay = manager.displayPos.localPosition - transform.parent.localPosition;

        linha.SetPointPosition(0, new Vector3(0, 0));
        linha.SetPointPosition(1, new Vector3(distanceToDisplay.x, 0));
        linha.SetPointPosition(2, new Vector3(distanceToDisplay.x, distanceToDisplay.y));
        linha.SetPointPosition(3, new Vector3(distanceToDisplay.x + offSet, distanceToDisplay.y));

        sprites[0].gameObject.GetComponent<SpriteRenderer>().color = cor;
        sprites[2].gameObject.GetComponent<SpriteRenderer>().color = cor;

        rect.Width = rectSize.x;
        rect.Height = rectSize.y;
    }

    void Return()
    {
        goSequence.Kill();
        returnEnded = false;
        returnSequence = DOTween.Sequence();
        returnSequence.Append(DOTween.To(() => manager.informacao.transparency, x => manager.informacao.transparency = x, 0f, stepTime));
        returnSequence.Append(DOTween.To(() => rectSize, x => rectSize = x, Vector2.zero, stepTime));
        returnSequence.Append(DOTween.To(() => offSet, x => offSet = x, 0.05f, stepTime));
        returnSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(distanceToDisplay.x, 0.05f, 0f), stepTime));
        returnSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(0.05f, 0.05f, 0f), stepTime));
        //manager.returnSequence.OnComplete(() => Reiniciar());
        returnSequence.OnComplete(() => returnEnded = true);
        returnSequence.OnComplete(() => rect.Color = new Color(1f, 1f, 1f, 0f));
        returnSequence.Play();
    }

    void Go()
    {
        rect.Color = Color.white;
        goEnded = false;
        manager.DefinirEstado(estadoNome2016, estadoHomicidiosFem2016.ToString(), estadoFeminicidios2016.ToString(), estadoHomicidiosFem2020.ToString(), estadoFeminicidios2020.ToString());
        returnSequence.Kill(false);
        linha.Color = Color.white;
        Vector3 targetDistance = manager.displayPos.localPosition - transform.parent.localPosition;

        goSequence = DOTween.Sequence();
        goSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(targetDistance.x, 0.05f, 0f), stepTime));
        goSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(targetDistance.x, targetDistance.y, 0f), stepTime));
        goSequence.Append(DOTween.To(() => offSet, x => offSet = x, offSetMax, stepTime));
        goSequence.Append(DOTween.To(() => rectSize, x => rectSize = x, textBoxSize, stepTime));
        goSequence.Append(DOTween.To(() => manager.informacao.transparency, x => manager.informacao.transparency = x, 1f, stepTime));
        returnSequence.OnComplete(() => goEnded = true);
        goSequence.Play();

        //DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(targetDistance.x, targetDistance.y, 0), stepTime);
        //DOTween.To(() => offSet, x => offSet = x, offSetMax, stepTime);
    }

    void Reiniciar()
    {
        offSet = 0.05f;
        distanceToDisplay = new Vector3(0.05f, 0.05f, 0f);
        linha.Color = new Color(1, 1, 1, 0);
        //manager.rectSize = Vector2.zero;
        manager.informacao.transparency = 0f;
    }

    //public override void DrawShapes(Camera cam)
    //{
    //    using (Draw.Command(cam))
    //    {
    //        Draw.Line(transform.position, transform.position + Vector3.right * 5f, 0.1f, Color.white, Color.white); // Drawing happens here
    //    }
    //}

    private void ScaleAll(Vector3 tamanho, float tempo)
    {
        foreach (Transform t in sprites)
        {
            if (t.gameObject.name == "Rectangle")
            {
                continue;
            }
            else
            {
                t.DOScale(tamanho, tempo);
            }
        }
    }

    private void HighPriority()
    {
        DOTween.To(() => cor, x => cor = x, new Color(0.9333333f, 0.4509804f, 0.007843138f), stepTime);
        linha.SortingOrder = 9;
        foreach (Transform t in sprites)
        {
            if (t.gameObject.name == "3D")
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 9;
            }
            else if (t.gameObject.name == "Rectangle")
            {
                continue;
            }
            else
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
        }
    }
    private void LowPriority()
    {
        DOTween.To(() => cor, x => cor = x, new Color(1f, 1f, 1f), stepTime);
        linha.SortingOrder = -1;
        foreach (Transform t in sprites)
        {
            if (t.gameObject.name == "3D")
            {
                t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
            else if (t.gameObject.name == "Rectangle")
            {
                continue;
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

    public void ChangeColor(Color novaCor)
    {
        DOTween.To(() => cor, x => cor = x, novaCor, stepTime);
    }

    private void Sinalizador(string type, bool value)
    {

    }
}
