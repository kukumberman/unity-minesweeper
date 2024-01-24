using UnityEngine;

namespace Kukumberman.Extra
{
    public enum ObjectFitType
    {
        None,
        Contain,
        Cover,
    }

    public sealed class CameraObjectFitBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ObjectFitType _fitType;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float _padding;

        [SerializeField]
        private bool _runInUpdate;

        private void Start()
        {
            UpdateSize();
        }

        private void Update()
        {
            if (_runInUpdate)
            {
                UpdateSize();
            }
        }

        private void UpdateSize()
        {
            if (_fitType == ObjectFitType.Contain)
            {
                Contain();
            }
            else if (_fitType == ObjectFitType.Cover)
            {
                Cover();
            }
        }

        private void Contain()
        {
            var size = _spriteRenderer.bounds.size;
            var cameraAspect = _camera.aspect;
            var boundsAspect = size.x / size.y;

            if (cameraAspect > boundsAspect)
            {
                _camera.orthographicSize = size.y * 0.5f;
            }
            else
            {
                _camera.orthographicSize = size.y * 0.5f * (boundsAspect / cameraAspect);
            }
        }

        private void Cover()
        {
            var size = _spriteRenderer.bounds.size;
            var cameraAspect = _camera.aspect;
            var boundsAspect = size.x / size.y;

            if (cameraAspect > boundsAspect)
            {
                size.x -= _padding;
                _camera.orthographicSize = size.x * _camera.pixelHeight / _camera.pixelWidth * 0.5f;
            }
            else
            {
                size.y -= _padding;
                _camera.orthographicSize = size.y * 0.5f;
            }
        }
    }
}
