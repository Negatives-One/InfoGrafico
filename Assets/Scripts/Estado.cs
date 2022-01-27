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
using TMPro;

[RequireComponent(typeof(PolygonCollider2D))]
//[ExecuteInEditMode]
public class Estado : ImmediateModeShapeDrawer
{
    public Vector3 offSetPingo = new Vector3();
    private Manager manager;

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
    [SerializeField] public List<Transform> sprites = new List<Transform>();

    public bool inside;

    private Polyline linha;
    private Rectangle rect;
    private Disc disc;
    private TMP_Text texto;
    private CaixaRuim cr;

    private Vector2 rectSize = new Vector2();
    private Vector2 textBoxSize = new Vector2(6f, 3.5f);

    private Vector3 distanceToDisplay = new Vector3();
    private float offSet = 0f;
    private float offSetMax = 0.6f;

    public static float stepTime = 0.15f;

    private Sequence returnSequence;
    private Sequence goSequence;
    private Sequence runningSequence;

    public bool goEnded = false;
    public bool returnEnded = false;

    public bool isInside = false;
    public bool clicked = false;

    public Color cor = Color.yellow;

    public Color HighlightColor;

    public void OnPointerDown()
    {
        if (!manager.geralMapa)
        {
            _onPress.Invoke();
            clicked = !clicked;
            if (clicked)
            {
                //manager.MoveBrasil(manager.cornerPos);
                Go();
                manager.ActiveExcept(nome);
                manager.ChangeSelected(this);
            }
            else
            {
                //manager.MoveBrasil(manager.centerPos);
                Return(stepTime);
                manager.selectedState = null;
            }
            Debug.Log("Press");
        }
    }

    public void OnPointerUp()
    {
        _onRelease.Invoke();
        Debug.Log("Release");
    }

    public void OnPointerEnter()
    {
        if (!manager.geralMapa && !clicked)
        {
            inside = true;
            //manager.ChangeSelected(this);
            ScaleAll(highlightSize, 0.5f);
            HighPriority();
            manager.DeactiveExcept(nome);
            Debug.Log("Enter");

            //Go();
        }
    }

    public void OnPointerExit()
    {
        if (!manager.geralMapa)
        {
            if (!clicked)
            {
                inside = false;
                ScaleAll(normalSize, 0.5f);
                LowPriority();
                manager.ActiveExcept(nome);
            }
            Debug.Log("Exit");
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

        nome = transform.parent.gameObject.name;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            sprites.Add(transform.parent.GetChild(i));
        }

        linha = transform.parent.gameObject.GetComponent<Polyline>();
        linha.SortingOrder = -3;
        linha.Thickness = 0.06f;
        linha.Color = Color.yellow;

        for (int p = 0; p < linha.points.Count; p++)
        {
            linha.points[p] = new PolylinePoint(linha.points[p].point, Color.yellow, linha.points[p].thickness);
        }
        Reiniciar();
        Load();

        rect = gameObject.transform.parent.GetChild(3).gameObject.GetComponent<Rectangle>();
        rect.gameObject.transform.position += (manager.displayPos.position - transform.parent.position) + Vector3.right * offSetMax;
        rect.gameObject.transform.position += Vector3.up * 0.36f;
        rect.gameObject.transform.position += Vector3.left * 0.3f;
        rect.SortingOrder = 20;
        rect.Color = new Color(1f, 1f, 1f, 0f);
        rect.Type = Rectangle.RectangleType.RoundedSolid;
        rect.CornerRadiusMode = Rectangle.RectangleCornerRadiusMode.PerCorner;
        rect.CornerRadii = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

        rect.SortingOrder = -12;

        GameObject a = Instantiate(manager.caixa, rect.gameObject.transform.position, Quaternion.identity, transform.parent);

        cr = a.GetComponent<CaixaRuim>();
        //cr.gameObject.transform.position += (manager.displayPos.position - transform.parent.position) + Vector3.right * offSetMax;
        cr.transform.localScale = Vector3.zero;


        texto = transform.parent.GetChild(4).gameObject.GetComponent<TMP_Text>();
        texto.text = nome;
        texto.fontSize = 2;



        GameObject bolinha = new GameObject("Bolinha");
        bolinha.transform.parent = rect.transform.parent;
        bolinha.transform.position = rect.transform.position + (Vector3.right * 0.3f) + (Vector3.down * 0.35f);
        disc = bolinha.AddComponent(typeof(Disc)) as Disc;
        disc.Radius = 0.06f;
        disc.SortingOrder = 20;
        disc.Color = new Color(1f, 0.92f, 0.016f, 0f);

        cor = Color.white;
    }

    void Update()
    {
        //distanceToDisplay = manager.displayPos.localPosition - transform.parent.localPosition;

        linha.SetPointPosition(0, new Vector3(0, 0));
        linha.SetPointPosition(1, new Vector3(distanceToDisplay.x, 0));
        linha.SetPointPosition(2, new Vector3(distanceToDisplay.x, distanceToDisplay.y));
        linha.SetPointPosition(3, new Vector3(distanceToDisplay.x + offSet, distanceToDisplay.y));

        sprites[2].gameObject.GetComponent<SpriteRenderer>().color = cor;

        rect.Width = rectSize.x;
        rect.Height = rectSize.y;

        //disc.transform.position = linha.points[linha.points.Count - 1].point;
        //disc.SortingOrder = linha.SortingOrder;
    }

