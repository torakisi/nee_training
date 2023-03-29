// ========== Loading Progress Indicator ==========

function showLoading() {
    $(".idika-loaded").hide();
    $(".idika-loading").show();
}

$(function () {
    $(window).on("beforeunload", function () { showLoading(); });
    // FIX: loading-indicator shown in case of forward/back browser navigation.
    $(window).bind("pageshow", function (event) {
        if (event.originalEvent.persisted) {
            //$(".gmi-loading").hide();     // hide loading-indicator scenario
            window.location.reload()        // reload page scenario
        }
    });
});


$(function () {

    // ========== My-HACK: fix IE not focusing and not changing radio buttons with arrows when displayed as bootstrap btn-group ...!! ==========

    $(".btn-group .btn input[type=radio]").change(function () {
        $(this).closest(".btn-group").find(".btn").removeClass("active");
        $(this).closest(".btn").addClass("active");
    });
    $(".btn-group .btn").click(function () {
        $(this).find("input[type=radio]").focus();
    });

    // ========== MVC: Set input maxlength to data-val-length-max ==========

    $("input[data-val-length-max]").each(function () {
        $(this).attr("maxlength", $(this).attr("data-val-length-max"));
    });

    // ========== GmiHtmlHelpers ==========

    $(".idika-field-reference i[data-reference-for]").click(function () {
        var editorId = $(this).data("reference-for");
        var $editor = $(editorId);
        var value = $(this).closest(".idika-field-reference").find("span").text();
        $editor.focus();
        $editor.val(value);
    });

    // ========== ReadOnly ==========

    $(function () {
			$("input").attr("autocomplete", "nope"); 
			$(".idika-readonly .idika-field-form-group input[placeholder]").attr("placeholder", "");
        $(".idika-readonly .idika-field-form-group input").prop("disabled", true);
        $(".idika-readonly .checkbox input").prop("disabled", true);
        $(".idika-readonly .idika-field-form-group select").prop("disabled", true);
			$(".idika-readonly .idika-field-form-group .btn").addClass("disabled");
			$(".idika-readonly .idika-field-form-group .btn-info").removeClass("disabled");
      $(".idika-readonly .idika-field-form-group .btn-group .btn.disabled").click(function (event) { event.stopPropagation(); });
      $(".idika-readonly").removeClass("invisible");

      $(".idika-readonly.idika-editor-Bool-RadioList .btn").addClass("disabled");
      $(".idika-readonly.idika-editor-Bool-RadioList .btn-group .btn").click(function (event) { event.stopPropagation(); });
      $(".idika-readonly.idika-editor-Bool-RadioList :input:not(:checked)").prop("disabled", true);
        

			if ($("#residencePermitUntil").is(":disabled")) {
				var datepicker = $("#residencePermitUntil").data("kendoDatePicker");
				datepicker.destroy();
        }

        //if ($("#AffirmationGiven").is(':checked')) {
        //    console.log("ok test");
        //}

        //$("input[name='AffirmationGiven']").each(function () {
        //    console.log("ok test from name");
        //    //$(this).is
        //});

    });

	//$(function () {
	//	if (!$("#residencePermitUntil").is(":disabled")) {
	//		$("#residencePermitUntil").prop("readonly", true);
	//	}
	//});
    // ========== Other ==========

});

// ========== Confirm-Dialog ==========

