$(document).ready(function () {
    $(".divSketchDraggable").draggable({
        opacity: 0.7,
        containment: "tbody",
        revert: "invalid",
        revertDuration: 200,
        stack: ".divSketchDroppable",
        delay: 150
    });

    $(".divDimensionDraggable").draggable({
        helper: "original", 
        opacity: 0.7,
        containment: "tbody",
        axis: "y",
        stack: ".divDimensionDroppable",
        delay: 150,
        revert: true
    });

    $(".divSketchDroppable").droppable({
        accept: ".divSketchDraggable",
        hoverClass: "divHoveredSolutionDroppable",
        drop: sketchDropped
    });

    $(".divDimensionDroppable").droppable({
        accept: ".divDimensionDraggable, .divSketchDraggable",
        hoverClass: "divHoveredDimensionDroppable",
        drop: dimensionDropped
    });
});

function sketchDropped(event, ui) {
    var sourceSketchId = ui.draggable.attr("data-sketchId");
    var targetSketchId = $(this).attr("data-sketchId");

    $.ajax({
        url: "/Sketch/ReplaceSketches?sourceSketchId=" + sourceSketchId + "&targetSketchId=" + targetSketchId,
        type: "POST",
        async: true,
        processData: false,
        cache: false,
        success: function () {
            location.reload();
        },
        error: function (eventArgs) {
            alert("Failed to move the sketch\n\nError status: " + eventArgs.status + "\nError message: " + eventArgs.statusText);
        }
    });
}

function dimensionDropped(event, ui) {
    var targetDimensionId = $(this).attr("data-dimensionId");

    if (ui.draggable.hasClass("divDimensionDraggable")) {
        var sourceDimensionId = ui.draggable.attr("data-dimensionId");

        $("#divMergeConfirmationDialog").dialog({
            resizable: false,
            height: 300,
            width: 400,
            modal: true,
            buttons: {
                Yes: function () {
                    $.ajax({
                        url: "/Dimension/MergeDimensions?sourceDimensionId=" + sourceDimensionId + "&targetDimensionId=" + targetDimensionId,
                        type: "POST",
                        async: true,
                        processData: false,
                        cache: false,
                        success: function () {
                            location.reload();
                        },
                        error: function (eventArgs) {
                            alert("Failed to merge the dimensions\n\nError status: " + eventArgs.status + "\nError message: " + eventArgs.statusText);
                        }
                    });

                    $(this).dialog("close");
                },
                No: function () {
                    $(this).dialog("close");
                }
            }
        });
    }
    else if (ui.draggable.hasClass("divSketchDraggable")) {
        var sourceSketchId = ui.draggable.attr("data-sketchId");

        $.ajax({
            url: "/Sketch/MoveSketchToDimension?sourceSketchId=" + sourceSketchId + "&targetDimensionId=" + targetDimensionId,
            type: "POST",
            async: true,
            processData: false,
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (eventArgs) {
                alert("Failed to move the sketch\n\nError status: " + eventArgs.status + "\nError message: " + eventArgs.statusText);
            }
        });
    }
}