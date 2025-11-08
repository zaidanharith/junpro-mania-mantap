using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Category;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Product;
using BOZea.ViewModels.Payment;
using BOZea.ViewModels.Admin;
using BOZea.Views.Auth;
using BOZea.Views.Category;
using BOZea.Views.Dashboard;
using BOZea.Views.Product;
using BOZea.Views.Payment;
using BOZea.Views.Admin; 

namespace BOZea.Helpers
{
    public class ViewModelViewSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null)
            {
                // Mapping ViewModel ke View

                // Auth ViewModels
                if (item is LoginViewModel)
                {
                    return CreateTemplate(typeof(LoginView), element);
                }
                else if (item is RegisterViewModel)
                {
                    return CreateTemplate(typeof(RegisterView), element);
                }
                else if (item is ProfileViewModel)
                {
                    return CreateTemplate(typeof(ProfileView), element);
                }
                else if (item is EditProfileViewModel)
                {
                    return CreateTemplate(typeof(EditProfileView), element);
                }

                // Dashboard ViewModels
                else if (item is DashboardViewModel)
                {
                    return CreateTemplate(typeof(DashboardView), element);
                }
                else if (item is DashboardAdminViewModel)
                {
                    return CreateTemplate(typeof(BOZea.Views.Admin.DashboardAdminView), element); // ✅ Hanya ini yang perlu fully qualified
                }

                // Category ViewModels
                else if (item is CategoryDetailViewModel)
                {
                    return CreateTemplate(typeof(CategoryDetailView), element);
                }

                // Product ViewModels
                else if (item is ProductDetailViewModel)
                {
                    return CreateTemplate(typeof(ProductDetailView), element);
                }
                else if (item is CreateProductViewModel)
                {
                    return CreateTemplate(typeof(CreateProductView), element);
                }

                // Payment ViewModels
                else if (item is PaymentViewModel)
                {
                    return CreateTemplate(typeof(PaymentView), element);
                }
                else if (item is ProductManagementViewModel)
                {
                    return CreateTemplate(typeof(ProductManagementView), element); // ✅ Bisa pakai short name karena tidak ada konflik
                }
            }

            // Fallback: log error instead of showing MessageBox to avoid Dispatcher crash
            if (item != null)
            {
                Console.WriteLine($"[ViewModelViewSelector] No view mapping found for ViewModel: {item.GetType().Name}");
            }

            return base.SelectTemplate(item, container);
        }

        private DataTemplate CreateTemplate(Type viewType, FrameworkElement element)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(viewType);
            template.VisualTree = factory;
            return template;
        }
    }
}
