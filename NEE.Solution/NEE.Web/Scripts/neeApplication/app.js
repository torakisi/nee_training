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

		$dlg.find(".confirm-prompt").html($self.attr('confirm-prompt'));

		var $butCancel = $dlg.find(".confirm-cancel");
		$butCancel.text($self.attr('confirm-cancel-text'));
		$butCancel.text($self.attr('confirm-cancel-class'));

		var $butOK = $dlg.find(".confirm-ok");
		$butOK.text($self.attr('confirm-ok-text'));
		$butOK.addClass($self.attr('confirm-ok-class'));
		$butOK.unbind("click");
		$butOK.click(function () { $self.trigger('click', 'confirmed'); });

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

// ========== Confirm-Cancel-Dialog ==========

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
			+ '                <button type="button" class="btn btn-danger cancel confirm-ok">OK</button>'
			+ '                <div style="padding-top:12px; float:none;" class="confirm-custom hidden">Custom...</div>'
			+ '            </div>'
			+ '        </div>'
			+ '    </div>'
			+ '</div>'
			+ '';
		return ret;
	}

	$("[confirm-cancel-dialog][onclick]").each(function () {
		var elm = $(this)[0];
		$(this).attr('confirm-onclick', elm.onclick);
		elm.onclick = null;
	});

	$("[confirm-cancel-dialog]").click(function (ev, p1) {
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

		$dlg.find(".confirm-prompt").html($self.attr('confirm-prompt'));

		var $butCancel = $dlg.find(".confirm-cancel");
		$butCancel.text($self.attr('confirm-cancel-text'));
		$butCancel.text($self.attr('confirm-cancel-class'));

		var $butOK = $dlg.find(".confirm-ok");
		$butOK.text($self.attr('confirm-ok-text'));
		$butOK.addClass($self.attr('confirm-ok-class'));
		$butOK.unbind("click");
		$butOK.click(function () { $self.trigger('click', 'confirmed'); });

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

