$(document).ready(function () {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = new fabric.Canvas('cnvSketch');
    var sketchActionStack = []

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
                    sketchElement.clear();
                    sketchElement.renderAll();
                },
                "No": function () {
                    $(this).dialog('close');
               }
            }
        }); 
    });

    function undoAction() {
        if (sketchElement.getObjects().length !== 0) {
            var lastItemIndex = (sketchElement.getObjects().length - 1);
            var item = sketchElement.item(lastItemIndex);

            sketchActionStack.push(item);
            sketchElement.remove(item);
            sketchElement.renderAll();
        }
    }

    function redoAction() {
        if (sketchActionStack.length !== 0) {
            var item = sketchActionStack.pop(item);
            sketchElement.add(item);
            sketchElement.renderAll();
        }
    }

    $("#lnkSketchUndo").click(function () {
        undoAction();
    });

    $("#lnkSketchRedo").click(function () {
        redoAction();
    });

    $(document).keydown(function (e) {
        if (e.which === 90 && e.ctrlKey) {
            undoAction();
        }
        else if (e.which === 89 && e.ctrlKey) {
            redoAction();
        }
    });

    $(".lnkSketchDrawColor").click(function () {
        sketchElement.freeDrawingBrush.color = this.getAttribute('data-color');
    });

    $(".lnkSketchDrawWidth").click(function () {
        sketchElement.freeDrawingBrush.width = this.getAttribute('data-size');
    });
});
