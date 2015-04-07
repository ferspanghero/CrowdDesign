$(document).ready(function() {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = $("#cnvSketch");

    sketchElement.attr("width", 800);
    sketchElement.attr("height", 600);

    sketchElement.sketch();

    if (sketchData) {
        $.each(sketchData, function() {
            sketchElement.sketch().actions.push(this);
        });

        sketchElement.sketch().redraw();
    }

    $.each(["#f00", "#ff0", "#0f0", "#0ff", "#00f", "#000", "#fff"], function () {
        $("#divSketchTools").append("<a href='#cnvSketch' data-color='" + this + "' style='border: 1px solid black; width: 30px; height: 30px; background: " + this + "; display: inline-block;'></a> ");
    });

    $.each([3, 5, 10, 15, 20], function () {
        $('#divSketchTools').append("<a href='#cnvSketch' data-size='" + this + "' style='border: 1px solid black; width: 30px; height: 30px; background: #ccc;'>" + this + "</a> ");
    });

    $("#btnSaveSketch").click(function () {
        $("#Data").val(JSON.stringify(sketchElement.sketch().actions));
    });

    $("#btnClearSketch").click(function () {
        $("#Data").val("");
    });
});