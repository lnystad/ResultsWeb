namespace WebResultsClient.Common
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    public class MbWindow : Window
    {
        public MbWindow()
        {
            Icon = ImageConversion.ToImageSource(Properties.Resources.Icon1);
            CheckChangesOnClose = true;
        }

        private bool m_checkChangesOnClose;
        public bool CheckChangesOnClose
        {
            get { return m_checkChangesOnClose; }
            set
            {
                if (value != m_checkChangesOnClose)
                {
                    if (value)
                    {
                        Closing += WindowClosingEvent;
                    }
                    else
                    {
                        Closing -= WindowClosingEvent;
                    }

                    m_checkChangesOnClose = value;
                }
            }
        }

        private bool m_closeConfirmed;

        private void WindowClosingEvent(object sender, CancelEventArgs e)
        {
            var userControl = Content as UserControl;

            if (m_closeConfirmed || userControl == null)
            {
                m_closeConfirmed = false;
                return;
            }

            if (CheckChangesOnClose && IsChanged(userControl))
            {
                // Ask for confirmation before closing
                const string DialogMessage = "There are unsaved changes.\n\nClose and discard changes?";

                var viewModel = userControl.DataContext as ViewModelBase;
                if (viewModel != null)
                {
                    //viewModel.Dialog = new ConfirmDialogViewModel(
                    //    Title,
                    //    DialogMessage,
                    //    yesCallback: () =>
                    //    {
                    //        viewModel.Dialog = null;
                    //        m_closeConfirmed = true;
                    //        Close();
                    //    },
                    //    noCallback: () => { viewModel.Dialog = null; });

                    //e.Cancel = true;
                }
                else
                {
                    if (MessageBox.Show(DialogMessage, Title, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private bool IsChanged(UserControl userControl)
        {
            var changeTracking = userControl.DataContext as IChangeTracking;
            if (changeTracking == null)
            {
                return false;
            }

            ForceDataValidation();

            return changeTracking.IsChanged;
        }

        private static void ForceDataValidation()
        {
            // Force update of bindings for text boxes. By default they are only updated when loosing focus.
            var textBox = Keyboard.FocusedElement as TextBox;

            if (textBox != null)
            {
                BindingExpression be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be != null && !textBox.IsReadOnly && textBox.IsEnabled)
                {
                    be.UpdateSource();
                }
            }
        }
    }
}
