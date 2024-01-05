using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace CompasPack.View
{
    [TemplateVisualState(Name = "Large", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Small", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class ProgressRing : Control
    {
        private static readonly DependencyPropertyKey BindableWidthPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BindableWidth),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double), OnBindableWidthPropertyChanged));
        public static readonly DependencyProperty BindableWidthProperty = BindableWidthPropertyKey.DependencyProperty;
        public double BindableWidth
        {
            get => (double)this.GetValue(BindableWidthProperty);
            protected set => this.SetValue(BindableWidthPropertyKey, value);
        }
        public static readonly DependencyProperty IsActiveProperty
            = DependencyProperty.Register(nameof(IsActive),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnIsActivePropertyChanged));
        public bool IsActive
        {
            get => (bool)this.GetValue(IsActiveProperty);
            set => this.SetValue(IsActiveProperty, BooleanBoxes.Box(value));
        }
        public static readonly DependencyProperty IsLargeProperty
            = DependencyProperty.Register(nameof(IsLarge),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnIsLargePropertyChanged));
        public bool IsLarge
        {
            get => (bool)this.GetValue(IsLargeProperty);
            set => this.SetValue(IsLargeProperty, BooleanBoxes.Box(value));
        }
        private static readonly DependencyPropertyKey MaxSideLengthPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(MaxSideLength),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MaxSideLengthProperty = MaxSideLengthPropertyKey.DependencyProperty;
        public double MaxSideLength
        {
            get => (double)GetValue(MaxSideLengthProperty);
            protected set => SetValue(MaxSideLengthPropertyKey, value);
        }
        private static readonly DependencyPropertyKey EllipseDiameterPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(EllipseDiameter),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double)));
        public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;
        public double EllipseDiameter
        {
            get => (double)this.GetValue(EllipseDiameterProperty);
            protected set => this.SetValue(EllipseDiameterPropertyKey, value);
        }
        private static readonly DependencyPropertyKey EllipseOffsetPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(EllipseOffset),
                                                  typeof(Thickness),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(Thickness)));
        public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;
        public Thickness EllipseOffset
        {
            get => (Thickness)GetValue(EllipseOffsetProperty);
            protected set => SetValue(EllipseOffsetPropertyKey, value);
        }
        public static readonly DependencyProperty EllipseDiameterScaleProperty
            = DependencyProperty.Register(nameof(EllipseDiameterScale),
                                          typeof(double),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(1D));
        public double EllipseDiameterScale
        {
            get => (double)GetValue(EllipseDiameterScaleProperty);
            set => SetValue(EllipseDiameterScaleProperty, value);
        }
        private List<Action>? deferredActions = new List<Action>();
        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
            VisibilityProperty.OverrideMetadata(
                typeof(ProgressRing),
                new FrameworkPropertyMetadata(
                    (ringObject, e) =>
                    {
                        if (e.NewValue != e.OldValue)
                        {
                            var ring = ringObject as ProgressRing;

                            ring?.SetCurrentValue(IsActiveProperty, BooleanBoxes.Box((Visibility)e.NewValue == Visibility.Visible));
                        }
                    }));
        }
        public ProgressRing()
        {
            SizeChanged += OnSizeChanged;
        }
        private static void OnBindableWidthPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is ProgressRing ring))
                return;

            var action = new Action(
                () =>
                {
                    ring.SetEllipseDiameter((double)dependencyPropertyChangedEventArgs.NewValue);
                    ring.SetEllipseOffset((double)dependencyPropertyChangedEventArgs.NewValue);
                    ring.SetMaxSideLength((double)dependencyPropertyChangedEventArgs.NewValue);
                });

            if (ring.deferredActions != null)
                ring.deferredActions.Add(action);
            else
                action();
            
        }
        private void SetMaxSideLength(double width)
        {
            SetValue(MaxSideLengthPropertyKey, width <= 20d ? 20d : width);
        }
        private void SetEllipseDiameter(double width)
        {
            SetValue(EllipseDiameterPropertyKey, (width / 8) * this.EllipseDiameterScale);
        }
        private void SetEllipseOffset(double width)
        {
            SetValue(EllipseOffsetPropertyKey, new Thickness(0, width / 2, 0, 0));
        }
        private static void OnIsLargePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;
            ring?.UpdateLargeState();
        }
        private void UpdateLargeState()
        {
            Action action;
            if (this.IsLarge)
                action = () => VisualStateManager.GoToState(this, "Large", true);
            else
                action = () => VisualStateManager.GoToState(this, "Small", true);

            if (deferredActions != null)
                deferredActions.Add(action);
            else
                action();
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.SetValue(BindableWidthPropertyKey, ActualWidth);
        }
        private static void OnIsActivePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;
            ring?.UpdateActiveState();
        }
        private void UpdateActiveState()
        {
            Action action;
            if (IsActive)
                action = () => VisualStateManager.GoToState(this, "Active", true);        
            else
                action = () => VisualStateManager.GoToState(this, "Inactive", true);

            if (deferredActions != null)
                deferredActions.Add(action);   
            else
                action();
        }
        public override void OnApplyTemplate()
        {
            UpdateLargeState();
            UpdateActiveState();
            base.OnApplyTemplate();
            if (deferredActions != null)
            {
                foreach (var action in deferredActions)
                {
                    action();
                }
            }
            deferredActions = null;
        }
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ProgressRingAutomationPeer(this);
        }
    }

    public static class BooleanBoxes
    {
        public static readonly object TrueBox = true;
        public static readonly object FalseBox = false;
        public static object Box(bool value) => value ? TrueBox : FalseBox;
        public static object? Box(bool? value)
        {
            if (value.HasValue)
            {
                return value.Value ? TrueBox : FalseBox;
            }
            return null;
        }
    }

    public class ProgressRingAutomationPeer : FrameworkElementAutomationPeer
    {
        public ProgressRingAutomationPeer([NotNull] ProgressRing owner)
            : base(owner) { }
        protected override string GetClassNameCore()
        {
            return nameof(ProgressRing);
        }
        protected override string GetNameCore()
        {
            string? nameCore = base.GetNameCore();

            if (this.Owner is ProgressRing { IsActive: true })
            {
                return nameof(ProgressRing.IsActive) + nameCore;
            }
            return nameCore!;
        }
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ProgressBar;
        }
    }

}
