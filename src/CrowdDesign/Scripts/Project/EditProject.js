$(document).ready(function () {
    $(".tdSketchDraggable").draggable({
        helper: "clone",
        opacity: 0.7,
        containment: "tbody",
        start: resizeDraggable
    });

    $(".tdDimensionDraggable").draggable({
        helper: "clone",
        opacity: 0.7,
        containment: "tbody",
        start: resizeDraggable
    });

    $(".tdSketchDroppable").droppable({
        accept: ".tdSketchDraggable",
        hoverClass: "tdHoveredDroppable",
        drop: sketchDropped
    });

    $(".tdDimensionDroppable").droppable({
        accept: ".tdDimensionDraggable, .tdSketchDraggable",
        hoverClass: "tdHoveredDroppable",
        drop: dimensionDropped
    });
});

function resizeDraggable(event, ui) {
    var element = $(event.target);

    ui.helper.width(element.width() / 2);
    ui.helper.height(element.height() / 2);
}

function sketchDropped(event, ui) {
    var sourceSketchId = ui.draggable.attr("data-sketchId");
    var targetSketchId = $(this).attr("data-sketchId");

    $.ajax({
        url: "/Project/ReplaceSketches?sourceSketchId=" + sourceSketchId + "&targetSketchId=" + targetSketchId,
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

    if (ui.draggable.hasClass("tdDimensionDraggable")) {
        var sourceDimensionId = ui.draggable.attr("data-dimensionId");

        $("#divMergeConfirmationDialog").dialog({
            resizable: false,
            height: 300,
            width: 400,
            modal: true,
            buttons: {
                Yes: function () {
                    $.ajax({
                        url: "/Project/MergeDimensions?sourceDimensionId=" + sourceDimensionId + "&targetDimensionId=" + targetDimensionId,
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
    else if (ui.draggable.hasClass("tdSketchDraggable")) {
        var sourceSketchId = ui.draggable.attr("data-sketchId");

        $.ajax({
            url: "/Project/MoveSketchToDimension?sourceSketchId=" + sourceSketchId + "&targetDimensionId=" + targetDimensionId,
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