using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Level_Array_Rotator : MonoBehaviour, Engine.IResetable
{
    //float speed_skybox = 0.25f;

    Transform gear = null;

    void Start() {
        gear = transform.Find("gear1");
    }

    public void Rotate_Level()
    {
        //StartCoroutine(Rotate_Level_Coroutine());
        gear.DOKill(); RenderSettings.skybox.DOKill();
        gear.DOLocalRotateQuaternion(Quaternion.Euler(-180f, 0f, 90f), 30f).SetSpeedBased(true).SetUpdate(true);
        RenderSettings.skybox.DOFloat(180f, "_RotationZ", 30f).SetSpeedBased(true).SetUpdate(true);
    }

    public void Reset()
    {
        // StopAllCoroutines();
        // StartCoroutine(Rotate_Level_Coroutine(true));

        gear.DOKill(); RenderSettings.skybox.DOKill();
        gear.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 90f), 360f).SetSpeedBased(true).SetUpdate(true);
        RenderSettings.skybox.DOFloat(0f, "_RotationZ", 360f).SetSpeedBased(true).SetUpdate(true);
    }

    public void OnDisable() {
        RenderSettings.skybox.SetFloat("_RotationZ", 0f);
        gear.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    //OBSOLETE
    // IEnumerator Rotate_Level_Coroutine(bool reset = false) {
    //     if (!reset) {
    //         for (int i = 0; i < 360; i++) {
    //             float cur = (float)i * 0.5f;
    //             RenderSettings.skybox.SetFloat("_RotationZ", -cur);
    //             gear.localRotation = Quaternion.Euler(cur, 0f, 90f);
    //             yield return null;
    //         }
    //         RenderSettings.skybox.SetFloat("_RotationZ", 180f);
    //         gear.localRotation = Quaternion.Euler(180f, 0f, 90f);
    //     } else {
    //         float current_rotation = gear.localRotation.eulerAngles.x;
    //         if ( Mathf.Approximately(current_rotation, 0f)) yield break;
    //         for (int i = Mathf.RoundToInt(current_rotation); i >= 0; i--) {
    //             float cur = (float)i;
    //             RenderSettings.skybox.SetFloat("_RotationZ", -cur);
    //             gear.localRotation = Quaternion.Euler(cur, 0f, 90f);
    //             yield return null;
    //         }
    //         RenderSettings.skybox.SetFloat("_RotationZ", 0f);
    //         gear.localRotation = Quaternion.Euler(0f, 0f, 90f);
    //     }
    // }
}
