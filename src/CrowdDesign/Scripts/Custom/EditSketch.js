$(document).ready(function () {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = new fabric.Canvas('cnvSketch');

    sketchElement.isDrawingMode = true;
    sketchElement.Selection = false;

    if (sketchData) {
        sketchElement.loadFromJSON(sketchData, sketchElement.renderAll.bind(sketchElement));
    }

    $("#btnSaveSketch").click(function () {
        $("#Data").val(JSON.stringify(sketchElement));
        $("#ImageUri").val(sketchElement.toDataURL());
    });

    $("#lnkClearSketch").click(function () {
        $("#dialog-confirm").html("This operation cannot be undone. Are you sure you want to continue?");

        // Define the Dialog and its properties.
        $("#dialog-confirm").dialog({
            resizable: false,
            modal: true,
            title: "Clear sketch",
            height: 300,
            width: 400,
            buttons: {
                "Yes": function () {
                    $(this).dialog('close');
                    clearSketch(sketchElement.sketch(), sketchActionsStack);
                },
                "No": function () {
                    $(this).dialog('close');
               }
            }
        }); 
    });

    $("#lnkSketchUndo").click(function () {
        undoAction(sketchElement.sketch(), sketchActionsStack);
    });

    $("#lnkSketchRedo").click(function () {
        redoAction(sketchElement.sketch(), sketchActionsStack);
    });

    $(document).keydown(function (e) {
        if (e.which === 90 && e.ctrlKey) {
            undoAction(sketchElement.sketch(), sketchActionsStack);
        }
        else if (e.which === 89 && e.ctrlKey) {
            redoAction(sketchElement.sketch(), sketchActionsStack);
        }
    });
});

function undoAction(sketch, sketchActionsStack) {
    if (sketch.actions.length > 0) {
        sketchActionsStack.push(sketch.actions.pop());
        sketch.redraw();
    }
}

function redoAction(sketch, sketchActionsStack) {
    if (sketchActionsStack.length > 0) {
        sketch.actions.push(sketchActionsStack.pop());
        sketch.redraw();
    }
}

function clearSketch(sketch, sketchActionsStack) {
    if (sketch.actions.length > 0) {
        sketchActionsStack.push(sketch.actions.pop());
        clearSketch(sketch, sketchActionsStack);
    } else {
        sketchActionsStack = [];
        sketch.redraw();   
    }
        
}
