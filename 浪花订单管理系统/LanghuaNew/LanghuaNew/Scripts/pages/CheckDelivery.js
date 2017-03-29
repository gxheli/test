jQuery(document).ready(function() {
    $('#SendState').find(".buttonradio[data-code=0]")
        .addClass("active")
        .siblings(".active").removeClass("active");

    $("#upfile").bind("click", function() {
        $("#files").trigger("click");
    })

    $("#files").bind("change", function(e) {
        var fileObj = document.getElementById("files").files[0];
        if (fileObj) {
            $('form').submit();
        }
    })

    $('div').on('copy', '.copy', function(e) {
        var tbid = $(this).data('tbid');
        e.clipboardData.clearData();
        e.clipboardData.setData("text/plain", tbid);
        e.preventDefault();
        $(this).success("复制成功")
    })



    var table =
        jQuery('table#DeliveryList')
        .DataTable({
            ordering: true,
            order: [],
            "dom": "<'table-scrollable't>" +
                "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7 LanghuaPaginator' l p>>",
            searching: true,
            serverSide: false,
            // buttons: [{
            //     extend: 'excel',
            //     text: '导出整个列表',
            //     tag: 'button',
            //     name: "usrdExport",
            //     className: 'btn btn-sm btn-default',
            //     exportOptions: {
            //         modifier: {
            //             page: 'current'
            //         },
            //         columns: [0, 1, 2],
            //         format: {
            //             body: function(data, rowId, columnId, node) {
            //                 if (columnId === 1) {
            //                     var arr = data.split('<br>');
            //                     for (var i in arr) {
            //                         arr[i] = arr[i] + '，';
            //                     }
            //                     console.log(arr);
            //                     return arr.join("");
            //                 } else if (columnId === 0) {
            //                     return $(node).text();
            //                 } else {
            //                     return data;
            //                 }
            //             }
            //         }
            //     }
            // }],
            columns: [
                { "orderable": true },
                { "orderable": true },
                { "orderable": true },
                { "orderable": false },
            ]


        });
    new $.fn.dataTable.Buttons(table, {
        buttons: [{
            extend: 'excel',
            text: '导出当前页',
            tag: 'button',
            name: "usrdExport",
            className: 'btn btn-sm btn-default',
            exportOptions: {
                modifier: {
                    page: 'current'
                },
                columns: [0, 1, 2],
                format: {
                    body: function(data, rowId, columnId, node) {
                        if (columnId === 1) {
                            var arr = data.split('<br>');
                            for (var i in arr) {
                                arr[i] = arr[i] + '，';
                            }
                            return arr.join("");
                        } else if (columnId === 0) {
                            return $(node).text();
                        } else {
                            return data;
                        }
                    }
                }
            }
        }, {
            extend: 'excel',
            text: '导出整个列表',
            tag: 'button',
            name: "usrdExportCurrent",
            className: 'btn btn-sm btn-default',
            exportOptions: {
                modifier: {
                    // page: 'current'
                },
                columns: [0, 1, 2],
                format: {
                    body: function(data, rowId, columnId, node) {
                        if (columnId === 1) {
                            var arr = data.split('<br>');
                            for (var i in arr) {
                                arr[i] = arr[i] + '，';
                            }
                            return arr.join("");
                        } else if (columnId === 0) {
                            return $(node).text();
                        } else {
                            return data;
                        }
                    }
                }
            }
        }],
    });
    table
        .buttons()
        .containers()
        .appendTo('#panel');

    //发货状态的筛选
    $("#SendState").ButtonRadio({
        selected: function(dom, code) {
            if (code == 1) {
                $('tbody tr.nodelivery').removeClass('hidden');
                table
                    .columns(2)
                    .search('漏发货')
                    .draw();
            } else if (code == 2) {
                table
                    .columns(2)
                    .search('无订单')
                    .draw();
            } else if (code == 3) {
                table
                    .columns(2)
                    .search('静默')
                    .draw();
            } else {
                table
                    .columns(2)
                    .search('')
                    .draw(true);
            }
        }
    })
})