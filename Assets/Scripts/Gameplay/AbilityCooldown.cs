using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private Image cooldownRing;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMPro.TextMeshProUGUI cooldownText;

    void Start()
    {
        backgroundImage.color = new Color(1f, 1f, 1f, 1f);
        cooldownRing.fillAmount = 0;
        cooldownText.gameObject.SetActive(false);
    }

    public void StartCooldown(float time)
    {
        StartCoroutine(Cooldown(time));
    }

    private IEnumerator Cooldown(float time)
    {
        float elapsedTime = 0;
        cooldownText.gameObject.SetActive(true);
        cooldownText.text = time.ToString();
        cooldownRing.fillAmount = 1;
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        while (elapsedTime < time)
        {
            // IF MORE THAN A SECOND IS LEFT THEN WE DISPLAY TIME ROUNDED TO INT
            if (time - elapsedTime > 1)
            {
                cooldownText.text = ((int)(time - elapsedTime)).ToString();
            }
            else // IF LESS THAN A SECOND IS LEFT THEN WE DISPLAY TIME ROUNDED TO 1 DECIMAL PLACE
            {
                cooldownText.text = ((double)(time - elapsedTime)).ToString("0.0");
            }
            elapsedTime += Time.deltaTime;
            cooldownRing.fillAmount = 1 - elapsedTime / time;
            yield return null;
        }
        backgroundImage.color = new Color(1f, 1f, 1f, 1f);
        cooldownText.gameObject.SetActive(false);
    }
}
