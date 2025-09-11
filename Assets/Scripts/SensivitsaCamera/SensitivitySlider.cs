using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float minSensitivity = 20f;
    public float maxSensitivity = 200f;

    void Start()
    {
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;

        // ✅ Set nilai awal (misalnya 100) atau dari PlayerPrefs
        float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 100f);
        sensitivitySlider.value = savedSensitivity;

        // Listener update
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }


    void UpdateSensitivity(float value)
    {
        // Simpan nilai ke PlayerPrefs
        PlayerPrefs.SetFloat("CameraSensitivity", value);
        PlayerPrefs.Save();
    }
}
