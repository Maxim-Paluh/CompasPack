using System.Windows;

using CompasPack.Model.Enum;

namespace CompasPack.Helper.Service
{
    public interface IMessageDialogService
    {
        MessageDialogResultEnum ShowOkCancelDialog(string text, string title);
        MessageDialogResultEnum ShowYesNoDialog(string text, string title);
        void ShowInfoDialog(string text, string header);
    }
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResultEnum ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
              ? MessageDialogResultEnum.OK
              : MessageDialogResultEnum.Cancel;
        }
        
        public MessageDialogResultEnum ShowYesNoDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes
              ? MessageDialogResultEnum.Yes
              : MessageDialogResultEnum.No;
        }

        public void ShowInfoDialog(string text, string header)
        {
            MessageBox.Show(text, header);
        }
    }

}