$(function () {

    function getConfirmDialogHtml() {
        var ret = ''
            + '<div class="modal fade" id="confirmDialog" tabindex="-1" role="dialog" aria-hidden="true" style="display: none;">'
            + '    <div class="modal-dialog">'
            + '        <div class="modal-content">'
            + '            <div class="modal-header">'
            + '                <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>'
            + '                <h4 class="modal-title confirm-title">Confirmation Dialog</h4>'
            + '            </div>'
            + '            <div class="modal-body confirm-prompt">Are you sure you want to continue?</div>'
            + '            <div class="modal-footer">'
            + '                <div style="padding-top:12px;" class="pull-left confirm-wait hidden">Please Wait...</div>'
            + '                <button data-dismiss="modal" type="button" class="btn btn-default confirm-cancel">Cancel</button>'
            + '                <button type="button" class="btn btn-primary cancel confirm-ok">OK</button>'
            + '                <div style="padding-top:12px; float:none;" class="confirm-custom hidden">Custom...</div>'
            + '            </div>'
            + '        </div>'
            + '    </div>'
            + '</div>'
            + '';
        return ret;
    }

    $("[confirm-dialog-prompt]").hide();

    $("[confirm-dialog][onclick]").each(function () {
        var elm = $(this)[0];
        $(this).attr('confirm-onclick', elm.onclick);
        elm.onclick = null;
    });

	$("[confirm-dialog]").click(function (ev, p1) {
        var $self = $(this);

        var $dlg = $('#confirmDialog');

        if (p1 === 'confirmed') {
            var confirmWait = $self.attr('confirm-wait');
            if (confirmWait !== undefined) {
                $dlg.find(".confirm-wait").html(confirmWait).removeClass('hidden');
                $dlg.find('button').prop('disabled', true);
            }

            var ret;
            var fn = $self.attr('confirm-onclick');
            if (fn !== undefined) {
                if ($.isFunction(fn))
                    ret = fn();
                else
                    ret = eval(fn);
            }
            else {
                var href = $self.attr('href');
                if (href != undefined) window.location.href = href;
            }
            return ((typeof ret === 'boolean') ? ret : true);
        }

        if ($dlg.length == 0) {
            $(getConfirmDialogHtml()).appendTo('body');
            var $dlg = $('#confirmDialog');
        }

        $dlg.find(".confirm-title").html($self.attr('confirm-title'));

        var confirm_prompt = $self.attr('confirm-prompt');
        if (!confirm_prompt) {
            var $elm = $self.find("[confirm-dialog-prompt]");
            confirm_prompt = $elm.html();
            if (confirm_prompt) {
                $self.attr('confirm-prompt', confirm_prompt);
                $elm.remove();
            }
        }
        else if (confirm_prompt.startsWith("#")) {
            var $elm = $(confirm_prompt);
            confirm_prompt = $elm.html();
            if (confirm_prompt) {
                $self.attr('confirm-prompt', confirm_prompt);
                $elm.remove();
            }
        }
        $dlg.find(".confirm-prompt").html(confirm_prompt);

        var $butCancel = $dlg.find(".confirm-cancel");
        $butCancel.text($self.attr('confirm-cancel-text'));
        $butCancel.text($self.attr('confirm-cancel-class'));

        var $butOK = $dlg.find(".confirm-ok");
        $butOK.text($self.attr('confirm-ok-text'));
        $butOK.addClass($self.attr('confirm-ok-class'));
        $butOK.unbind("click");
		$butOK.click(function ()
		{
			$dlg.modal('hide');
			$self.trigger('click', 'confirmed');
		});

        var $datepickers = $dlg.find("input[kendo-date-picker]").not("[kendo-date-picker-init]");
        if ($datepickers.length > 0) {
            //alert($datepickers.length)
            $datepickers.attr("kendo-date-picker-init", "");
            $datepickers.kendoDatePicker({
              //start: "century"
              format: "d/M/yyyy",
							parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
							dateInput: true
            });
        }

        var fnOnLoad = $self.attr('confirm-onload');
        if (fnOnLoad !== undefined) {
            if ($.isFunction(fnOnLoad))
                fnOnLoad();
            else
                eval(fnOnLoad);
        }

        $dlg.modal('show');

        return false;
    });
});


// ========== Collapse-Next Panel ==========

