using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowEffect : MonoBehaviour
{
    private MaskableGraphic _graphicComponent;
    private Renderer _rendererComponent;
    private float _myHue;
    [SerializeField] private bool _startRandom;
    [Range(0,1)]
    [SerializeField] private float _baseSaturation, _baseValue;
    [SerializeField] private float _hueShiftSpd;
    private Color32 myColor;

    void Start()
    {
        _graphicComponent = GetComponent<MaskableGraphic>();
        _rendererComponent = GetComponent<Renderer>();
        if (_startRandom) _myHue = Random.Range(0f,1f);
    }

    // Update is called once per frame
    void Update()
    {
        _myHue += _hueShiftSpd * Time.deltaTime;
        if (_myHue > 1f)
            _myHue--;
        myColor = Color.HSVToRGB(_myHue, _baseSaturation, _baseValue);
        if (_graphicComponent)
            _graphicComponent.color = myColor;
        if (_rendererComponent)
            _rendererComponent.material.color = myColor;
    }

}
