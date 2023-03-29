using NEE.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace NEE.Web.Code
{
    public static class IdikaHtmlHelpersExtensions
    {
        public static IdikaHtmlBuilder<TModel> IdikaHtml<TModel>(this HtmlHelper<TModel> helper) => new IdikaHtmlBuilder<TModel>(helper);
    }

    public class IdikaHtmlBuilder<TModel> : IHtmlString
    {

        private readonly HtmlHelper<TModel> _helper;

        public IdikaHtmlBuilder(HtmlHelper<TModel> helper) { _helper = helper; }

        public IdikaFieldBuilder<TModel, TValue> FieldFor<TValue>(Expression<Func<TModel, TValue>> expression) => new IdikaFieldBuilder<TModel, TValue>(_helper, expression);
        public IdikaFieldBuilder<TModel, TValue> FieldFor<TValue>(Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items) => new IdikaFieldBuilder<TModel, TValue>(_helper, expression, items);

        public string InfoMessageIdFor<TValue>(Expression<Func<TModel, TValue>> expression) => this.InfoMessageId(_helper.IdFor(expression).ToString());
        public string InfoMessageId(string expression) => string.Format("{0}_InfoMessage", expression);

        public IdikaBooleanTableBuilder<TModel> BooleanTable() => new IdikaBooleanTableBuilder<TModel>(_helper);

        public string ToHtmlString() => null;
    }

    public class IdikaFieldBuilder<TModel, TValue> : IHtmlString
    {
        // Constants

        private const string Prefix_CssFormGroup = "idika-field-form-group form-group";
        private const string Prefix_CssLabel = "idika-field-label control-label";
        private const string Prefix_CssEditor = "idika-field-editor form-control";
        private const string Prefix_CssEditorFit = "idika-field-editor";
        private const string Prefix_CssValidationMessage = "idika-field-validation-message text-danger";
        private const string Prefix_CssInfoMessage = "idika-field-info-message collapse";
        private const string Prefix_CssReference = "idika-field-reference";

        private const bool Default_HasFormGroup = false;
        private const bool Default_HasLabel = true;
        private const bool Default_HasValidationMessage = true;
        private const bool Default_ReadOnly = false;
        private const bool Default_AllowCopyReference = true;
        private const bool Default_AutocompleteInput = false;
        private const bool Default_AutocompleteForm = false;


        private const string Default_CssFormGroup = "col-md-6";
        private const string Default_TitReference = "Τιμή αναφοράς";
        private const string Default_TitReferenceWithButton = "Τιμή αναφοράς, click στο βελάκι για μεταφορά τιμής στο πεδίο";

        // FormGroup
        private bool _hasFormGroup = Default_HasFormGroup;
        private string _cssFormGroup = null;

        // Label
        private bool _hasLabel = Default_HasLabel;
        private string _cssLabel = null;
        private string _msgLabel = null;

        // ValidationMessage
        private bool _hasValidationMessage = Default_HasValidationMessage;
        private string _cssValidationMessage = null;
        private string _msgValidationMessage = null;

        // InfoMessage
        private bool _hasInfoMessage = false;
        private string _cssInfoMessage = null;
        private string _msgInfoMessage = null;
        //PrefixIcon
        private bool _hasPrefixIcon = false;
        private string _cssPrefixIcon = null;

        // Reference
        private bool _hasReference = false;
        private string _cssReference = null;
        private string _msgReference = null;
        private bool _allowCopyReference = Default_AllowCopyReference;

        // Editor
        private Func<HtmlString> _editorFactory;
        private string cssEditor = null;
        private bool _readonly = Default_ReadOnly;
        private string _placeholderText = "";
        private string _additionalFieldRemark = "";
        private string _additionalFieldRemarkClass = "";
        private bool _fitWidth = false;

        private string _editorId = null;

        // System-Control-Fields
        private readonly HtmlHelper<TModel> _helper;
        private readonly Expression<Func<TModel, TValue>> _expression;

        public IdikaFieldBuilder(HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            _helper = helper;
            _expression = expression;

            _editorFactory = () => _helper.EditorFor(_expression, new { htmlAttributes = this.GetEditorHtmlAttributes() });
        }
        public IdikaFieldBuilder(HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items)
        {
            _helper = helper;
            _expression = expression;

            _editorFactory = () => _helper.DropDownListFor(_expression, items, this.GetEditorHtmlAttributes());
        }

        #region Support

        private string GetPlaceholder()
        {
            var ret = _helper.WatermarkFor(_expression)?.ToString();
            if (string.IsNullOrWhiteSpace(ret)) ret = _helper.DisplayNameFor(_expression)?.ToString();
            return ret;
        }

        private RouteValueDictionary GetEditorHtmlAttributes()
        {
            var ret = new RouteValueDictionary();
            ret["id"] = this.GetId();
            ret["class"] = (_fitWidth ? Prefix_CssEditorFit : Prefix_CssEditor) + " " + cssEditor;
            if (_readonly)
                ret["readonly"] = _readonly;
            else
            {
                if (string.IsNullOrEmpty(_placeholderText))
                {
                    ret["placeholder"] = this.GetPlaceholder();
                }
                else
                {
                    ret["placeholder"] = _placeholderText;
                }
            }


            ret["autocomplete"] = "nope"; // Add a value that does not correspond to a valid one *https://developer.mozilla.org/en-US/docs/Web/Security/Securing_your_site/Turning_off_form_autocompletion*

            return ret;
        }

        #endregion

        public IdikaFieldBuilder<TModel, TValue> ReadOnly(bool @readonly = true)
        {
            _readonly = @readonly;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> Placeholder(string @placeholderText = "")
        {
            _placeholderText = @placeholderText;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> AdditionalFieldRemark(string @additionalFieldRemark = "", string @class = "")
        {
            _additionalFieldRemark = @additionalFieldRemark;
            _additionalFieldRemarkClass = @class;
            return this;
        }


        public IdikaFieldBuilder<TModel, TValue> FormGroup(string @class = Default_CssFormGroup) => this.FormGroup(true, @class);
        public IdikaFieldBuilder<TModel, TValue> FormGroup(bool include, string @class = Default_CssFormGroup)
        {
            _hasFormGroup = include;
            _cssFormGroup = @class;
            return this;
        }


        public IdikaFieldBuilder<TModel, TValue> Label(bool include = true, string labelText = null, string @class = null)
        {
            _hasLabel = include;
            _cssLabel = @class;
            _msgLabel = labelText;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> ValidationMessage(bool include = true, string validationMessage = null, string @class = null)
        {
            _hasValidationMessage = include;
            _cssValidationMessage = @class;
            _msgValidationMessage = validationMessage;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> InfoMessage(Func<object, HelperResult> infoMessage, string @class = null) => this.InfoMessage(infoMessage(null).ToString(), @class);
        public IdikaFieldBuilder<TModel, TValue> InfoMessage(string infoMessage, string @class = null)
        {
            _hasInfoMessage = !string.IsNullOrWhiteSpace(infoMessage);
            _cssInfoMessage = @class;
            _msgInfoMessage = infoMessage;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> PrefixIcon(string @class)
        {
            _hasPrefixIcon = !string.IsNullOrWhiteSpace(@class);
            _cssPrefixIcon = @class;
            return this;
        }

        private string ReferenceToString(object value)
        {
            if (value == null) return null;
            if (value.GetType().IsEnum) return ((Enum)value).GetDisplayName();
            if (value.GetType() == typeof(DateTime)) return ((DateTime)value).ToString("dd/MM/yyyy");
            if (value.GetType() == typeof(decimal)) return ((decimal)value).ToString("0.00");
            return value.ToString();
        }

        public IdikaFieldBuilder<TModel, TValue> ReferenceFor(Expression<Func<TModel, TValue>> referenceExpression, bool allowCopyReference = Default_AllowCopyReference, string @class = null) => this.Reference(this.ReferenceToString(referenceExpression.Compile()(_helper.ViewData.Model)), allowCopyReference, @class);
        public IdikaFieldBuilder<TModel, TValue> Reference(string referenceMessage, bool allowCopyReference = Default_AllowCopyReference, string @class = null)
        {
            _hasReference = !string.IsNullOrWhiteSpace(referenceMessage);
            _cssReference = @class;
            _msgReference = referenceMessage;
            _allowCopyReference = allowCopyReference;
            return this;
        }

        public IdikaFieldBuilder<TModel, TValue> FitWidth()
        {
            _fitWidth = true;
            return this;
        }
        public IdikaFieldBuilder<TModel, TValue> Id(string editorId)
        {
            _editorId = editorId;
            return this;
        }

        private string GetId() => _editorId ?? _helper.IdFor(_expression).ToString();

        public string ToHtmlString()
        {
            var editorId = this.GetId();
            var infoMessageId = _helper.IdikaHtml().InfoMessageId(editorId);
            string prefixIcon = _hasPrefixIcon ? @"<span class=""input-group-addon""><i class=""" + _cssPrefixIcon + @"""></i></span>" : "";

            var ret = "";

            if (_hasLabel)
                ret += _helper.LabelFor(_expression, _msgLabel, new { @class = Prefix_CssLabel + " " + _cssLabel });

            if (_hasReference)
            {
                var elm = new TagBuilder("div");
                elm.AddCssClass(Prefix_CssReference + " " + _cssReference);
                var iTitle = _allowCopyReference ? Default_TitReferenceWithButton : Default_TitReference;
                elm.Attributes.Add("title", Default_TitReference);
                bool showCopyReference = _allowCopyReference && (!_readonly);
                var innerHtml = @"<span>{0}</span>" + (showCopyReference ? @"<i class=""glyphicon glyphicon-save"" data-reference-for=""#{1}"" title=""{2}""></i>" : "");
                innerHtml = string.Format(innerHtml, _msgReference, editorId, iTitle);
                elm.InnerHtml = innerHtml;
                ret += elm.ToString();
            }

            var editorHtml = _editorFactory().ToString();

            var additionalFieldRemarkHTML = "";
            if (!string.IsNullOrEmpty(_additionalFieldRemark))
            {
                additionalFieldRemarkHTML = $"<span class='{_additionalFieldRemarkClass}'>{_additionalFieldRemark}</span>";
            }

            if (_hasInfoMessage)
            {
                var html = "";
                if (_fitWidth)
                {
                    html = @"<div class=""btn-toolbar"" role=""toolbar"">
                                {2}
                                {1}
                                <button class=""btn btn-info"" type=""button"" data-toggle=""collapse"" href=""#{0}"" aria-expanded=""false"" aria-controls=""{0}"">
                                    <i class=""fa fa-info-circle""></i>
                                </button>
								{3}
                            </div>
                            ";
                }
                else
                {
                    html = @"<div class=""input-group"">
                                {2}
                                {1}
                                <span class=""input-group-btn"">
                                    <button class=""btn btn-info"" type=""button"" data-toggle=""collapse"" href=""#{0}"" aria-expanded=""false"" aria-controls=""{0}"">
                                        <i class=""fa fa-info-circle""></i>
                                    </button>
                                </span>
                            </div>
							{3}
                            ";
                }
                html = string.Format(html, infoMessageId, editorHtml, prefixIcon, additionalFieldRemarkHTML);
                editorHtml = html;
            }
            else if (_hasPrefixIcon)
            {
                var html = "";
                if (_fitWidth)
                {
                    html = @"<div class=""btn-toolbar"" role=""toolbar"">
                                {2}
                                {1}
                            </div>
							{3}
                            ";
                }
                else
                {
                    html = @"<div class=""input-group"">
                                {2}
                                {1}
                            </div>
							{3}
                            ";
                }
                html = string.Format(html, infoMessageId, editorHtml, prefixIcon, additionalFieldRemarkHTML);
                editorHtml = html;
            }
            else
            {
                if (_fitWidth)
                {
                    var html = @"<div class=""btn-toolbar"" role=""toolbar"">
                                    {0}
                                 </div>
									{1}";
                    html = string.Format(html, editorHtml, additionalFieldRemarkHTML);
                    editorHtml = html;
                }
            }

            ret += editorHtml;


            if (_hasValidationMessage)
                ret += _helper.ValidationMessageFor(_expression, _msgValidationMessage, new { @class = Prefix_CssValidationMessage + " " + _cssValidationMessage });

            if (_hasInfoMessage)
            {
                var elm = new TagBuilder("div");
                elm.AddCssClass(Prefix_CssInfoMessage + " " + _cssInfoMessage);
                elm.GenerateId(infoMessageId);
                var innerHtml = @"
                    <div class=""alert alert-info"">
                        {0}
                    </div>
                    ";
                innerHtml = string.Format(innerHtml, _msgInfoMessage);
                elm.InnerHtml = innerHtml;
                ret += elm.ToString();
            }

            if (_hasFormGroup)
            {
                var elm = new TagBuilder("div");
                elm.AddCssClass(Prefix_CssFormGroup + " " + _cssFormGroup);
                elm.InnerHtml = ret;
                ret = elm.ToString();
            }

            return ret;
        }
    }

    public class IdikaBooleanTableBuilder<TModel> : IHtmlString
    {
        // Constants
        private const string Prefix_CssFormGroup = "idika-field-form-group form-group";
        private const string Prefix_CssTable = "idika-boolean-table table table-hover table-condensed table-responsive";
        private const string Prefix_CssReference = "idika-field-reference";
        private const string Prefix_CssValidationMessage = "idika-field-validation-message text-danger";

        private const bool Default_HasFormGroup = false;
        private const string Default_CssFormGroup = "col-md-12";
        private const string Default_TitReference = "Τιμή αναφοράς";
        private const bool Default_ReadOnly = false;

        private bool _readonly = Default_ReadOnly;

        // Sub-Class: Entry
        private class Entry
        {
            public string Id { get; set; }
            public Expression<Func<TModel, bool?>> Expression { get; set; }
            public bool? ReferenceValue { get; set; }
            public bool ReadOnly { get; set; }
        }

        // FormGroup
        private bool _hasFormGroup = Default_HasFormGroup;
        private string _cssFormGroup = null;

        // Benefit
        private bool _hasFormatBenefitGroup = Default_HasFormGroup;
        
        // Table
        private string _cssTable = null;

        // Reference
        private string _cssReference = null;

        // System-Control-Fields
        private readonly HtmlHelper<TModel> _helper;
        private readonly List<Entry> _entries = new List<Entry>();

        public IdikaBooleanTableBuilder(HtmlHelper<TModel> helper, string @class = null, string referenceClass = null)
        {
            _helper = helper;
            _cssTable = @class;
            _cssReference = referenceClass;
        }

        public IdikaBooleanTableBuilder<TModel> FormGroup(string @class = Default_CssFormGroup) => this.FormGroup(true, @class);
        public IdikaBooleanTableBuilder<TModel> FormGroup(bool include, string @class = Default_CssFormGroup)
        {
            _hasFormGroup = include;
            _cssFormGroup = @class;
            return this;
        }

        public IdikaBooleanTableBuilder<TModel> ReadOnly(bool @readonly = true)
        {
            _readonly = @readonly;
            return this;
        }

        
        public IdikaBooleanTableBuilder<TModel> Add(Expression<Func<TModel, bool?>> expression, bool? referenceValue = null, bool readOnly = false, string id = null)
        {
            var entry = new Entry { Id = id, Expression = expression, ReferenceValue = referenceValue, ReadOnly = readOnly };
            _entries.Add(entry);
            return this;
        }

        public string ToHtmlString()
        {
            var ret = "";

            foreach (var entry in _entries)
            {
                var optionalViewData = entry.ReadOnly ? new { htmlAttributes = new { @class = "idika-readonly" } } : null;

                var editorHtml = entry.ReadOnly
                        ? _helper.DisplayFor(entry.Expression, "Object", optionalViewData).ToString()
                        : _helper.EditorFor(entry.Expression, "Idika/Bool.RadioList", optionalViewData).ToString();

                var labelHtml = _helper.LabelFor(entry.Expression).ToString();
                var referenceHtml = "";
                var validationHtml = _helper.ValidationMessageFor(entry.Expression, null, new { @class = Prefix_CssValidationMessage })?.ToString();
                var trIdHtml = (entry.Id == null) ? "" : " id=\"" + entry.Id + "\"";

                if (entry.ReferenceValue != null)
                {
                    var elm = new TagBuilder("div");
                    elm.AddCssClass(Prefix_CssReference + " " + _cssReference);
                    var title = Default_TitReference;
                    if (title != null) elm.Attributes.Add("title", title);
                    var innerHtml = @"<span>{0}</span>";
                    innerHtml = string.Format(innerHtml, entry.ReferenceValue.Value ? "Ναι" : "Οχι");
                    elm.InnerHtml = innerHtml;
                    referenceHtml = elm.ToString();
                }

                var html = "<tr{4}><td>{0}</td><td>{1}{2}<div>{3}</div></td></tr>";

                html = string.Format(html, editorHtml, labelHtml, referenceHtml, validationHtml, trIdHtml);
                ret += html;
            }

            {
                var elm = new TagBuilder("table");
                elm.AddCssClass(Prefix_CssTable + " " + _cssTable);
                var innerHtml = @"
                    <tbody>
                        {0}
                    </tbody>
                    ";
                innerHtml = string.Format(innerHtml, ret);
                elm.InnerHtml = innerHtml;
                ret = elm.ToString();
            }

            if (_hasFormGroup)
            {
                var elm = new TagBuilder("div");
                elm.AddCssClass(Prefix_CssFormGroup + " " + _cssFormGroup);
                elm.InnerHtml = ret;
                ret = elm.ToString();
            }

            return ret;
        }
    }
}