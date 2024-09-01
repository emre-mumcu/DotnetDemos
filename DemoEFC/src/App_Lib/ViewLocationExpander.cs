using Microsoft.AspNetCore.Mvc.Razor;

namespace src.App_Lib
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        // Add custom folders to default search locations for views.

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //{2} is area, {1} is controller,{0} is the action
            string[] locations = new string[] {
            "/Content/Partials/{0}" + RazorViewEngine.ViewExtension,
            "/Content/{0}" + RazorViewEngine.ViewExtension,
        };
            return locations.Union(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }
    }
}
