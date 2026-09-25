namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    public class SpriteFlasher
    {
        private static readonly int _levelId = Shader.PropertyToID("_Level");
        private static readonly int _saturationColorId = Shader.PropertyToID("_SaturationColor");
        private static readonly int _saturationPhaseId = Shader.PropertyToID("_SaturationPhase");
        
        private readonly GameObject _gameObject;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly MaterialPropertyBlock _propBlock;
        
        private Tween _flashTween;

        public SpriteFlasher(GameObject gameObject, SpriteRenderer spriteRenderer)
        {
            _gameObject = gameObject;
            _spriteRenderer = spriteRenderer;
            _propBlock = new MaterialPropertyBlock();
        }

        public void FlashColor(Color color, float maxValue, float duration)
        {
            _spriteRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_saturationColorId, color);
            _spriteRenderer.SetPropertyBlock(_propBlock);

            _flashTween?.Kill();
            _flashTween = DOVirtual.Float(0f, maxValue, duration, SetFillPhase)
                .SetLink(_gameObject, LinkBehaviour.KillOnDisable)
                .SetLoops(2, LoopType.Yoyo);

            void SetFillPhase(float phase)
            {
                _spriteRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(_saturationPhaseId, phase);
                _spriteRenderer.SetPropertyBlock(_propBlock);
            }
        }
    }
}