using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenLink : MonoBehaviour
{
    public string urlToOpen = "";

    public void OpenURL()
    {
        Application.OpenURL(urlToOpen);
    }
}
