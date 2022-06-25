
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Finish : MonoBehaviour
{
    public int level;
    // Start is called before the first frame update
    public void Finish()
    {
        PlayerPrefs.SetInt("LevelComplete", level);
        if (level == 4)
        {
            SceneManager.LoadScene("Main_Menu");
        }
        else
        {
            SceneManager.LoadScene("Level_" + (level + 1));
        }
        
    }
}
