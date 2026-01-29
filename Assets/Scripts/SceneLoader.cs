using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public void GoSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
