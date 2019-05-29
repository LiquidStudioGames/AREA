using UnityEngine;
using UnityEngine.UI;

public class TestGamemode : MonoBehaviour
{
    public GameObject Gate1;
    public GameObject Gate2;
    public Text CountdownText;

    void Start ()
    {
        Countdown (20.0f);
    }

    public void Countdown (float _Seconds)
    {
        CountdownText.enabled = true;

        while (_Seconds >= 0)
        {
            CountdownText.text = _Seconds.ToString ("F0");

            _Seconds -= Time.deltaTime;
        }

        CountdownText.enabled = false;
        SwitchGateState ();
    }

    public void SwitchGateState ()
    {
        Gate1.SetActive (!Gate1.activeInHierarchy);
        Gate2.SetActive (!Gate2.activeInHierarchy);
    }
}
