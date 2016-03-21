using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace To_Do_List_2
{
    /// <summary>
    /// Helper for set right width of ListView
    /// </summary>
    public static class UIHelper
    {
        public static T FindChildByName<T>(FrameworkElement parentControl, string name) where T : FrameworkElement
        {
            T child = default(T);
            if (!string.IsNullOrEmpty(name))
            {
                List<T> similarChildren = FindChildren<T>(parentControl);
                if (similarChildren != null && similarChildren.Count > 0)
                {
                    child = (from c in similarChildren where c.Name == name select c).FirstOrDefault();
                }
            }
            return child;
        }
        public static List<T> FindChildrenByName<T>(FrameworkElement parentControl, string name) where T : FrameworkElement
        {
            List<T> children = default(List<T>);
            if (!string.IsNullOrEmpty(name))
            {
                List<T> similarChildren = FindChildren<T>(parentControl);
                if (similarChildren != null && similarChildren.Count > 0)
                {
                    children = (from c in similarChildren where c.Name == name select c).ToList();
                }
            }
            return children;
        }
        public static List<T> FindChildren<T>(FrameworkElement parentControl) where T : FrameworkElement
        {
            List<T> foundChildren = null;
            if (parentControl != null)
            {
                List<FrameworkElement> children = null;
                GetChildren(parentControl, ref children);
                if (children != null && children.Count > 0)
                {
                    foundChildren = (from c in children where c is T select c as T).ToList();
                }
            }
            return foundChildren;
        }
        public static void GetChildren(FrameworkElement parentControl, ref List<FrameworkElement> children)
        {
            if (parentControl != null)
            {
                if (VisualTreeHelper.GetChildrenCount(parentControl) > 0)
                {
                    if (children == null)
                    {
                        children = new List<FrameworkElement>();
                    }
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentControl); i++)
                    {
                        FrameworkElement element = VisualTreeHelper.GetChild(parentControl, i) as FrameworkElement;
                        children.Add(element);
                        if (VisualTreeHelper.GetChildrenCount(parentControl) > 0)
                        {
                            GetChildren(element, ref children);
                        }
                    }
                }
            }
        }
    }
}
