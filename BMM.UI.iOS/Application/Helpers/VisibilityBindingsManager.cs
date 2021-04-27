using System;
using System.Collections.Generic;
using UIKit;

namespace BMM.UI.iOS.Helpers
{
    public class VisibilityBindingsManager<T>
    {
        private readonly IDictionary<UIViewVisibilityController, Func<T, bool>> _bindings =
            new Dictionary<UIViewVisibilityController, Func<T, bool>>();

        public void AddBinding(UIView view, Func<T, bool> isVisible)
        {
            _bindings.Add(new UIViewVisibilityController(view), isVisible);
        }

        public void Update(T data)
        {
            foreach (var binding in _bindings)
            {
                var visibilityController = binding.Key;
                var isVisibleFunc = binding.Value;

                visibilityController.ViewIsVisible = isVisibleFunc(data);
            }
        }
    }
}