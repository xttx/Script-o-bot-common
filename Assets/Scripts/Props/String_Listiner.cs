using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class String_Listiner : MonoBehaviour
{
    public string required_string = "";
    public bool required_string_is_concatenate_all_readables = false;
    public UnityEvent OnSayOk;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSay(string txt)
    {
        string req = required_string;

        if (required_string_is_concatenate_all_readables) {
            req = string.Join(" ", Engine.Level_Readable.Select((x)=>x.text));
        }

        if (txt == req) {
            //Check BOT position
            var bot_pos = BOT.bot_obj.transform.position;
            var required_bot_pos = transform.position + transform.forward;
            bot_pos.y = 0f; required_bot_pos.y = 0f;
            if (Vector3.Distance(bot_pos, required_bot_pos) > 0.25f ) return;

            //Check BOT rotation
            var lookPos = transform.position - bot_pos; lookPos.y = 0f;
            var lookRot = Quaternion.LookRotation(lookPos);
            if (Quaternion.Angle(BOT.bot_obj.transform.rotation, lookRot) > 15f) return;

            if (OnSayOk != null) OnSayOk.Invoke();
        }
    }
}
