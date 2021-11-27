using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    RaycastHit2D hit;
    RaycastHit2D pHit;

    [SerializeField] private float zOffSet = 0;
    void Update()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, zOffSet);
        if (!pHit)
        {
            pHit = hit;
        }

        //ENTER BUTTON
        if (hit && !hit.collider.gameObject.GetComponent<GameButton>().inside)
        {
            hit.collider.gameObject.GetComponent<GameButton>().OnPointerEnter();
        }

        //EXIT BUTTON
        if(hit != pHit)
        {
            pHit.collider.gameObject.GetComponent<GameButton>().OnPointerExit();
        }

        //CLICK BUTTON
        if(hit && Input.GetMouseButtonDown(0))
        {
            hit.collider.gameObject.GetComponent<GameButton>().OnPointerDown();
        }

        //RELEASE BUTTON
        if (hit && Input.GetMouseButtonUp(0))
        {
            hit.collider.gameObject.GetComponent<GameButton>().OnPointerUp();
        }

        pHit = hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * zOffSet, Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 1f);
    }
}