$(function () {
    $('.panel [data-toggle="collapse-next"]').click(function () {
        var $parent = $(this).parents(".panel");
        var $target = $parent.find(".panel-collapse");
        $target.collapse('toggle');
    });

    $('.panel').on('show.bs.collapse', function () {
        $(this).closest('.panel').find('.collapse-next i').removeClass('fa-chevron-right').addClass('fa-chevron-down');
    });
    $('.panel').on('hide.bs.collapse', function () {
        $(this).closest('.panel').find('.collapse-next i').removeClass('fa-chevron-down').addClass('fa-chevron-right');
    });
});


// ========== Pay On Demand ==========

function submitPayOnDemand(btn, url, applicationId, amka) {

    var dtNow = new Date();

    var limits =
        {
            min_amount: -900.00,
            max_amount: 900.00,
            min_refmonth: new Date(2017, 2 - 1, 1),
            max_refmonth: new Date(dtNow.getFullYear(), dtNow.getMonth(), 1),
            min_reason_length: 10,
            max_reason_length: 250,
        };


    //--- get amount ---
    var amountText = $("#_amount_control_").val();
    if (!amountText) {
        alert("Παρακαλώ εισάγετε Ποσό Πληρωμής!");
        return false;
    }
    if (!/^(\-|\+)?\d{1,4}(\.|\,)\d{2}$/i.test(amountText)) {
        alert("Το Ποσό Πληρωμής πρέπει να αποτελείται από ακέραιο μέρος και δύο δεκάδικά!");
        return false;
    }
    amountText = amountText.replace(',', '.');
    var amount = parseFloat(amountText);
    if (amount == 0) {
        alert("Το Ποσό Πληρωμής δεν μπορεί να είναι 0");
        return false;
    }
    if ((amount < limits.min_amount) || (amount > limits.max_amount)) {
        alert("Το Ποσό Πληρωμής δεν μπορεί να είναι <" + limits.min_amount.toFixed(2) + " ή >" + limits.max_amount.toFixed(2));
        return false;
    }

    //--- get refmonth ---
    var refmonthText = $("#_refmonth_control_").val();
    if (!refmonthText) {
        alert("Παρακαλώ εισάγετε Μήνα Αναφοράς!");
        return false;
    }
    match = /^(\d{2})-(\d{4})$/i.exec(refmonthText);
    if (!match) {
        alert("Ο Μήνας Αναφοράς πρέπει να έχει τη μορφή MM-EEEE (πχ: 07-2017)!");
        return false;
    }
    var m = parseInt(match[1]);
    var y = parseInt(match[2]);
    if ((m < 1) || (m > 12)) {
        alert("Μη αποδεκτός Μήνας Αναφοράς: " + m);
        return false;
    }
    var refmonth = new Date(y, m - 1, 1);
    if ((refmonth < limits.min_refmonth) || (refmonth > limits.max_refmonth)) {
        alert("Ο Μήνας Αναφοράς είναι εκτός Ορίων: " + refmonthText);
        return false;
    }

    //--- get reason ---
    var reason = $("#_reason_control_").val();
    if (!reason) {
        alert("Παρακαλώ εισάγετε Αιτιολογία!");
        return false;
    }
    reason = reason.trim();
    if (reason.length < limits.min_reason_length) {
        alert("Παρακαλώ εισάγετε επαρκή Αιτιολογία!");
        return false;
    }
    if (reason.length > limits.max_reason_length) {
        alert("Η Αιτιολογία είναι υπερβολικά μεγάλη!");
        return false;
    }

    //--- prompt...
    var msg = "Θέλετε να "
        + ((amount > 0) ? "πιστώσετε " : "ΧΡΕΩΣΕΤΕ ")
        + Math.abs(amount).toFixed(2)
        + " Ευρώ στη πιο πρόσφατη Αίτηση με μήνα αναφορας "
        + refmonthText
        + " και αιτιολογία \""
        + reason
        + "\"?"
        ;
    if (!confirm(msg)) return false;

    var data = { applicationId: applicationId, amka: amka, amount: amount, refMonth: refmonth, reason: reason };
    var d = JSON.stringify(data);

    $.ajax({ type: "POST", url: url, data: d, dataType: "json", contentType: 'application/json' })
        .done(function (result) {
            if (result.message) alert(result.message);
            if (result.reload) {
                alert("Επιτυχής Καταχώρηση Αναδρομικών, Επιλέξτε OK για ανανέωση...");
                showLoading();
                window.location.reload();
            }
            //$('#confirmDialog').modal('hide');
        })
        .fail(function (xhr, textStatus, errorThrown) {
            alert("ERROR: " + xhr.status + " - " + xhr.statusText);
            //$('#confirmDialog').modal('hide');
        });

}


