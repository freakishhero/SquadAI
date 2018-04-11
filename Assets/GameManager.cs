using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static byte mouse_clicks = 0;
    static float double_click_delay = 0.5f;
    static float first_click_time = 0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(mouse_clicks == 1)
        {
            if ((Time.time - first_click_time) > double_click_delay)
            {
                mouse_clicks = 0;
                Debug.Log("Too long to double click.");
            }
        }
    }

    public static float First_Click_Time
    {
        get
        {
            return first_click_time;
        }
        set
        {
            first_click_time = value;
        }
    }

    public static float Double_Click_Delay
    {
        get
        {
            return double_click_delay;
        }
    }

    public static byte MouseClicks
    {
        get
        {
            return mouse_clicks;
        }
        set
        {
            mouse_clicks = value;
        }
    }
}
