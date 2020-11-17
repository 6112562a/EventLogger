using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace WPFComponent.MarkupExtension
{
    public class DataResource:Freezable
    {
        /// <summary>
        /// <see cref="DataResource"/> 的默认构造.
        /// </summary>
        public DataResource()
        {
            
        }

        public object Binding
        {
            get { return (object)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        /// <summary>
        /// 绑定的依赖属性
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(object), typeof(DataResource), new PropertyMetadata(null));


        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)Activator.CreateInstance(GetType());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFreezable"></param>
        protected sealed override void CloneCore(Freezable sourceFreezable)
        {
            base.CloneCore(sourceFreezable);
        }
    }

    public class SourceBinding : System.Windows.Markup.MarkupExtension
    {

        private DataResource _resource;

        public DataResource Resource
        {
            get { return _resource; }
            set
            {
                if (_resource != value)
                {
                    if (_resource!=null)
                    {
                        _resource.Changed -= ResourceChanged;
                    }
                    _resource = value;
                    if (_resource != null)
                    {
                        _resource.Changed += ResourceChanged;
                    }
                }
                _resource = value;
            }
        }
        /// <summary>
        /// 目标
        /// </summary>
        private object _targetObject;
        /// <summary>
        /// 目标属性
        /// </summary>
        private object _targetProperty;

        /// <summary>
        /// SourceBinding默认构造
        /// </summary>
        public SourceBinding()
        {
            
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            _targetObject = target.TargetObject;
            _targetProperty = target.TargetProperty;
            Debug.Assert(_targetProperty!=null||DesignerProperties.GetIsInDesignMode(new DependencyObject()));

            if (Resource.Binding == null && _targetProperty != null)
            {
                PropertyInfo propertyInfo = _targetProperty as PropertyInfo;
                if (propertyInfo != null)
                {
                    try
                    {
                        return Activator.CreateInstance(propertyInfo.PropertyType);
                    }
                    catch (MissingMethodException)
                    {
                        //无构造引起的异常
                    }
                }
                DependencyProperty dpProperty = _targetProperty as DependencyProperty;

                if (dpProperty != null)
                {
                    DependencyObject dpObj = (DependencyObject) _targetObject;
                    return dpObj.GetValue(dpProperty);
                }
            }
            return Resource.Binding;
        }

        #region Function
        /// <summary>
        /// 源变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceChanged(object sender, EventArgs e)
        {
            DataResource dataResource = (DataResource) sender;
            DependencyProperty dpProperty = _targetProperty as DependencyProperty;
            if (dpProperty != null)
            {
                DependencyObject dpObject = _targetObject as DependencyObject;
                object value = ConvertType(dataResource.Binding, dpProperty.PropertyType);
                if (dpObject != null) 
                    dpObject.SetValue(dpProperty,value);
            }
            else
            {
                PropertyInfo propertyInfo = _targetProperty as PropertyInfo;
                if (propertyInfo != null)
                {
                    object value = ConvertType(dataResource.Binding, propertyInfo.PropertyType);
                    propertyInfo.SetValue(_targetObject,value,new object[0]);
                }
            }
        }
        /// <summary>
        /// 转换目标Type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        private object ConvertType(object obj, Type toType)
        {
            try
            {
                return System.Convert.ChangeType(obj, toType);
            }
            catch (InvalidCastException)
            {
                return obj;
                throw;
            }
        }
        #endregion
    }
}
