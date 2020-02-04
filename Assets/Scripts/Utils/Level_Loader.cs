using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level_Loader : MonoBehaviour
{
    Slider sld = null;

    // Start is called before the first frame update
    void Start()
    {
        sld = GameObject.Find("Slider").GetComponent<Slider>();
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        //asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            //var p = Mathf.RoundToInt(asyncOperation.progress * 100);
            sld.value = asyncOperation.progress;
            if (Mathf.Approximately(asyncOperation.progress, 1f)) {
                //asyncOperation.allowSceneActivation = false;
                yield break;
            }
            yield return null;
        }
    }
}
