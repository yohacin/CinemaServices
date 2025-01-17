
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace wDualAssistence.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            if (controller != null) controller = controller.ToLower();
            if (action != null) action = action.ToLower();

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static HtmlString ToJson(this IHtmlHelper helper, object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JavaScriptDateTimeConverter());
            return (HtmlString)helper.Raw(JsonConvert.SerializeObject(obj, settings));
        }

        public static string Autofocus(this IHtmlHelper htmlHelper, bool valor)
        {
            return valor ? "autofocus" : "";
        }
        //public static HtmlString HelloWorldString(this IHtmlHelper htmlHelper, object obj)
        //   =>  (HtmlString)htmlHelper.Raw(JsonConvert.SerializeObject(obj));
    }

}
