using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace junpro_mania_mantap.Helpers
{
    public class ViewModelViewSelector : DataTemplateSelector
    {
        private static readonly Assembly ViewsAssembly = Assembly.GetExecutingAssembly();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return base.SelectTemplate(null!, container);

            var viewModelType = item.GetType();
            var viewType = FindViewTypeForViewModel(viewModelType);
            if (viewType == null) return base.SelectTemplate(item, container);

            var frameworkElement = container as FrameworkElement;
            if (frameworkElement == null) return base.SelectTemplate(item, container);

            // Buat DataTemplate
            var template = new DataTemplate
            {
                DataType = viewModelType
            };

            var factory = new FrameworkElementFactory(viewType);
            factory.SetBinding(FrameworkElement.DataContextProperty, new System.Windows.Data.Binding());

            template.VisualTree = factory;
            return template;
        }

        private Type? FindViewTypeForViewModel(Type viewModelType)
        {
            // Asumsi: ViewModel di namespace *.ViewModels.* dan View di *.Views.*
            var viewNamespace = viewModelType.Namespace!.Replace(".ViewModels", ".Views");
            var viewTypeName = $"{viewNamespace}.{viewModelType.Name.Replace("ViewModel", "View")}";

            // Cari type langsung sesuai nama dan namespace
            var viewType = ViewsAssembly.GetType(viewTypeName);
            if (viewType != null) return viewType;

            // Jika tidak ketemu, cari di seluruh assembly dengan nama sama
            return ViewsAssembly.GetTypes()
                .FirstOrDefault(t =>
                    t.IsSubclassOf(typeof(FrameworkElement)) &&
                    t.Name == viewModelType.Name.Replace("ViewModel", "View") &&
                    t.Namespace != null &&
                    t.Namespace.Contains("Views"));
        }
    }
}
