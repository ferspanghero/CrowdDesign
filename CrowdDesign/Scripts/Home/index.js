$(document).ready(function () {
    $("#btnCategory").click(function () {
        $("#tblMorphChart tr").first().append("<th contenteditable=\"true\">New Category</th>");
        $("#tblMorphChart tbody tr").append("<td class=\"tblCellSketch\"><a href=\"designSketch.html\">Empty sketch</a></td>");
    });

    $("#btnProposal").click(function () {
        var columnsCount = $("#tblMorphChart th").length;
        var lastRow = $("#tblMorphChart tr").last();
        var newRowHtml = "<tr>";

        newRowHtml += "<td contenteditable=\"true\">New user</td>";

        for (var i = 0; i < columnsCount - 1; i++) {
            newRowHtml += "<td class=\"tblCellSketch\"><a href=\"..\Sketch\index.html\">Empty sketch</a></td>";
        }

        newRowHtml += "</tr>";

        lastRow.after(newRowHtml);
    });
});