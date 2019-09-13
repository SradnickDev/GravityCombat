using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class CompTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var q = Quaternion.Euler(194, 47, 322);
        var c = Compression.PackQuaternion(q);
        var r = Compression.UnpackQuaternion(c);
        Debug.Log($"Origin {q} : unpacked {r}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
