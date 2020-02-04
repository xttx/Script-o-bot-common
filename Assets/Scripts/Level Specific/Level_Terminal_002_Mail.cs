using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Level_Terminal_002_Mail : MonoBehaviour, Engine.ISetable, Engine.IResetable
{
    public Terminal_Universal terminal = null;
    public RectTransform paper = null;

    List<Coroutine> Show_Paper_Coroutines = new List<Coroutine>();

    void OnEnable() {
        paper.anchoredPosition = new Vector2(-450f, 0f);
    }

    public void Set() {
        //On script play
        StopAllCoroutines(); 
    }
    public void Reset() {
        //On script stop
        StopAllCoroutines();
        ShowHide_Paper(false);
    }

    public void On_Terminal_Read() {
        Show_Paper_Coroutines.Add( StartCoroutine(Show_Paper_After_Time()) );
    }
    public void On_Terminal_ResponceCorrect() {
        ShowHide_Paper(false);
    }
    public void On_Terminal_ResponceIncorrect() {
        ShowHide_Paper(false);
    }

    IEnumerator Show_Paper_After_Time() {
        yield return new WaitForSecondsRealtime(1.25f);
        ShowHide_Paper();
    }
    void ShowHide_Paper(bool show = true) {
        paper.DOKill();
        foreach (var cr in Show_Paper_Coroutines) StopCoroutine(cr);
        Show_Paper_Coroutines.Clear();

        if (show) {
            paper.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = terminal.current_string_orig;
            if ( paper.anchoredPosition.x < -49f )paper.DOAnchorPosX(50f, 0.5f);
        } else {
            if ( paper.anchoredPosition.x > -451f ) paper.DOAnchorPosX(-450f, 0.5f);
        }
    }
}
