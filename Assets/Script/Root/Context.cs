namespace Project
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Context : MonoBehaviour
    {
#pragma warning disable IDE0090
        private readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();
#pragma warning restore IDE0090

        public virtual T Resolve<T>()
        {
            if (_bindings.TryGetValue(typeof(T), out object binding))
            {
                return (T)binding;
            }

            bool canBeInChildren = typeof(Component).IsAssignableFrom(typeof(T)) || typeof(T).IsInterface;
            if (!canBeInChildren)
            {
                return ResolveByParent<T>();
            }

            T[] children = GetComponentsInChildren<T>(true);
            if (children.Length == 0)
            {
                return ResolveByParent<T>();
            }

            if (children.Length > 1)
            {
                Debug.LogError($"Only 1 instance of {typeof(T).Name} is expected, but {children.Length} instances are found", this);
                return default;
            }

            T child = children[0];
            if (child == null || child.Equals(null))
            {
                Debug.LogError($"Instance of {typeof(T).Name} is null", this);
                return default;
            }

            _bindings[typeof(T)] = child;
            return child;
        }
        
        protected virtual void Bind<T>(T obj)
        {
            _bindings[typeof(T)] = obj;
        }

        private T ResolveByParent<T>()
        {
            if (transform.parent == null)
            {
                Debug.LogError($"No binding found for {typeof(T).Name}", this);
                return default;
            }

            Context parentContext = transform.parent.GetComponentInParent<Context>();
            if (parentContext == null)
            {
                Debug.LogError($"No binding found for {typeof(T).Name}", this);
                return default;
            }

            return parentContext.Resolve<T>();
        }
        
    }
}