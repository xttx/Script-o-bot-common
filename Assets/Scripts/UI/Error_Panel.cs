using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Error_Panel : MonoBehaviour {
	//public static bool Error_Is_Shown = false;

	static Error_Panel inst;

	CanvasGroup cv;
	RectTransform rt;
	Text txt;

	// Use this for initialization
	void Start () {
		inst = this;
		cv = GetComponent<CanvasGroup>();
		rt = transform.GetChild(0).GetComponent<RectTransform>();
		txt = transform.GetChild(0).Find("Error_Text").GetComponent<Text>();
	}
	
	public static void ShowError(string err) { inst.ShowErrorInst(err); }
	public static void HideError_st()	{ inst.HideError();	}
	void ShowErrorInst(string err)
	{
		//Error_Is_Shown = true;

		txt.text = err;
		rt.DOKill(); cv.DOKill();

		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 100);
		rt.DOAnchorPosY(0, 0.75f).SetUpdate(true);
		cv.DOFade(1f, 0.75f).SetUpdate(true).OnComplete( () => {cv.interactable = true; cv.blocksRaycasts = true;});
	}
	public void HideError()
	{
		if (DOTween.IsTweening(cv)) return;

		cv.interactable = false;
		cv.blocksRaycasts = false;
		rt.DOKill(); cv.DOKill();

		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
		rt.DOAnchorPosY(100, 0.75f).SetUpdate(true);
		cv.DOFade(0f, 0.75f).SetUpdate(true); //.OnComplete( () => Error_Is_Shown = false );
	}

	public static bool IsShown {
		get { return inst.IsShownInst; }
	}
	public bool IsShownInst {
		get { return DOTween.IsTweening(cv) || cv.alpha > 0f; }
	}
}
