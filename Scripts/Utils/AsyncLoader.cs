using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AsyncLoader : MonoBehaviour {

    public GameObject loader;
    public GameObject[] buttons;


    private AsyncOperation async;

    public void ClickAsync(int level)
    {
        loader.SetActive(true);
        for (int i = 0; i<buttons.Length; i++) 
        {
            if (i != level-1)
            {
                buttons[i].SetActive(false);
            }
        }
        StartCoroutine(LoadLevelWithBar(level));
    }


    IEnumerator LoadLevelWithBar(int level)
    {
        Slider loadingBar = loader.GetComponentInChildren<Slider>();
        async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            loadingBar.value = async.progress;
            yield return null;
        }
    }
}
