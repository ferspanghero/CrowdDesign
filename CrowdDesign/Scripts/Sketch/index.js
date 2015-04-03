$(document).ready(function () {
    $.each(["#f00", "#ff0", "#0f0", "#0ff", "#00f", "#000", "#fff"], function () {
        $("#divSketchTools").append("<a href='#cnvSketch' data-color='" + this + "' style='border: 1px solid black; width: 30px; height: 30px; background: " + this + "; display: inline-block;'></a> ");
    });

    var sketch = $("#cnvSketch");

    sketch.attr("width", 1024);
    sketch.attr("height", 768);

    sketch.sketch();
});