using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    [SerializeField] private int maxScore = 20;
    [SerializeField] private int minScore = 1;
    [SerializeField] private float timer = 20f;
    public int currentScore;

    private float currentTime;
    private bool stopTimer = false;

    protected virtual void Start()
    {
        currentTime = timer;
        currentScore = maxScore;
    }

    protected virtual void Update()
    {
        Timer();
    }

    public void Timer()
    {
        if (stopTimer) return;

        currentTime -= Time.deltaTime;

        switch (currentTime)
        {
            case float n when n > (timer/4) * 3:
                currentScore = maxScore;
                break;
            case float n when n > (timer/2):
                currentScore = (maxScore/4) * 3;
                break;
            case float n when n > (timer/4):
                currentScore = maxScore/2;
                break;
            case float n when n > 0:
                currentScore = maxScore/4;
                break;
            case float n when n <= 0:
                currentScore = minScore;
                stopTimer = true;
                break;
            default:
                break;
        }
    }

}
