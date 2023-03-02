using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Waypoint : MonoBehaviour
{
    public float Timer = 5;
    private float InitialTimerTime;
    public float WaitBeforeStartTimer = 15;
    public bool triggered = false;
    public Transform WhoTriggered;
    private Gradient gradient;
    [SerializeField] TMP_Text text;
    private bool LifeOver = false;
    private bool FireOnce = true;
    private float AddScoreWhileInDistance = 15;
    private float ScoreMultiplier = 5;
    
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    public Color[] GradientColors;
    [SerializeField] GameObject area;
    [SerializeField] Color TriggeredAreaColor;
    private Material mat;
    private void Start()
    {
        text.text = "";

        InitialTimerTime = Timer;

        mat=area.GetComponent<MeshRenderer>().material;
        gradient = new Gradient();
        int colors = GradientColors.Length;
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[colors];
        alphaKey = new GradientAlphaKey[colors];
        for(int i=0;i<colors;i++){
            colorKey[i].color=GradientColors[i];
            colorKey[i].time=Mathf.InverseLerp(0,colors-1,i);
            alphaKey[0].alpha = 1.0f;
        }

        gradient.SetKeys(colorKey, alphaKey);
        text.color = gradient.Evaluate(0.0f);
    }
    private void Update()
    {
        if(WaitBeforeStartTimer >= 0) {
            WaitBeforeStartTimer -= Time.deltaTime;
            if(WaitBeforeStartTimer <= 0) {
                LifeOver = true;
            }
        }
        if(triggered || LifeOver) {
            text.text = (Timer + Time.deltaTime).ToString("0.0");
            mat.SetColor("_Color",TriggeredAreaColor);
            Timer -= Time.deltaTime;
            if(WhoTriggered != null) {
                if(Vector3.Distance(transform.position, WhoTriggered.position) < AddScoreWhileInDistance)
                    Score.score += ScoreMultiplier * Time.deltaTime;
            }
        }
        if(Timer <= 0 && FireOnce) {
            GetComponentInParent<WaypointManager>().RemoveObj(this.gameObject);
            FireOnce = false;
            Destroy(this.gameObject);
        }

        text.color = gradient.Evaluate(1 - (Timer / InitialTimerTime));
    }
}
