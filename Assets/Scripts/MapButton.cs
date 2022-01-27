using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    public Color ativadoColor = new Color(1f, 1f, 1f, 1f);
    public Color desativadoColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    public bool map1Ativado = true;

    public TMP_Text map1Text;
    public Image map1Image;

    public TMP_Text map2Text;
    public Image map2Image;


    public bool showingHomicide = true;

    public Sprite alternarClaro;
    public Sprite alternarEscuro;
    public Image buttonImage;
    public Image buttonText;

    void Update()
    {
        if (map1Ativado)
        {
            map1Text.color = ativadoColor;
            map1Image.color = ativadoColor;
            map2Text.color = desativadoColor;
            map2Image.color = Color.clear;
        }
        else
        {
            map1Text.color = desativadoColor;
            map1Image.color = Color.clear;
            map2Text.color = ativadoColor;
            map2Image.color = ativadoColor;
        }

        if (showingHomicide)
        {

        }
    }

    public void UpdateAtivado(bool value)
    {
        map1Ativado = value;
    }

    public void ChangeSpriteEnter()
    {
        buttonImage.sprite = alternarEscuro;
    }
    public void ChangeSpriteExit()
    {
        buttonImage.sprite = alternarClaro;
    }
}
