using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CaixaRuim : MonoBehaviour
{
    public TMP_Text tEstado;
    public TMP_Text tCorpo;
    public Botao b2016;
    public Botao b2020;

    public bool clicked2016 = true;

    public string text2016;
    public string text2020;
    public string textNome;

    public float alpha = 1f;

    public Color highlightColor = Color.white;
    public Color otherColor = new Color(0.02959237f, 0.09729432f, 0.3301887f, 1f);
    void Start()
    {
        b2016.cr = this;
        b2016.is2016 = true;
        b2020.cr = this;
        b2020.is2016 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked2016)
        {
            b2016.text.fontStyle = FontStyles.Underline;
            b2016.text.color = highlightColor;
            b2020.text.fontStyle = FontStyles.Normal;
            b2020.text.color = otherColor;

            tCorpo.text = text2016;
        }
        else
        {
            b2016.text.fontStyle = FontStyles.Normal;
            b2016.text.color = otherColor;
            b2020.text.fontStyle = FontStyles.Underline;
            b2020.text.color = highlightColor;

            tCorpo.text = text2020;
        }
        tEstado.text = textNome;
    }

    public void Refresh()
    {

    }
}
