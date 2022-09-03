using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CompasPac.View.Service
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        MessageDialogResult ShowYesNoDialog(string text, string title);
        void ShowInfoDialog(string text, string header);
    }

    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
              ? MessageDialogResult.OK
              : MessageDialogResult.Cancel;
        }
        
        public MessageDialogResult ShowYesNoDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes
              ? MessageDialogResult.Yes
              : MessageDialogResult.No;
        }

        public void ShowInfoDialog(string text, string header)
        {
            MessageBox.Show(text, header);
        }
    }

    public enum MessageDialogResult
    {
        OK,
        Cancel,
        Yes,
        No
    }
}
