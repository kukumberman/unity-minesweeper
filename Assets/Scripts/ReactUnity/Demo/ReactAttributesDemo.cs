using UnityEngine;
using ReactUnity;
using ReactUnity.UGUI;
using ReactUnity.UIToolkit;
using Kukumberman.ReactUnity.Extra;

namespace Kukumberman.ReactUnity.Extra.Demo
{
    [ReactInjectGlobal(Name = kInjectedName)]
    public sealed class ReactAttributesDemo : MonoBehaviour
    {
        private const string kInjectedName = "MyComponent";

        [ReactBeforeStart(Order = -1)]
        private void LogGlobal(ReactRendererBase renderer)
        {
            var component = renderer.Globals[kInjectedName] as ReactAttributesDemo;
            Debug.Log(component);
        }

        [ReactBeforeStart(Order = 0)]
        private void ReactRendererUIToolkit(ReactRendererUIToolkit renderer)
        {
            Debug.Log($"ReactRendererUIToolkit");
        }

        [ReactBeforeStart(Order = 1)]
        private void ReactRendererUGUI(ReactRendererUGUI renderer)
        {
            Debug.Log($"ReactRendererUGUI");
        }

        [ReactBeforeStart(Order = 50)]
        private void BeforeStart()
        {
            Debug.Log("BeforeStart");
        }

        [ReactBeforeStart(Order = 100)]
        private void BeforeStartWithArg(ReactRendererBase renderer)
        {
            Debug.Log($"BeforeStartWithArg {renderer.GetType().Name}");
        }

        [ReactAfterStart(Order = 150)]
        private void AfterStart()
        {
            Debug.Log("AfterStart");
        }

        [ReactAfterStart(Order = 100)]
        private void AfterStartWithArg(ReactRendererBase renderer)
        {
            Debug.Log($"AfterStartWithArg {renderer.GetType().Name}");
        }
    }
}
