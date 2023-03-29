(function () {
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

    $(document).ready(function () {    
        $.validator.methods.date = function (value, element) {
            return this.optional(element) || validate_date(value);
        }
        $.validator.methods.range = function (value, element, param) {
            var globalizedValue = value.replace(",", ".");
            return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
        }

        $.validator.methods.number = function (value, element) {
            return this.optional(element) || /^(?:-?\d+|-?\d{1,3}(?:,\d{3})+)?(?:\,\d+)?$/.test(value);
		}

		$.validator.setDefaults({
			ignore: ':hidden, [readonly="readonly"], [readonly="True"]'
		});
    })
})()