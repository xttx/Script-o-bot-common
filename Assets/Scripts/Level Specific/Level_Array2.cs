using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Level_Array2 : MonoBehaviour
{
    public Transform[] containers = null;
    public Vector3[] new_positions = new Vector3[]{};
    public Vector2Int[] new_positions_X = new Vector2Int[]{};
    public Vector2Int[] new_positions_Y = new Vector2Int[]{};
    public Vector2Int[] new_positions_Z = new Vector2Int[]{};

    public Transform[] top_obj = new Transform[]{};
    public Transform[] middle_obj = new Transform[]{};

    public Switch switch_template = null;

    Transform[] obj_indicators = null;
    Transform[] obj_switches = new Transform[27];
    Dictionary<Transform, Vector3> orig_positions = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Vector3> position_step1 = new Dictionary<Transform, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        int c = 0;
        foreach (var t in containers) c += t.childCount;

        obj_indicators = new Transform[c];

        c = 0;
        foreach (var t in containers) {
            for (int i = 0; i < t.childCount; i++) {
                var in_tr = t.GetChild(i);
                obj_indicators[c] = in_tr;
                orig_positions.Add(obj_indicators[c], obj_indicators[c].localPosition);

                position_step1.Add(in_tr, orig_positions[in_tr]);
                if ( Mathf.Approximately( orig_positions[in_tr].z, 2f) ) position_step1[in_tr] += transform.forward;
                if ( Mathf.Approximately( orig_positions[in_tr].z, -2f) ) position_step1[in_tr] -= transform.forward;
                if ( Mathf.Approximately( orig_positions[in_tr].x, 5f) ) position_step1[in_tr] += transform.right;
                if ( Mathf.Approximately( orig_positions[in_tr].x, 1f) ) position_step1[in_tr] -= transform.right;
                if ( Mathf.Approximately( orig_positions[in_tr].y, 2f) ) position_step1[in_tr] += transform.up;
                if ( Mathf.Approximately( orig_positions[in_tr].y, -2f) ) position_step1[in_tr] -= transform.up;
                c++;
            }
        }

        //Create switches
        var cnt = switch_template.transform.parent;
        var g_top = new GameObject("sw_top").transform; g_top.SetParent(cnt); g_top.localPosition = Vector3.zero;
        var g_mid = new GameObject("sw_mid").transform; g_mid.SetParent(cnt); g_mid.localPosition = Vector3.zero;
        for (int i = 0; i < obj_indicators.Length; i++) {
            var sw = Instantiate(switch_template.gameObject, cnt);
            sw.GetComponent<Switch>().requirement_from_statable = obj_indicators[i].gameObject;
            sw.transform.localPosition = position_step1[ obj_indicators[i] ] + (Vector3.up * 0.72f);
            obj_switches[i] = sw.transform;

            if (sw.transform.localPosition.y > 1) sw.transform.SetParent(g_top, true);
            else if (sw.transform.localPosition.y > -1) sw.transform.SetParent(g_mid, true);
        }
        top_obj = top_obj.Append(g_top).ToArray();
        middle_obj = middle_obj.Append(g_mid).ToArray();

        foreach (var t in top_obj) { orig_positions.Add(t, t.position); }
        foreach (var t in middle_obj) { orig_positions.Add(t, t.position); }
    }

    void OnEnable(){
        Camera.main.transform.position = new Vector3(-7.4f, 8.5f, -12f);
        Camera.main.transform.rotation = Quaternion.Euler(29.6f, 46.4f, 0f);
    }

    public void Give_A_Way2() {
        BOT_Helpers.Platform.wait = true;
        var y = BOT_Helpers.Platform.target_pos.y;
        
        Camera.main.transform.DOKill();
        //Camera.main.transform.DOMoveY(y + 1f, 0.75f);

        if ( Mathf.Approximately(y, -2f) ) {
            //Bottom level
            foreach (var t in top_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y + 1f, 0.75f ); }
            foreach (var t in middle_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y + 1f, 0.75f ).SetDelay(0.25f); }
            Camera.main.transform.DOMove(new Vector3(-5.93f, 2.8f, -7.26f), 0.75f);
            Camera.main.transform.DORotate(new Vector3(27f, 53.6f, 0f), 0.75f);
        } else if ( Mathf.Approximately(y, 0f) ) {
            //Middle level
            foreach (var t in middle_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y, 0.75f ) ; }
            foreach (var t in top_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y + 1f, 0.75f ).SetDelay(0.25f); }
            Camera.main.transform.DOMove(new Vector3(-5.93f, 4.7f, -7.26f), 0.75f);
            Camera.main.transform.DORotate(new Vector3(27f, 53.6f, 0f), 0.75f);
        } else if ( Mathf.Approximately(y, 2f) ) {
            //Top level
            foreach (var t in top_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y, 0.75f ); }
            foreach (var t in middle_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y, 0.75f ).SetDelay(0.25f); }
            Camera.main.transform.DOMove(new Vector3(-5.93f, 7f, -7.26f), 0.75f);
            Camera.main.transform.DORotate(new Vector3(27f, 53.6f, 0f), 0.75f);
        }
        StartCoroutine( clear_platform_wait_flag_after_time(0.5f) );
    }

    public void Restore_Position2() {
        StartCoroutine( Swap_Indicators_to_Switches_Coroutine(true) );
        foreach (var t in top_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y, 0.75f ); }
        foreach (var t in middle_obj) { t.DOKill(); t.DOMoveY( orig_positions[t].y, 0.75f ).SetDelay(0.25f) ; }
        Camera.main.transform.DOKill();
        Camera.main.transform.DOMove( new Vector3(-7.4f, 8.5f, -12f), 0.75f );
        Camera.main.transform.DORotateQuaternion(Quaternion.Euler(29.6f, 46.4f, 0f), 0.75f);
    }

    //OBSOLETE
    // public void Restore_Position() {
    //     BOT_Helpers.Platform.wait = true;
    //     for (int i = 0; i < obj_indicators.Count(); i++) {
    //         //obj[i].DOMove(orig_positions[i], 0.5f);
    //         var t = obj_indicators[i];
    //         if (Mathf.Approximately(t.position.y, orig_positions[t].y)) {
    //             if (Mathf.Approximately(t.position.z, orig_positions[t].z)) {
    //                 float origX = (t.position.x < 2f) ? 1f : 5f;
    //                 t.DOMoveX(origX, 0.3f).OnComplete( ()=> t.DOMove(orig_positions[t], 0.3f) );
    //             } else {
    //                 var p = new Vector3(orig_positions[t].x, t.position.y, t.position.z);
    //                 t.DOMove(p, 0.3f).OnComplete( ()=> t.DOMove(orig_positions[t], 0.3f) );
    //             }
    //         } else {
    //             var p = new Vector3(orig_positions[t].x, t.position.y, orig_positions[t].z);
    //             t.DOMove(p, 0.3f).OnComplete( ()=> t.DOMove(orig_positions[t], 0.3f) );
    //         }
    //     }
    //     StartCoroutine( clear_platform_wait_flag_after_time(0.6f, true) );
    // }

    public void Swap_Indicators_to_Switches() {
        StartCoroutine( Swap_Indicators_to_Switches_Coroutine() );
    }
    IEnumerator Swap_Indicators_to_Switches_Coroutine(bool reverse = false) {
        float step_anim_time = 0.6f;
        if (!reverse) {
            if (obj_switches[0].gameObject.activeSelf && Vector3.Distance( obj_switches[0].position, orig_positions[ obj_indicators[0] ] ) < 0.1f ) yield break;
            foreach (var t in obj_indicators) { t.DOKill(); t.DOLocalMove(position_step1[t], step_anim_time); }
            yield return new WaitForSeconds(step_anim_time);
            foreach (var t in obj_indicators) { t.gameObject.SetActive(false); }
            for (int i = 0; i < obj_switches.Length; i++) {
                obj_switches[i].gameObject.SetActive(true);
                obj_switches[i].DOKill();
                obj_switches[i].DOLocalMove( orig_positions[ obj_indicators[i] ] + transform.up * 0.72f, step_anim_time );
            }
        } else {
            if (obj_indicators[0].gameObject.activeSelf && Vector3.Distance( obj_indicators[0].position, orig_positions[ obj_indicators[0] ] ) < 0.1f ) yield break;
            for (int i = 0; i < obj_switches.Length; i++) {
                obj_switches[i].DOKill();
                obj_switches[i].DOLocalMove( position_step1[ obj_indicators[i] ] + transform.up * 0.72f, step_anim_time );
            }
            yield return new WaitForSeconds(step_anim_time);
            foreach (var t in obj_switches) { t.gameObject.SetActive(false); }
            foreach (var t in obj_indicators) { t.gameObject.SetActive(true); t.DOKill(); t.DOLocalMove(orig_positions[t], step_anim_time); }
        }

        yield return new WaitForSeconds(step_anim_time);
    }

    IEnumerator clear_platform_wait_flag_after_time(float t, bool restore_colliders = false) {
        yield return new WaitForSeconds(t);
        if (restore_colliders) {
            for (int i = 0; i < obj_indicators.Count(); i++) {
                obj_indicators[i].GetChild(0).GetComponent<BoxCollider>().enabled = true;
                obj_indicators[i].GetChild(1).GetComponent<BoxCollider>().enabled = true;
            }
        }
        BOT_Helpers.Platform.wait = false;
    }

}
