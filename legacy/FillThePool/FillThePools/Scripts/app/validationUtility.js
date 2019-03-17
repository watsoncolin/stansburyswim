var ValidationUtility = function () {
	var validationElements = $("[data-role='validate']"), inputCount = 0, invalidTitle = "";

	validationElements.popover({
		placement: "top"
	});

	validationElements.on('invalid', function () {
		if (inputCount == 0) {
			$(this).popover('show');
			invalidTitle = $(this).attr("data-original-title");
			inputCount++;
		}
	});
	validationElements.on('blur', function () {
		$(this).popover("hide");
	});

	var validate = function (formSelector) {
		inputCount = 0;
		if (formSelector.indexOf("#") === -1) {
			formSelector = "#" + formSelector;
		}

		return $(formSelector)[0].checkValidity();
	};

	var title = function() {
		return invalidTitle;
	};

	return {
		validate: validate,
		invalidTitle: title
};
};
