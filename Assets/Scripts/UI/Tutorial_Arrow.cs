using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tutorial_Arrow : MonoBehaviour {

	RectTransform rt = null;
	RawImage rw = null;

	// Use this for initialization
	void Start () {
		rw = GetComponent<RawImage>();
		rt = GetComponent<RectTransform>();
	}
	
	public void SetPosition (float x, float y) {
		rt.anchoredPosition = new Vector2 (x, y);
	}

	public void Activate () {
		transform.DOKill();
		float to = transform.localPosition.x + 20;
		transform.DOLocalMoveX(to, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
		rw.DOFade(1f, 1f).SetUpdate(true);
	}

	public void DeActivate (bool immediate = false) {
		if (immediate) {
			rw.color = new Color(1f, 1f, 1f, 0f);
			DOTween.Kill (transform);
		} else {
			rw.DOFade(0f, 1f).SetUpdate(true).OnComplete(() => { DOTween.Kill (transform); });
		}
	}

	public bool is_active {
		get { return rw.color.a > 0f; }
	}
}
