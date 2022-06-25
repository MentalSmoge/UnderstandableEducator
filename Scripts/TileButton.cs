using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButton : MonoBehaviour
{
    public Sprite sprite_pressed;
    public Sprite sprite_not_pressed;
    public bool Pressed = false;
    public bool Press()
	{
        if (Pressed)
		{
            return false;
		}
        else
		{
            this.GetComponent<SpriteRenderer>().sprite = sprite_pressed;
            Pressed = true;
            return true;
		}
	}
    public bool unPress()
    {
        this.GetComponent<SpriteRenderer>().sprite = sprite_not_pressed;
        Pressed = false;
        return true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Press();
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        manager.GetComponent<HandleInput>().buttons.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
