$(document).ready(function () {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = $("#cnvSketch");
    var sketchActionsStack = new Array();    

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

    $("#lnkClearSketch").click(function () {
        clearSketch(sketchElement.sketch(), sketchActionsStack);
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
