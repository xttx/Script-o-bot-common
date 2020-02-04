using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Suggestion : MonoBehaviour
{
    CanvasGroup cv;
    Text txt;
    RectTransform selector;
    static TMP_InputField script_field;
    float last_suggest_time;
    int selected_element = 0;
    int lines_count = 0;

    bool called_from_suggest_submit = false;

    // Start is called before the first frame update
    void Start()
    {
        cv = GetComponent<CanvasGroup>();
        txt = transform.GetChild(1).GetComponent<Text>();
        selector = transform.GetChild(0).GetComponent<RectTransform>();

        GameObject tmpro = GameObject.Find("Script_InputField_TMPRO");
        if (tmpro == null) { gameObject.SetActive(false); return; }
        script_field = tmpro.GetComponent<TMP_InputField>();
        if (script_field == null) { gameObject.SetActive(false); return; }

        cv.alpha = 0f;
        txt.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
            if (Input.GetKeyDown(KeyCode.Keypad8)) {
                if (selected_element > 0) selected_element--;
            }
            if (Input.GetKeyDown(KeyCode.Keypad2)) {
                if (selected_element < lines_count - 1) selected_element++;
            }
            if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Keypad2)) {
                last_suggest_time = Time.time;
                selector.anchoredPosition = new Vector2(10, (selected_element * -25) - 16);
            }
        }

        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
            var arr = get_script_last_word_pos();
            //Debug.Log(arr[0].ToString() + ", " + arr[1].ToString());
            if (arr[0] >= 0 && arr[1] >= 0) {
                var cur_sug_arr = txt.text.Split(new string[]{"\n"}, System.StringSplitOptions.None);
                if (selected_element < cur_sug_arr.Length) {
                    //Debug.Log("Selected el: " + cur_sug_arr[cur_sel]);
                    //Because when changing script_field.text, Suggest() is callen onTextChange, and selected_element is reseted there
                    called_from_suggest_submit = true;
                    script_field.text = script_field.text.Remove(arr[0], arr[1]).Insert(arr[0], cur_sug_arr[selected_element]);
                    called_from_suggest_submit = false;
                    //Debug.Log("Selected el: " + cur_sug_arr[cur_sel] + ", Word pos: " + arr[0].ToString() + ", Suggested length: " + cur_sug_arr[selected_element].Length.ToString());
                    script_field.caretPosition = arr[0] + cur_sug_arr[selected_element].Length;
                }
            }
        }

        if (Time.time > last_suggest_time + 3 && Mathf.Approximately ( cv.alpha, 1f )) {
            cv.DOKill();
            cv.DOFade(0f, 1f);
        }
    }

    public void Suggest_Clear() {
        //txt.text = "";
    }
    public void Suggest(string sug) {
        last_suggest_time = Time.time;
        if (called_from_suggest_submit) return;

        txt.text = sug;
        lines_count = System.Text.RegularExpressions.Regex.Matches(sug, "\n").Count;

        selected_element = 0;
        selector.anchoredPosition = new Vector2(10, -16);

        cv.DOKill();
        cv.alpha = 1f;
    }

    public static int[] get_script_last_word_pos() {
        string str = " " + script_field.text;
        if (str.Length == 0) return new int[]{-1, -1};

		int pos = script_field.caretPosition + 1;
		//if (pos >= str.Length) return new int[]{-1, -1};
		int pos2 = str.LastIndexOfAny(new char[]{';', ' ', '\n', '.'}, pos - 1);
		//Debug.Log ("corrPos = " + pos.ToString() + ", pos2 = " + pos2.ToString() + ", char last = " + str.Substring(pos - 1, 1));
		//Debug.Log (str.Substring(pos2, pos - pos2));
        return new int[]{pos2, pos - pos2 - 1};
    }
}
