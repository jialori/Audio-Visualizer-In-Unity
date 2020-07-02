using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// Contains the audio data subjects
public class AudioData : MonoBehaviour
{
    AudioSource _audioSource;

    public bool selfCreate = true;
    // VARIABLES for circular-shaped visualization
    public float radius = 3f;
    public int numFragments = 8;
    public float minScale = .8f;
    public float maxScale = 2f;
    // this prototype needs to be positioned as the expected look of the element that aligns with the x-axis
    public GameObject prototype;

    [Header("Health Settings")]
    public bool stayPositive;
    public bool times10;


    public GameObject[] _visualizers;

    // The data bands
    public int lenSampleArray = 512; // needs to be a power of 2
    public float[] _samples;
    public float[] _freqBands;
    public float[] _freqBandsHighest;
    public float[] _bufferedBands;
    public static float[] _audioBands;
    public static float[] _audioBandsBuffered;
    float[] _bufferDecrease;

    // used in buffering (visualizer)
    private Vector3 initLocalScale;
    public bool useBuffer;
    public float bufferDegree;
    public float initBufferAmount = 0.005f;
    float _bufferPrevSample;
    float _bufferDecreased;
    float _bufferIncrease;
    bool _wasDecrease;

    // Singleton
    public static AudioData instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject);}
        else {instance = this;}
    }


    void Start()
    {

        if (selfCreate) {
            initLocalScale = prototype.transform.localScale;
            InitializeVariables();
            CreateVisualizers();
        }

    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        MakeBufferedBands();
        MakeAudioBands();

        UpdateVisualizers();
    }

    void CreateVisualizers()
    {
        // For 2D visualizers, manipulate x and y axes, and use z position as given

        // circular shaped 
        float angleOffset = 2 * Mathf.PI / numFragments;

        float origin_x = transform.position.x;
        float origin_y = transform.position.y;
        float pos_z = transform.position.z;
        float angle = 0;
        for (int i = 0; i < numFragments; i++) {
            float pos_x = origin_x + radius * Mathf.Sin(angle);
            float pos_y = origin_y + radius * Mathf.Cos(angle);
            Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
            Quaternion rot = Quaternion.Euler(0, 0, - angle * 180 / Mathf.PI);

            GameObject visualizer = UnityEngine.Object.Instantiate(prototype, pos, rot, transform);
            _visualizers[i] = visualizer;

            angle += angleOffset;
        }
    }

    void InitializeVariables()
    {
        _audioSource = GetComponent<AudioSource>();
        _samples = new float[lenSampleArray];

        _freqBands = new float[numFragments];
        _freqBandsHighest = new float[numFragments];
        _bufferedBands = new float[numFragments];
        _audioBands = new float[numFragments];
        _audioBandsBuffered = new float[numFragments];
        _bufferDecrease = new float[numFragments];        

        _visualizers = new GameObject[numFragments];
    }

    void GetSpectrumAudioSource()
    {
        // _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        int count_prev = 0;
        int sampleCount = lenSampleArray / numFragments;
        for (int i = 0; i < numFragments; i++) {
            float average = 0;
            // int sampleCount = (int)Math.Pow(2, i) * 2;
            // if (i == 7) sampleCount += 2;
            // for (int j = 0; j < sampleCount; j++) {
            //     // average += _samples[count];
            //     average += _samples[count] * (count + 1);
            //     count++;
            // }

            for (int j = 0; j < sampleCount; j++) {
                average += _samples[count] * (count + 1);
                count++;
            }

            // Debug.Log("freqband: " + count.ToString());
            // Debug.Log("freqband: " + average.ToString());
            // average /= (count - count_prev);
            average /= count;
            count_prev = count;

            if (times10) {
                _freqBands[i] = average * 10;                
            } else {
                _freqBands[i] = average;
            }
        }

    }

    void MakeBufferedBands()
    {
        for (int i = 0; i < numFragments; i++)
        {
            if (_freqBands[i] > _bufferedBands[i])
            {
                _bufferedBands[i] = _freqBands[i];
                _bufferDecrease[i] = 0.005f;
            }

            if (_freqBands[i] < _bufferedBands[i])
            {
                _bufferedBands[i] -= _bufferDecrease[i];
                if (stayPositive) {_bufferedBands[i] = Math.Max(_bufferedBands[i], 0);}
                _bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void MakeAudioBands()
    {
        for (int i = 0; i < numFragments; i++)
        {
            if (_freqBands[i] > _freqBandsHighest[i]) _freqBandsHighest[i] = _freqBands[i];
            _audioBands [i] = (_freqBands[i] / _freqBandsHighest[i]);
            _audioBandsBuffered[i] = (_bufferedBands[i] / _freqBandsHighest[i]);
        }
    }

    void UpdateVisualizers()
    {
        for (int i = 0; i < numFragments; i++)
        {
            float scale;
            if (useBuffer)
            {
                float bufferedSample = CreateBuffer(AudioData._audioBandsBuffered[i]);
                scale = LerpInScale(bufferedSample);
                // variational buffering
            }
            else
            {
                scale = LerpInScale(AudioData._audioBandsBuffered[i]);
            }
           Vector3 original = _visualizers[i].transform.localScale;
           _visualizers[i].transform.localScale = new Vector3(original.x, scale * initLocalScale.y, original.z);
        }

    }


    private float LerpInScale(float x)
    {
        return (1 - x) * minScale + x * maxScale;
    }

    float CreateBuffer(float newSample)
    {
        if (newSample > _bufferPrevSample)
        {
            if (!_wasDecrease) // still increasing
            {
                if (newSample - _bufferIncrease > _bufferPrevSample) 
                    newSample = newSample - _bufferIncrease;
                else
                    newSample = _bufferPrevSample;
                _bufferIncrease *= bufferDegree;
            } 
            else 
            { // new increase
                _bufferIncrease = initBufferAmount;
                _wasDecrease = false;
            }
            _bufferDecreased = initBufferAmount * bufferDegree;
        }
        else if (newSample < _bufferPrevSample)
        {
            if (_wasDecrease) // still decresing
            {
                if (newSample + _bufferIncrease < _bufferPrevSample) 
                    newSample = newSample + _bufferIncrease;
                else
                    newSample = _bufferPrevSample;
                // newSample += _bufferDecrease;
                _bufferDecreased *= bufferDegree;
            } 
            else 
            { // new decrease
                _bufferDecreased = initBufferAmount;
                _wasDecrease = true;
            }
        }

        _bufferPrevSample = newSample;

        return newSample;
    }

}
