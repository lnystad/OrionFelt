using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.WpfBase
{
    using System.ComponentModel;
    using System.Linq.Expressions;

    public class ChangeTracker : IChangeTracking, INotifyPropertyChanged
    {
        private readonly List<string> m_excludedProperties = new List<string> { "IsChanged" };
        private bool m_trackChanges;
        private INotifyPropertyChanged m_trackedObject;

        public ChangeTracker()
        {
        }

        public ChangeTracker(INotifyPropertyChanged trackedObject)
        {
            TrackedObject = trackedObject;
        }

        public virtual void AcceptChanges()
        {
            IsChanged = false;
        }

        private bool m_isChanged;
        public bool IsChanged
        {
            get { return m_isChanged; }
            set
            {
                if (value != m_isChanged)
                {
                    m_isChanged = value;
                    OnPropertyChanged("IsChanged");
                }
            }
        }

        public INotifyPropertyChanged TrackedObject
        {
            get { return m_trackedObject; }

            set
            {
                if (!Equals(value, m_trackedObject))
                {
                    SubscribeEvent(false);

                    m_trackedObject = value;

                    SubscribeEvent(m_trackChanges);
                }
            }
        }

        public void ExcludeProperty(string propertyName)
        {
            m_excludedProperties.Add(propertyName);
        }

        public void ExcludeProperty<T>(Expression<Func<T>> propertyDelegate)
        {
            ExcludeProperty(GetPropertyName(propertyDelegate));
        }

        public bool TrackChanges
        {
            get { return m_trackChanges; }
            set
            {
                if (value != m_trackChanges)
                {
                    m_trackChanges = value;

                    SubscribeEvent(value);
                }
            }
        }

        private void SubscribeEvent(bool trackChanges)
        {
            if (m_trackedObject == null)
            {
                return;
            }

            if (trackChanges)
            {
                m_trackedObject.PropertyChanged += TrackedObject_OnPropertyChanged;
            }
            else
            {
                m_trackedObject.PropertyChanged -= TrackedObject_OnPropertyChanged;
            }
        }

        private void TrackedObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null && !m_excludedProperties.Contains(e.PropertyName))
            {
                IsChanged = true;
            }
        }

        private static string GetPropertyName<T>(Expression<Func<T>> property)
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
