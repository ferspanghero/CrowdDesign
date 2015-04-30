$(document).ready(function () {
    initializeMorphChartDraggable(".tdSketchDraggable");
    initializeMorphChartDraggable(".tdDimensionDraggable");

    $(".tdSketchDroppable").droppable({
        accept: ".tdSketchDraggable",
        hoverClass: "tdHoveredDroppable",
        drop: function (event, ui) {
            var dimensionId = $(this).attr("data-dimensionId");
            var sketchId = ui.draggable.attr("data-sketchId");

            $.ajax({
                url: "/Project/UpdateSketchDimension?dimensionId=" + dimensionId + "&sketchId=" + sketchId,
                type: "POST",
                async: true,
                processData: false,
                cache: false,
                success: function (eventArgs) {
                    ui.draggable.insertBefore($(event.target));
                },
                error: function (eventArgs) {
                    alert("Failed to move the sketch\n\nError status: " + eventArgs.status + "\nError message: " + eventArgs.statusText);
                }
            });
        }
    });

    $(".tdDimensionDroppable").droppable({
        accept: ".tdDimensionDraggable",
        hoverClass: "tdHoveredDroppable",
        drop: function (event, ui) {
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
                            success: function (eventArgs) {
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
    });
});

function resizeDraggable(event, ui) {
    var element = $(event.target);

    ui.helper.width(element.width());
    ui.helper.height(element.height());
}

function initializeMorphChartDraggable(elementName) {
    $(elementName).draggable({
        helper: "clone",
        opacity: 0.7,
        containment: "tbody",
        start: resizeDraggable
    });
}