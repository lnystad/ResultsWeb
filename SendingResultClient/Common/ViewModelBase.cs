using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingResultClient.Common
{
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows;

    using FileUploaderService.Diagnosis;

    public abstract class ViewModelBase : INotifyPropertyChanged, IChangeTracking
    {
     
  
       public ChangeTracker ChangeTracker { get; private set; }

        public static bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

        protected ViewModelBase()
        {
             ChangeTracker = new ChangeTracker(this);
            
            ChangeTracker.PropertyChanged += ChangeTracker_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //public DataModelState State
        //{
        //    get { return m_state; }
        //    set { SetProperty(ref m_state, value, () => State); }
        //}

        //public IErrorMessage ErrorMessage
        //{
        //    get { return m_errorMessage; }
        //    set { SetProperty(ref m_errorMessage, value, () => ErrorMessage); }
        //}

        //public IBusyMessage BusyMessage
        //{
        //    get { return m_busyMessage; }
        //    set { SetProperty(ref m_busyMessage, value, () => BusyMessage); }
        //}

        //public IBaseDialogViewModel Dialog
        //{
        //    get { return m_dialog; }
        //    set { SetProperty(ref m_dialog, value, () => Dialog); }
        //}

        protected void SetErrorMessageAndLogException(string solution, Exception exception)
        {
            if (solution == null)
            {
                throw new ArgumentNullException("solution");
            }

            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

          //  BusyMessage = null;
            var exceptionToUse = exception is AggregateException
                                ? exception.InnerException ?? exception
                                : exception;
            Log.Error(exceptionToUse, solution);
            //var desktopException = new DesktopException(string.Empty, string.Empty, exceptionToUse.Message, exceptionToUse);
            //ErrorMessage = new ErrorMessage(solution, desktopException);
            //State = DataModelState.Active;
        }

        protected bool SetProperty<T>(ref T property, T value, string propertyName)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected bool SetProperty<T>(ref T property, T value, Expression<Func<T>> propertyDelegate)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            OnPropertyChanged(propertyDelegate);

            return true;
        }

        protected bool SetProperty<TItem, T>(TItem item, Expression<Func<TItem, T>> itemProperty, T value, Expression<Func<T>> property) where TItem : class
        {
            if (item == null)
            {
                return false;
            }

            var prop = GetPropertyInfo(itemProperty);

            var oldValue = prop.GetValue(item, null);

            if (Equals(value, oldValue))
            {
                return false;
            }

            prop.SetValue(item, value, null);

            OnPropertyChanged(property);

            return true;
        }

        private static PropertyInfo GetPropertyInfo<TItem, T>(Expression<Func<TItem, T>> itemProperty) where TItem : class
        {
            var memberExpr = GetMemberExpression(itemProperty);
            return (PropertyInfo)memberExpr.Member;
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> property)
        {
            var memberExpr = GetMemberExpression(property);
            return memberExpr.Member.Name;
        }

        private static MemberExpression GetMemberExpression(LambdaExpression property)
        {
            var body = property.Body;

            var unaryExpr = body as UnaryExpression;
            if (unaryExpr != null)
            {
                return (MemberExpression)unaryExpr.Operand;
            }

            return (MemberExpression)body;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(GetPropertyName(property));
        }

        #region IChangeTracking members

        public virtual void AcceptChanges()
        {
            //ChangeTracker.AcceptChanges();
        }

        public virtual bool IsChanged
        {
            get { return ChangeTracker.IsChanged; }
            set { ChangeTracker.IsChanged = value; }
        }

        #endregion

        public void ChangeTracker_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "IsChanged")
            {
                OnPropertyChanged(() => IsChanged);
            }
        }
    }
}
