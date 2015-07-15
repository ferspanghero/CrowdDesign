$(document).ready(function() {
    var jsonSketchData = $("#Data").val();
    var sketchData = jsonSketchData ? JSON.parse($("#Data").val()) : undefined;
    var sketchElement = new fabric.Canvas("mainCanvas");
    var sketchActionStack = [];

    sketchElement.isDrawingMode = true;
    sketchElement.freeDrawingBrush.width = 5;
    sketchElement.Selection = false;

    if (sketchData) {
        sketchElement.loadFromJSON(sketchData, sketchElement.renderAll.bind(sketchElement));
    }

    $("#btnSaveSketch").click(function () {
        $("#returnToProject").val("true");
        $("#startNewSketch").val("false");
        $("#duplicateSketch").val("false");
        $("#Data").val(JSON.stringify(sketchElement));
        $("#ImageUri").val(sketchElement.toDataURL());
    });

    $("#btnDuplicateSketch").click(function() {
        $("#returnToProject").val("false");
        $("#duplicateSketch").val("true");
        $("#Data").val(JSON.stringify(sketchElement));
        $("#ImageUri").val(sketchElement.toDataURL());
        $("#saveSketchForm").submit();
    });

    $("#btnNewSketch").click(function () {
        $("#returnToProject").val("false");
        $("#startNewSketch").val("true");
        $("#Data").val(JSON.stringify(sketchElement));
        $("#ImageUri").val(sketchElement.toDataURL());
        $("#saveSketchForm").submit();
    })

    $("#lnkClearSketch").click(function() {
        $("#dialog-confirm").html("This operation cannot be undone. Are you sure you want to continue?");

        // Define the Dialog and its properties.
        $("#dialog-confirm").dialog({
            resizable: false,
            modal: true,
            title: "Clear sketch",
            height: 300,
            width: 400,
            buttons: {
                "Yes": function() {
                    $(this).dialog("close");
                    sketchElement.clear();
                    sketchElement.renderAll();
                },
                "No": function() {
                    $(this).dialog("close");
                }
            }
        });
    });

    function disableEraser() {
        sketchElement.off("mouse:down");
    }

    function enableEraser() {
        sketchElement.on("mouse:down", function (e) {
            if (sketchElement.getActiveGroup()) {
                sketchElement.getActiveGroup().forEachObject(function (a) {
                    sketchElement.remove(a);
                });
                sketchElement.discardActiveGroup();
            } else {
                sketchElement.remove(sketchElement.getActiveObject());
            }
            sketchElement.renderAll();
        });
    }

    $("#lnkSketchDrawingMode").click(function() {
        sketchElement.isDrawingMode = false;
        sketchElement.Selection = true;
        $("a.active-tool").removeClass("active-tool").addClass("inactive-tool");
        $(this).addClass("active-tool");
        $(this).removeClass("inactive-tool");
        disableEraser();
    });

    $(".lnkSketchEraser").click(function() {
        sketchElement.isDrawingMode = false;
        sketchElement.Selection = true;
        enableEraser();

        $("a.active-tool").removeClass("active-tool").addClass("inactive-tool");
        $(this).addClass("active-tool");
        $(this).removeClass("inactive-tool");
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

    $("#lnkSketchUndo").click(function() {
        undoAction();
    });

    $("#lnkSketchRedo").click(function() {
        redoAction();
    });

    $(document).keydown(function(e) {
        if (e.which === 90 && e.ctrlKey) {
            undoAction();
        } else if (e.which === 89 && e.ctrlKey) {
            redoAction();
        }
    });

    function makeActiveTool(anchor) {
        $("a.active-tool").removeClass("active-tool").addClass("inactive-tool");
        $(anchor).addClass("active-tool");
        $(anchor).removeClass("inactive-tool");
        disableEraser();
    }

    function makeActiveColor(anchor) {
        $("a.active-color").removeClass("active-color").addClass("inactive-color");
        $(anchor).addClass("active-color");
        $(anchor).removeClass("inactive-color");
        disableEraser();
    }

    $(".lnkSketchDrawColor").click(function () {
        sketchElement.freeDrawingBrush.color = this.getAttribute("data-color");
        makeActiveColor(this);

        if (sketchElement.getActiveGroup()) {
            sketchElement.getActiveGroup().forEachObject(function (a) {
                a.set('stroke', sketchElement.freeDrawingBrush.color);
            });
        }
        else if (sketchElement.getActiveObject()) {
            var target = sketchElement.getActiveObject();
            target.set('stroke', sketchElement.freeDrawingBrush.color);
        } else {
            sketchElement.isDrawingMode = true;
            sketchElement.Selection = false;
            var searchString = '[data-size="' + sketchElement.freeDrawingBrush.width + '"]';
            var element = $(searchString);
            makeActiveTool(element);
        }

        sketchElement.renderAll();
    });

    $(".lnkSketchDrawWidth").click(function() {
        sketchElement.freeDrawingBrush.width = this.getAttribute("data-size");
        sketchElement.isDrawingMode = true;
        sketchElement.Selection = false;
        makeActiveTool(this);
    });

    $(".lnkSketchText").click(function() {
        var input = prompt("Please enter the text you would like to add to your canvas");
        if (input != null) {
            var textElement = new fabric.Text(input, { left: 100, top: 100 });
            sketchElement.add(textElement);
            sketchElement.isDrawingMode = false;
        }
        disableEraser();
    });

    $(".lnkAddShape").click(function () {
        var shapeType = this.getAttribute("data-shapeType");
        var currentColor = sketchElement.freeDrawingBrush.color;
        if (shapeType === "rect") {
            var shape = new fabric.Rect({
                left: 100,
                top: 100,
                stroke: currentColor,
                fill: "transparent",
                strokeWidth: 2,
                width: 100,
                height: 100
            });
            sketchElement.add(shape);
        }
        else if (shapeType === "circle") {
            var shape = new fabric.Circle({
                radius: 50, stroke: currentColor, fill: "transparent", strokeWidth: 2, left: 100, top: 100
            });
            sketchElement.add(shape);
        }

        sketchElement.isDrawingMode = false;
        sketchElement.Selection = true;
        disableEraser();
        makeActiveTool($("#lnkSketchDrawingMode"));
    });
});