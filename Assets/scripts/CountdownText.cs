using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;
    Text countdown;

    void OnEnable() {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown() {
        for(int i = 3; i >= 0; i--) {
            countdown.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        OnCountdownFinished();
    }
}