// ========== Gray-List ==========

function initSubmitReleaseGrayList(btn, dateText) {
    var datepicker = $("#_reldate_control_").data("kendoDatePicker");
    datepicker.value(new Date(dateText));
};

function submitReleaseGrayList(btn, url, graylistId, amka) {

    var dtNow = new Date();

    var limits =
        {
            min_reldate: new Date("2018-08-01"),
            max_reldate: new Date(dtNow.getFullYear(), dtNow.getMonth(), dtNow.getDate()),
            min_relremark_length: 10,
            max_relremark_length: 250,
        };


    //--- get reldate ---
    var datepicker = $("#_reldate_control_").data("kendoDatePicker");
    var date = datepicker.value();
    if (date == null) {
        alert("Παρακαλώ εισάγετε Ημερομηνία!");
        return false;
    }
    var reldate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    var reldateText = reldate.getDate() + "/" + (reldate.getMonth() + 1) + "/" + reldate.getFullYear();
    if ((reldate < limits.min_reldate) || (reldate > limits.max_reldate)) {
        alert("Η Ημερομηνία είναι εκτός Ορίων: " + reldateText);
        return false;
    }

    //--- get reltype ---
    var reltypeControl = $("#_reltype_control_");
    var reltype = reltypeControl.val();
    var reltypeText = $("option:selected").text();
    if (!reltype) {
        alert("Παρακαλώ επιλέξτε Λόγο-Εξαίρεσης!");
        return false;
    }

    //--- get relremark ---
    var relremark = $("#_relremark_control_").val();
    if (relremark) {
        relremark = relremark.trim();
        if (relremark.length < limits.min_relremark_length) {
            alert("Παρακαλώ εισάγετε επαρκή Σχόλιο-Χρήστη (" + limits.min_relremark_length.toString() + " χαρακτήρες τουλάχιστον)!");
            return false;
        }
        if (relremark.length > limits.max_relremark_length) {
            alert("Το Σχόλιο-Χρήστη είναι υπερβολικά μεγάλο (" + limits.max_relremark_length.toString() + " χαρακτήρες μέγιστο)!");
            return false;
        }
    }

    //--- prompt...
    var msg = "Θέλετε να συνεχίσετε με τα παρακάτω στοιχεία?\r"
        + "\r"
        + "Hμερομηνία: " + reldateText + "\r"
        + "Λόγος-Εξαίρεσης: \"" + reltypeText + "\"\r"
        + "Σχόλιο-Χρήστη: \"" + relremark + "\"\r"
        ;
    if (!confirm(msg)) return false;

    var data = { graylistId: graylistId, amka: amka, releaseDate: reldate, releaseReason: reltype, releaseRemark: relremark };
    var d = JSON.stringify(data);

    $.ajax({ type: "POST", url: url, data: d, dataType: "json", contentType: 'application/json' })
        .done(function (result) {
            if (result.message) alert(result.message);
            if (result.reload) {
                alert("Επιτυχής Διεκπεραίωση, Επιλέξτε OK για ανανέωση...");
                showLoading();
                window.location.reload();
            }
            //$('#confirmDialog').modal('hide');
        })
        .fail(function (xhr, textStatus, errorThrown) {
            alert("ERROR: " + xhr.status + " - " + xhr.statusText);
            //$('#confirmDialog').modal('hide');
        });

}

