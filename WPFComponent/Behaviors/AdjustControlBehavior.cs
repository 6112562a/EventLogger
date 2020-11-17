using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace WPFComponent.Behaviors
{
    public class AdjustControlBehavior:Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var frameworkElement = AssociatedObject as FrameworkElement;
            if (frameworkElement!=null)
            {
                frameworkElement.Loaded += frameworkElement_Loaded;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var frameworkElement = AssociatedObject as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Loaded -= frameworkElement_Loaded;
            }
        }

        void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            //---控件加载完调整属性---
            //OnBindingChanged(sender, this.Binding);
        }

        #region Properties


        public string Param
        {
            get { return (string)GetValue(ParamProperty); }
            set { SetValue(ParamProperty, value); }
        }

        /// <summary>
        /// 调整参数
        /// （eg:"v|h v|c"）
        /// </summary>
        public static readonly DependencyProperty ParamProperty =
            DependencyProperty.Register("Param", typeof(string), typeof(AdjustControlBehavior), new PropertyMetadata(string.Empty));




        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 对比的值
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(AdjustControlBehavior), new PropertyMetadata(string.Empty));



        public object Binding
        {
            get { return (object)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        /// <summary>
        /// 绑定值
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(object), typeof(AdjustControlBehavior), new PropertyMetadata(null,OnBindingChanged));

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as AdjustControlBehavior;
            if (behavior == null) return;
            var target = behavior.AssociatedObject;
            if (target == null) return;
            var param = behavior.Param.ToLower();
            var paramlist = param.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var singleParam in paramlist)
            {
                if (e.NewValue.ToString().ToLower() == behavior.Value.ToLower())
                {
                    ChangeProperty(target, singleParam);
                }
                else
                {
                    ChangeProperty(target, singleParam,false);
                    
                }
            }
        }

        private static void ChangeProperty(DependencyObject target,  string strParam,bool isPostive=true)
        {
            var targetType = target.GetType();
            switch (strParam)
            {
                case "v|c":
                {
                    var memberInfo = targetType.GetProperty("Visibility");
                    if (memberInfo == null) return;
                    memberInfo.SetValue(target, isPostive ? Visibility.Visible : Visibility.Collapsed, null);
                    break;
                }
                case "v|h":
                {
                    var memberInfo = targetType.GetProperty("Visibility");
                    if (memberInfo == null) return;
                    memberInfo.SetValue(target, isPostive ? Visibility.Visible : Visibility.Hidden, null);
                    break;
                }
                case "c|v":
                {
                    var memberInfo = targetType.GetProperty("Visibility");
                    if (memberInfo == null) return;
                    memberInfo.SetValue(target, isPostive ? Visibility.Collapsed : Visibility.Visible, null);
                    break;
                }
                case "h|v":
                {
                    var memberInfo = targetType.GetProperty("Visibility");
                    if (memberInfo == null) return;
                    memberInfo.SetValue(target, isPostive ? Visibility.Hidden : Visibility.Visible, null);
                    break;
                }
            }
        }
        #endregion
    }
}