    public void Return(float time)
    {
        ChangeColor(new Color(1f, 1f, 1f, 1f));
        LowPriority();
        goSequence.Kill();
        returnEnded = false;
        returnSequence = DOTween.Sequence();
        returnSequence.Append(DOTween.To(() => manager.informacao.transparency, x => manager.informacao.transparency = x, 0f, stepTime));
        goSequence.Append(DOTween.To(() => disc.Color, x => disc.Color = x, new Color(1f, 0.92f, 0.016f, 0f), 0.01f));
        //returnSequence.Append(DOTween.To(() => rectSize, x => rectSize = x, Vector2.zero, stepTime));
        returnSequence.Append(cr.transform.DOScale(Vector3.zero, stepTime));

        returnSequence.Append(DOTween.To(() => offSet, x => offSet = x, 0.05f, stepTime));
        returnSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(distanceToDisplay.x, 0.05f, 0f), stepTime));
        returnSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(0.05f, 0.05f, 0f), stepTime));
        //manager.returnSequence.OnComplete(() => Reiniciar());
        returnSequence.OnComplete(() => returnEnded = true);
        returnSequence.OnComplete(() => rect.Color = new Color(1f, 1f, 1f, 0f));
        returnSequence.OnComplete(() => ChangeColor(new Color(1f, 1f, 1f, 1f)));
        returnSequence.OnComplete(() => ScaleAll(normalSize, stepTime));
        returnSequence.Play();
    }

    public void Go()
    {
        rect.Color = Color.white;
        goEnded = false;
        manager.DefinirEstado(estadoNome2016, estadoHomicidiosFem2016.ToString(), estadoFeminicidios2016.ToString(), estadoHomicidiosFem2020.ToString(), estadoFeminicidios2020.ToString(), cr);
        returnSequence.Kill(false);
        linha.Color = Color.white;
        Vector3 targetDistance = manager.displayPos.localPosition - transform.parent.localPosition;

        goSequence = DOTween.Sequence();
        goSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(targetDistance.x, 0.05f, 0f), stepTime));
        goSequence.Append(DOTween.To(() => distanceToDisplay, x => distanceToDisplay = x, new Vector3(targetDistance.x, targetDistance.y, 0f), stepTime));
        goSequence.Append(DOTween.To(() => offSet, x => offSet = x, offSetMax, stepTime));
        //goSequence.Append(DOTween.To(() => rectSize, x => rectSize = x, textBoxSize, stepTime));
        goSequence.Append(DOTween.To(() => cr.transform.localScale, x => cr.transform.localScale = x, Vector3.one, stepTime));
        //returnSequence.Append(cr.transform.DOScale(Vector3.one, stepTime));

        goSequence.Append(DOTween.To(() => disc.Color, x => disc.Color = x, Color.yellow, 0.01f));
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
    //        Draw.Disc(linha.points[linha.points.Count - 1].point- offSetPingo, 0.06f, DiscColors.Flat(Color.black));
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
        DOTween.To(() => cor, x => cor = x, Color.yellow, stepTime);
        DOTween.To(() => texto.color, x => texto.color = x, new Color(0.90980f, 0.78824f, 0.14902f, 1f), stepTime);
        linha.SortingOrder = 10;
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
            else if (t.gameObject.name == "Text")
            {
                t.gameObject.GetComponent<MeshRenderer>().sortingOrder = 11;
            }
            else
            {
                if (t.gameObject.GetComponent<SpriteRenderer>())
                    t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
        }
    }
    private void LowPriority()
    {
        DOTween.To(() => cor, x => cor = x, new Color(1f, 1f, 1f), stepTime);
        DOTween.To(() => texto.color, x => texto.color = x, new Color(0f, 0f, 0f, 1f), stepTime);
        linha.SortingOrder = -0;
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
            else if (t.gameObject.name == "Text")
            {
                t.gameObject.GetComponent<MeshRenderer>().sortingOrder = 1;
            }
            else
            {
                if (t.gameObject.GetComponent<SpriteRenderer>())
                    t.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    public void Resetar()
    {
        ScaleAll(normalSize, .5f);
        LowPriority();
        isInside = false;
        inside = false;
        clicked = false;
    }

    public void ChangeColor(Color novaCor)
    {
        DOTween.To(() => cor, x => cor = x, novaCor, stepTime);
    }

    public void ChangeCor3D(Color novaCor)
    {
        DOTween.To(() => sprites[2].gameObject.GetComponent<SpriteRenderer>().color, x => sprites[2].gameObject.GetComponent<SpriteRenderer>().color = x, novaCor, stepTime);
    }
    public void ChangeCorLine(Color novaCor)
    {
        DOTween.To(() => sprites[1].gameObject.GetComponent<SpriteRenderer>().color, x => sprites[1].gameObject.GetComponent<SpriteRenderer>().color = x, novaCor, stepTime);
    }
    public void ChangeCorBase(Color novaCor)
    {
        DOTween.To(() => sprites[0].gameObject.GetComponent<SpriteRenderer>().color, x => sprites[0].gameObject.GetComponent<SpriteRenderer>().color = x, novaCor, stepTime);
    }
    public void ChangeCorTXT(Color novaCor)
    {
        DOTween.To(() => sprites[4].gameObject.GetComponent<TMP_Text>().color, x => sprites[4].gameObject.GetComponent<TMP_Text>().color = x, novaCor, stepTime);
    }

    private void Sinalizador(string type, bool value)
    {

    }
}
