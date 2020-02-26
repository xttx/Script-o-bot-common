using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Loot : MonoBehaviour
{
    public loot_type_enum loot_type;
    public int quantity = 1;
    public int quantyty_random_min = 0;
    public int quantyty_random_max = 0;
    public bool randomize_only_on_start = false;

    public GameObject[] additional_resources = new GameObject[]{};    

    int skip_frame = 3;
    int skip_frame_cur = 0;
    Run run_to_kill = null;

    public enum loot_type_enum {
        HP,
        MP,
        QuanticEnergy,
        Generic
    }

    List<Tweener> tweens_to_kill = new List<Tweener>();
    List<Material> materials_to_dissolve = new List<Material>();
    List<Quaternion> default_rotations = new List<Quaternion>();
    TextMeshPro tmp;
    bool deactivating = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            default_rotations.Add( transform.GetChild(i).localRotation );

        if (loot_type == loot_type_enum.QuanticEnergy) {
            //materials_to_dissolve.Add (GetComponent<MeshRenderer>().material);
            materials_to_dissolve.Add (transform.GetChild(0).GetComponent<MeshRenderer>().material);
            materials_to_dissolve.Add (transform.GetChild(1).GetComponent<MeshRenderer>().material);
            materials_to_dissolve.Add (transform.GetChild(2).GetComponent<MeshRenderer>().material);
            materials_to_dissolve.Add (transform.GetChild(3).GetComponent<MeshRenderer>().material);

            //Change color, if it's a static container
            if (randomize_only_on_start) materials_to_dissolve[0].SetColor("_EmissionColor", new Color(1f, 1f, 1.7f, 1f) * Mathf.LinearToGammaSpace(1.25f));

            SetAnim();
        } else if (loot_type == loot_type_enum.Generic) {
            for (int i = 0; i < transform.childCount; i++) {
                MeshRenderer mr = transform.GetChild(i).GetComponent<MeshRenderer>();
                if (mr != null && mr.enabled) {
                    //Debug.Log("Generic lot mat to disolve add: " + mr.gameObject);
                    materials_to_dissolve.Add (transform.GetChild(i).GetComponent<MeshRenderer>().material);
                }
            }
        }
        Randomize_Quantity();
    }

    void OnEnable()
    {
        Randomize_Quantity();
    }

    // Update is called once per frame
    void Update()
    {
        if (skip_frame_cur < skip_frame) {
            skip_frame_cur += 1; return;
        }
        skip_frame_cur = 0;

        if (!randomize_only_on_start && !deactivating) Randomize_Quantity();

        //TEST CLONINIG
        //if (Input.GetKeyDown(KeyCode.C)) Destroy_FX1();
    }

    void SetAnim()
    {
        tweens_to_kill.Clear();
        if (loot_type == loot_type_enum.QuanticEnergy) {
            tweens_to_kill.Add( transform.GetChild(1).DORotate(new Vector3(0f, 450f, 450f), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine) );
            tweens_to_kill.Add( transform.GetChild(2).DORotate(new Vector3(0f, 450f, 450f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear) );
            tweens_to_kill.Add( transform.GetChild(3).DORotate(new Vector3(0f, 450f, 450f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear) );
        }
    }

    public void Randomize_Quantity()
    {
        if (quantyty_random_max > 0) {
            quantity = Random.Range(quantyty_random_min, quantyty_random_max + 1);
            if(tmp == null) tmp = transform.GetChild(4).GetComponent<TextMeshPro>();
            tmp.text = quantity.ToString();
        }
    }

    //Called when script starts
    public void Set() {
        Randomize_Quantity();
    }

    //Called when picked up
    public void PickUpAndDestroy(int fx = 0) {
        //transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_DissolveCutoff", 0.5f);
        deactivating = true;

        if (fx == 1) { Destroy_FX1(); return; }

        foreach (Material m in materials_to_dissolve) {
            tweens_to_kill.Add ( m.DOFloat(1f, "_DissolveCutoff", 1f).OnComplete(()=> {
                foreach (Tweener t in tweens_to_kill) t.Kill();

                tweens_to_kill.Clear();
                gameObject.SetActive(false);
                //Debug.Log("Loot container deactivating.");
            }));
        }
    }

    //Called when script stops
    public void Reset(bool immediate = false) {
        //Reset with immedate = true is called from Engine when loading new scene

        //Debug.Log("Spawnable reset called.");
        deactivating = false;
        foreach (Material m in materials_to_dissolve)
            m.DOKill(true);
        // foreach (Tweener t in tweens_to_kill)
        //     t.Complete();

        //Destroy clones
        bool fx_in_process = false;
        int cnt = transform.childCount;
        for (int c = cnt - 1; c >= 0; c--) {
            Transform ch = transform.GetChild(c);
            if (ch.name.ToUpper().Contains("(CLONE)")) {
                fx_in_process = true;
                ch.DOKill();
                foreach (var mat in ch.GetComponent<MeshRenderer>().materials) { mat.DOKill(); }
                //Destroy(ch.gameObject);
                DestroyImmediate(ch.gameObject); //Destroy NON immediate cause problem when loop through childs below and there are still clones
            }
        }

        if (!gameObject.activeSelf || fx_in_process) {
            //Debug.Log("Reset_FX");
            //StopAllCoroutines();
            if (run_to_kill != null) { run_to_kill.Abort(); run_to_kill = null; }
            gameObject.SetActive(true);

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).localRotation = default_rotations[i];

            foreach (Material m in materials_to_dissolve) {
                if (immediate) {
                    m.SetFloat("_DissolveCutoff", 0f);
                } else {
                    m.DOFloat(0f, "_DissolveCutoff", 2.5f);
                }
            }

            Rigidbody r = GetComponent<Rigidbody>();
            r.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            if (immediate) {
                transform.position = new Vector3(transform.position.x, 0.12f, transform.position.z);
            } else {
                transform.position = new Vector3(transform.position.x, 4f, transform.position.z);
            }
            SetAnim();
        }
    }

    //void OnDestroy()
    void Destroy()
    {

    }

    public bool isDeactivating()
    {
        return deactivating && gameObject.activeSelf;
    }

    void Destroy_FX1()
    {
        //Debug.Log("FX_Called");
        int cnt = transform.childCount;
        for (int c = 0; c < cnt; c++) {
            Transform ch = transform.GetChild(c);
            if (ch.name.ToUpper().Contains("TEXTMESH")) continue;
            if (ch.GetComponent<MeshRenderer>() == null) continue;

            GameObject new_obj = Instantiate(ch.gameObject, ch.position, ch.rotation, ch.parent);
            new_obj.transform.localScale = ch.localScale + new Vector3(0.001f, 0.001f, 0.001f);

            new_obj.GetComponent<MeshRenderer>().material = Engine.Ref_objs.Fully_Transparent_Mat;
            GameObject mesh_effect = Instantiate(Engine.Ref_objs.Destroy_Container_Mesh_Effect, new_obj.transform);
            PSMeshRendererUpdater mru = mesh_effect.GetComponent<PSMeshRendererUpdater>();
            mru.FadeTime = 0.35f; mru.UpdateMeshEffect(new_obj);
            var mat_fx = new_obj.GetComponent<MeshRenderer>().materials[1];
            mat_fx.SetFloat("_GradientStrength", 1f);
            mat_fx.DOFloat(5f, "_GradientStrength", 0.25f).OnComplete(()=> {
                ch.GetComponent<MeshRenderer>().material.SetFloat("_DissolveCutoff", 1f);
                mat_fx.DOFloat(1f, "_GradientStrength", 0.5f).OnComplete(()=> {
                    mru.IsActive = false;
                    run_to_kill = Run.After(mru.FadeTime, ()=> { 
                        new_obj.transform.DOKill();
                        Destroy(new_obj);
                        //TODO: tweens in list are killed multiple times (for every object - 3 torus and 1 cube)
                        //      but need only once

                        //TODO: probably better solution will be to NOT make new created objects childs of main loot component
                        //      this way we can deactivate main loot obj right after a flash and we will not have collider problems
                        foreach (Tweener t in tweens_to_kill) t.Kill();
                        tweens_to_kill.Clear();
                        gameObject.SetActive(false); 
                    });
                });
            });

            var lt = DOTween.TweensByTarget(ch);
            if (lt != null) {
                Tweener new_t = null;
                if (c == 1) { 
                    new_obj.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    new_t = new_obj.transform.DORotate(new Vector3(0f, 450f, 450f), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine); 
                }
                if (c == 2) {
                    new_obj.transform.localRotation = Quaternion.Euler(120f, 90f, 90f);
                    new_t = new_obj.transform.DORotate(new Vector3(0f, 450f, 450f), 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear); 
                }
                if (c == 3) {
                    new_obj.transform.localRotation = Quaternion.Euler(240f, 90f, 90f);
                    new_t = new_obj.transform.DORotate(new Vector3(0f, 450f, 450f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }

                if (new_t != null) { new_t.SetAs(lt[0]); new_t.fullPosition = lt[0].fullPosition; }
            }
        }
    }
}
