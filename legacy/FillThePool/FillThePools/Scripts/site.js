
$(document).ready(function () {
    $("#date").change(function () {
        var date = $(this).val();
        getSchedule(date);
    });

	$("#loginLink").click(function() {
		$("#loginModal").modal("show");
	});

	$.get("/Home/LoginForm").done(function(data) {
		$("#loginModalBody").html(data);
	});
});

function getSchedule(date)
{
    $.get("./Schedule/List/?start=" + date, function (data) {
        $("#schedule").html(data);
        addBindings();
    });
}

function addBindings() {
    $("#addSchedule").click(function () {
        $("#newScheduleForm").show();
        $("#saveNewSchedule").show();
        $("#addSchedule").hide();
    });

    $("#saveNewSchedule").click(function () {

        $("#newScheduleForm").hide();
        $("#saveNewSchedule").hide();
        $("#addSchedule").show();
        var start = $("#date").val();
        var hour = $("#startTime").val();
        hour = parseInt(hour.substring(0, hour.indexOf(":")));
        var minute = $("#startTime").val();
        minute = parseInt(minute.substring(minute.indexOf(":") + 1));

        if (hour < 10)
            hour = "0" + hour;
        if (minute < 10)
            minute = "0" + minute;
        start += "T" + hour + ":" + minute + ":00.000Z";

        var end = $("#date").val();
        hour = $("#endTime").val();
        hour = parseInt(hour.substring(0, hour.indexOf(":")));
        minute = $("#endTime").val();
        minute = parseInt(minute.substring(minute.indexOf(":") + 1));

        if (hour < 10)
            hour = "0" + hour;
        if (minute < 10)
            minute = "0" + minute;
        end += "T" + hour + ":" + minute + ":00.000Z";

        $.ajax({
            url: "/api/Schedule/",
            type: "POST",
            data: JSON.stringify({ Start: start, End: end, Registration: null }),
            contentType: "application/json",
            success: function(data)
            {
                getSchedule($("#date").val());
            }
        });

    });

    $("#startTime").change(function () {
        var hour = $("#startTime").val();
        hour = parseInt(hour.substring(0, hour.indexOf(":")));
        var minute = $("#startTime").val();
        minute = parseInt(minute.substring(minute.indexOf(":") + 1));

        if (minute + 20 > 59) {
            hour++;
            minute = 60 - minute;
        }
        else
            minute = minute + 20;

        var endTime = "";

        if (hour < 10)
            endTime = "0" + hour;
        else
            endTime = hour;

        if (minute < 10)
            endTime += ":0" + minute + ":00.00";
        else
            endTime += ":" + minute + ":00.00";

        
        $("#endTime").val(endTime);
    });
}

$("#theCarousel").carousel(); { }

$('.carousel-inner').click(function () {
	$("#theCarousel").carousel('pause');
});
