$(function () {
	var creationDateDatePicker = $("#creationDate");

	creationDateDatePicker.kendoMaskedDatePicker({
		format: "d/M/yyyy",
		parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
		dateInput: true
	});

	var dateFromDatePicker = $("#dateFrom");

	dateFromDatePicker.kendoMaskedDatePicker({
		format: "d/M/yyyy",
		parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
		dateInput: true
	});

	var dateToDatePicker = $("#dateTo");

	dateToDatePicker.kendoMaskedDatePicker({
		format: "d/M/yyyy",
		parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
		dateInput: true
	});
});