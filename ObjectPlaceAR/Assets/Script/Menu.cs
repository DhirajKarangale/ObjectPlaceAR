using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ButtonItem(int item)
    {
        SpawnManager.currItem = item;
        SceneManager.LoadScene(1);
    }
}
