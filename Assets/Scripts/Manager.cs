using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class Manager : ImmediateModeShapeDrawer
{
    public bool geralMapa = false;
    public Transform Brasil;

    public Color claro = new Color(0.98431f, 0.92157f, 0.37255f);
    public Color escuro = new Color(0.93333f, 0.45098f, 0.00784f);

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

    public GameObject caixa;
    private void Start()
    {
        Transform brasil = GameObject.Find("Brasil").transform;
        centerPos = brasil.position.x;
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
        if (hit )
        {
            if (hit.collider.gameObject.GetComponent<Estado>())
                hit.collider.gameObject.GetComponent<Estado>().OnPointerEnter();
            else
                hit.collider.gameObject.GetComponent<Botao>().OnPointerEnter();
        }

        //EXIT BUTTON
        if (hit != pHit)
        {
            if (pHit.collider.gameObject.GetComponent<Estado>())
                pHit.collider.gameObject.GetComponent<Estado>().OnPointerExit();
            else
                pHit.collider.gameObject.GetComponent<Botao>().OnPointerExit();
        }

        //CLICK BUTTON
        if (hit && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.gameObject.GetComponent<Estado>())
                hit.collider.gameObject.GetComponent<Estado>().OnPointerDown();
            else
                hit.collider.gameObject.GetComponent<Botao>().OnPointerDown();
        }

        //RELEASE BUTTON
        if (hit && Input.GetMouseButtonUp(0))
        {
            if (hit.collider.gameObject.GetComponent<Estado>())
                hit.collider.gameObject.GetComponent<Estado>().OnPointerUp();
            else
                hit.collider.gameObject.GetComponent<Botao>().OnPointerUp();
        }

        pHit = hit;

        Header.color = new Color(0, 0, 0, informacao.transparency);
        TMP2016.color = new Color(0, 0, 0, informacao.transparency);
        TMP2020.color = new Color(0, 0, 0, informacao.transparency);

        for (int i = 0; i < legendaRects.Count; i++)
        {
            legendaRects[i].Color = new Color(legendaRects[i].Color.r, legendaRects[i].Color.g, legendaRects[i].Color.b, legendaTransparency);
        }
        legendaTxt.color = new Color(1f, 1f, 1f, legendaTransparency);
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

    public void DefinirEstado(string _estadoNome, string _homicidio2016, string _feminicidio2016, string _homicidio2020, string _feminicidio2020, CaixaRuim cr)
    {
        informacao.estadoNome = _estadoNome;
        informacao.homicidio2016 = _homicidio2016;
        informacao.feminicidio2016 = _feminicidio2016;
        informacao.homicidio2020 = _homicidio2020;
        informacao.feminicidio2020 = _feminicidio2020;

        string text2016 = informacao.homicidio2016 + " Homicídios\n" + informacao.feminicidio2016 + " Feminicídios";
        string text2020 = informacao.homicidio2020 + " Homicídios\n" + informacao.feminicidio2020 + " Feminicídios";

        cr.text2016 = text2016;
        cr.text2020 = text2020;
        cr.textNome = informacao.estadoNome;

        Header.text = informacao.estadoNome;
        TMP2016.text = text2016;
        TMP2020.text = text2020;
    }

    public void Map3()
    {
        geralMapa = true;
        DOTween.To(() => legendaTransparency, x => legendaTransparency = x, 1f, 0.5f);

        for (int k = 0; k < estados.Count; k++)
        {
            Estado estado = estados[k];

            if (estado.estadoFeminicidios2016 > estado.estadoFeminicidios2020)
            {
                estado.ChangeCor3D(Color.white);
                estado.ChangeCorBase(claro);
                estado.ChangeCorLine(Color.white);
            }
            else
            {
                estado.ChangeCor3D(Color.white);
                estado.ChangeCorBase(escuro);
                estado.ChangeCorLine(Color.white);
            }
        }


        cornerSequence.Kill();
        centerSequence = DOTween.Sequence();
        centerSequence.Append(Brasil.DOMoveX(centerPos, 0.5f));
        centerSequence.Play();
    }
    public void Map2()
    {
        geralMapa = true;
        DOTween.To(() => legendaTransparency, x => legendaTransparency = x, 1f, 0.5f);

        for (int k = 0; k < estados.Count; k++)
        {
            Estado estado = estados[k];

            if (estado.estadoHomicidiosFem2016 > estado.estadoHomicidiosFem2020)
            {
                estado.ChangeCor3D(Color.white);
                estado.ChangeCorBase(claro);
                estado.ChangeCorLine(Color.white);
            }
            else
            {
                estado.ChangeCor3D(Color.white);
                estado.ChangeCorBase(escuro);
                estado.ChangeCorLine(Color.white);
            }
        }


        cornerSequence.Kill();
        centerSequence = DOTween.Sequence();
        centerSequence.Append(Brasil.DOMoveX(centerPos, 0.5f));
        centerSequence.Play();
    }

    public void Map1()
    {
        geralMapa = false;
        DOTween.To(() => legendaTransparency, x => legendaTransparency = x, 0f, 0.5f);
        for (int k = 0; k < estados.Count; k++)
        {
            Estado estado = estados[k];
            //DOTween.To(() => estado.sprites[0].gameObject.GetComponent<SpriteRenderer>().color, x => estado.sprites[0].gameObject.GetComponent<SpriteRenderer>().color = x, Color.white, 0.5f);
            estado.ChangeCor3D(Color.white);
            estado.ChangeCorBase(Color.white);
            estado.ChangeCorLine(new Color(0.8196079f, 0.8196079f, 0.8196079f, 1f));
        }
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
        foreach (Estado e in estados)
        {
            e.Resetar();
            e.Return(0f);
        }
    }

    public void SelectClear()
    {
        selectedState = null;
    }

    public void InitialScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
