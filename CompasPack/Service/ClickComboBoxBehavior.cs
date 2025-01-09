using CompasPack.Settings;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CompasPack.Service
{
    public class ClickComboBoxBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ClickComboBoxBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                // тут доводиться перевіряти який саме елемент викликав подію, щоб оптимізувати код використано саме такий метод де весь клас ClickComboBoxBehavior строрений лише для одного ComboBoxItem
                // якщо подію викликав ComboBoxItem то його DataContext повинно бути UserPreset, якщо це сам ComboBox то там DataContext буде ProgramsViewModel
                if (e.OriginalSource is FrameworkElement element && element.DataContext is UserPreset)
                {
                    Command.Execute(null);
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                Command.Execute(null);
            }
        }

    }
}
