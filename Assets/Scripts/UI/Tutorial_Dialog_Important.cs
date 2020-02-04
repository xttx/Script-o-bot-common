using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RedBlueGames.Tools.TextTyper;

public class Tutorial_Dialog_Important : MonoBehaviour {

	CanvasGroup DialogImportantCG = null;
	RectTransform DialogImportantRT = null;
	Text DialogImportanText = null;
	TextTyper TextTyper = null;

	bool hidden_by_script = false;
	bool hidden_by_user = false;

	// Use this for initialization
	void Start () {
		DialogImportantCG = GameObject.Find("Panel_Dialog_Important").GetComponent<CanvasGroup>();
		DialogImportantRT = GameObject.Find("Panel_Dialog_Important").GetComponent<RectTransform>();
		DialogImportanText = GameObject.Find("Dialog_Important_Text").GetComponent<Text>();
		TextTyper = DialogImportanText.GetComponent<TextTyper>();
		DeActivateImmediate();
	}
	
	public void Type (string text, float delay, int to, int from) {
		DialogImportanText.text = "";

		if (Mathf.Approximately(DialogImportantCG.alpha, 1f)) {
			Type_Sub(text, delay, to, from);
		} else {
			hidden_by_script = false;
			DialogImportantRT.sizeDelta = new Vector2(DialogImportantRT.sizeDelta.x, 160f);
			if (!hidden_by_user) Animate(false, new System.Action( ()=> { Type_Sub(text, delay, to, from); }) );
			else DialogImportanText.text = text;
			// DialogImportantCG.DOKill();
			// DialogImportantRT.DOKill();
			// DialogImportantCG.DOFade(1f, 1f).SetUpdate(true);
			// DialogImportantRT.sizeDelta = new Vector2(DialogImportantRT.sizeDelta.x, 160f);
			// DialogImportantRT.DOAnchorPosX(-10, 1f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete( ()=> { Type_Sub(text, delay, to, from); });
		}
	}

	public void TypeContinue (string text, float delay, int to) {
		DialogImportantRT.DOComplete(); //to set initial text if it's not yet started to print
		TextTyper.TypeText_Continue(text, delay, to);
		
		float target_height = (TextTyper.printingText.Count(c => c == '\n') * 40) + 160f;
		if (DialogImportantRT.sizeDelta.y < target_height - 1 || DialogImportantRT.sizeDelta.y > target_height + 1){
			DialogImportantRT.DOSizeDelta(new Vector2(DialogImportantRT.sizeDelta.x, target_height), 1f).SetUpdate(true);
		}
	}

	void Type_Sub(string text, float delay, int to, int from) {
		TextTyper.TypeText(text, delay, to, from);

		float target_height = (TextTyper.printingText.Count(c => c == '\n') * 40) + 160f;
		if (DialogImportantRT.sizeDelta.y < target_height - 1 || DialogImportantRT.sizeDelta.y > target_height + 1){
			DialogImportantRT.DOSizeDelta(new Vector2(DialogImportantRT.sizeDelta.x, target_height), 1f).SetUpdate(true);
		}
	}

	public void Activate_By_User() {
		hidden_by_user = false;
		if (!hidden_by_script) Animate();
	}

	public void DeActivate_By_User() {
		hidden_by_user = true;
		if (!hidden_by_script) Animate(true);
	}

	public void DeActivate () {
		hidden_by_script = true; Animate(true);
	}

	public void DeActivateImmediate () {
		hidden_by_script = true;
		DialogImportantCG.DOKill();
		DialogImportantRT.DOKill();
		DialogImportantCG.alpha = 0f;
		DialogImportantRT.anchoredPosition = new Vector2(-80, DialogImportantRT.anchoredPosition.y);
	}

	void Animate(bool animate_out = false, System.Action OnComplete = null) {
		if (!animate_out) {
			//Show
			if (!Mathf.Approximately(DialogImportantCG.alpha, 1f)) {
				DialogImportantCG.DOKill();
				DialogImportantRT.DOKill();
				DialogImportantCG.DOFade(1f, 1f).SetUpdate(true);
				var tw = DialogImportantRT.DOAnchorPosX(-10, 1f).SetEase(Ease.InOutSine).SetUpdate(true);				
				if (OnComplete != null) tw.OnComplete(()=> OnComplete.Invoke());
			} else {
				if (OnComplete != null) OnComplete.Invoke();
			}
		} else {
			//Hide
			if (Mathf.Approximately(DialogImportantCG.alpha, 0f)) return;
			DialogImportantCG.DOKill();
			DialogImportantRT.DOKill();
			DialogImportantCG.DOFade(0f, 1f).SetUpdate(true);
			DialogImportantRT.DOAnchorPosX(-80, 1f).SetEase(Ease.OutSine).SetUpdate(true);
		}
	}
}
