using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MVVM;

namespace wms.Client.ViewModel
{
    public class AlarmConfirmationBoxViewModel
    {
        private bool _showOk = false;

        private Action _onCloseAction = null;

        private MessageBoxButton _messageBoxButton = MessageBoxButton.OK;

        public ICommand HitOK { get; private set; }

        public MessageBoxResult MessageBoxResult = MessageBoxResult.Cancel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="messageBoxButton">指定显示按钮</param>
        /// <param name="messageBoxImage">指定显示图标</param>
        /// <param name="closeAction">点击按钮时触发的方法</param>
        public AlarmConfirmationBoxViewModel(MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, Action onClose)
        {
            _messageBoxButton = messageBoxButton;
            _onCloseAction = onClose;
            HitOK = new SimpleCommand(a => { return _showOk; }, OnHitOK);
        }

        private void OnHitOK(object obj)
        {
            MessageBoxResult = MessageBoxResult.OK;
            _onCloseAction?.Invoke();
        }

        public void HitOkes()
        {
            MessageBoxResult = MessageBoxResult.OK;
            _onCloseAction?.Invoke();
        }

    }
}
