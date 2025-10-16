using System.Windows;

namespace CompasPack.Helper.Converter
{
    public sealed class BooleanToVisibilityConverter : BooleanConverterBase<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
