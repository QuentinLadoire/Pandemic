using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] GameObject m_gauge = null;

    void OnUpdateTime(float percent)
    {
        //2.0f is the start size
        float subStractSize = 2.0f - (2.0f * percent);

        m_gauge.transform.localPosition = new Vector3(0.0f, -subStractSize, 0.0f);
        m_gauge.transform.localScale = new Vector3(2.0f, 2.0f - subStractSize, 2.0f);
    }

    private void Start()
    {
        GameStaticRef.gameController.onUpdateTimer += OnUpdateTime;
    }
    private void OnDestroy()
    {
        GameStaticRef.gameController.onUpdateTimer -= OnUpdateTime;
    }
}
