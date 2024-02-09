using UnityEngine;
using ReactUnity;
using ReactUnity.UGUI;
using ReactUnity.UIToolkit;

namespace Kukumberman.ReactUnity.Extra.Demo
{
    [ReactInjectGlobal(kInjectedName)]
    public sealed class ReactAttributesDemo : MonoBehaviour
    {
        private const string kInjectedName = "MyComponent";

        #region ReactInjectGlobal - fields
        [ReactInjectGlobal("foo")]
        private readonly int _foo = 42;

        [ReactInjectGlobal("bar")]
        private readonly float _bar = 123.456f;

        // same name to demonstrate log warning
        [ReactInjectGlobal("bar")]
        private readonly bool _baz = true;
        #endregion

        [ReactBeforeStart(Order = -1)]
        private void LogGlobal(ReactRendererBase renderer)
        {
            var component = renderer.Globals[kInjectedName] as ReactAttributesDemo;
            Debug.Log(component);
            renderer.Context.Script.ExecuteScript(
                @$"
console.log('Beginning of <b>ReactAttributesDemo.LogGlobal</b>')
console.log(Globals.{kInjectedName})
console.log(Globals.foo)
console.log(Globals.bar)
console.log('Ending of <b>ReactAttributesDemo.LogGlobal</b>')
"
            );
        }

        #region BeforeStart - type mismatch
        // one of this methods would not be called due to type mismatch
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
        #endregion

        #region BeforeStart
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
        #endregion

        #region AfterStart
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
        #endregion
    }
}
