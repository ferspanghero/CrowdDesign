var __slice = Array.prototype.slice;
(function ($) {
    var Sketch;
    $.fn.sketch = function () {
        var args, key, sketch;
        key = arguments[0], args = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
        if (this.length > 1) {
            $.error('Sketch.js can only be called on one element at a time.');
        }
        sketch = this.data('sketch');
        if (typeof key === 'string' && sketch) {
            if (sketch[key]) {
                if (typeof sketch[key] === 'function') {
                    return sketch[key].apply(sketch, args);
                } else if (args.length === 0) {
                    return sketch[key];
                } else if (args.length === 1) {
                    return sketch[key] = args[0];
                }
            } else {
                return $.error('Sketch.js did not recognize the given command.');
            }
        } else if (sketch) {
            return sketch;
        } else {
            this.data('sketch', new Sketch(this.get(0), key));
            return this;
        }
    };
    Sketch = (function () {
        function Sketch(el, opts) {
            this.el = el;
            this.canvas = $(el);
            this.context = el.getContext('2d');
            this.options = $.extend({
                toolLinks: true,
                defaultTool: 'marker',
                defaultColor: '#000000',
                defaultSize: 5
            }, opts);
            this.painting = false;
            this.color = this.options.defaultColor;
            this.size = this.options.defaultSize;
            this.tool = this.options.defaultTool;
            this.actions = [];
            this.action = [];
            this.canvas.bind('click mousedown mouseup mousemove mouseleave mouseout touchstart touchmove touchend touchcancel', this.onEvent);
            if (this.options.toolLinks) {
                $('body').delegate("a[href=\"#" + (this.canvas.attr('id')) + "\"]", 'click', function (e) {
                    var $canvas, $this, key, sketch, i, len, ref;
                    $this = $(this);
                    $canvas = $($this.attr('href'));
                    sketch = $canvas.data('sketch');
                    ref = ['color', 'size', 'tool'];
                    for (i = 0, len = ref.length; i < len; i++) {
                        key = ref[i];
                        if ($this.attr("data-" + key)) {
                            sketch.set(key, $(this).attr("data-" + key));
                        }
                    }
                    if ($(this).attr('data-download')) {
                        sketch.download($(this).attr('data-download'));
                    }
                    return false;
                });
            }
        }
        Sketch.prototype.download = function (format) {
            var mime;
            format || (format = "png");
            if (format === "jpg") {
                format = "jpeg";
            }
            mime = "image/" + format;
            return window.open(this.el.toDataURL(mime));
        };
        Sketch.prototype.set = function (key, value) {
            this[key] = value;
            return this.canvas.trigger("sketch.change" + key, value);
        };
        Sketch.prototype.startPainting = function () {
            this.painting = true;
            return this.action = {
                tool: this.tool,
                color: this.color,
                size: parseFloat(this.size),
                events: []
            };
        };
        Sketch.prototype.stopPainting = function () {
            if (this.action) {
                this.actions.push(this.action);
            }
            this.painting = false;
            this.action = null;
            return this.redraw();
        };
        Sketch.prototype.onEvent = function (e) {
            if (e.originalEvent && e.originalEvent.targetTouches && e.originalEvent.targetTouches.length > 0) {
                e.pageX = e.originalEvent.targetTouches[0].pageX;
                e.pageY = e.originalEvent.targetTouches[0].pageY;
            }
            $.sketch.tools[$(this).data('sketch').tool].onEvent.call($(this).data('sketch'), e);
            e.preventDefault();
            return false;
        };
        Sketch.prototype.redraw = function () {
            var sketch;
            this.el.width = this.canvas.width();
            this.context = this.el.getContext('2d');
            sketch = this;
            $.each(this.actions, function () {
                if (this.tool) {
                    return $.sketch.tools[this.tool].draw.call(sketch, this);
                }
            });
            if (this.painting && this.action) {
                return $.sketch.tools[this.action.tool].draw.call(sketch, this.action);
            }
        };
        return Sketch;
    })();
    $.sketch = {
        tools: {}
    };
    $.sketch.tools.marker = {
        onEvent: function (e) {
            switch (e.type) {
                case 'mousedown':
                case 'touchstart':
                    if (this.painting) {
                        this.stopPainting();
                    }

                    this.startPainting();
                    break;
                case 'mouseup':
                case 'mouseout':
                case 'mouseleave':
                case 'touchend':
                case 'touchcancel':
                    this.stopPainting();
            }
            if (this.painting) {
                this.action.events.push({
                    x: e.pageX - this.canvas.offset().left,
                    y: e.pageY - this.canvas.offset().top,
                    event: e.type
                });
                return this.redraw();
            }
        },
        draw: function (action) {
            var event, previous, i, len, ref;
            this.context.lineJoin = "round";
            this.context.lineCap = "round";
            this.context.beginPath();
            this.context.moveTo(action.events[0].x, action.events[0].y);
            ref = action.events;
            for (i = 0, len = ref.length; i < len; i++) {
                event = ref[i];
                this.context.lineTo(event.x, event.y);
                previous = event;
            }
            this.context.strokeStyle = action.color;
            this.context.lineWidth = action.size;
            return this.context.stroke();
        }
    };

    // Represents a tool where users can input text in the canvas
    $.sketch.tools.text = {
        onEvent: function (e) {
            switch (e.type) {
                case 'mousedown':
                case 'touchstart':
                    // If the event is a mousedown, only triggers if the left button was pressed
                    if ((e.type === "mousedown" && e.button === 0) || e.type === "touchstart") {
                        this.startPainting();

                        var tempTextArea = document.getElementById("tempTextArea");

                        // Adds a temporary text area where the text will be written
                        if (tempTextArea == null) {
                            var $textArea = $("<textarea>", {
                                type: "text",
                                id: "tempTextArea"
                            });

                            // Adds style to position the text right in the position where the click/touch was performed
                            $textArea.css({
                                position: "absolute",
                                left: e.pageX,
                                top: e.pageY,
                                height: "100px",
                                width: "200px"
                            });                            

                            $("body").append($textArea);

                            $textArea.focus();

                            // If the user presses ESC, then the textarea will be removed
                            $textArea.keyup(function (e) {
                                if (e.which === 27) {
                                    e.preventDefault();

                                    document.body.removeChild(this);
                                }
                            });

                            // If the textarea loses focus, then it is removed
                            $textArea.focusout(function (e) {
                                document.body.removeChild(this);
                            });

                        // If the temporary text area already exists, it means the user wants to exit it
                        } else {
                            e.preventDefault();

                            // Retrieves the text information
                            var lineHeight = this.context.measureText("M").width * 1.2;
                            var x = parseInt($(tempTextArea).css("left")) - this.canvas.offset().left;
                            var y = parseInt($(tempTextArea).css("top")) - this.canvas.offset().top;
                            var lines = $(tempTextArea).val().split("\n");

                            // Create a new event for each line of the text
                            for (var i = 0; i < lines.length; i++) {
                                this.action.events.push({
                                    x: x,
                                    y: y,
                                    text: lines[i],
                                    event: e.type
                                });

                                y += lineHeight;
                            }
                            
                            // Makes the textarea lose focus
                            $("#tempTextArea").blur();

                            this.stopPainting();
                        }
                    }
                    break;
            }
        },
        draw: function (action) {
            this.context.font = "20px Georgia";

            if (action.events != null) {
                for (var i = 0; i < action.events.length; i++) {
                    this.context.fillText(action.events[i].text, action.events[i].x, action.events[i].y);
                }
            }
        }
    }

    return $.sketch.tools.eraser = {
        onEvent: function (e) {
            return $.sketch.tools.marker.onEvent.call(this, e);
        },
        draw: function (action) {
            var oldcomposite;
            oldcomposite = this.context.globalCompositeOperation;
            this.context.globalCompositeOperation = "destination-out";
            action.color = "#000000";
            $.sketch.tools.marker.draw.call(this, action);
            return this.context.globalCompositeOperation = oldcomposite;
        }
    };
})(jQuery);