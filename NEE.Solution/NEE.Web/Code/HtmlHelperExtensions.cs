using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NEE.Web
{
    public static partial class HtmlHelperExtensions
    {
        public static bool IsDEBUG(this HtmlHelper helper)
        {
            return Code.NEE_Environment.IsDEBUG();
        }
        public static bool IsStaging(this HtmlHelper helper)
        {
            return Code.NEE_Environment.IsStaging();
        }
        public static bool IsAdmin(this HtmlHelper helper)
        {
            return Code.NEE_Environment.IsAdmin();
        }
        public static bool IsAdminEnabled(this HtmlHelper helper)
        {
            return Code.NEE_Environment.IsAdminEnabled();
        }

        public static MvcHtmlString WatermarkFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var watermark = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Watermark;
            var htmlEncoded = HttpUtility.HtmlEncode(watermark);
            return new MvcHtmlString(htmlEncoded);
        }

        public static MvcHtmlString EnvironmentInfo(this HtmlHelper helper)
        {
            string description = "Πανελλαδική εφαρμογή";

            string environmentDescription = string.Empty;
            var environmentTag = System.Configuration.ConfigurationManager.AppSettings["AppInsightsEnvironmentTag"];
            if (environmentTag == "Dev")
            {
                description = $"{description} - Δοκιμαστικό περιβάλλον";
            }
            else if (environmentTag == "Production")
            {
                //description = $"{description} - Παραγωγικό περιβάλλον";
            }
            else if (environmentTag == "Staging")
            {
                description = $"{description} - Περιβάλλον staging";
            }
            else if (environmentTag == "Debug")
            {
                description = $"{description} - Περιβάλλον ανάπτυξης";
            }

            return MvcHtmlString.Create(description);
        }

        public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
        {
            var concatKeys = new string[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
            RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (var item in htmlAttributes)
            {
                if (concatKeys.Contains(item.Key))
                {
                    defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null)
                        ? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
                else
                {
                    defaultHtmlAttributes[item.Key] = item.Value;
                }
            }

            return defaultHtmlAttributes;
        }

        public static string ConnectionStringInfo(this HtmlHelper helper)
        {
            var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["NEEDataConnection"].ConnectionString;
            var schema = DefaultDbSchemaHelper.ResolveDbSchema("(auto)", conStr);
            return schema;
        }

        public static JsonResult JsonValidation(this ModelStateDictionary state, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            var result = new JsonResult
            {
                Data = new
                {
                    success = false,
                    Tag = "ValidationError",
                    State = from e in state
                            where e.Value.Errors.Count > 0
                            select new
                            {
                                Name = e.Key,
                                Errors = (e.Value.Errors.Select(x => x.ErrorMessage)
                                    .Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message)))
                                    .Where(x => !String.IsNullOrEmpty(x)).Select(x => x)
                            }
                }
            };
            result.JsonRequestBehavior = behavior;
            return result;
        }
    }
}