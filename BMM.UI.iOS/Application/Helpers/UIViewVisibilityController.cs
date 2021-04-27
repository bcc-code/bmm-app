using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace BMM.UI.iOS.Helpers
{
    /// <summary>
    /// Uses the width constraints of the view to hide UIView components
    /// </summary>
    public class UIViewVisibilityController
    {
        private readonly UIView _view;

        private bool _viewIsVisible;

        private readonly nfloat _initialWidth;

        public UIViewVisibilityController(UIView view)
        {
            _view = view;

            //Assuming that all width constraints for the view are equal. Therefore taking the first one.
            var widthConstraint =
                WidthConstraints.First(constraint => constraint.FirstAttribute == NSLayoutAttribute.Width);

            _initialWidth = widthConstraint.Constant;
        }

        public bool ViewIsVisible
        {
            get { return _viewIsVisible; }
            set
            {
                _viewIsVisible = value;

                foreach (var constraint in WidthConstraints)
                {
                    constraint.Constant = ViewIsVisible ? _initialWidth : 0f;
                }
            }
        }

        // Enumerable as there may be multiple WidthConstraints if there are nested views.
        public IEnumerable<NSLayoutConstraint> WidthConstraints
        {
            get
            {
                return _view.Constraints
                    .Where(constraint => constraint.FirstAttribute == NSLayoutAttribute.Width);
            }
        }
    }
}