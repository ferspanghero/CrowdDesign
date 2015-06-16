$(document).ready(function () {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = $("#cnvSketch");

    sketchElement.attr("width", 1280);
    sketchElement.attr("height", 800);

    sketchElement.sketch();

    if (sketchData) {
        $.each(sketchData, function () {
            sketchElement.sketch().actions.push(this);
        });

        sketchElement.sketch().redraw();
    }

    $("#btnSaveSketch").click(function () {
        $("#Data").val(JSON.stringify(sketchElement.sketch().actions));
        $("#ImageUri").val(document.getElementById("cnvSketch").toDataURL());
    });

    $("#btnClearSketch").click(function () {
        $("#Data").val("");
    });
});