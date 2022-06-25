using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommandInText : MonoBehaviour
{
    
    // Start is called before the first frame update
   
        public InputField myInput;
        int caretposition;
        public void InsertCommand(string command)
        {
            myInput.text = myInput.text.Insert(caretposition, command+"\n");
            

        }

        private void Update()
        {
            if (myInput.isFocused)
            {
                caretposition = myInput.caretPosition;
            }
        }
    
    
    
}
