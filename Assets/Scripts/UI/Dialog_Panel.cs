using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Dialog_Panel_Event : UnityEvent<int>{}

public class Dialog_Panel : MonoBehaviour
{
    public delegate void OnButtonPressedDelegate(int i);
    public static event OnButtonPressedDelegate OnButtonPressed;

	public static Dialog_Panel_Event OnButtonPressed_U = new Dialog_Panel_Event();

    static Dialog_Panel inst;

	CanvasGroup cv;
	RectTransform rt;
	Text txt;
    Text txt_btn1;
    Text txt_btn2;

    // Start is called before the first frame update
    void Start()
    {
		//Test bool expression - unrelated to dialog_panel :)		
		// int x = 150;
		// bool res = x > 5 && x < 10 || x > 100 && x < 110;
		// Debug.Log(res);

		inst = this;
		cv = GetComponent<CanvasGroup>();
		rt = transform.GetChild(0).GetComponent<RectTransform>();
		txt = transform.GetChild(0).Find("Dialog_Text_Ask").GetComponent<Text>();
        txt_btn1 = transform.GetChild(0).Find("Dialog_Button1").GetChild(3).GetComponent<Text>();
        txt_btn2 = transform.GetChild(0).Find("Dialog_Button2").GetChild(3).GetComponent<Text>();
    }

    public void ButtonPress(int n)
    {
		if (DOTween.IsTweening(cv)) return;

        HideDialog();
        if (OnButtonPressed != null) OnButtonPressed(n);
		if (OnButtonPressed_U != null) OnButtonPressed_U.Invoke(n);
    }

	public static void ShowDialog(string t, string b1, string b2)
	{
		inst.ShowDialogInst(t, b1, b2);
	}
	public static void InvokeButtonPress(int n)
	{
		inst.ButtonPress(n);
	}

	void ShowDialogInst(string t, string b1, string b2)
	{
		txt.text = t;
		if (b2 == "") {
			txt_btn2.text = b1;
			txt_btn1.transform.parent.gameObject.SetActive(false);
		} else {
        	txt_btn1.text = b1;
        	txt_btn2.text = b2;
			txt_btn1.transform.parent.gameObject.SetActive(true);
		}
		//float cur_y = rt.anchoredPosition.y;
		//Debug.Log(cur_y.ToString());
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 100);
		rt.DOAnchorPosY(0, 0.33f).SetUpdate(true);
		cv.DOFade(1f, 0.33f).SetUpdate(true).OnComplete( () => {cv.interactable = true; cv.blocksRaycasts = true;});
	}

	public void HideDialog()
	{
		cv.interactable = false;
		cv.blocksRaycasts = false;
		//float cur_y = rt.anchoredPosition.y;
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
		rt.DOAnchorPosY(100, 0.33f).SetUpdate(true);
		cv.DOFade(0f, 0.33f).SetUpdate(true);
	}

	public static bool IsShown {
		get { return inst.IsShownInst; }
	}
	public bool IsShownInst {
		get { return DOTween.IsTweening(cv) || cv.alpha > 0f; }
	}
}
