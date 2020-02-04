using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingMy : MonoBehaviour {

	Text DialogText = null;
	Button DialogOK = null;

	int current_index = 0;
	string current_str = "";
	float time_from_last_char = 99999f;
	float time_between_char = 0.075f;

	// Use this for initialization
	void Start () {
		DialogText = GameObject.Find("Dialog_Text").GetComponent<Text>();
		DialogOK = GameObject.Find("Dialog_OK_Button").GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		if (current_index <= current_str.Length)
		{
			if (time_from_last_char >= time_between_char)
			{
				if (current_index > 0 && current_str.Substring(current_index - 1, 1) == "<")
				{
					current_index = current_str.IndexOf(">", current_index) + 2;
				}
				DialogText.text = current_str.Substring(0, current_index);
				current_index++;
				time_from_last_char = 0f;
			}
			else
			{
				time_from_last_char += Time.deltaTime;
			}
		}
		else
		{
			DialogOK.gameObject.SetActive(true);	
		}
	}

	public void TypeText (string str)
	{
		//DialogText.text = str;
		//DialogOK.gameObject.SetActive(true);
		current_index = 0;
		current_str = str;
	}
}
