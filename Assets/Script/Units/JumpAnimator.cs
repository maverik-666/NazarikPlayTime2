namespace Project
{
    using System;
    using UnityEngine;

    public class JumpAnimator
    {
        private static readonly float[] Heights = { 0.25f, 0.125f, 0.05f };
        private static readonly float[] Durations = { 0.50f, 0.35f, 0.25f };
        private static readonly float XDistance = 1f;
        
        private readonly Transform _transform;

        private float _startX;
        private float _groundY;
        private float _totalX;
        private float _totalDuration;
        private float _elapsed;
        private bool _playing;
        private Action _onComplete;

        public bool IsPlaying => _playing;

        public JumpAnimator(Transform transform)
        {
            _transform = transform;
        }

        public void Play(float direction, Action onComplete = null)
        {
            _startX = _transform.position.x;
            _groundY = _transform.position.y;
            _elapsed = 0f;
            _playing = true;
            _onComplete = onComplete;

            _totalDuration = 0f;
            _totalX = XDistance * direction;
            foreach (float duration in Durations)
            {
                _totalDuration += duration;
            }
        }

        public void Tick(float deltaTime)
        {
            if (!_playing) return;

            _elapsed += deltaTime;

            float x = _startX + _totalX * Mathf.Clamp01(_elapsed / _totalDuration);
            float y = _groundY + GetBounceY(_elapsed);

            _transform.position = new Vector3(x, y, _transform.position.z);

            if (_elapsed >= _totalDuration)
            {
                _playing = false;
                _onComplete?.Invoke();
            }
        }

        private float GetBounceY(float t)
        {
            float jumpStart = 0f;
            for (int i = 0; i < Durations.Length; i++)
            {
                float jumpEnd = jumpStart + Durations[i];
                if (t < jumpEnd)
                {
                    float tNorm = (t - jumpStart) / Durations[i];
                    return Heights[i] * 4f * tNorm * (1f - tNorm);
                }
                jumpStart = jumpEnd;
            }
            return 0f;
        }
    }
}
