using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCT;

public class ScriptableTextHelper : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ScriptableTextDisplay m_scriptableTextDisplay = null;
    [SerializeField] private ScriptableTextTypeList m_scriptableTextTypeList = null;
    [SerializeField] private Camera m_camera = null;

    [Header("Settings")]
    [SerializeField] private float m_ticTime = 0.5f;
    [SerializeField] private string m_text = "12345";

    [Header("Randomize On Horizontal Axis")] [SerializeField]
    private Vector2 m_range = new Vector2(-5, 5);
    
    private void OnGUI()
    {
        GUI.Box(new Rect(0, Screen.height - 100, 250, 100), "Helper");
        if(GUI.Button(new Rect(20,Screen.height - 70,80,40),"Start"))
        {
            StartCoroutine(StartTic());
        }
        if (GUI.Button(new Rect(20+80+50, Screen.height - 70, 80, 40), "Stop"))
        {
            StopAllCoroutines();
            m_scriptableTextDisplay.DisableAll();
        }
    }

    private void HelperClass()
    {
        for (int i = 0; i < m_scriptableTextTypeList.ListSize; i++)
        {
            var rndPos = m_camera.transform.forward + new Vector3(Random.Range(m_range.x, m_range.y), 0, 0);
            rndPos.y = 0;

            if (m_scriptableTextDisplay.TextTypeList.ScriptableTextTyps[i].StackValues == true)
            {
                m_scriptableTextDisplay.InitializeStackingScriptableText(i, rndPos, m_text, "Test" + i);
            }
            else
            {
                m_scriptableTextDisplay.InitializeScriptableText(i, rndPos, m_text);
            }
        }
    }

    private IEnumerator StartTic()
    {
        HelperClass();
        yield return new WaitForSeconds(m_ticTime);
        StartCoroutine(StartTic());
    }
}
