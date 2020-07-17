using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void OpenBrowserURL(string url)
    {
        Application.OpenURL(url);
    }
}
