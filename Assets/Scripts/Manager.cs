using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using DG.Tweening;
using TMPro;

public class Manager : ImmediateModeShapeDrawer
{
    public bool geralMapa = false;
    public Transform Brasil;

    private Color claro = new Color(0.47451f, 0.61569f, 0.85882f);
    private Color escuro = new Color(0.06667f, 0.22745f, 0.50588f);

    //claro rgba(0.47451, 0.61569, 0.85882)
    //escuro rgba(0.06667, 0.22745, 0.50588)

    private float legendaTransparency = 0f;

    public List<Rectangle> legendaRects;
    public TMP_Text legendaTxt;

    RaycastHit2D hit;
    RaycastHit2D pHit;

    public Estado selectedState;
    public List<Estado> estados = new List<Estado>();

    [SerializeField] private float zOffSet = 0;
    [SerializeField] public Transform displayPos;

    public Vector2 textBoxSize = new Vector2(5f, 3.5f);

    public Rectangle rectangle;

    public Vector2 rectSize;

    public Informacao informacao;

    public Estado toReset;

    public struct Informacao
    {
        public string estadoNome;
        public string homicidio2016;
        public string feminicidio2016;
        public string homicidio2020;
        public string feminicidio2020;
        public float transparency;
    }

    public TMP_Text Header;
    public TMP_Text TMP2016;
    public TMP_Text TMP2020;

    public Sequence centerSequence;
    public Sequence cornerSequence;

    public float cornerPos = -3.7f;
    public float centerPos = 0f;

    private void Start()
    {
        Transform brasil = GameObject.Find("Brasil").transform;
        for (int i = 0; i < brasil.childCount - 2; i++)
        {
            estados.Add(brasil.GetChild(i).GetChild(0).gameObject.GetComponent<Estado>());
        }
        rectangle = GameObject.Find("Text").GetComponent<Rectangle>();
        informacao = new Informacao();
        informacao.transparency = 0;
        toReset = estados[0];
        Map1();
    }

    void Update()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, zOffSet);
        if (!pHit)
        {
            pHit = hit;
        }

        //ENTER BUTTON
        if (hit && !hit.collider.gameObject.GetComponent<Estado>().inside)
        {
            hit.collider.gameObject.GetComponent<Estado>().OnPointerEnter();
        }

        //EXIT BUTTON
        if (hit != pHit)
        {
            pHit.collider.gameObject.GetComponent<Estado>().OnPointerExit();
        }

        //CLICK BUTTON
        if (hit && Input.GetMouseButtonDown(0))
        {
            hit.collider.gameObject.GetComponent<Estado>().OnPointerDown();
        }

        //RELEASE BUTTON
        if (hit && Input.GetMouseButtonUp(0))
        {
            hit.collider.gameObject.GetComponent<Estado>().OnPointerUp();
        }

        pHit = hit;

        Header.color = new Color(0, 0, 0, informacao.transparency);
        TMP2016.color = new Color(0, 0, 0, informacao.transparency);
        TMP2020.color = new Color(0, 0, 0, informacao.transparency);

        for(int i = 0; i < legendaRects.Count; i++)
        {
            legendaRects[i].Color = new Color(legendaRects[i].Color.r, legendaRects[i].Color.g, legendaRects[i].Color.b, legendaTransparency);
        }
        legendaTxt.color = new Color(0f, 0f, 0f, legendaTransparency);
    }

    public void ChangeSelected(Estado state)
    {
        if (selectedState == null)
        {
            selectedState = state;
        }
        else
        {
            selectedState.Resetar();
            selectedState.Return(0.1f);
            selectedState = state;
        }
    }

    public void DeactiveExcept(string nome)
    {
        foreach (Estado e in estados)
        {
            if (e.nome != nome)
            {
                e.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }
    }
    public void ActiveExcept(string nome)
    {
        foreach (Estado e in estados)
        {
            if (e.nome != nome)
            {
                e.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
            }
        }
    }

    void ScaleBox(Vector2 size)
    {
        DOTween.To(() => rectSize, x => rectSize = x, size, 0.3f);
    }

    public void DefinirEstado(string _estadoNome, string _homicidio2016, string _feminicidio2016, string _homicidio2020, string _feminicidio2020)
    {
        informacao.estadoNome = _estadoNome;
        informacao.homicidio2016 = _homicidio2016;
        informacao.feminicidio2016 = _feminicidio2016;
        informacao.homicidio2020 = _homicidio2020;
        informacao.feminicidio2020 = _feminicidio2020;
        
        string text2016 = "2016\n\nHomicídio Feminino\n"+ informacao.homicidio2016 +"\n\nFeminicídios\n" + informacao.feminicidio2016;
        string text2020 = "2020\n\nHomicídio Feminino\n" + informacao.homicidio2020 + "\n\nFeminicídios\n" + informacao.feminicidio2020;

        Header.text = informacao.estadoNome;
        TMP2016.text = text2016;
        TMP2020.text = text2020;
    }

    public void Map2()
    {
        DOTween.To(() => legendaTransparency, x => legendaTransparency = x, 1f, 0.5f);
        for (int k = 0; k < estados.Count; k++)
        {
            Estado estado = estados[k];
            if(estado.estadoHomicidiosFem2016 < estado.estadoHomicidiosFem2020)
            {
                estado.ChangeColor(claro);
            }
            else
            {
                estado.ChangeColor(escuro);
            }
        }
        geralMapa = true;
        cornerSequence.Kill();
        centerSequence = DOTween.Sequence();
        centerSequence.Append(Brasil.DOMoveX(centerPos, 0.5f));
        centerSequence.Play();
    }

    public void Map1()
    {
        DOTween.To(() => legendaTransparency, x => legendaTransparency = x, 0f, 0.5f);
        for (int k = 0; k < estados.Count; k++)
        {
            Estado estado = estados[k];
            estado.ChangeColor(Color.white);
        }
        geralMapa = false;
        centerSequence.Kill();
        cornerSequence = DOTween.Sequence();
        centerSequence.Append(Brasil.DOMoveX(centerPos, 0.5f));
        centerSequence.OnComplete(() => geralMapa = false);
        centerSequence.Play();
    }

    public void MoveBrasil(float pos)
    {
        cornerSequence = DOTween.Sequence();
        centerSequence.Append(Brasil.DOMoveX(pos, 0.5f));
        centerSequence.OnComplete(() => geralMapa = false);
        centerSequence.Play();
    }

    public void Clean()
    {
        foreach(Estado e in estados)
        {
            e.Resetar();
            e.Return(0f);
        }
    }

    public void SelectClear()
    {
        selectedState = null;
    }
}
