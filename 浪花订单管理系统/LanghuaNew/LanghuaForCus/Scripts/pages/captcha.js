jQuery(document).ready(function() {
    var loadedtimes = {
        countter: 0
    };

    function updateCaptcha() {
        var tagImg = $('#valiCode');
        tagImg.attr("src", "/langhua/GetValidateCode?id=" + Math.random());

    }

    $('#valiCode').bind('load', { loadedtimes: loadedtimes }, function(e) {
        $('#input-captcha').val("");
        if (e.data.loadedtimes.countter === 0) {
            e.data.loadedtimes.countter++;
        } else {
            $('#input-captcha').focus();
        }
        $('#input-captcha').siblings("i").addClass("hidden");
        var value = $.trim($('#input-captcha').val());
        if ((/^[a-zA-Z0-9]{4}$/).test(value)) {
            // $(this).siblings("i").removeClass("hidden");
            $.ajax({
                url: "/langhua/CheckValidateCode",
                type: 'GET',
                contentType: "application/json; charset=utf-8;",
                data: {
                    str: value.toLowerCase()
                },
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        $('#input-captcha').siblings("i.fa-check").removeClass("hidden");
                        $('#input-captcha').siblings("i.fa-times").addClass("hidden");
                    } else {
                        $('#input-captcha').siblings("i.fa-times").removeClass("hidden");
                        $('#input-captcha').siblings("i.fa-check").addClass("hidden");
                    }
                }

            });
        } else {
            $(this).siblings("i").addClass("hidden");
        }
    });





    if ($('#valiCode').length !== 0) {
        updateCaptcha();
        $(".updateCaptcha").on('click', function() {
            updateCaptcha();
        });
        $(".updateCaptcha").on('touchstart', function(e) {
            e.preventDefault();
            updateCaptcha();
        });
        $('#input-captcha').on("keypress", function(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "")
            if (evt === "") {
                return true;
            }
            var key = evt.keyCode ? evt.keyCode : evt.which;
            console.log(key)
            if ((key >= 65 && key <= 90) || (key >= 97 && key <= 122) || (key >= 48 && key <= 57) || key === 13) {
                return true;
            } else {
                return false;
            }
        });
        $('#input-captcha').on("keyup", function(evt) {
            var value = $.trim($(this).val());
            if ((/^[a-zA-Z0-9]{4}$/).test(value)) {
                // $(this).siblings("i").removeClass("hidden");
                $.ajax({
                    url: "/langhua/CheckValidateCode",
                    type: 'GET',
                    contentType: "application/json; charset=utf-8;",
                    data: {
                        str: value.toLowerCase()
                    },
                    dataType: 'json',
                    success: function(data) {
                        console.log(data)
                        if (data.ErrorCode == 200) {
                            $('#input-captcha').siblings("i.fa-check").removeClass("hidden");
                            $('#input-captcha').siblings("i.fa-times").addClass("hidden");
                        } else {
                            $('#input-captcha').siblings("i.fa-times").removeClass("hidden");
                            $('#input-captcha').siblings("i.fa-check").addClass("hidden");
                        }
                    }
                });
            } else {
                if (value.length > 4) {

                    $('#input-captcha').siblings("i.fa-times").removeClass("hidden");
                    $('#input-captcha').siblings("i.fa-check").addClass("hidden");
                } else {
                    $(this).siblings("i").addClass("hidden");
                }
            }
        });
    }



    // var LangHuaCookie = {
    //     'set': function(name, value, expireObj, path, domain, secure) {
    //         console.log(name)
    //         console.log(value)
    //         console.log(expireObj)
    //         var cookieStr = encodeURIComponent(name) + '=' + encodeURIComponent(value);
    //         if (expireObj instanceof Date) {
    //             cookieStr += ";expires=" + expireObj.toGMTString();
    //         }
    //         if (!isNaN(expireObj)) {
    //             if (expireObj > 0) {
    //                 var date = new Date();
    //                 date.setTime(date.valueOf() + parseInt(expireObj) * 60 * 1000);
    //                 cookieStr += "; expires=" + date.toGMTString();
    //             }
    //         }
    //         if (path) {
    //             cookieStr += ";path=" + path;
    //         }
    //         if (domain) {
    //             cookieStr += ";domain=" + domain;
    //         }
    //         if (secure) {
    //             cookieStr += ";secure";
    //         }
    //         console.log(cookieStr)
    //         document.cookie = cookieStr;

    //     },
    //     'get': function(name) {
    //         var cookieName = encodeURIComponent(name);
    //         var cookieStart = document.cookie.indexOf(cookieName);
    //         var cookieValue = null;
    //         if (cookieStart > -1) {
    //             var cookieEnd = document.cookie.indexOf(';', cookieStart);
    //             if (cookieEnd == -1) {
    //                 cookieEnd = document.cookie.length;
    //             }
    //             cookieValue = decodeURIComponent(document.cookie.substring(cookieStart, cookieEnd));
    //             cookieValue = cookieValue.split("=")[1] ? cookieValue.split("=")[1] : null;
    //         }
    //         return cookieValue;
    //     },
    //     'delete': function(name, path, domain, secure) {
    //         this.set(name, '', new Date(0), path, domain, secure);
    //     }
    // };
    // $('form:eq(0)').on("submit", function(e) {
    //     var LONGINTIMES = LangHuaCookie.get("LONGINTIMES");
    //     console.log(LONGINTIMES)
    //     if (LONGINTIMES === null) {
    //         console.log("dsf")
    //         LangHuaCookie.set('LONGINTIMES', 1, 10);
    //     } else {
    //         LangHuaCookie.set('LONGINTIMES', parseInt(LONGINTIMES) + 1, 10);
    //     }

    // });
});