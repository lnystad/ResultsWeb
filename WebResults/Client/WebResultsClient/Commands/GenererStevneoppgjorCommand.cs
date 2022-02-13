using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WebResultsClient.Viewmodels;

namespace WebResultsClient.Commands
{
    public class GenererStevneoppgjorCommand : ICommand
    {
        private StevneoppgjorSelectionViewModel m_stevneoppgjorViewModel;

        public GenererStevneoppgjorCommand(StevneoppgjorSelectionViewModel stevneoppgjorSelectionViewModel)
        {
            m_stevneoppgjorViewModel = stevneoppgjorSelectionViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var filename = Path.Combine(m_stevneoppgjorViewModel.StevneDir, m_stevneoppgjorViewModel.StevneNavn, "Stevneoppgjør", "Stevneoppgjør-" + m_stevneoppgjorViewModel.StevneNavn + ".xml");

            if(File.Exists(filename))
            {

            }
            else
            {
                MessageBox.Show("Fant ikke fil for stevneoppgjør. Har du husket å generere stevneoppgjøret i Leon?");
            }
        }
    }
}
