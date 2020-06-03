using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tokens : MonoBehaviour
{
    public void PlaceAt(Transform target)
    {
        var position = transform.position;

        position = target.position;
        position += new Vector3(0.0f, 0.05f, 0.0f);

        transform.position = position;
    }
    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }
}
