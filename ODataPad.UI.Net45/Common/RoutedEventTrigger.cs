﻿using System;
using System.Windows;
using System.Windows.Interactivity;

namespace ODataPad.UI.Net45.Common
{
    public class RoutedEventTrigger : EventTriggerBase<DependencyObject>
    {
        RoutedEvent _routedEvent;

        public RoutedEvent RoutedEvent
        {
            get { return _routedEvent; }
            set { _routedEvent = value; }
        }

        protected override void OnAttached()
        {
            var behavior = base.AssociatedObject as Behavior;
            var associatedElement = base.AssociatedObject as FrameworkElement;

            if (behavior != null)
            {
                associatedElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;
            }
            if (associatedElement == null)
            {
                throw new ArgumentException("Routed Event trigger can only be associated to framework elements");
            }
            if (RoutedEvent != null)
            {
                associatedElement.AddHandler(RoutedEvent, new RoutedEventHandler(this.OnRoutedEvent));
            }
        }
        void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            base.OnEvent(args);
        }
        protected override string GetEventName()
        {
            return RoutedEvent.Name;
        }
    }
}