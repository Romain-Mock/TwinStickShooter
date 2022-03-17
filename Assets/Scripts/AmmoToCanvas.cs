using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoToCanvas : MonoBehaviour
{
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI currentAmmoText;
    public GameObject reloadBar;
    private Slider _reloadSlider;

    private void OnEnable()
    {
        WeaponManager.OnWeaponUpdateAmmo += UpdateAmmoText;
        Weapon.OnReload += ReloadAnim;
    }

    private void OnDisable()
    {
        WeaponManager.OnWeaponUpdateAmmo -= UpdateAmmoText;
        Weapon.OnReload -= ReloadAnim;
    }

    private void Awake()
    {
        _reloadSlider = reloadBar.GetComponent<Slider>();
    }

    void UpdateAmmoText(int currentAmmo, int ammosInMag)
    {
        currentAmmoText.text = currentAmmo.ToString();
        magAmmoText.text = ammosInMag.ToString();
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
}
