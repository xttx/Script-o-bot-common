using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Dialog_Panel_Txt_Event : UnityEvent<string>{}

public class Dialog_Panel_Txt : MonoBehaviour
{
    public delegate void OnButtonPressedDelegate(string t);
    public static event OnButtonPressedDelegate OnButtonPressed;

	public static Dialog_Panel_Txt_Event OnButtonPressed_U = new Dialog_Panel_Txt_Event();

    static Dialog_Panel_Txt inst;

	CanvasGroup cv;
	RectTransform rt;
	Text txt;
    Text txt_btn;
	Text txt_warning;
	Button btn;
	Button btn_cancel;
    InputField txt_field;

	string[] filter = null;

    // Start is called before the first frame update
    void Start()
    {
		inst = this;
		cv = GetComponent<CanvasGroup>();
		rt = transform.GetChild(0).GetComponent<RectTransform>();
		txt = transform.GetChild(0).Find("Dialog_Text_Ask").GetComponent<Text>();
		btn = transform.GetChild(0).Find("Dialog_Button").GetComponent<Button>();
        txt_btn = transform.GetChild(0).Find("Dialog_Button").GetChild(3).GetComponent<Text>();
		txt_warning = transform.GetChild(0).Find("Dialog_Text_Label").GetComponent<Text>();
        txt_field = transform.GetChild(0).Find("InputField").GetComponent<InputField>();

		txt_warning.enabled = false;
    }

    public void ButtonPress()
    {
        HideDialog();
        if (OnButtonPressed != null) OnButtonPressed(txt_field.text);
		if (OnButtonPressed_U != null) OnButtonPressed_U.Invoke(txt_field.text);
    }
	public void ButtonPress_Cancel()
	{
		if (DOTween.IsTweening(cv)) return;

        HideDialog();
        if (OnButtonPressed != null) OnButtonPressed("");
		if (OnButtonPressed_U != null) OnButtonPressed_U.Invoke("");
	}

	public static void ShowDialog(string t, string button_text, string default_text, string[] _filter = null)
	{
		inst.ShowDialogInst(t, button_text, default_text, _filter);
	}
	public static void InvokeCancel()
	{
		inst.ButtonPress_Cancel();
	}

	void ShowDialogInst(string t, string button_text, string default_text, string[] _filter = null)
	{
		txt.text = t;
        txt_btn.text = button_text;
		txt_field.text = default_text;
		filter = _filter;

		btn.interactable = true;
		txt_warning.enabled = false;

		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 100);
		rt.DOAnchorPosY(0, 0.33f).SetUpdate(true);
		cv.DOFade(1f, 0.33f).SetUpdate(true).OnComplete( () => { 
			cv.interactable = true; cv.blocksRaycasts = true;
			txt_field.Select(); txt_field.ActivateInputField();
		});
	}

	public void HideDialog()
	{
		cv.interactable = false;
		cv.blocksRaycasts = false;
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
		rt.DOAnchorPosY(100, 0.33f).SetUpdate(true);
		cv.DOFade(0f, 0.33f).SetUpdate(true);
	}

	public void OnTextChange()
	{
		if (filter != null) {
			foreach (string f in filter) {
				var data = f.Split(new string[]{"@@@"}, System.StringSplitOptions.RemoveEmptyEntries);
				var r = new Regex(data[0], RegexOptions.IgnoreCase);
				if (r.Match(txt_field.text.Trim()).Success) {
					txt_warning.text = data[1];
					btn.interactable = false;
					txt_warning.enabled = true;
					return;
				}
			}
		}
		btn.interactable = true;
		txt_warning.enabled = false;
	}

	public static bool IsShown {
		get { return inst.IsShownInst; }
	}
	public bool IsShownInst {
		get { return DOTween.IsTweening(cv) || cv.alpha > 0f; }
	}
}
