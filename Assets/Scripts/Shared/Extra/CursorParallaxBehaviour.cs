using UnityEngine;

namespace Kukumberman.Extra
{
    public sealed class CursorParallaxBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private Vector2 _magnitude;

        private Vector2 _mousePosition;
        private Vector2 _viewportPosition;

        private Vector2 _result;

        private void Update()
        {
            _mousePosition = Input.mousePosition;
            _viewportPosition = _camera.ScreenToViewportPoint(_mousePosition);

            _viewportPosition.x = Mathf.Clamp01(_viewportPosition.x);
            _viewportPosition.y = Mathf.Clamp01(_viewportPosition.y);

            _viewportPosition.x = _viewportPosition.x * 2 - 1;
            _viewportPosition.y = _viewportPosition.y * 2 - 1;

            _result = _viewportPosition;
            _result.Scale(_magnitude);

            _target.localPosition = _result;
        }
    }
}
