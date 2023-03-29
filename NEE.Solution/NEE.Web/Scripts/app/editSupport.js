// ========== Edit Support ==========

(function ($) {
	var kendo = window.kendo,
		ui = kendo.ui,
		Widget = ui.Widget,
		proxy = $.proxy,
		CHANGE = "change",
		PROGRESS = "progress",
		ERROR = "error",
		NS = ".generalInfo";

	var MaskedDatePicker = Widget.extend({
		init: function (element, options) {
			var that = this;
			Widget.fn.init.call(this, element, options);

			$(element).kendoMaskedTextBox({ mask: that.options.dateOptions.mask || "00/00/0000" })
				.kendoDatePicker({
					format: that.options.dateOptions.format || "dd/MM/yyyy",
					parseFormats: that.options.dateOptions.parseFormats || ["dd/MM/yyyy", "dd/MM/yy"]
				})
				.closest(".k-datepicker")
				.add(element)
				.removeClass("k-textbox");

			that.element.data("kendoDatePicker").bind("change", function () {
				that.trigger(CHANGE);
			});
		},
		options: {
			name: "MaskedDatePicker",
			dateOptions: {}
		},
		events: [
			CHANGE
		],
		destroy: function () {
			var that = this;
			Widget.fn.destroy.call(that);

			kendo.destroy(that.element);
		},
		value: function (value) {
			var datepicker = this.element.data("kendoDatePicker");

			if (value === undefined) {
				return datepicker.value();
			}

			datepicker.value(value);
		}
	});

	ui.plugin(MaskedDatePicker);

})(window.kendo.jQuery);

$(function () {


    var culture = "el-GR";
    kendo.culture(culture);

    $.validator.methods.date = function (value, element) {
        return this.optional(element) || validate_date(value);
    }


    $("form").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    var residencePermitUntilDatePicker = $("#residencePermitUntil");
    //residencePermitUntilDatePicker.kendoMaskedTextBox({
    //	mask: "00/00/0000"
    //});

    // create DateTimePicker from input HTML element
    //residencePermitUntilDatePicker.kendoDatePicker({
    //     //start: "century"
    //     format: "d/M/yyyy",
    //		parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
    //		dateInput: true
    //});


    residencePermitUntilDatePicker.kendoMaskedDatePicker({
        format: "d/M/yyyy",
        parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
        dateInput: true
    });

    //residencePermitUntilDatePicker.closest(".k-datepicker")
    //	.add(residencePermitUntilDatePicker)
    //	.removeClass("k-textbox");
    //});

   //     $(function () {
   //             console.log("OK from inside");
			////if(Model.AddressInfo.City == null)
   ////         {
   ////             //var sortBySelect = document.querySelector("postcode");
   ////             //sortBySelect.value = Model.AddressInfo.ZipCode;
   ////             //console.log(sortBySelect.value);
   ////             //sortBySelect.dispatchEvent(new Event("change"));
   ////         }
   //     });

    //$("#postcode").kendoAutoComplete({
    //    template: "#= Code # | #= City # | #= District #",
    //    virtual: true,
    //    noDataTemplate: false,
    //    dataSource: {
    //        transport: {
    //            read: {
    //                url: "../postcode",
    //                dataType: "json",
    //                data: {
    //                    q: function () {
    //                        return $("#postcode").val();
    //                    }
    //                }
    //            },
    //            parameterMap: function (data, type) {
    //                var ret = null;
    //                if (data.filter.filters[0] != undefined) {
    //                    ret = data.filter.filters[0].value;
    //                }
    //                return { q: ret };
    //            }
    //        },
    //        serverFiltering: true,

    //    },
    //    minLength: 2,
    //    dataTextField: "Code",
    //    select: function (e) {
    //        var dataItem = this.dataItem(e.item.index());
    //        $("#municipality").val(dataItem.City);
    //        $("#county").val(dataItem.District);
    //    },
    //    close: function (e) {
    //        if (this.dataItems().length == 1) {
    //            var dataItem = this.dataItem(0);
    //            $("#municipality").val(dataItem.City);
    //            $("#county").val(dataItem.District);
    //        }
    //    }
    //});



    function validate_date(date) {
        var pattern = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
        //return pattern.test(date);
        if (pattern.test(date)) {
            var dArr = date.split("/");
            var d = new Date(dArr[1] + "/" + dArr[0] + "/" + dArr[2]);
            return d.getDate() == dArr[0] && d.getMonth() + 1 == dArr[1] && d.getFullYear() == dArr[2] && d.getFullYear() >= 1900;
        }
        else {
            return false;
        }
    }

    function showContainerFormGroup($elm, show) {
        var $container = $elm.closest(".form-group");
        if (show) $container.show(); else $container.hide();
    }    

});
