$(document).ready(function() {
    $("#mainContainer").prepend("<div id='notifications'></div>");
    $.getJSON(
        "/Scripts/json/notification.json",
        null,
        function(data, textStatus, jqXHR) {
            if (textStatus !== "success") {
                return;
            }
            notificationsCenter(data, $("#mainContainer #notifications"));
        }
    );
});


function notificationsCenter(userdata, container) {
    var defaultSetting = {
        "enable": true,
        "displayLength": 1,
        "list": []
    };
    var data = $.extend(true, {}, defaultSetting, userdata);
    if (data.enable !== true) {
        return;
    }
    if (data.list.length === 0) {
        return;
    }
    var oneNotificationDefault = {
        "timeStart": "",
        "timeEnd": "",
        "content": "这是一条空消息",
        "timesClosedToDisapper": 1
    };
    var i, display, arrTimeStart, arrTimeStartYMD, arrTimeStartHMS, timeStampTimeStart, arrTimeEnd, arrTimeEndYMD, arrTimeEndHMS, timeStampTimeEnd;
    var counter = 0;
    var timeStampNow = (new Date()).valueOf();
    var div = $("<div></div>");
    var toclose = $("<span>×</span>");
    toclose.addClass("toclose");
    var timeZoneOffsetLocal = new Date().getTimezoneOffset() * 60000;
    var timeZoneOffsetBase = -8 * 60 * 60000;
    for (i in data.list) {
        var one = $.extend(true, {}, oneNotificationDefault, data.list[i]);
        var closeTimesCookieSetted = $.LangHuaCookie.get(one.timeEnd);
        if (closeTimesCookieSetted) {
            if (parseInt(closeTimesCookieSetted) >= parseInt(one.timesClosedToDisapper)) { //主动关闭的通知不会出现在dom内
                continue;
            }
        }
        arrTimeStart = one.timeStart.split("T");
        arrTimeStartYMD = arrTimeStart[0].split("-");
        arrTimeStartHMS = arrTimeStart[1].split(":");
        timeStampTimeStart = (new Date(arrTimeStartYMD[0], parseInt(arrTimeStartYMD[1]) - 1, arrTimeStartYMD[2], arrTimeStartHMS[0], arrTimeStartHMS[1], 0)).valueOf() - timeZoneOffsetLocal + timeZoneOffsetBase;
        arrTimeEnd = one.timeEnd.split("T");
        arrTimeEndYMD = arrTimeEnd[0].split("-");
        arrTimeEndHMS = arrTimeEnd[1].split(":");
        timeStampTimeEnd = (new Date(arrTimeEndYMD[0], parseInt(arrTimeEndYMD[1]) - 1, arrTimeEndYMD[2], arrTimeEndHMS[0], arrTimeEndHMS[1], 0)).valueOf() - timeZoneOffsetLocal + timeZoneOffsetBase;
        if (timeStampTimeEnd < timeStampTimeStart) { //时间有误直接取消
            continue;
        }
        if (!((timeStampNow >= timeStampTimeStart) && (timeStampNow <= timeStampTimeEnd))) {
            continue;
        }
        if (counter >= parseInt(data.displayLength)) {
            display = 'none';
        } else {
            display = "inline-block";
        }

        var oneNotification = div.clone();
        oneNotification.addClass("notification").attr("times", one.timesClosedToDisapper);
        oneNotification.addClass("notification").attr("timeend", one.timeEnd);
        oneNotification.addClass("notification").attr("timestamptimeend", timeStampTimeEnd);
        oneNotification.css("display", display);
        oneNotification.html(one.content).append(toclose.clone());
        container.append(oneNotification);
        counter++;
    }
    container.on("click", ".toclose", function(e) {
        $(this).closest('.notification').remove(); //主动关闭的通知不会出现在dom内
        $(e.delegateTarget).find(".notification:hidden:eq(0)").css("display", "inline-block"); //主动关闭的通知不会出现在dom内
        var times = $(this).closest(".notification").attr("times");
        var timeEnd = $(this).closest(".notification").attr("timeend");
        var timeStampTimeEnd = parseInt($(this).closest(".notification").attr("timestamptimeend"));
        var timesCookieSetted = $.LangHuaCookie.get(timeEnd);
        if (timesCookieSetted) {
            timesCookieSetted = (parseInt(timesCookieSetted) + 1);
            $.LangHuaCookie.set(timeEnd, timesCookieSetted, new Date(timeStampTimeEnd), "/");
        } else {
            $.LangHuaCookie.set(timeEnd, 1, new Date(timeStampTimeEnd), "/");
        }
    });


}