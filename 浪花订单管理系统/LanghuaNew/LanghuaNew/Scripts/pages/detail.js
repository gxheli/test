jQuery(document).ready(function($) {
    var table = jQuery('table').first();
    if (table.length !== 0) {
        var arr = table.css("background-color").split(',')
        var opacity = arr[arr.length - 1].split(')')[0];
        if (opacity == 0) {
            table.css("background", '#fff');
        }
        table.attr('id', 'detialtable');
        table.addClass('table-bordered');
        var width = table.css("width");
        if (table.closest(".bookingorder").data("servicetype") == 2 || table.closest(".bookingorder").data("servicetype") == 3) {
            table.closest(".bookingorder").find('.orderdetailtips:eq(0)').css("width", width + "").html(
                '<span style="color:red">重要提醒</span>：' +
                '<span>' +
                '如有司机接人的行程，请您按最早的接人时间在酒店大堂等司机，司机到达酒店后只等待5分钟，如果由于您的迟到没有赶上当天行程，船公司会按NoShow算，不退款的哦！' +
                '</span>'
            );
        }
    }

    $('body').on('shown.bs.modal', '#advancedsearch', function() {
        $(this).find('#picname').text($('#picturetitle').text() + '.jpeg');
        $(this).find('#title').val($('#picturetitle').text());
        $(this).find('#toMail').val($('#toMailHidden').val());
        $(this).find('#confirmadvancedsearch').text("发送");

    });

    $('body').on('click', '#confirmadvancedsearch', function() {
        var t = {};
        t.toMail = $('#advancedsearch').find("#toMail").val().trim();
        t.title = $('#advancedsearch').find("#title").val().trim();
        t.customerName = $('#advancedsearch').find("#customerName").val().trim();
        var cancel = false;
        if (!isEmail(t.toMail)) {
            $('#advancedsearch').find("#toMail").warning("");
            cancel = true;
        }
        if ((t.title) == '') {
            $('#advancedsearch').find("#title").warning("");
            cancel = true;

        }
        if (cancel) {

            return;
        }
        var _this = $(this);
        var postmail = false;
        html2canvas(
            document.getElementById("onebookingorder"), {
                letterRendering: true,
                onrendered: function(canvas) {
                    if (canvas.msToBlob) { //for IE
                        var data = canvas.msToBlob();

                    } else {
                        var data = canvas.toDataURL("image/png", 2);

                    }
                    imgData = data.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
                    jQuery("#showimg").attr("src", data)
                    jQuery.ajax({
                        url: '/Orders/UploadPic',
                        contentType: "application/json; charset=utf-8;",
                        data: JSON.stringify({
                            imageData: imgData,
                            imageName: $('#picturetitle').text() + '.png'
                        }),
                        type: 'post',
                        dataType: 'json',
                        beforeSend: function(xhr) {
                            _this.text("正在发送图片..")
                            _this.prepend('<i class="fa fa-spinner fa-spin"></i>');

                        },
                        success: function(data) {
                            if (data.ErrorCode == 200) {
                                jQuery.ajax({
                                    url: '/Orders/SendMail',
                                    contentType: "application/json; charset=utf-8;",
                                    data: JSON.stringify({
                                        OrderID: $('#OrderID').val(),
                                        title: t.title,
                                        toMail: t.toMail,
                                        customerName: t.customerName,
                                        fileName: $('#picturetitle').text() + '.jpeg',
                                        filePath: data.Pic_Path
                                    }),
                                    type: 'post',
                                    dataType: 'json',
                                    success: function(data2) {
                                        if (data.errorCode == 200) {

                                        }
                                    },
                                    beforeSend: function(xhr) {
                                        _this.text("正在发送邮件..")
                                        _this.prepend('<i class="fa fa-spinner fa-spin"></i>');

                                    },
                                    complete: function(XHR, TS) {
                                        var response = JSON.parse(XHR.responseText);
                                        if (TS == "success" && response.statusCode == 200) {
                                            postmail = true;

                                            setTimeout(function() {
                                                _this.find('i').remove();
                                                // _this.text("发送邮件成功！ ");
                                                _this.success("发送邮件成功！ ");
                                            }, 500)
                                            setTimeout(function() {
                                                _this.closest('#advancedsearch').modal("hide");
                                            }, 1000)

                                        } else {
                                            setTimeout(function() {
                                                _this.find('i').remove();
                                                _this.text("发送邮件失败！ ");
                                            }, 500)
                                        }
                                    }

                                })
                            } else {
                                setTimeout(function() {
                                    _this.find('i').remove();
                                    _this.text("发送图片失败！");
                                }, 500);
                            }
                        },

                        complete: function(XHR, TS) {
                            if (TS !== "success") {
                                setTimeout(function() {
                                    _this.find('i').remove();
                                    _this.text("发送图片失败！");
                                }, 500);

                            }

                        }
                    });

                }
            }
        );
    });

    $('input#totalcost,input#TrafficSurcharge').onlyNum();
    if ($("input#TrafficSurcharge").val() != 0) {
        $("input#TrafficSurcharge").css({
            "color": "red",
            "font-weight": "bold"
        })
    } else {
        $("input#TrafficSurcharge").css({
            "color": "#333",
            "font-weight": "normal"
        })
    }
    $("input#TrafficSurcharge").on("change", function() {
        if ($("input#TrafficSurcharge").val() != 0) {
            $("input#TrafficSurcharge").css({
                "color": "red",
                "font-weight": "bold"
            })
        } else {
            $("input#TrafficSurcharge").css({
                "color": "#333",
                "font-weight": "normal"
            })
        }
    })


    //修改
    $('#reviseGT').one('click', function a() {
        var _this = $(this);

        var orderID = $("#OrderID").val();

        var p = {};
        p.OrderID = orderID,
            p.GroupNo = $('#GroupNo').val().trim() || '';
        p.ReceiveManTime = $('#ReceiveManTime').val().trim() || '';
        p.Remark = $('#Remark').val().trim() || '';
        p.TrafficSurcharge = $('#TrafficSurcharge').val().trim() || '';
        p.CurrencyID = $('#CurrencyID').val().trim() || '';
        p.CurrencyName = $('#CurrencyID').find("option:selected").text() || '';


        $.ajax({
            url: '/Orders/UpdateGroupNo',
            type: "post",
            contentType: "application/json,charset=utf-8;",
            data: JSON.stringify(p),
            dataType: 'json',
            beforeSend: function() {
                _this.text("正在保存..");
                _this.prepend('<i class="fa fa-spinner fa-spin"></i>');
            },
            success: function(data) {
                if (data.ErrorCode == 200) {

                    setTimeout(function() {
                        _this.text("保存成功！");
                        window.location.reload();

                    }, 500)
                    setTimeout(function() {
                        _this.text("保存");
                        _this.one("click", a);
                    }, 1500)
                } else {

                    setTimeout(function() {
                        _this.text("保存失败!");

                    }, 500)
                    setTimeout(function() {
                        _this.text("保存");
                        _this.one("click", a);
                    }, 1500)
                }


            },
            error: function() {

                setTimeout(function() {
                    _this.text("保存失败!");

                }, 500)
                setTimeout(function() {
                    _this.text("保存");
                    _this.one("click", a);
                }, 1500)
            }
        })



    });

    $('#reiveseCost').one('click', function a() {
        var _this = $(this);
        var orderID = $('#OrderID').val();
        var TotalCost = $('input#totalcost').val().trim();

        if (TotalCost == '') {
            $('input#totalcost').addClass("warning").one("focus", function() {
                $(this).removeClass("warning")
            }).one("click", function() {
                $(this).removeClass("warning");
            });
            return;

        }
        $.ajax({
            url: '/Orders/UpdateTotalCost',
            type: "post",
            contentType: "application/json,charset=utf-8;",
            data: JSON.stringify({
                OrderID: orderID,
                TotalCost: TotalCost
            }),
            dataType: 'json',
            beforeSend: function() {
                _this.text("正在保存..");
                _this.prepend('<i class="fa fa-spinner fa-spin"></i>');
            },
            success: function(data) {
                if (data.ErrorCode == 200) {

                    setTimeout(function() {
                        _this.text("保存成功！");

                    }, 500)
                    setTimeout(function() {
                        _this.text("保存");
                        _this.one("click", a);
                    }, 1500)
                } else {

                    setTimeout(function() {
                        _this.text("保存失败");

                    }, 500)
                    setTimeout(function() {
                        _this.text("保存");
                        _this.one("click", a);
                    }, 1500)
                }
            },
            error: function() {

                setTimeout(function() {
                    _this.text("保存失败");

                }, 500)
                setTimeout(function() {
                    _this.text("保存");
                    _this.one("click", a);
                }, 1500)
            }
        });
    });



    //保存功能
    $('#download').bind('click', function() {
        html2canvas(
            document.getElementById("onebookingorder"), {
                letterRendering: true,
                onrendered: function(canvas) {
                    if (canvas.msToBlob) { //for IE
                        var blob = canvas.msToBlob();
                        window.navigator.msSaveBlob(blob, $('#picturetitle').text() + '.png');
                    } else {

                        var data = canvas.toDataURL("image/png", 1);

                        var link = document.createElement("a");
                        link.download = $('#picturetitle').text() + '.png';
                        link.href = data;
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }
                }
            }
        );
    });

    //下载PDF
    $('#downloadPDF').bind('click', function() {
        var isNotes　 = "";
        var tips = "";
        var orderstatus = ($("#orderState").text() == "RequestCancel" || $("#orderState").text() == "CancelReceive") ? true : false;
        var table = jQuery('table').first();
        if (table.length != 0) {
            isNotes = (table.closest(".bookingorder").data("servicetype") == 2 || table.closest(".bookingorder").data("servicetype") == 3) ? true : false;
        }
        var varURL = "/Orders/DownloadPdf";
        var orderID = $('#OrderID').val();
        var link = document.createElement("a");
        link.id = "downloadingPdf";
        var fileName = $('#picturetitle').text().trim() + ".pdf";
        var supplierEnName = table.closest(".bookingorder").data("supplierenname");
        link.href = varURL + "?id=" + orderID + "&fileName=" + fileName.urlSwitch() + "&isNotes=" + isNotes + "&isCancel=" + orderstatus + "&supplierenname=" + supplierEnName.toString().urlSwitch();
        link.className = "LINKDOWNLOAD";
        document.body.appendChild(link);
        link.click();
        delete link;
        $("#downloadingPdf").remove();
    });

    //下载Word
    $('#downloadWord').bind('click', function() {
        var isNotes = "";
        var tips = "";
        var orderstatus = ($("#orderState").text() == "RequestCancel" || $("#orderState").text() == "CancelReceive") ? true : false;
        var table = jQuery('table').first();
        if (table.length != 0) {
            isNotes = (table.closest(".bookingorder").data("servicetype") == 2 || table.closest(".bookingorder").data("servicetype") == 3) ? true : false;;
        }
        var varURL = "/Orders/DownloadWord";
        var orderID = $('#OrderID').val();
        var link = document.createElement("a");
        link.id = "downloadingWord";
        var fileName = $('#picturetitle').text().trim() + ".doc";
        var supplierEnName = table.closest(".bookingorder").data("supplierenname");
        link.href = varURL + "?id=" + orderID + "&fileName=" + fileName.urlSwitch() + "&isNotes=" + isNotes + "&isCancel=" + orderstatus + "&supplierenname=" + supplierEnName.toString().urlSwitch();
        link.className = "LINKDOWNLOAD";
        document.body.appendChild(link);
        link.click();
        delete link;
        $("#downloadingWord").remove();
    });

    $('#print2x').bind('click', function() {
            var _this = $(this);
            var tipsWidth = "";
            var tips = "";
            var table = jQuery('table').first();
            if (table.length != 0) {
                if (table.closest(".bookingorder").data("servicetype") == 2 || table.closest(".bookingorder").data("servicetype") == 3) {
                    tipsWidth = table.css("width");
                    tips = '<span style="color:red">重要提醒</span>：' +
                        '<span>' +
                        '如有司机接人的行程，请您按最早的接人时间在酒店大堂等司机，司机到达酒店后只等待5分钟，如果由于您的迟到没有赶上当天行程，船公司会按NoShow算，不退款的哦！' +
                        '</span>'
                }

            }
            var varURL = "/Orders/Print";
            var orderID = $('#OrderID').val();
            varURL = varURL + "?id=" + orderID + "&tipsWidth=" + tipsWidth
            jQuery.ajax({
                url: '/Orders/Print',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    id: orderID,
                    tipsWidth: tipsWidth
                }),
                type: 'post',
                dataType: 'json',
                beforeSend: function(xhr) {
                    _this.text("正在加载..")
                    _this.prepend('<i class="fa fa-spinner fa-spin"></i>');

                },
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        _this.find('i').remove();
                        _this.text("加载成功！");
                        var link = document.createElement("a");
                        link.href = data.UrlPath;
                        link.className = "LINKDOWNLOAD";
                        document.body.appendChild(link);
                        link.click();
                        delete link;
                    } else {
                        setTimeout(function() {
                            _this.find('i').remove();
                            _this.text("加载失败！");
                        }, 500)
                    }
                },

                complete: function(XHR, TS) {
                    if (TS !== "success") {
                        setTimeout(function() {
                            _this.find('i').remove();
                            _this.text("加载失败！");
                        }, 500)
                    } else {
                        setTimeout(function() {
                            _this.find('i').remove();
                            _this.text("打印");
                        }, 500)
                    }

                }
            })

        })
        //打印订单
    jQuery('#print2').bind("click", function() {

        jQuery('#noprinting').css("display", "none");
        jQuery('#printing').html(jQuery('#onebookingorder').clone()).show();
        var bodyMin = $('body').css("min-width");
        var boypadding = $('body').css("padding");
        $('body').css("padding", '0px');
        $('body').css("min-width", '0px');

        var orderPaddingLeftPX = $('#printing #onebookingorder').css("padding-left");
        var orderPaddingRightPX = $('#printing #onebookingorder').css("padding-right");

        var orderPaddingLeft = orderPaddingLeftPX.split("px")[0];
        var orderPaddingRight = orderPaddingRightPX.split("px")[0];


        var orderWidthPX = $('#printing #onebookingorder').css("width");
        var orderWidth = orderWidthPX.split("px")[0];
        var orderHeightPX = $('#printing #onebookingorder').css("height");
        var orderHeight = orderHeightPX.split("px")[0];
        var pageFullWidthPX = jQuery('#printing').css('width');
        var pageFullWidth = pageFullWidthPX.split("px")[0];

        var Width = parseInt(orderWidth) + parseInt(orderPaddingLeft) + parseInt(orderPaddingRight);
        $('#printing ').css("left", "50%");
        $('#printing ').css("position", "relative");
        $('#printing ').css("display", "inline-block");

        $('#printing ').css("margin-left", '-' + orderWidth / 2 + "px");
        var count = 100;

        function　 detect() {
            var loop = false;
            if (jQuery("#noprinting").css("display") != 'none') {
                loop = true;
            }
            if ($('body').css("min-width") === bodyMin) {
                loop = true;
            }
            count--;
            if (count === 0) {
                loop = false;
            }
            return loop;
        }
        while (detect()) {}
        window.print();
        $('body').css("min-width", bodyMin);
        $('body').css("padding", boypadding);
        jQuery('#noprinting').show();
        jQuery('#printing').hide();
    });

    // 更新模板
    $("body").on('click', '#updateTemp', function() {
        var _this = $(this);

        var orderID = $('#OrderID').val();
        jQuery.ajax({
            url: '/Orders/UpdateTemplte',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                OrderID: orderID
            }),
            type: 'post',
            dataType: 'json',
            beforeSend: function(xhr) {
                _this.text("正在更新模板..")
                _this.prepend('<i class="fa fa-spinner fa-spin"></i>');

            },
            success: function(data) {

                if (data.ErrorCode == 200) {
                    _this.find('i').remove();
                    _this.text("更新模板成功！");
                    window.location.reload();
                } else {
                    setTimeout(function() {
                        _this.find('i').remove();
                        _this.text("更新模板失败！");
                    }, 500)
                }
            },

            complete: function(XHR, TS) {
                if (TS !== "success") {
                    setTimeout(function() {
                        _this.find('i').remove();
                        _this.text("更新模板失败！");
                    }, 500)
                } else {
                    setTimeout(function() {
                        _this.find('i').remove();
                        _this.text("更新模板");
                    }, 500)
                }

            }
        })
    });

    //请求变更显示
    $('#onebookingorder')
        .on("mouseenter", '.orderchange', function() {
            var displayText = $(this).data("change");
            var base = $('#hiddenRelative');
            var wrapperTD = $(this).closest("td");
            var width = wrapperTD.css("width");
            var str = '<div class="rechagne">' + displayText + '</div>';
            var jq = (str);
            var leftBase = base.offset().left;
            var topBase = base.offset().top;
            var left = wrapperTD.offset().left;
            var top = wrapperTD.offset().top;
            var leftR = left - leftBase;
            var topR = top - topBase + parseInt(wrapperTD.css('height').split('px')[0]); //css默认是border-box
            var divOffset = {
                left: leftR,
                top: topR,
                width: parseInt(width.split('px')[0]) + 1,
                position: 'absolute',
                'z-index': 300,
                background: 'white',
                border: '1px solid #797979',
                'box-shadow': '5px 5px 5px rgba(0,0,0,0.35)',
                padding: '5px'
            };
            $('#hiddenRelative').append(jq).find(".rechagne").css(divOffset)
        })
        .on('mouseleave', '.orderchange', function() {
            $('#hiddenRelative').empty();
        });

    //日志处理
    $(".table-dairy-mini:eq(0)").DataTable({
        'ordering': false,
        'searching': false,
        'stateSave': false,
        'pageLength': 10,
        'pagingType': 'bootstrap_Langhua_mini',
        "dom": "<'bootstrap-Langhua-mini-container' t>" +
            "<'bootstrap-Langhua-mini-page-container'<'row'<'col-md-6 col-sm-6'i><'col-md-6 col-sm-6'   p>>>",
    });

    //订单流转
    $('#operations').on("click", "a", function() {
        var state = $(this).data('next-code');
        var OrderID = $('#OrderID').val();
        $.ajax({
            url: "/Orders/UpdateState",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                OrderID: OrderID,
                state: state
            }),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
                if (data.failed.length == 0) {

                    $.LangHua.alert({
                        tip1: "订单操作结果",
                        tip2: "操作成功！",
                        callback: function() {
                            window.location.reload();
                        }
                    });
                    return;
                }

                var failed = data.failed;
                var str = '';
                for (var i in failed) {
                    var arr = [
                        '<div style="margin:10px 0px">',
                        '<span style="color:#0099cc">' + failed[i]['OrderNo'] + '：</span>',
                        '<spanstyle="color:#333" >' + failed[i]['reason'] + '</span>',
                        '</div>',
                    ].join('\n');
                    str += arr;

                }


                var t = [
                    '<div  class="modal modal-animate" tabindex="-1" data-backdrop="static" data-width="500" data-max-height=200>',
                    '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                    '<div class="modal-header">',
                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
                    '<h4 class="modal-title">操作结果</h4>',
                    '</div>',
                    '<div class="modal-body">',
                    str,
                    '</div>',
                    '<div class="modal-footer">',
                    '<a  class="btn btn-sm btn-primary button70" data-dismiss="modal">确定</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                ].join("\n");
                $(t).modal();
            }
        });
    });


});