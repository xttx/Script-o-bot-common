using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tablo : MonoBehaviour
{
    [TextArea(4, 100)]
    public string text = "";

    public float animation_delay = 0.2f;

    public Text txt_back = null;
    public Text txt_front = null;

    public UnityEvent OnAnimationEnd = null;

    string letters_for_randomization = "ABCDEFGHiJKLMNOPQRSTUVWXYZ1234567890";
    List<int> animated_rows = new List<int>();

    public enum clear_mode { none, clear_animated_rows, clear_below_first_animated_row };

    public void SetText(string t) {
        text = t;
        txt_front.text = t;
    }
    public void SetTextRow(string t, int r) {
        string[] txt_rows = txt_front.text.Split(new char[]{'\n'}, System.StringSplitOptions.None);
        if (r >= txt_rows.Length) return;
        txt_rows[r] = t;

        text = string.Join("\n", txt_rows);
        txt_front.text = text;
    }

    public void AbortAnimations(clear_mode clear = clear_mode.none) {
        StopAllCoroutines();
        if (animated_rows.Count() > 0) {
            if (clear == clear_mode.clear_animated_rows) {
                foreach (int i in animated_rows.Distinct() ) { SetTextRow("", i); }
            } 
            else if (clear == clear_mode.clear_below_first_animated_row) {
                int row_count = txt_front.text.Split(new char[]{'\n'}, System.StringSplitOptions.None).Count();
                for (int i = animated_rows.Min(); i < row_count; i++ ) { SetTextRow("", i); }
            }
        }
        animated_rows.Clear();
    }

    public void AnimateRow(int i) {
        StartCoroutine(AnimateRow_coroutine(i));
    }
    public void AnimateRowFromRandom(int r, float delay_between_characters, int frame_delay_for_randomization, float randomization_time, int start = 0) {
        string[] txt_rows = text.Split(new char[]{'\n'}, System.StringSplitOptions.None);
        if (r >= txt_rows.Length) return;
        string[] txt_rows_orig = txt_rows.ToArray();

        float delay = 0f;
        bool in_tag = false;
        for (int i = start; i < txt_rows[r].Length; i++ ) {
            if (txt_rows[r][i] == ' ') continue;
            if (txt_rows[r][i] == '<') in_tag = true;
            if (txt_rows[r][i] == '>') { in_tag = false; continue; }
            if (in_tag) continue;
            animated_rows.Add(r);
            StartCoroutine( AnimateRowFromRandom_coroutine(r, i, delay, frame_delay_for_randomization, randomization_time) );
            delay += delay_between_characters;
        }
    }
    IEnumerator AnimateRowFromRandom_coroutine(int r, int c, float start_delay, int frame_delay_for_randomization, float randomization_time) {
        yield return new WaitForSeconds(start_delay);
        string txt_rows_at_start = txt_front.text.Split(new char[]{'\n'}, System.StringSplitOptions.None)[r];
        char symbol_orig = txt_rows_at_start[c];
        
        float end_time = Time.time + randomization_time;
        while (true) {
            if (Time.time >= end_time) break;
            string t = txt_front.text;
            string[] txt_rows = t.Split(new char[]{'\n'}, System.StringSplitOptions.None);
            string txt_row_orig = txt_rows[r];
            txt_rows[r] = txt_row_orig.Substring(0, c);
            txt_rows[r] += letters_for_randomization[Random.Range(0, letters_for_randomization.Length)];
            txt_rows[r] +=txt_row_orig.Substring(c + 1);
            txt_front.text = string.Join("\n", txt_rows);
            for (int f = 1; f <= frame_delay_for_randomization; f++) { yield return null; }
        }

        string[] txt_rows2 = txt_front.text.Split(new char[]{'\n'}, System.StringSplitOptions.None);
        string txt_row_orig2 = txt_rows2[r];
        txt_rows2[r] = txt_row_orig2.Substring(0, c) + symbol_orig + txt_row_orig2.Substring(c + 1);
        txt_front.text = string.Join("\n", txt_rows2);
        animated_rows.Remove(r);
    }

    IEnumerator AnimateRow_coroutine(int r)
    {
        string[] txt_rows = text.Split(new char[]{'\n'}, System.StringSplitOptions.None);
        if (r >= txt_rows.Length) yield break;
        string[] txt_rows_orig = txt_rows.ToArray();

        int row_char_count = txt_back.text.Split(new char[]{'\n'}, System.StringSplitOptions.None)[r].Length;

        for (int i = row_char_count; i >= 0; i-- ) {
            txt_rows[r] = new string(' ', i) + txt_rows_orig[r];
            txt_front.text = string.Join("\n", txt_rows);
            yield return new WaitForSeconds(animation_delay);
        }
        txt_front.text = string.Join("\n", txt_rows_orig);

        if (OnAnimationEnd != null) OnAnimationEnd.Invoke();
    }

    public bool IsRowAnimating(int r) { return animated_rows.Contains(r); }
}
