using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Engine.IResetable
{
    bool is_opened = false;
    Animation anim = null;

    void Start() {
        anim = transform.GetChild(0).GetChild(1).GetComponent<Animation>();
        anim["close"].speed = 3f;
    }

    public void Open() {
        if (!Control_UI.isPlaying()) { Close(); return; }

        if (!is_opened) {
            anim.Play("open"); is_opened = true;
        }
    }

    public void Close() {
        if (is_opened) {
            anim.Play("close"); is_opened = false;
        }
    }

    public void Reset() { Close(); }
}
