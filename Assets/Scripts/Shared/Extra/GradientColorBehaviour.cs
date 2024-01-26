using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Kukumberman.Extra
{
    public sealed class GradientColorBehaviour : MonoBehaviour
    {
        public UnityEvent<Color> OnColorChanged;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _updateCountPerSecond;

        private float _progress01;
        private Color _color;

        private Coroutine _coroutine;

        public Color CurrentColor => _color;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(MyCoroutine());
        }

        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void Update()
        {
            _progress01 += _speed * Time.deltaTime;
            _progress01 %= 1;
        }

        private IEnumerator MyCoroutine()
        {
            while (true)
            {
                _color = Color.HSVToRGB(_progress01, 1, 1);

                OnColorChanged.Invoke(_color);

                yield return new WaitForSeconds(1f / _updateCountPerSecond);
            }
        }
    }
}
