using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilesExplorer.Converters
{
    /// <summary>
    /// DataTemplateSelector which selects template using the item type name
    /// </summary>
    public class ImplicitDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets a dictionary which contains the template for each type name
        /// </summary>
        public Dictionary<string, DataTemplate> Templates { get; } = new Dictionary<string, DataTemplate>(StringComparer.OrdinalIgnoreCase);

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            // Check if the dictionary contains a template for the item type name
            if (Templates.TryGetValue(item.GetType().Name, out DataTemplate template))
            {
                return template;
            }

            // Fallback to the default implementation
            return base.SelectTemplateCore(item, container);
        }

    }
}
