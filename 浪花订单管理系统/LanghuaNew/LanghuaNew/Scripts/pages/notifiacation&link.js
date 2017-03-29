$(document).ready(function() {


    $.getJSON(
        "/Scripts/json/link.json",
        null,
        function(data, textStatus, jqXHR) {
            if (textStatus !== "success") {
                return;
            }
            linkCenter(data, $("#links #linksContainer"));

        }
    );


});

function linkCenter(userdata, container) {
    var defaultSetting = {
        "enable": true,
        "displayLength": true,
        "list": []
    };
    var data = $.extend(true, {}, defaultSetting, userdata);
    if (data.enable !== true) {
        return;
    }
    if (data.list.length === 0) {
        return;
    }
    var oneLinkDefault = {
        "enable": true,
        "text": "请填写",
        "href": "",
        "target": "_blank"
    };
    var i, counter = 0,
        rowRef = 0,
        colRef, linkLableRef;
    var step = 4;
    var arrRow = [];
    var row = $('<div></div>').addClass("row");
    var col = $('<div></div>').addClass("col-lg-3 col-md-6 col-sm-6 col-xs-12");
    var linkLable = $('<a></a>').addClass("link-lable");
    for (i in data.list) {
        var one = $.extend({}, oneLinkDefault, data.list[i]);
        if (one.enable) {
            if (one.enable === false) {
                continue;
            }
        }
        if (counter / step === 0) {
            if (rowRef !== 0) {
                arrRow.push(rowRef);
            }
            rowRef = row.clone();
        }
        colRef = col.clone();
        linkLableRef = linkLable.clone().attr("target", one.target).text(one.text).attr("href", one.href);
        colRef.append(linkLableRef);
        rowRef.append(colRef);
        counter++;
    }
    arrRow.push(rowRef);
    for (i in arrRow) {
        container.append(arrRow[i]).closest("#links").show();
    }
}