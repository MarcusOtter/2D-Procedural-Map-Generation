using System;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralMapUI : MonoBehaviour
{
    [Header("Slider objects")]
    [SerializeField] private Slider _mapLengthSlider;
    [SerializeField] private Slider _maxHeightSlider;
    [SerializeField] private Slider _maxDepthSlider;
    [SerializeField] private Slider _levelHeightSlider;

    [Space(10)]
    [Header("Text objects")]
    [SerializeField] private Text _mapLengthText;
    [SerializeField] private Text _maxHeightText;
    [SerializeField] private Text _maxDepthText;
    [SerializeField] private Text _levelHeightText;

    [Space(10)]
    [Header("Other objects")]
    [SerializeField] private RectTransform _panelArrow;
    [SerializeField] private Animator _menuAnimator;

    private ProceduralMapGenerator _generator;

    private int _panelArrowRotation = -90;

    private void Start()
    {
        _generator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<ProceduralMapGenerator>();
        _menuAnimator.GetComponentInChildren<Animator>();

        _menuAnimator.SetBool("Enabled", true);
        _panelArrow.rotation = Quaternion.Euler(new Vector3(0, 0, -90));

        // Make sure text changes when the slider does
        _mapLengthSlider.onValueChanged.AddListener(delegate { SetSliderText(_mapLengthSlider, _mapLengthText); });
        _maxHeightSlider.onValueChanged.AddListener(delegate { SetSliderText(_maxHeightSlider, _maxHeightText); });
        _maxDepthSlider.onValueChanged.AddListener(delegate { SetSliderText(_maxDepthSlider, _maxDepthText); });
        _levelHeightSlider.onValueChanged.AddListener(delegate { SetSliderText(_levelHeightSlider, _levelHeightText); });


        SetSliderText(_mapLengthSlider, _mapLengthText);
        SetSliderText(_maxHeightSlider, _maxHeightText);
        SetSliderText(_maxDepthSlider, _maxDepthText);
        SetSliderText(_levelHeightSlider, _levelHeightText);
    }

    public void TogglePanel()
    {
        _menuAnimator.SetBool("Enabled", !_menuAnimator.GetBool("Enabled"));
        _panelArrowRotation = -_panelArrowRotation;
        _panelArrow.rotation = Quaternion.Euler(0, 0, _panelArrowRotation);
    }

    private void SetSliderText(Slider slider, Text text)
    {
        text.text = slider.value.ToString();
    }

    // Called by UI Button
    public void GenerateMap()
    {
        _generator.ResetGame();
        _generator.mapLength = Convert.ToUInt16(_mapLengthSlider.value);
        _generator.heightLimit = Convert.ToUInt16(_maxHeightSlider.value);
        _generator.depthLimit = Convert.ToUInt16(_maxDepthSlider.value);
        _generator.levelHeight = Convert.ToUInt16(_levelHeightSlider.value);

        if (_mapLengthSlider.value <= 0)
        {
            return;
        }

        _generator.GenerateMap();
    }

}
