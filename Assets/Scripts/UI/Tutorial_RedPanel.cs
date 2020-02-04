using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tutorial_RedPanel : MonoBehaviour {

	RectTransform rt = null;
	Image img = null;

	// Use this for initialization
	void Start () {
		img = GetComponent<Image>();
		rt = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void SetPositionAndSize (float x, float y, float w, float h) {
		rt.anchoredPosition = new Vector2 (x, y);
		rt.sizeDelta = new Vector2 (w, h);
	}

	public void Activate () {
		//transform.DOLocalMoveX(to, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		//rw.DOFade(1f, 1f);
		img.DOFade(0.7f, 1f).SetUpdate(true).OnComplete(() => { 
			img.DOFade(0.45f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
		});
	}

	public void DeActivate (bool immediate = false) {
		DOTween.Kill (img);
		if (immediate) {
			img.color = new Color(1f, 0f, 0f, 0f);
		} else {
			img.DOFade(0f, 1f).SetUpdate(true);
		}
	}

	public bool is_active {
		get { return img.color.a > 0f; }
	}
}
