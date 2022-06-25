using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class MainMenuLevel : MonoBehaviour
{
    public UnityEngine.UI.Button[] buttons;
    int LevelComplete;
    
    // Start is called before the first frame update
    void Start()
    {
        LevelComplete = PlayerPrefs.GetInt("LevelComplete");

        for (int i= LevelComplete; i < buttons.Length; i++)
        {
            
         buttons[i].interactable = false;
            
        }
    }
    public void Reset()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            
        }
        PlayerPrefs.DeleteAll();
    }


}
