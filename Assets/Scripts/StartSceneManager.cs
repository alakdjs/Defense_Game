using UnityEngine;
using UnityEngine.SceneManagement;


public class StartSceneManager : MonoBehaviour
{
    public void GoSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
