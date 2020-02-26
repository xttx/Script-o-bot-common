//IMPORTANT: When using with requirement_from_statable (leds indicators)
//              the indicators must be above switches in hierarchy!!!
//              otherwise they are reset AFTER switch requirement check is done

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Switch : MonoBehaviour, Engine.ISetable, Engine.IStatableBool, Engine.IResetable
{
    public bool default_state = false;
    public bool randomize_state = false;
    public bool require_ON = false;
    public bool send_engine_event = false;
    public float suspend_script_on_changeState = 0f;
    public GameObject requirement_from_statable = null;
    public Switch_Group switch_group = null;
    public UnityEngine.Events.UnityEvent OnRequirementMet = null;

    [HideInInspector] bool switch_is_in_required_position = false;

    Transform pimpa = null;
    //MeshRenderer mr_for_emission = null;
    Material mat_for_emission = null;

    // Start is called before the first frame update
    void Start()
    {
        pimpa = transform.GetChild(0);
        mat_for_emission = transform.GetChild(4).GetComponent<MeshRenderer>().material;
        mat_for_emission.EnableKeyword("_EMISSION");

        if (switch_group != null) if(!switch_group.switch_list.Contains(this)) switch_group.switch_list.Add(this);
        SetState(default_state);
    }

    public void SetState(bool b, bool from_bot = false)
    {
        //Because in level array2 switches spawns disabled, this cause null reference to pimpa when starting script
        if (pimpa == null) { Start(); }

        if (b) {
            pimpa.localRotation = Quaternion.Euler(-40f, 0f, 0f);
            mat_for_emission.SetColor("_EmissionColor", new Color(0.24f, 0.9f, 0.13f, 1f));
        } else {
            pimpa.localRotation = Quaternion.Euler(40f, 0f, 0f);
            mat_for_emission.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 1f));
        }

        if (from_bot) {
            StartCoroutine(ResumeBotScriptAfterDelay());
        }
        check_requirement();
    }
    IEnumerator ResumeBotScriptAfterDelay()
    {
        if (suspend_script_on_changeState > 0f) {
            yield return new WaitForSecondsRealtime(suspend_script_on_changeState);
        }
        BOT.suspend_script--;
    }

    public bool GetState()
    {
        Color c = mat_for_emission.GetColor("_EmissionColor");
        return c.r > 0.5f || c.g > 0.5f || c.b > 0.5f;
    }

    //Called when script starts
    public void Set()
    {
        if (randomize_state) {
            int r = Random.Range(0, 2);
            if (r == 0) SetState(false); else SetState(true);
        } else {
            SetState(default_state);
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        SetState(default_state);
    }

    public void check_requirement()
    {
        bool b = GetState();
        bool req = (b == require_ON);
        if (requirement_from_statable != null) {
            var s = requirement_from_statable.GetComponent<Engine.IStatableBool>();
            if (s != null) {
                req = b == s.GetState();
            }
        }

        if (req) {
            //Switch is in awaited position
            switch_is_in_required_position = true;

            //if (OnRequirementMet == null) return;
            if (switch_group == null) {
                if (OnRequirementMet != null) { OnRequirementMet.Invoke(); } 
                if (send_engine_event) { Engine.SetRequirement("Switch"); } //Debug.Log(gameObject.name); }
            } else {
                int req_met_count = switch_group.switch_list.Where(sw=> sw.switch_is_in_required_position).Count();
                if (req_met_count == switch_group.switch_list.Count()) { 
                    if (OnRequirementMet != null) { OnRequirementMet.Invoke(); }
                    if (send_engine_event) { Engine.SetRequirement("Switch"); } //Debug.Log(gameObject.name); }
                }
            }
        } else {
            switch_is_in_required_position = false;
        }

        // string str = gameObject.name + ": Switches in required position: ";
        // foreach (var t in switch_group.switch_list) {
        //     str += t.gameObject.name + " " + t.switch_is_in_required_position + "; ";
        // }
        // Debug.Log(str);
    }
}
