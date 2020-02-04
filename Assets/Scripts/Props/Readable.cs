using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Readable : MonoBehaviour
{
    [TextArea(4, 100)]
    public string text = "";
    public bool randomize_on_play = false;
}
