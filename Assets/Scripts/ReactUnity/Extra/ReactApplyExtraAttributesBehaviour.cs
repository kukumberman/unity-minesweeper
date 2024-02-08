using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ReactUnity;

namespace Kukumberman.ReactUnity.Extra
{
    [DefaultExecutionOrder(-1000)]
    public sealed class ReactApplyExtraAttributesBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ReactRendererBase _renderer;

        [SerializeField]
        private bool _verboseLogging;

        private object[] _argsArrayWithTarget;

        private void Awake()
        {
            _argsArrayWithTarget = new object[] { _renderer };

            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                RegisterComponent(monoBehaviours[i]);
                RegisterGlobalEntry(monoBehaviours[i]);
            }
        }

        private void RegisterComponent(MonoBehaviour component)
        {
            var componentType = component.GetType();

            var methods = componentType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes().OfType<ReactCallbackAttribute>().Any())
                .ToArray();

            var beforeStart = new List<MethodInfo>();
            var afterStart = new List<MethodInfo>();

            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                if (!IsValidMethod(method))
                {
                    if (_verboseLogging)
                    {
                        Debug.LogWarning(
                            string.Format(
                                "Method <b>{0}.{1}</b> is ignored due to type mismatch",
                                method.DeclaringType.Name,
                                method.Name
                            )
                        );
                    }

                    continue;
                }

                var beforeStartAttribute = method.GetCustomAttribute<ReactBeforeStartAttribute>();

                if (beforeStartAttribute != null)
                {
                    beforeStart.Add(method);
                }

                var afterStartAttribute = method.GetCustomAttribute<ReactAfterStartAttribute>();

                if (afterStartAttribute != null)
                {
                    afterStart.Add(method);
                }
            }

            beforeStart.Sort(SortByCallbackOrder);
            afterStart.Sort(SortByCallbackOrder);

            if (beforeStart.Count > 0)
            {
                _renderer.AdvancedOptions.BeforeStart.AddListener(() =>
                {
                    CallMethods(component, beforeStart);
                });
            }

            if (afterStart.Count > 0)
            {
                _renderer.AdvancedOptions.AfterStart.AddListener(() =>
                {
                    CallMethods(component, afterStart);
                });
            }
        }

        private bool IsValidMethod(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                return true;
            }

            if (
                parameters.Length == 1
                && parameters[0].ParameterType.IsAssignableFrom(_renderer.GetType())
            )
            {
                return true;
            }

            return false;
        }

        private void CallMethods(MonoBehaviour target, List<MethodInfo> methods)
        {
            for (int i = 0; i < methods.Count; i++)
            {
                CallMethod(target, methods[i]);
            }
        }

        private void CallMethod(MonoBehaviour target, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                methodInfo.Invoke(target, null);
            }
            else if (parameters.Length == 1)
            {
                methodInfo.Invoke(target, _argsArrayWithTarget);
            }

            if (_verboseLogging)
            {
                Debug.Log(
                    string.Format(
                        "Method <b>{0}.{1}</b> has been called",
                        methodInfo.DeclaringType.Name,
                        methodInfo.Name
                    )
                );
            }
        }

        private static int SortByCallbackOrder(MethodInfo lhs, MethodInfo rhs)
        {
            var lhsAttribute = lhs.GetCustomAttribute<ReactCallbackAttribute>();
            var rhsAttribute = rhs.GetCustomAttribute<ReactCallbackAttribute>();
            return lhsAttribute.Order - rhsAttribute.Order;
        }

        private void RegisterGlobalEntry(MonoBehaviour component)
        {
            var componentType = component.GetType();
            var attribute = componentType.GetCustomAttribute<ReactInjectGlobalAttribute>();

            if (attribute != null)
            {
                if (!_renderer.Globals.ContainsKey(attribute.Name))
                {
                    _renderer.Globals.Add(attribute.Name, component);
                }
                else
                {
                    if (_verboseLogging)
                    {
                        var existing = _renderer.Globals[attribute.Name];
                        var existingType = existing.GetType();

                        Debug.LogError(
                            string.Format(
                                "Record with key <b>{0}</b> already added as <b>{1}</b>, ignoring <b>{2}</b>",
                                attribute.Name,
                                existingType.Name,
                                componentType.Name
                            )
                        );
                    }
                }
            }
        }
    }
}
