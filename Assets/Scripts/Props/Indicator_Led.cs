using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator_Led : MonoBehaviour, Engine.ISetable, Engine.IStatableBool
{
    public bool default_state = false;
    public bool randomize_state = false;

    public MeshRenderer emission_mesh = null;
    public MeshRenderer emission_mesh2 = null;
    public int emission_mat_index = 0;
    public int emission_mat_index2 = 0;
    
    [ColorUsageAttribute(true, true)]
    public Color[] emission_colours = new Color[]{ new Color(1.7f, 1f, 0.4f, 1f), new Color(1.7f, 1f, 0.4f, 1f), new Color(1.7f, 1f, 0.4f, 1f) };

    List<Material> mat_for_emission = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        if (mat_for_emission.Count != 0) return;

        mat_for_emission.Add ( transform.GetChild(0).GetComponent<MeshRenderer>().material );
        if (emission_mesh != null) { mat_for_emission.Add ( emission_mesh.materials[emission_mat_index] ); }
        if (emission_mesh2 != null) { mat_for_emission.Add ( emission_mesh2.materials[emission_mat_index2] ); }
        
        foreach (var m in mat_for_emission) { m.EnableKeyword("_EMISSION"); }
        SetState(default_state);
    }

    public void SetState(bool b)
    {
        if (b) {
            //foreach (var m in mat_for_emission) { m.SetColor("_EmissionColor", new Color(1.7f, 1f, 0.4f, 1f)); }
            foreach (var m in mat_for_emission) { m.SetColor("_EmissionColor", emission_colours[mat_for_emission.IndexOf(m)] ); }
        } else {
            foreach (var m in mat_for_emission) { m.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 1f)); }
        }
    }

    public bool GetState()
    {
        if (mat_for_emission.Count == 0) Start();

        Color clr = mat_for_emission[0].GetColor("_EmissionColor");
        return clr.r > 1f || clr.g > 1f || clr.b > 1f;
    }

    //Called when script starts
    public void Set()
    {
        if (randomize_state) {
            int r = Random.Range(0, 2);
            //Debug.Log("Indicator Led: Randomize state to: " + r);
            if (r == 0) SetState(false); else SetState(true);
        }
    }
}
