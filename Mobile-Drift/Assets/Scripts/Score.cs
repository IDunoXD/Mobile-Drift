using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static float score = 0;
    private TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }
    void Update()
    {
        text.text = score.ToString("0");
    }
}
