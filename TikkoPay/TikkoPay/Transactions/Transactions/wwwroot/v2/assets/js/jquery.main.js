function disableF5(e) { if ((e.which || e.keyCode) == 116) e.preventDefault(); };

$(document).on("keydown", function (e) {
    e = e || window.event;
    if (e.ctrlKey) {
        var c = e.which || e.keyCode;
        if (c == 82) {
            e.preventDefault();
            e.stopPropagation();
        }
    }
});


var IsTimerStart = false;

$(document).ready(function () {

    $(document).on("keydown", disableF5);

});

