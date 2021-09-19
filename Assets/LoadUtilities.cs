using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUtilities : MonoBehaviour
{
    public string sceneToLoad;
    public bool loadAsync;
    public bool loadInStart;
    // Start is called before the first frame update
    void Start()
    {
        if (loadInStart) LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(float time)
    {
        if (loadAsync) StartCoroutine(LoadYourAsyncScene(time));
        else Invoke("SimpleLoad", time);
    }

    public void SimpleLoad()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator LoadYourAsyncScene( float time)
    {
        yield return new WaitForSeconds(0.5f);
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (time != 0f) yield return new WaitForSeconds(time);
    }
}
