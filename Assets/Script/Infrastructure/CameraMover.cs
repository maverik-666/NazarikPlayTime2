using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField]
    private Vector2 _boundsCenter = Vector2.zero;

    [SerializeField]
    private float _boundsWidth = 20f;

    [SerializeField]
    private float _boundsHeight = 20f;

    [Header("Zoom")]
    [SerializeField]
    private float _minZoom = 2f;

    [SerializeField]
    private float _maxZoom = 15f;

    [SerializeField]
    private float _zoomStep = 1f;

    [SerializeField]
    private float _zoomSmoothSpeed = 8f;

    [Header("Floor")]
    [SerializeField]
    private float _floorY = 0f;

    [Header("Pan")]
    [SerializeField]
    private float _panSmoothSpeed = 10f;

    private Camera _camera;
    private float _targetOrthoSize;
    private Vector3 _targetPosition;
    private bool _isLocked;

    private float _lastAspect;

    public void Init()
    {
        _camera = GetComponent<Camera>();
        _lastAspect = _camera.aspect;
        SnapAll();
    }

    public void LockAt(Vector3 position, float orthoSize)
    {
        _isLocked = true;
        _targetPosition = position;

        _targetOrthoSize = ClampOrthoSize(orthoSize);
        ClampTargetPosition();
    }
    
    private void SnapAll()
    {
        _camera.orthographicSize = _targetOrthoSize = ClampOrthoSize(_camera.orthographicSize);

        ClampTargetPosition();

        var pos = _targetPosition;
        pos.z = transform.position.z;
        transform.position = pos;
    }
    
    private void Update()
    {
        CheckAspectChanged();

        if (!_isLocked)
        {
            HandleZoom(); 
            HandleDrag();
        }
        
        ApplyMovement();
    }
    
    private void CheckAspectChanged()
    {
        if (Mathf.Approximately(_camera.aspect, _lastAspect))
        {
            return;
        }
        
        _lastAspect = _camera.aspect;
        SnapAll();
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        float oldOrthoSize = _targetOrthoSize;
        _targetOrthoSize -= Mathf.Sign(scroll) * _zoomStep;
        _targetOrthoSize = ClampOrthoSize(_targetOrthoSize);

        float mouseX = Mouse.current.position.ReadValue().x;
        float cursorOffsetX = mouseX - Screen.width * 0.5f;
        _targetPosition.x += cursorOffsetX * (oldOrthoSize - _targetOrthoSize) * 2f / Screen.height;

        ClampTargetPosition();
    }
    
    private void HandleDrag()
    {
        if (!Mouse.current.leftButton.isPressed)
        {
            return;
        }

        var screenDelta = Mouse.current.delta.ReadValue();
        if (screenDelta == Vector2.zero) return;

        float worldUnitsPerPixel = _camera.orthographicSize * 2f / Screen.height;
        _targetPosition.x += -screenDelta.x * worldUnitsPerPixel;
        ClampTargetPosition();
    }

    private void ApplyMovement()
    {
        float t = Time.unscaledDeltaTime;
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthoSize, _zoomSmoothSpeed * t);

        float halfH = _camera.orthographicSize;
        float halfW = _camera.orthographicSize * _camera.aspect;

        float minX = _boundsCenter.x - _boundsWidth * 0.5f + halfW;
        float maxX = _boundsCenter.x + _boundsWidth * 0.5f - halfW;
        float minY = _boundsCenter.y - _boundsHeight * 0.5f + halfH;
        float maxY = _boundsCenter.y + _boundsHeight * 0.5f - halfH;

        float x = Mathf.Lerp(transform.position.x, _targetPosition.x, _panSmoothSpeed * t);
        x = minX > maxX ? _boundsCenter.x : Mathf.Clamp(x, minX, maxX);

        float y = minY > maxY ? _boundsCenter.y : Mathf.Clamp(_floorY + halfH, minY, maxY);

        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void ClampTargetPosition()
    {
        float halfH = _targetOrthoSize;
        float halfW = _targetOrthoSize * _camera.aspect;

        float minX = _boundsCenter.x - _boundsWidth * 0.5f + halfW;
        float maxX = _boundsCenter.x + _boundsWidth * 0.5f - halfW;
        float minY = _boundsCenter.y - _boundsHeight * 0.5f + halfH;
        float maxY = _boundsCenter.y + _boundsHeight * 0.5f - halfH;

        _targetPosition.x = minX > maxX ? _boundsCenter.x : Mathf.Clamp(_targetPosition.x, minX, maxX);
        _targetPosition.y = minY > maxY ? _boundsCenter.y : Mathf.Clamp(_floorY + halfH, minY, maxY);
    }

    private float ClampOrthoSize(float size)
    {
        float maxFromHeight = _boundsHeight * 0.5f;
        float maxFromWidth = _boundsWidth * 0.5f / _camera.aspect;

        float min = Mathf.Min(maxFromWidth, maxFromHeight);
        min = Mathf.Min(_maxZoom, min);

        float effectiveMax = min;
        return Mathf.Clamp(size, _minZoom, effectiveMax);
    }

    public void Shake(float duration, float strength)
    {
        enabled = false;
        DOTween.Shake(() => _camera.transform.position, (x) =>
        {
            _camera.transform.position = x;

        }, duration, Vector3.right * strength)
        .SetLink(_camera.gameObject, LinkBehaviour.KillOnDestroy)
        .OnComplete(Enable);
    }

    private void Enable()
    {
        enabled = true;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(_boundsCenter.x, _boundsCenter.y, 0f),
            new Vector3(_boundsWidth, _boundsHeight, 0f)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(_boundsCenter.x - _boundsWidth * 0.5f, _floorY, 0f),
            new Vector3(_boundsCenter.x + _boundsWidth * 0.5f, _floorY, 0f)
        );
    }
#endif
}