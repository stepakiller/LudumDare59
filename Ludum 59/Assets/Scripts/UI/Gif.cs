using UnityEngine;
using UnityEngine.UI;
public class Gif : MonoBehaviour
{
    [SerializeField] Sprite[] _frames;
    [SerializeField] float _framesPerSecond = 10f;
    [SerializeField] bool _loop = true;
    [Header("References")]
    [SerializeField] Image _targetImage;

    int _currentFrameIndex;
    float _timer;
    bool _isPlaying = true;

    void Awake()
    {
        if (_targetImage == null) _targetImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!_isPlaying || _frames == null || _frames.Length == 0) return;
        _timer += Time.deltaTime;
        if (_timer >= 1f / _framesPerSecond)
        {
            _timer = 0;
            _currentFrameIndex++;

            if (_currentFrameIndex >= _frames.Length)
            {
                if (_loop)
                    _currentFrameIndex = 0;
                else
                {
                    _isPlaying = false;
                    return;
                }
            }

            _targetImage.sprite = _frames[_currentFrameIndex];
        }
    }
    public void Play(bool play) => _isPlaying = play;
}