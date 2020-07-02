using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Interfaces;


// Contains the audio data subjects
public class AudioBeat : MonoBehaviour, ISubject
{
    AudioSource _audioSource;

    public bool selfCreate = true;
    // VARIABLES for circular-shaped visualization
    public float radius = 3f;
    public int numFragments = 8;
    // this prototype needs to be positioned as the expected look of the element that aligns with the x-axis
    public GameObject prototype;

    public float[] _freqBands;
    public GameObject[] _visualizers;

    // The data
    public int lenSampleArray = 512; // needs to be a power of 2
    private float[] _samples;

    private float spectrumValue;

    // visualizer
    // public float maxBias;
    public float standardBias;
    public float timeStep;
    public float timeToBeat;
    public float restSmoothSpeed;

    private float[] bias;
    private float[] m_previousAudioValue;
    private float[] m_audioValue;
    private float[] m_timer;

    protected bool[] m_isBeat;

    public Vector3 beatScale;
    public Vector3 restScale;

    public int beat_count;

    // Singleton
    public static AudioBeat instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject);}
        else {instance = this;}
    }


    void Start()
    {

        if (selfCreate) {
            InitializeVariables();
            CreateVisualizers();
        }

    }

    void Update()
    {
        GetSpectrumAudioSource();

        UpdateData();

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

            GameObject visualizer = UnityEngine.Object.Instantiate(prototype, pos, rot);
            _visualizers[i] = visualizer;

            angle += angleOffset;
        }
    }

    void InitializeVariables()
    {
        _audioSource = GetComponent<AudioSource>();
        _samples = new float[lenSampleArray];   
        _freqBands = new float[numFragments];
        _visualizers = new GameObject[numFragments];

        bias = new float[numFragments];
        m_previousAudioValue = new float[numFragments];
        m_audioValue = new float[numFragments];
        m_timer = new float[numFragments];
        m_isBeat = new bool[numFragments];

        // float biasCumulator = 0f;
        // float offset = maxBias / numFragments;
        for (int i = 0; i < numFragments; i++) {
            // float b = biasCumulator + offset;
            // bias[i] = b;
            bias[i] = standardBias;
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Hamming);
        if (_samples != null && _samples.Length > 0)
        {
            int c = 0;
            float average = 0;
            int sampleCount = lenSampleArray / numFragments;
            for (int i = 0; i < numFragments; i++) {
                // average = 0; // line added by me
                for (int j = 0; j < sampleCount; j++) {
                    // average += _samples[c] * (c + 1);
                    // average += _samples[c];
                    average += Mathf.Log(_samples[c]);
                    c++;
                }
                // average /= c;
                average /= sampleCount;

                _freqBands[i] = average * 10;
            }

            // spectrumValue = _samples[0] * 100;
        }

    //     for (int i = 1; i < _samples.Length - 1; i++)
    //     {
    //         Debug.DrawLine(new Vector3(i - 1, _samples[i] + 10, 0), new Vector3(i, _samples[i + 1] + 10, 0), Color.red);
    //         Debug.DrawLine(new Vector3(i - 1, Mathf.Log(_samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(_samples[i]) + 10, 2), Color.cyan);
    //         Debug.DrawLine(new Vector3(Mathf.Log(i - 1), _samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), _samples[i] - 10, 1), Color.green);
    //         Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(_samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(_samples[i]), 3), Color.blue);
    //     }
    }

    public void OnBeat(int i)
    {
        // Debug.Log("beat");
        m_timer[i] = 0;
        m_isBeat[i] = true;

        StopCoroutine("MoveToScale");
        StartCoroutine(MoveToScale(beatScale, i));

    }

    public void UpdateData()
    { 
        for (int i = 0; i < numFragments; i++) {
            // float b = bias[i];
            float b = standardBias;

            // update audio value
            m_previousAudioValue[i] = m_audioValue[i];
            m_audioValue[i] = _freqBands[i];
            // m_audioValue[i] = spectrumValue;

            // if audio value went below the bias during this frame
            // if (m_previousAudioValue[i] > b &&
            //     m_audioValue[i] <= b)
            if (Math.Abs(m_previousAudioValue[i] - m_audioValue[i]) > b)
            {
                // if minimum beat interval is reached
                if (m_timer[i] > timeStep)
                    OnBeat(i);
            }

            // // if audio value went above the bias during this frame
            // if (m_previousAudioValue[i] <= b &&
            //     m_audioValue[i] > b)
            // {
            //     // if minimum beat interval is reached
            //     if (m_timer[i] > timeStep)
            //         OnBeat(i);
            // }

            m_timer[i] += Time.deltaTime;

        }
    }

    void UpdateVisualizers()
    {
        beat_count = 0;
        for (int i = 0; i < numFragments; i++)
        {
            if (m_isBeat[i]) {
                beat_count += 1;
                continue;
            }

            // Individual updates
            if (selfCreate){
                _visualizers[i].transform.localScale = Vector3.Lerp(_visualizers[i].transform.localScale, restScale, restSmoothSpeed * Time.deltaTime);            
            }
        }

        // Global updates 
        Notify();
    }

    private IEnumerator MoveToScale(Vector3 _target, int i)
    {
        Vector3 _curr = _visualizers[i].transform.localScale;
        Vector3 _initial = _curr;
        float _timer = 0;

        while (_curr != _target)
        {
            _curr = Vector3.Lerp(_initial, _target, _timer / timeToBeat);
            _timer += Time.deltaTime;

            _visualizers[i].transform.localScale = _curr;

            yield return null;
        }

        m_isBeat[i] = false;
    }


    // Subscription Pattern 
    private List<IObserver> _observers = new List<IObserver>();

    // The subscription management methods.
    public void Attach(IObserver observer)
    {
        this._observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        this._observers.Remove(observer);
    }

    // Trigger an update in each subscriber.
    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.UpdateOnChange(this);
        }
    }
}