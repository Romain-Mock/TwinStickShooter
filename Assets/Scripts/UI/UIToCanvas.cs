using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIToCanvas : MonoBehaviour
{
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI currentAmmoText;
    public GameObject reloadBar;
    private Slider _reloadSlider;

    public TextMeshProUGUI healthPoints;

    private void OnEnable()
    {
        Weapon.OnWeaponUpdateAmmo += UpdateAmmoText;
        Weapon.OnReload += ReloadAnim;
        Player.OnPlayerUpdateHealth += UpdateHealthBar;
    }

    private void OnDisable()
    {
        Weapon.OnWeaponUpdateAmmo -= UpdateAmmoText;
        Weapon.OnReload -= ReloadAnim;
        Player.OnPlayerUpdateHealth -= UpdateHealthBar;
    }

    private void Awake()
    {
        _reloadSlider = reloadBar.GetComponent<Slider>();
    }

    void UpdateAmmoText(int currentAmmo, int ammosInMag)
    {
        if (ammosInMag > 1000)
            magAmmoText.text = "Infinity";
        else
            magAmmoText.text = ammosInMag.ToString();
        currentAmmoText.text = currentAmmo.ToString();
    }

    void ReloadAnim(float reloadTime)
    {
        reloadBar.SetActive(true);

        StartCoroutine(FillReloadBar(reloadTime));
    }

    IEnumerator FillReloadBar(float reloadTime)
    {
        _reloadSlider.value = _reloadSlider.minValue;

        float totalTime = 0f;

        while (_reloadSlider.value < _reloadSlider.maxValue)
        {
            totalTime += Time.deltaTime;
            _reloadSlider.value = Mathf.Lerp(_reloadSlider.minValue, _reloadSlider.maxValue, totalTime / reloadTime);
            yield return null;
        }

        _reloadSlider.value = _reloadSlider.maxValue;

        reloadBar.SetActive(false);
    }

    private void UpdateHealthBar(float value)
    {
        healthPoints.text = value.ToString();
    }
}
