using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLLoader : MonoBehaviour
{
    public string givenURL;
    public void GoToUrl()
    {
        StopAllCoroutines();
        StartCoroutine(WaitThenGo());
    }

    public IEnumerator WaitThenGo()
    {
        yield return new WaitForSeconds(0.75f);
        Application.OpenURL(givenURL);
    }
}
