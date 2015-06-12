﻿$(document).ready(function () {
    $(".divSketchDraggable").draggable({
        opacity: 0.7,
        containment: "tbody",
        revert: "invalid",
        revertDuration: 200,
        stack: ".divSketchDroppable, .divNewSketchDroppable",
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
        accept: function (dropElem) {
            if ($(this).attr("data-sketchId") != dropElem.attr("data-sketchId")) {
                return true;
            }
        },
        hoverClass: "divHoveredSolutionDroppable",
        drop: sketchDropped
    });

    $(".divNewSketchDroppable").droppable({
        accept: ".divSketchDraggable",
        hoverClass: "divHoveredNewSolutionDroppable",
        drop: sketchDroppedOnNew
    });

    $(".divDimensionDroppable").droppable({
        accept: ".divDimensionDraggable",
        hoverClass: "divHoveredDimensionDroppable",
        drop: dimensionDropped
    });

    $("#prompt").dialog({
        autoOpen: false,
        show: {
            effect: "blind",
            duration: 300
        },
        hide: {
            effect: "blind",
            duration: 300
        },
        minWidth: 800,
        resizable: false
    });

    $("#openPrompt").click(function () {
        $("#prompt").dialog("open");
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

function sketchDroppedOnNew(event, ui) {
    var sourceSketchId = ui.draggable.attr("data-sketchId");
    var targetDimensionId = $(this).attr("data-dimensionId");

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
    })
}

function dimensionDropped(event, ui) {
    var targetDimensionId = $(this).attr("data-dimensionId");
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