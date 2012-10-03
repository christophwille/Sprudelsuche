using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SprudelSuche.ThirdParty.U2U
{
    // 
    // http://blogs.u2u.be/diederik/post/2012/03/07/Databinding-to-the-VariableSizedWrapGrid-in-Windows-8-Metro.aspx
    //
    // but also see: http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/966aa897-1413-46f0-bef7-663de36f9423/
    // 
    public class VariableGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var viewModel = item as IResizable;

            if (null == viewModel)
            {
                base.PrepareContainerForItemOverride(element, item);
                return;
            }

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, viewModel.Width);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.Height);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
