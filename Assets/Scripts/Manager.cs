using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class Manager : MonoBehaviour
{
    RaycastHit2D hit;
    RaycastHit2D pHit;

    public Estado selectedState;
    public List<Estado> estados = new List<Estado>();

    [SerializeField] private float zOffSet = 0;
    [SerializeField] public Transform displayPos;

    private void Start()
    {
        Transform brasil = GameObject.Find("Brasil").transform;
        for (int i = 0; i < brasil.childCount - 1; i++)
        {
            estados.Add(brasil.GetChild(i).GetChild(0).gameObject.GetComponent<Estado>());
        }
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
}
