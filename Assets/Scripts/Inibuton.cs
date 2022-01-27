using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inibuton : MonoBehaviour
{
    public Sprite buttonNormal;
    public Sprite buttonHigh;
    public Image button;
 public void avancaCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    
    }

    public void changeOnHover()
    {
        button.sprite = buttonHigh;
    }
    public void changeOnExit()
    {
        button.sprite = buttonNormal;
    }

}
