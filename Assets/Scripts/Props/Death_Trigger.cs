using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (BOT.script_thread == null) return;
        if(other.GetType() == typeof(SphereCollider)) {
            Hud.ShowDamage(BOT.bot_obj.transform.position, BOT.HP, true, "Death surface");
            BOT.HP = 0;
        }
    }
}
