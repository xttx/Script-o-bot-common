using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tutorial_Dialog : MonoBehaviour {

	CanvasGroup cv;
	RectTransform rt;
	RectTransform rt_dialog_important;

	Canvas cnv;
	Transform context_menu;

	[HideInInspector] public bool deactivated_by_user = false;

	// Use this for initialization
	void Start () {
		cv = GetComponent<CanvasGroup>();
		rt = GetComponent<RectTransform>();
		rt_dialog_important = GameObject.Find("Panel_Dialog_Important").GetComponent<RectTransform>();
		
		cnv = transform.GetComponentInParent<Canvas>();
		context_menu = transform.Find("Context_Menu");
		DeActivateImmediate ();
	}
	
	public void Activate () {
		if (Mathf.Approximately(cv.alpha, 1f)) return;
		if (deactivated_by_user) ActivateByUser(true); //TODO: This will fail if the panel is animating

		cv.DOKill();
		rt.DOKill();
		cv.DOFade(1f, 0.75f).SetUpdate(true);
		rt.DOAnchorPosY(-15f, 0.75f).SetUpdate(true);
		rt_dialog_important.DOAnchorPosY(-280f, 0.75f).SetUpdate(true).OnComplete(()=> {cv.interactable = true;});
	}

	public void DeActivate () {
		if (Mathf.Approximately(cv.alpha, 0f)) return;

		cv.DOKill();
		rt.DOKill();
		cv.interactable = false;
		cv.DOFade(0f, 0.75f).SetUpdate(true);
		rt.DOAnchorPosY(265f, 0.75f).SetUpdate(true);
		rt_dialog_important.DOAnchorPosY(0f, 0.75f).SetUpdate(true);
	}

	public void ActivateImmediate () {
		cv.DOKill();
		rt.DOKill();

		cv.alpha = 1f;
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -15f);
		rt_dialog_important.anchoredPosition = new Vector2(rt_dialog_important.anchoredPosition.x, -280f);
		cv.interactable = true;
	}

	public void DeActivateImmediate () {
		cv.DOKill();
		rt.DOKill();

		cv.interactable = false;
		cv.alpha = 0f;
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 265f);
		rt_dialog_important.anchoredPosition = new Vector2(rt_dialog_important.anchoredPosition.x, 0f);
	}

	public void ActivateByUser (bool force = false) {
		//Reactivating after desactivation by user in window mode
		if (!deactivated_by_user) return;
		if (DOTween.IsTweening(rt)) {
			if (!force) return;
			else rt.DOKill();
		}
		
		deactivated_by_user = false;
		rt.DOAnchorPosX(15f, 0.75f ).SetEase(Ease.OutQuad).SetUpdate(true);
	}
	public void DeActivateByUser () {
		//Desactivation by user in window mode
		if (deactivated_by_user) return;
		if (DOTween.IsTweening(rt)) return;

		deactivated_by_user = true;
		rt.DOAnchorPosX(-1350f, 0.75f ).SetEase(Ease.InQuad).SetUpdate(true);
	}

	public void Show_FastForward_Menu()
	{
		var item_cnt = context_menu.GetChild(0).GetChild(0).GetChild(0);
		for (int i = item_cnt.childCount - 1; i >= 1; i--) { Destroy(item_cnt.GetChild(i).gameObject); }


		var scaler = cnv.GetComponent<CanvasScaler>();
		float x = Input.mousePosition.x * (scaler.referenceResolution.x / Screen.width) - 50f;
		float y = Input.mousePosition.y * (scaler.referenceResolution.y / Screen.height) - scaler.referenceResolution.y + 50f;
		var item_cnt_rt = context_menu.GetComponent<RectTransform>();
		item_cnt_rt.anchoredPosition = new Vector2(x, y);

		var item = item_cnt.GetChild(0).gameObject;
		foreach (var kv in Scenario.steps_for_fastForward[Engine.current_scene-1]) {
			var new_item = Instantiate(item, item_cnt);
			new_item.transform.GetChild(0).GetComponent<Text>().text = kv.Key;
			new_item.SetActive(true);
		
			new_item.GetComponent<Button>().onClick.AddListener(()=>{
				Engine.current_step = kv.Value - 1; Engine.engine_inst.DoNextStep();
				context_menu.gameObject.SetActive(false);
			});
		}

		context_menu.gameObject.SetActive(true);
	}

	void Update()
	{
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) &&  context_menu.gameObject.activeSelf) {
			var scaler = cnv.GetComponent<CanvasScaler>();
			float x = Input.mousePosition.x * (scaler.referenceResolution.x / Screen.width);
			float y = Input.mousePosition.y * (scaler.referenceResolution.y / Screen.height) - scaler.referenceResolution.y;

			var parent_pos = context_menu.parent.GetComponent<RectTransform>().anchoredPosition;
			var menu_rt = context_menu.GetComponent<RectTransform>();
			var menu_pos = menu_rt.anchoredPosition + parent_pos;
			var menu_size = menu_rt.sizeDelta;

			if (x < menu_pos.x || x > menu_pos.x + menu_size.x || y < menu_pos.y - menu_size.y || y > menu_pos.y)
			{ context_menu.gameObject.SetActive(false); }
		}
	}
}
