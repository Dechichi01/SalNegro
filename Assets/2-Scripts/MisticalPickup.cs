using UnityEngine;
using System.Collections;

public class MisticalPickup : Pickup {

    Light holyLight;
    public float maxIntensityMultiplier = 5f;
    [Range(0,7)]
    public float maxShineDist = 7f;
    [Range(.3f,6f)]
    public float minShineDist = .8f;
    public float lightVanishSpeed = 2f;

    float initialIntensity;

    bool shineEnabled = true;
    private void Start()
    {
        holyLight = GetComponentInChildren<Light>();
        initialIntensity = holyLight.intensity;
    }
    protected override void Update()
    {
        base.Update();

        if (shineEnabled && hit)
        {
            float distPercent = Mathf.Abs(transform.position.x - hit.transform.position.x) / (maxShineDist - minShineDist);
            holyLight.intensity = Mathf.Lerp(initialIntensity * maxIntensityMultiplier, initialIntensity, distPercent);
        }
    }

    protected override void GetPicked()
    {
        shineEnabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        Debug.Log(hit.transform.root.name);
        hit.transform.root.GetComponent<Character2D>().equippedWeapon.gameObject.SetActive(true);
        StartCoroutine(VanishLight());
    }

    IEnumerator VanishLight()
    {
        float percent = 0;
        float startIntensity = holyLight.intensity;
        while (percent < 1)
        {
            percent += Time.deltaTime * lightVanishSpeed;
            holyLight.intensity = Mathf.Lerp(startIntensity, 0f, percent);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
