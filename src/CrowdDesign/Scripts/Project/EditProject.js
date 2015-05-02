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
        accept: ".tdDimensionDraggable",
        hoverClass: "tdHoveredDroppable",
        drop: dimensionDropped
    });
});

function resizeDraggable(event, ui) {
    var element = $(event.target);

    ui.helper.width(element.width());
    ui.helper.height(element.height());
}

function sketchDropped(event, ui) {
    var sourceSketchId = ui.draggable.attr("data-sketchId");
    var targetSketchId = $(this).attr("data-sketchId");
    
    $.ajax({
        url: "/Project/MoveSketch?sourceSketchId=" + sourceSketchId + "&targetSketchId=" + targetSketchId,
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
    var sourceDimensionId = ui.draggable.attr("data-dimensionId");
    var targetDimensionId = $(this).attr("data-dimensionId");

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