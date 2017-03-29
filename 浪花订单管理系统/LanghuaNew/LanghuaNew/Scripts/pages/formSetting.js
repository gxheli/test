    // 删除缓存code 和 空值 的{} 

    function BasicElements(reactType, serviceType, meaningType, arr, defaultCodes) {
        var codes = defaultCodes ? defaultCodes : new Object();

        this.type = reactType;
        this.mandatory = "";
        this.serviceType = serviceType;
        this.meaningType = meaningType;




        this.needExtraInfo = "";
        if (meaningType == "DivingMember") {
            this.needExtraInfo = "2";
        }
        this.timeAdvanced = "";
        this.timeSince = "";
        this.timeBefore = "";
        this.areaID = "";
        this.countryID = "";
        this.list = [];
        this.isForMemberlist = '';
        this.cancelAble = false;
        this.nights = 0;
        this.ageMin = -1;
        this.ageMax = -1;


        this.elements = {};
        var systemFieldMapUnique = new Array();
        for (var i in arr) {

            this.elements[arr[i]['name']] = {
                title: codes[arr[i]['coderelated']] ? codes[arr[i]['coderelated']]['FieldName'] : "",
                tips: codes[arr[i]['coderelated']] ? codes[arr[i]['coderelated']]['Remark'] : "",
                code: codes[arr[i]['coderelated']] ? codes[arr[i]['coderelated']]['FieldNo'] : "",
                systemFieldMap: arr[i]['systemFeildsArrs'] //未映射则是undefined
            }
            if (arr[i]['systemFeildsArrs']) {
                for (var j in arr[i]['systemFeildsArrs']) {
                    systemFieldMapUnique.push(arr[i]['systemFeildsArrs'][j]['systemfield'])
                }
            }
        }
        this.systemFieldMapUnique = systemFieldMapUnique;


    }

    function initMenu(defaultCodes) {
        var arr = [{
                text: "名单类",
                children: (function() {
                    var arrs = new Array();
                    var obj = new Object();
                    obj.text = "参加行程的客人";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'PersonPicker',
                        'PersonPicker',
                        'PersonPicker', [{ "name": "single", "coderelated": "MemberList" }],
                        defaultCodes
                    );
                    arrs.push(obj)


                    var obj = new Object();
                    obj.text = "参加深潜的客人";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'PersonPicker',
                        'PersonPicker',
                        'DivingMember', [{ "name": "single", "coderelated": "DivingMember" }],
                        defaultCodes
                    );

                    arrs.push(obj)


                    // var obj = new Object();
                    // obj.text="入住酒店的客人";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'PersonPicker',
                    //     'PersonPicker',
                    //     'PersonPicker',
                    //     [{"name":"single","coderelated":"HotalsList"}],
                    //     defaultCodes
                    // );

                    // arrs.push(obj)

                    return arrs;
                })()
            },
            {
                text: "日期时间类",
                children: (function() {
                    var arrs = new Array();
                    var obj = new Object();
                    obj.text = "航班起飞+到达日期时间";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Flight_takeofftime_arrivaltime',
                        'Flight_takeofftime_arrivaltime',
                        'Flight_takeofftime_arrivaltime', [
                            { "name": "takeofftime", "coderelated": 'DepartureTime' },
                            {
                                "name": "arrivaltime",
                                "coderelated": 'ArrivalTime',
                                'systemFeildsArrs': new Array({
                                    'systemfield': "ServiceDate",
                                    'postKey': 'TravelDate'
                                })
                            }
                        ],
                        defaultCodes

                    );
                    arrs.push(obj)


                    var obj = new Object();
                    obj.text = "航班起飞+接人日期时间";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Flight_takeofftime_pickuptime',
                        'Flight_takeofftime_pickuptime',
                        'Flight_takeofftime_pickuptime', [
                            { "name": "takeofftime", "coderelated": "DepartureTime" },
                            {
                                "name": "pickuptime",
                                "coderelated": "PickupTime",
                                'systemFeildsArrs': new Array({
                                        'systemfield': "ServiceDate",
                                        'postKey': 'TravelDate'
                                    }

                                )
                            }
                        ],
                        defaultCodes

                    );
                    arrs.push(obj)


                    // var obj = new Object();
                    // obj.text="出行+返回日期";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'Date_setoutdate_returndate',
                    //     'Date_setoutdate_returndate',
                    //     'Date_setoutdate_returndate',
                    //     [
                    //         {"name":"setoutdate","coderelated":"ServiceDate"},
                    //         {"name":"returndate","coderelated":"BackDate"}
                    //     ],
                    //     defaultCodes
                    // );
                    // arrs.push(obj)

                    // var obj = new Object();
                    // obj.text="出行日期";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'DatePicker',
                    //     'DatePicker',
                    //     'DateService',
                    //     [
                    //         {"name":"single","coderelated":"ServiceDate"},

                    //     ],
                    //     defaultCodes
                    // );
                    // arrs.push(obj)

                    // var obj = new Object();
                    // obj.text="返回日期";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'DatePicker',
                    //     'DatePicker',
                    //     'Dateout',
                    //     [
                    //         {"name":"single","coderelated":"BackDate"},

                    //     ],
                    //     defaultCodes
                    // );
                    // arrs.push(obj)

                    var obj = new Object();
                    obj.text = "日期选择";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'DatePicker',
                        'DatePicker',
                        'DatePicker', [
                            { "name": "single", "coderelated": "SomeDate" },

                        ],
                        defaultCodes
                    );
                    arrs.push(obj)


                    var obj = new Object();
                    obj.text = "服务时间";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'TimePicker',
                        'TimePicker_service',
                        'TimePicker_service', [
                            { "name": "single", "coderelated": "ServiceTime" },

                        ],
                        defaultCodes
                    );
                    arrs.push(obj)


                    // var obj = new Object();
                    // obj.text="接人时间";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'TimePicker',
                    //     'TimePicker_pickup',
                    //     'TimePicker_pickup',
                    //     [
                    //         {"name":"single","coderelated":"PickupTime"},

                    //     ],
                    //     defaultCodes
                    // );
                    // arrs.push(obj)


                    return arrs;
                })()

            },
            {
                text: "酒店地址类",
                children: (function() {
                    var arrs = new Array();
                    var obj = new Object();
                    obj.text = "出发酒店区域+名称+电话";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Hotel_area_name_tel',
                        'Hotel_area_name_tel',
                        'Hotel_area_name_tel', [
                            { "name": "area", "coderelated": "PickupHotelArea" },
                            { "name": "name", "coderelated": "PickupHotelName" },
                            { "name": "tel", "coderelated": "PickupHotelTel" },
                        ],
                        defaultCodes
                    );
                    arrs.push(obj)

                    var obj = new Object();
                    obj.text = "返回酒店区域+名称+电话";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Hotel_area_name_tel',
                        'Hotel_area_name_tel',
                        'Hotel_area_name_tel', [
                            { "name": "area", "coderelated": "ReturnHotelArea" },
                            { "name": "name", "coderelated": "ReturnHotelName" },
                            { "name": "tel", "coderelated": "ReturnHotelTel" },
                        ],
                        defaultCodes
                    );
                    arrs.push(obj)

                    var obj = new Object();
                    obj.text = "出发酒店区域+名称+电话+地址";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Hotel_area_name_tel_address',
                        'Hotel_area_name_tel_address',
                        'Hotel_area_name_tel_address', [
                            { "name": "area", "coderelated": "PickupHotelArea" },
                            { "name": "name", "coderelated": "PickupHotelName" },
                            { "name": "tel", "coderelated": "PickupHotelTel" },
                            { "name": "address", "coderelated": "PickupHotelAddress" },
                        ],
                        defaultCodes
                    );
                    arrs.push(obj)

                    var obj = new Object();
                    obj.text = "返回酒店区域+名称+电话+地址";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Hotel_area_name_tel_address',
                        'Hotel_area_name_tel_address',
                        'Hotel_area_name_tel_address', [
                            { "name": "area", "coderelated": "ReturnHotelArea" },
                            { "name": "name", "coderelated": "ReturnHotelName" },
                            { "name": "tel", "coderelated": "ReturnHotelTel" },
                            { "name": "address", "coderelated": "ReturnHotelAddress" },
                        ],
                        defaultCodes
                    );
                    arrs.push(obj)

                    var obj = new Object();
                    obj.text = "酒店名称";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'Hotelname',
                        'Hotelname',
                        'Hotelname', [
                            { "name": "single", "coderelated": "x" },

                        ],
                        defaultCodes
                    );
                    arrs.push(obj)



                    return arrs;
                })()
            },

            {
                text: "常见文本类",
                children: (function() {
                    var arrs = new Array();
                    var obj = new Object();
                    obj.text = "航班号";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'FlightNO',
                        'FlightNO',
                        'FlightNO', [{ "name": "single", "coderelated": "FlightNo" }],
                        defaultCodes
                    );
                    arrs.push(obj)


                    var obj = new Object();
                    obj.text = "包车路线（多行文本）";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'BusRoute',
                        'BusRoute',
                        'BusRoute', [{ "name": "single", "coderelated": "x" }],
                        defaultCodes
                    );

                    arrs.push(obj)


                    // var obj = new Object();
                    // obj.text="目的地名称";
                    // obj.enable =true;
                    // obj.elements=new BasicElements(
                    //     'Destination',
                    //     'Destination',
                    //     'Destination',
                    //     [{"name":"single","coderelated":"x"}],
                    //     defaultCodes
                    // );

                    // arrs.push(obj)

                    return arrs;
                })()
            },
            {
                text: "自定义类",
                children: (function() {
                    var arrs = new Array();
                    var obj = new Object();
                    obj.text = "文本输入框";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'TEXTinput',
                        'TEXTinput',
                        'TEXTinput', [{ "name": "single", "coderelated": "x" }],
                        defaultCodes
                    );
                    arrs.push(obj)


                    var obj = new Object();
                    obj.text = "下拉菜单";
                    obj.enable = true;
                    obj.elements = new BasicElements(
                        'SelectMenu',
                        'SelectMenu',
                        'SelectMenu', [{ "name": "single", "coderelated": "x" }],
                        defaultCodes
                    );

                    arrs.push(obj)



                    return arrs;
                })()
            },
        ];
        var str1 = [
            '<span class="onegroup" >',
            '<span class="grouphead">#groupTitle#',
            '<span class="span_fold  active">－</span>',
            '</span>',
            '<span class="groupmember">',
            '#list#',
            '</span>',
            '</span>',
        ].join('\n');
        var str2 = [
            '<span class=#onemember#  data-elements=#element#>',
            '<a class="delete">删除</a>',
            '<span class="name">#oneTitle#</span>',
            '<span class="unedit">未编辑</span>',
            '</span>',
        ].join('\n');
        var all = '';
        for (var i in arr) {
            var fullstr1 = str1;
            fullstr1 = fullstr1.replace("#groupTitle#", arr[i]['text']);
            var children = arr[i]['children'];
            var strings = '';
            for (var j in children) {
                var fullstr2 = str2;
                var onemember = children[j]['enable'] ? "onemember" : "onememberfalse"
                fullstr2 = fullstr2.replace("#element#", JSON.stringify(children[j]['elements']));
                fullstr2 = fullstr2.replace("#oneTitle#", children[j]['text']);
                fullstr2 = fullstr2.replace("#onemember#", onemember);
                strings += fullstr2;

            }
            fullstr1 = fullstr1.replace("#list#", strings);

            all += fullstr1;
        }
        $('#basicComponentList').empty().append(all)
    }












    jQuery(document).ready(function($) {

        //生成表单预选单
        ///////////////////////////////////////////////////////////
        var codes = (JSON.parse($('#FormField').text()));

        var systemCodes = new Object();
        var defaultCodes = new Object();

        var suffix = 0;
        for (var i in codes) {
            if (codes[i]['Key'] == "systemfield") {
                suffix++;
                systemCodes['systemfield' + suffix] = codes[i];
            } else {
                defaultCodes[codes[i]['Key']] = codes[i];
            }
        }
        initMenu(defaultCodes);

        //////////////////////////////////////////////////////////
        //系统字段映射值 配置
        //////////////////////////////////////////////////////////

        var systemFeildsNeedtoMap = new Object();

        systemFeildsNeedtoMap.time = new Array({
            code: 'ServiceDate',
            postField: "dateout",
            which: "-1",
            unique: true,
            text: '系统出行日期'
        }, {
            code: 'BackDate',
            which: (function() {
                var itemServiceType = $('#ItemType').text();
                if (itemServiceType == 2) {
                    return '-2';
                } else {
                    return "-1";
                }
            })(),
            postField: "dateback",
            unique: true,
            text: '系统返回日期',
        });
        systemFeildsNeedtoMap.person = new Array({
            code: 'MemberList',
            which: "-1",
            postField: "person",
            unique: true,
            text: '系统行程客人'
        })
        var systemFieldsCheck = new Object();
        for (var zz in systemFeildsNeedtoMap.time) {
            systemFieldsCheck[systemFeildsNeedtoMap.time[zz]['code']] = new Object();
            systemFieldsCheck[systemFeildsNeedtoMap.time[zz]['code']]['unique'] = systemFeildsNeedtoMap.time[zz]['unique'];
            systemFieldsCheck[systemFeildsNeedtoMap.time[zz]['code']]['which'] = systemFeildsNeedtoMap.time[zz]['which'];
        }

        for (var zz in systemFeildsNeedtoMap.person) {
            systemFieldsCheck[systemFeildsNeedtoMap.person[zz]['code']] = new Object();
            systemFieldsCheck[systemFeildsNeedtoMap.person[zz]['code']]['unique'] = systemFeildsNeedtoMap.person[zz]['unique'];
            systemFieldsCheck[systemFeildsNeedtoMap.person[zz]['code']]['which'] = systemFeildsNeedtoMap.person[zz]['which'];
        }



        /////////////////////////////////////////////////////////




        var IDNow = 1;

        // 自增加、无重复id
        var elementsTEMP = new Object();
        elementsTEMP['elements'] = new Object();
        elementsTEMP['existingCODE'] = new Object();
        elementsTEMP['systemFieldMapUnique'] = new Object();


        var saveFormSetting = jQuery('#savedFormSetting').text();

        if (saveFormSetting && saveFormSetting != {} && saveFormSetting != '{}') {
            if (!(saveFormSetting instanceof Object)) {
                saveFormSetting = JSON.parse(saveFormSetting);
            }
            var elementList = saveFormSetting.elementList;
            var systemFieldMapUnique = saveFormSetting.systemFieldsMap;

            var str = '';
            var stringx = '';
            var obj = {};
            var stARRSORT = [];

            for (var i in elementList) {
                IDNow = (parseInt(i) > parseInt(IDNow)) ? i : IDNow;
                if ('must' in elementList[i]) {
                    var deleting = 'deleteFALSE';
                } else {
                    var deleting = "delete";
                }

                var objx = {
                    sortIndex: elementList[i]['orderIndex'],
                    text: [
                        '<span data-edit=1 id="' + i + '" class="onemember" >',
                        '<a class="' + deleting + '">删除</a>\n',
                        '<span class="name ui-sortable-handle">' + elementList[i]['text'] + '</span>',
                        '<span class="edited">已编辑</span>',

                        '</span>',
                    ].join("\n")
                };
                stARRSORT.push(objx);

                str += [
                    '<span data-edit=1 id="' + i + '" class="onemember" >',
                    '<a class="delete">删除</a>\n',
                    '<span class="name ui-sortable-handle">' + elementList[i]['text'] + '</span>',
                    '</span>',
                ].join("");


                //code
                var arr = new Array();
                for (var j in elementList[i]['elements']) {
                    arr.push(elementList[i]['elements'][j]['code']);
                }
                obj[i] = arr;

            }
            stARRSORT.sort(function(a, b) {
                if (a.sortIndex > b.sortIndex) {
                    return true;
                }
            })

            for (var i in stARRSORT) {
                stringx += stARRSORT[i].text;
            }




            elementsTEMP['elements'] = elementList;
            elementsTEMP['existingCODE'] = obj;
            elementsTEMP['systemFieldMapUnique'] = systemFieldMapUnique;
        } else {
            elementsTEMP['elements'][0] = {
                areaID: "",
                countryID: "",
                timeAdvanced: "",
                timeBefore: "",
                timeSince: "",
                must: "",

                needExtraInfo: "",
                mandatory: "",
                type: "Date_setoutdate_returndate",
                serviceType: "Date_setoutdate_returndate",
                meaningType: "Date_setoutdate_returndate",
                systemFieldMapUnique: new Array('ServiceDate', 'BackDate'),
                elements: {
                    setoutdate: {
                        code: 'ServiceDate',
                        tips: '',
                        title: '出行日期',
                        systemFieldMap: new Array({
                            'postKey': "TravelDate",
                            'systemfield': 'ServiceDate'
                        }),
                        visible: true
                    },
                    returndate: {
                        code: 'BackDate',
                        tips: '',
                        title: '返回日期',
                        systemFieldMap: new Array({
                            'postKey': "ReturnDate",
                            'systemfield': 'BackDate'
                        }),
                        visible: (function() {
                            if ($('#isFixedDays').text() != 0) {
                                return false;
                            } else {
                                if ($('#ItemType').text() == 3) { //门票
                                    return false;
                                } else if ($('#ItemType').text() == 2) { //行程
                                    return true;
                                } else if ($('#ItemType').text() == 4) { //酒店
                                    return true;
                                } else if ($('#ItemType').text() == 1) { //交通
                                    return false;
                                } else {
                                    return true
                                }

                            }
                        })()
                    }
                }
            }
            var stringx = [
                '<span id="0" data-edit="0" class="onemember" >',
                '<a class="deleteFALSE">删除</></a>\n',
                '<span class="name">出行+返回日期</span>',
                '<span class="unedit">未编辑</span>',

                '</span>',
            ].join('\n')
        }


        $("#formSchedule").append(stringx);


















        // 列表收展
        $('#basicComponentList').on("click", '.span_fold', function() {
            if ($(this).hasClass("active")) {
                $(this).text("+");
                $(this).closest(".grouphead").siblings(".groupmember").hide(50)
            } else {
                $(this).text("-");
                $(this).closest(".grouphead").siblings(".groupmember").show(50)
            }
            $(this).toggleClass("active")
        })


        // 表单的位置拖拽
        $("#basicComponentList .onemember").draggable({
            connectToSortable: ".formSchedule",
            helper: 'clone',
            addClasses: false,

        });
        //组件位置的更改和表单接受组件
        $("#formSchedule").sortable({
            cursor: "pointer",
            handle: '.name',
            items: ".onemember",
            distance: "2",
            receive: function(event, ui) {
                jQuery(ui.helper).css("width", "350px");
                jQuery(ui.helper).css("height", "40px");
                //修复ie 问题

                IDNow++;
                $(ui.helper).attr("id", IDNow);
                $(ui.helper).data("edit", 0);
                elementsTEMP['elements'][IDNow] = $(ui.helper).data('elements'); //缓存ele数据

                elementsTEMP['existingCODE'][IDNow] = new Array(); //缓存字段代码数据
                var elements = $(ui.helper).data('elements')['elements'];
                for (var i in elements) {
                    elementsTEMP['existingCODE'][IDNow].push(elements[i]['code']);
                };
                var systemFieldMapUnique = $(ui.helper).data('elements')['systemFieldMapUnique'];

                for (var j in systemFieldMapUnique) {
                    if (systemFieldMapUnique[j] in elementsTEMP['systemFieldMapUnique']) {
                        // alert("the same");
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '组件重复',
                            tip2: '组件重复',
                            button: '确定',
                            callback: function() {
                                $(ui.helper).find("a.delete:eq(0)").trigger("click");
                            }
                        });
                        break;

                    } else {
                        elementsTEMP['systemFieldMapUnique'][systemFieldMapUnique[j]] = IDNow;
                    }

                }


            },
            over: function(event, ui) {
                jQuery(ui.helper).css("width", "350px");
                //修复ie 问题
            },
            sort: function(event, ui) {
                jQuery(ui.helper).css("width", "350px");
                //修复ie 问题
            }
        });



        //删除组件
        $("#formSchedule").on("click", ".delete", function() {
            var id = $(this).closest(".onemember").attr("id")
            delete elementsTEMP['elements'][id]; //删除对应的缓存数据
            delete elementsTEMP['existingCODE'][id];

            //还有清除这些 代码字段
            for (var d in elementsTEMP['systemFieldMapUnique']) {
                if (elementsTEMP['systemFieldMapUnique'][d] == id) {
                    delete elementsTEMP['systemFieldMapUnique'][d];
                }
            }

            if ($(this).closest(".onemember").hasClass('active')) {
                $(this).siblings(".name:eq(0)").trigger('click')
            }

            $(this).closest(".onemember").remove();

        })


        // 表单的内容配置
        $('#formSchedule').on("click", ".name", function() {
            $(this).closest(".onemember").toggleClass("active");
            $(this).closest(".onemember").siblings('.onemember').removeClass("active");
            if (!$(this).closest(".onemember").hasClass("active")) {
                $('#formEditWrapper').empty()
                return
            }


            var alt = $(this).text();
            var id = jQuery(this).closest(".onemember").attr('id');

            var elements = elementsTEMP['elements'][id];
            if (!(elements instanceof Object)) {
                elements = JSON.parse(elements);
            }
            settingFORM({
                id: id,
                alt: alt,
                reactType: elements.type,
                elements: elements,
                serviceType: elements.serviceType
            })

        })


        function settingFORM(obj) {
            var needExtraInfoFirst = "checked";
            var needExtraInfoSecond = "";
            if (obj.elements.needExtraInfo == 1) {
                needExtraInfoFirst = "checked";
                needExtraInfoSecond = "false";

            }
            if (obj.elements.needExtraInfo == 2) {
                needExtraInfoFirst = "false";
                needExtraInfoSecond = "checked";
            }


            var mandatoryFirst = "checked";
            var mandatorySecond = "";
            if (obj.elements.mandatory == 1) {
                mandatoryFirst = "checked";
                mandatorySecond = "";

            }
            if (obj.elements.mandatory == 2) {
                mandatoryFirst = "";
                mandatorySecond = "checked";
            }


            var isForMemberlistFirst = "checked";
            var isForMemberlistSecond = "";
            if (obj.elements.isForMemberlist == 1) {
                isForMemberlistFirst = "checked";
                isForMemberlistSecond = "";

            }
            if (obj.elements.isForMemberlist == 2) {
                isForMemberlistFirst = "";
                isForMemberlistSecond = "checked";
            }

            var isForMemberlistFirst = "checked";
            var isForMemberlistSecond = "";
            if (obj.elements.isForMemberlist == 1) {
                isForMemberlistFirst = "checked";
                isForMemberlistSecond = "";

            }
            if (obj.elements.isForMemberlist == 2) {
                isForMemberlistFirst = "";
                isForMemberlistSecond = "checked";
            }


            var renderFunctions = {
                // 选人
                PersonPicker: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }

                    var otherList = [
                        getNeedExtraInfo(needExtraInfoFirst, needExtraInfoSecond),
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getAgeRange(obj.elements)
                        // getIsForMemberlist(isForMemberlistFirst,isForMemberlistSecond)

                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')

                    $('#formEditWrapper').empty().append(head + rows + footer);
                },
                // 选日期
                DatePicker: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }
                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        // getSystemFieldTimeMap(systemFeildsNeedtoMap.time,-1)
                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')


                    $('#formEditWrapper').empty().append(head + rows + footer);
                },

                BaseDatePikcer: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                Flight_takeofftime_arrivaltime: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                //接人去机场
                Flight_takeofftime_pickuptime: function(obj) {

                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }

                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getTimeAdvanced(obj.elements.timeAdvanced)


                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')



                    $('#formEditWrapper').empty().append(head + rows + footer);

                },
                Date_setoutdate_returndate: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                TimePicker_pickup: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                //服务时间
                TimePicker_service: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }
                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getServicetime(obj.elements),
                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')


                    $('#formEditWrapper').empty().append(head + rows + footer);
                },
                Hotel_area_name_tel: function(obj) {

                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }
                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getHotelCountryArea({
                            countryID: obj.elements.countryID,
                            areaID: obj.elements.areaID,

                        }),
                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')





                    $('#formEditWrapper').empty().append(head + rows + footer);
                },

                Hotel_area_name_tel_address: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }
                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getHotelCountryArea({
                            countryID: obj.elements.countryID,
                            areaID: obj.elements.areaID,

                        }),
                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')
                    $('#formEditWrapper').empty().append(head + rows + footer);
                },
                Hotelname: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }

                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getHotelCountryArea({
                            countryID: obj.elements.countryID,
                            areaID: obj.elements.areaID,

                        }),


                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')

                    $('#formEditWrapper').empty().append(head + rows + footer);
                },
                FlightNO: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                BusRoute: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                Destination: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                TEXTinput: function(obj) {
                    var str = baserender(obj);
                    $('#formEditWrapper').empty().append(str);
                },
                SelectMenu: function(obj) {
                    var head = [
                        getHead(obj.alt, obj.serviceType, obj.id)

                    ].join('');
                    var elerows = [];
                    var eleList = obj.elements.elements;
                    for (var i in eleList) {
                        var signal = i;

                        elerows.push(
                            [
                                getChineseNname(eleList[i].title, signal),
                                getCode(eleList[i].code, signal),
                                getTips(eleList[i].tips, signal),
                            ].join('')
                        );

                    }

                    var otherList = [
                        getMandatory(mandatoryFirst, mandatorySecond),
                        getMenulist(obj.elements.list)


                    ];
                    var rows = elerows.concat(otherList).join('');

                    var footer = [
                        getFooter(),
                    ].join('')

                    $('#formEditWrapper').empty().append(head + rows + footer);
                }
            };

            renderFunctions[obj.serviceType](obj);

            $('.code').forbidChar(['_']);
            if ($('#timeAdvanced').length != 0) {
                $('#timeAdvanced').lastNum();
            }
            if ($('#timeSince').length != 0) {
                $('#timeSince').timepicker({
                    defaultTime: false,
                    showMeridian: false,
                }).on('show.timepicker', function(e) {
                    if (!$(this).val()) {
                        $('#timeSince').timepicker('setTime', '12:00');
                    }
                });;

                $('#timeBefore').timepicker({
                    defaultTime: false,
                    showMeridian: false,
                }).on('show.timepicker', function(e) {
                    if (!$(this).val()) {
                        $('#timeBefore').timepicker('setTime', '12:00');
                    }

                });
            }

            if ($('#ageMin').length != 0) {
                $('#ageMin').onlyNumWithEmpty();
            }
            if ($('#ageMax').length != 0) {
                $('#ageMax').onlyNumWithEmpty();
            }




            //国家区域切换

            $('body').on('change', '#countries', function(e, data) {
                var aID = data ? data : '-1';

                var option = $(this).find("option:selected");
                var classname = option.data("class");
                $("#areas option." + classname).removeClass("hidden");
                $("#areas").val(aID);

                $("#areas option").not("." + classname).addClass('hidden');

            })

            if ($('#countries').length != 0) {
                var lastvalue = $('#countries').data("value");
                if (!(lastvalue instanceof Object)) {
                    lastvalue = JSON.parse(lastvalue);
                }
                var cID = lastvalue.countryID ? lastvalue.countryID : "-1";
                var aID = lastvalue.areaID ? lastvalue.areaID : "-1";
                $('#countries').val(cID);
                $('#countries').trigger("change", [aID]);
            }

            function baserender(obj) {
                var head = [
                    getHead(obj.alt, obj.serviceType, obj.id)

                ].join('');
                var elerows = [];
                var eleList = obj.elements.elements;
                for (var i in eleList) {
                    var signal = i;

                    elerows.push(
                        [
                            getChineseNname(eleList[i].title, signal),
                            getCode(eleList[i].code, signal),
                            getTips(eleList[i].tips, signal),
                        ].join('')
                    );

                }

                var otherList = [
                    // getNeedExtraInfo(needExtraInfoFirst,needExtraInfoSecond),
                    getMandatory(mandatoryFirst, mandatorySecond),

                ];
                var rows = elerows.concat(otherList).join('');

                var footer = [
                    getFooter(),
                ].join('')
                return (head + rows + footer);
            }

            function getChineseNname(lastValue, which) {
                return [
                    '<div class="form-group" data-which="' + which + '">',
                    '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>中文名称：</label>',
                    '<div class="col-md-8">',
                    '<input id="chinseseName "  type="text" class=" must chinseseName form-control  input-sm" value="' + lastValue + '" placeholder="">',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getCode(lastValue, which) {
                return [
                    '<div class="form-group" data-which="' + which + '">',
                    '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;" >*</span>字段代号：</label>',
                    '<div class="col-md-8">',
                    '<input id="code" type="text" class=" must code form-control  input-sm" value="' + lastValue + '" placeholder="">',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getTips(lastValue, which) {
                return [
                    '<div class="form-group" data-which="' + which + '">',
                    '<label class="col-md-4 control-label" >提示信息：</label>',
                    '<div class="col-md-8">',
                    '<input id="tips" type="text" class=" tips form-control  input-sm" value="' + lastValue + '" placeholder="">',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getMandatory(mandatoryFirst, mandatorySecond) {
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">是否必填：</label>',
                    '<div class="col-md-8">',
                    '<label class="radio-inline">',
                    '<input value="1"  type="radio" name="mandatory"  ' + mandatoryFirst + '><span class="verticalMiddle">必填项</span>',
                    '</label>',
                    '<label class="radio-inline">',
                    '<input value="2" type="radio" name="mandatory" ' + mandatorySecond + '><span class="verticalMiddle">非必填项</span>',
                    '</label>',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getIsForMemberlist(mandatoryFirst, mandatorySecond) {
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">是否必填：</label>',
                    '<div class="col-md-8">',
                    '<label class="radio-inline">',
                    '<input value="1"  type="radio" name="isForMemberlist"  ' + mandatoryFirst + '>作为主要客人',
                    '</label>',
                    '<label class="radio-inline">',
                    '<input value="2" type="radio" name="isForMemberlist" ' + mandatorySecond + '>不作为主要客人',
                    '</label>',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getNeedExtraInfo(needExtraInfoFirst, needExtraInfoSecond) {
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">是否需要附加资料：</label>',
                    '<div class="col-md-8">',
                    '<label class="radio-inline">',
                    '<input type="radio" value="1" name="needExtraInfo" id="inlineCheckbox1" ' + needExtraInfoFirst + '><span class="verticalMiddle">不需要</span>',
                    '</label>',
                    '<label class="radio-inline">',
                    '<input type="radio" value="2" name="needExtraInfo" id="inlineCheckbox1" ' + needExtraInfoSecond + '><span class="verticalMiddle">需要</span>',
                    '</label>',
                    '</div>',
                    '</div>',
                ].join('');
            }

            function getFooter() {
                return [
                    '<div class="form-group">',

                    '<div class=" col-md-offset-4 col-md-8">',
                    '<a id="confirm" class="btn btn-sm btn-default button70  ">保存</a>',
                    '<a id ="cancel" class="btn btn-sm btn-default button70 alignRight">取消</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</form>',
                    '</div>',
                ].join('')
            }

            function getHead(alt, serviceType, id) {
                return [
                    '<div id="formEdit" data-id-for = ' + id + ' class="formEdit    ' + serviceType + ' ">',
                    '<div class="title">' + alt + '</div>',
                    '<form class="form-horizontal" role="form">',
                    '<div class="form-body" style="padding-top:0px;padding-bottom:0px">',
                ].join('')
            }


            function getTimeAdvanced(lastValue) {
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">配置：</label>',
                    '<div class="col-md-8">',
                    '必须提前 &nbsp<input id="timeAdvanced" style="width:30px;" type="text" class=" form-control input-inline input-sm" value="' + lastValue + '" placeholder="">&nbsp小时接人',
                    '</div>',
                    '</div>',
                ].join('')
            }


            function getHotelCountryArea(lastValue) {
                var allCountries = JSON.parse($('#allCountries').text());
                var country = '<option value="-1" class="country"  data-class="foralt">请选择</option>';
                var area = '<option value="-1" class="foralt">请选择</option>';
                for (var i in allCountries) {
                    country +=
                        '<option   class="country " value="' + allCountries[i]['CountryID'] + '"  data-class="' + allCountries[i]['CountryID'] + '" >' + allCountries[i]['CountryName'] + '</option>';
                    var citys = allCountries[i]['Citys'];
                    area += '<option value="-1" class="hidden ' + allCountries[i]['CountryID'] + '">请选择</option>';
                    for (var j in citys) {
                        area +=
                            '<option value="' + citys[j]['CityID'] + '" class="hidden ' + allCountries[i]['CountryID'] + '">' + citys[j]['CityName'] + '</option>';
                    }
                }

                // $("#formEdit #countries").append(country);
                // $("#formEdit #areas").append(area);

                return [
                    '<div class="form-group">',

                    '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;" >*</span>酒店国家地区：</label>',
                    '<div class="col-md-8">',
                    '<select  id="countries" data-value=' + JSON.stringify(lastValue) + ' class=" must  input100 input-inline form-control">',
                    country,
                    '</select>',
                    '<select id="areas" class=" must input-sm input100 input-inline form-control">',
                    area,
                    '</select>',
                    '</div>',
                    '</div>',
                ].join('')
            }

            function getOptionList(lastValue) {
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">配置：</label>',
                    '<div class="col-md-8">',
                    '<textarea class=" form-control"></textarea>',
                    '</div>',
                    '</div>',
                ].join('')
            }

            function getServicetime(obj) {
                var timeSinceCheck = obj.timeSinceCheck ? "checked" : "";
                var timeBeforeCheck = obj.timeBeforeCheck ? "checked" : "";
                var timeSince = obj.timeSince ? obj.timeSince : "";
                var timeBefore = obj.timeBefore ? obj.timeBefore : "";
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">配置：</label>',
                    '<div class="col-md-8">',
                    '<div>',
                    '<label class="checkbox-inline">',
                    '<input ' + timeSinceCheck + ' id="timeSinceCheck" type="checkbox"  >必须晚于：',
                    '<input id="timeSince" value="' + timeSince + '" style="width:62px" class="form-control input-inline input-sm " type="text"/>',

                    '</label>',
                    '</div>',
                    '<div>',
                    '<label class="checkbox-inline">',
                    '<input ' + timeBeforeCheck + ' id="timeBeforeCheck" type="checkbox"   ' + needExtraInfoSecond + '>必须早于：',
                    '<input id="timeBefore" value="' + timeBefore + '" style="width:62px" class="form-control input-inline input-sm " type="text"/>',
                    '</label>',
                    '</div>',

                    '</div>',
                    '</div>',
                ].join('')
            }

            function getAgeRange(obj) {
                var ageMinChecked = obj.ageMinChecked ? "checked" : "";
                var ageMaxChecked = obj.ageMaxChecked ? "checked" : "";
                var ageMin = (obj.ageMin != -1 && obj.ageMin != null && obj.ageMin != undefined) ? obj.ageMin : "";
                var ageMax = (obj.ageMax != -1 && obj.ageMax != null && obj.ageMax != undefined) ? obj.ageMax : "";
                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label">年龄限制</label>',
                    '<div class="col-md-8">',
                    '<div>',
                    '<label class="checkbox-inline">',
                    '<input ' + ageMinChecked + ' id="ageMinChecked" type="checkbox"  >必须大于：',
                    '<input id="ageMin" value="' + ageMin + '" style="width:62px" class="form-control input-inline input-sm " type="text"/>岁',
                    '</label>',
                    '</div>',
                    '<div>',
                    '<label class="checkbox-inline">',
                    '<input ' + ageMaxChecked + ' id="ageMaxChecked" type="checkbox"  >必须小于：',
                    '<input id="ageMax" value="' + ageMax + '" style="width:62px" class="form-control input-inline input-sm " type="text"/>岁',
                    '</label>',
                    '</div>',

                    '</div>',
                    '</div>',
                ].join('')
            }

            function getMenulist(arr) {


                return [
                    '<div class="form-group">',
                    '<label class="col-md-4 control-label"><span class="redspark" >*</span>选项配置：</label>',
                    '<div class="col-md-8">',
                    '<textarea id="selectList" class="form-control" rows=10 >',
                    (function() {
                        var str = "";
                        for (var i in arr) {
                            str += arr[i] + '\n';
                        }
                        return str;
                    })(),
                    '</textarea>',

                    '</div>',
                    '</div>',
                ].join('')

            }

            function getSystemFieldTimeMap(systemTime, lastValue) {
                var str = '<option value="-1">' + '请选择' + '</option>';;
                for (var j in systemTime) {
                    str +=
                        '<option data-systemField=' + JSON.stringify(systemTime[j]) + ' value="' + systemTime[j]['code'] + '">' + systemTime[j]['text'] + '</option>';
                };
                return (
                    [
                        '<div class="form-group">',
                        '<label class="col-md-4 control-label">映射系统字段：</label>',
                        '<div class="col-md-8">',
                        '<select  id="systemFieldMap" value=' + (lastValue) + ' class=" must  input160 input-inline form-control">',
                        str,
                        '</select>',
                        '</div>',
                        '</div>'
                    ].join('\n')
                );


            }




        }





        //暂存
        $('body').on('click', '#formEditWrapper  #confirm', function(e) {
            var cancel = false;
            var id = jQuery('#formEdit').data("id-for");
            var codeArr = elementsTEMP['existingCODE'];

            //判断 title 是否为中文
            jQuery('#formEdit input[type=text].must.chinseseName ').each(function() {
                    var value = $(this).val();
                    //  if(!isChineseChar(value)){
                    //      $(this).warning("必须是非空中文");
                    //      cancel = true;
                    //  }

                    if (!value) {
                        $(this).warning("必须是非空");
                        cancel = true;
                    }

                })
                // 判断字段 是否重复
            jQuery('#formEdit input[type=text].must.code  ').each(function() {
                var value = $(this).val();
                if (value.indexOf("_") > -1) {
                    $(this).warning('含有非法字符：" _ "');
                    ancel = true;
                }

                value = value.replace(" ", '');

                if (!(value)) {
                    $(this).warning("必须是非空");
                    cancel = true;
                }
                //外部比较
                for (var i in codeArr) {
                    for (var j in codeArr[i]) {
                        if (i == id) {
                            continue;
                        }
                        if (value == codeArr[i][j]) {
                            $(this).warning("与别的控件字段重复");
                            cancel = true;
                            break;
                        }
                    }
                }

                //与全局字段比较
                for (var sys in systemCodes) {
                    if (systemCodes[sys]['FieldNo'] == value) {
                        $(this).warning("与系统字段代号重复");
                        cancel = true;
                        break;
                    }
                }
            })



            var length = jQuery('#formEdit input[type=text].must.code').length;
            for (var i = 0; i < length - 1; i++) {

                for (var j = i; j < length - 1; j++) {
                    var next = parseInt(j) + 1;
                    var _thisValue = jQuery('#formEdit input[type=text].must.code:eq(' + j + ')').val();
                    var nextValue = jQuery('#formEdit input[type=text].must.code:eq(' + next + ')').val();
                    if (_thisValue == nextValue) {
                        jQuery('#formEdit input[type=text].must.code:eq(' + j + ')').warning('控件内部字段代号重复')
                        jQuery('#formEdit input[type=text].must.code:eq(' + next + ')').warning('控件内部字段代号重复')
                    }
                }
            }





            if ($('#timeSinceCheck').length != 0) {
                var timeSinceCheck = $('#timeSinceCheck').prop("checked");
                var timeBeforeCheck = $('#timeBeforeCheck').prop("checked");

                var timeSince = $('#timeSince').val();
                var timeBefore = $('#timeBefore').val();
                if (timeSinceCheck && !timeSince) {
                    $('#timeSince').warning("请输时间");
                    cancel = true;
                }
                if (timeBeforeCheck && !timeBefore) {
                    $('#timeBefore').warning("请输时间");
                    cancel = true;
                }
                if (timeSinceCheck && timeSince && timeBeforeCheck && timeBefore) {
                    var sinceValue = parseInt(timeSince.split(":")[0] * 60 + timeSince.split(":")[1]);
                    var beforeValue = parseInt(timeBefore.split(":")[0] * 60 + timeBefore.split(":")[1]);
                    if (sinceValue >= beforeValue) {
                        $('#timeSince').closest('.col-md-8').warning("时间冲突");
                        cancel = true;
                    }
                }


            }


            if ($('#ageMin').length != 0) {
                var ageMinChecked = $('#ageMinChecked').prop("checked");
                var ageMaxChecked = $('#ageMaxChecked').prop("checked");

                var ageMin = $('#ageMin').val();
                var ageMax = $('#ageMax').val();
                if (ageMinChecked && !ageMin) {
                    $('#ageMin').warning("请输入年龄");
                    cancel = true;
                }
                if (ageMaxChecked && !ageMax) {
                    $('#ageMax').warning("请输入年龄");
                    cancel = true;
                }
                if (ageMinChecked && ageMin && ageMaxChecked && ageMax) {
                    var ageMinValue = parseInt(ageMin);
                    var ageMaxValue = parseInt(ageMax);
                    if (ageMinValue > ageMaxValue) {
                        $('#ageMax').closest('.col-md-8').warning("年龄大小冲突");
                        cancel = true;
                    }
                }


            }


            if ($('#countries').length != 0) {

                var cID = $('#countries').val();
                var aID = $('#areas').val();
                if ((cID == -1) || (aID == -1)) {
                    $('#countries').closest(".col-md-8").warning('请选择国家地区');
                    cancel = true;
                }
            }

            //下拉菜单
            if ($('#selectList').length != 0) {
                var str = $('#selectList').val().trim();
                var arr = str.split("\n");
                for (var j in arr) {
                    arr[j] = arr[j].trim();
                }
                var z = 0;
                while (z < arr.length) {
                    if (arr[z] == "") {
                        arr.splice(z, 1);
                        z--;
                    }
                    z++;
                }
                if (arr.length == 0) {
                    cancel = true;
                    $('#selectList').warning("请输入至少一个选项");
                }

            }




            if (cancel) {
                return;
            }

            var ele = new Object();
            ele['elements'] = new Object();
            //中文名字
            jQuery('#formEdit .chinseseName ').each(function() {
                    var which = $(this).closest('.form-group').data('which');


                    if (!(which in ele['elements'])) {
                        ele['elements'][which] = new Object();
                    }
                    ele['elements'][which]['title'] = $(this).val();

                })
                //代码
            var thiscode = new Array();
            jQuery('#formEdit .code').each(function() {
                    var which = $(this).closest('.form-group').data('which');

                    if (!(which in ele['elements'])) {
                        ele['elements'][which] = new Object();
                    }
                    ele['elements'][which]['code'] = $(this).val();


                    thiscode.push($(this).val());


                })
                //提示语
            jQuery('#formEdit .tips ').each(function() {
                    var which = $(this).closest('.form-group').data('which');

                    if (!(which in ele['elements'])) {
                        ele['elements'][which] = new Object();
                    }
                    ele['elements'][which]['tips'] = $(this).val();

                })
                //必填 
            ele.mandatory = jQuery('input[name=mandatory]:checked').val();
            //必填 
            ele.isForMemberlist = jQuery('input[name=isForMemberlist]:checked').val();

            //额外资料
            if (jQuery('input[name=needExtraInfo]:checked').length != 0) {
                ele.needExtraInfo = jQuery('input[name=needExtraInfo]:checked').val();
            }

            //提前接送时间
            if ($('#timeAdvanced').length != 0) {
                var value = $('#timeAdvanced').val();
                var saveValue = value ? value : "";
                ele.timeAdvanced = saveValue;
            }

            // 年龄范围
            if ($('#ageMin').length != 0) {
                var ageMinChecked = $('#ageMinChecked').prop("checked");
                var ageMaxChecked = $('#ageMaxChecked').prop("checked");

                var ageMin = $('#ageMin').val() ? $('#ageMin').val() : -1;
                var ageMax = $('#ageMax').val() ? $('#ageMax').val() : -1;
                ele.ageMinChecked = ageMinChecked;
                ele.ageMaxChecked = ageMaxChecked;
                ele.ageMin = ageMin;
                ele.ageMax = ageMax;

            }

            if ($('#timeSinceCheck').length != 0) {
                var timeSinceCheck = $('#timeSinceCheck').prop("checked");
                var timeBeforeCheck = $('#timeBeforeCheck').prop("checked");

                var timeSince = $('#timeSince').val();
                var timeBefore = $('#timeBefore').val();
                ele.timeSinceCheck = timeSinceCheck;
                ele.timeBeforeCheck = timeBeforeCheck;
                ele.timeSince = timeSince;
                ele.timeBefore = timeBefore;

            }
            //国家去区域
            if ($('#countries').length != 0) {
                var countryID = $('#countries').val();
                var areaID = $('#areas').val();
                ele.countryID = countryID;
                ele.areaID = areaID;

            }


            //下拉菜单
            if ($('#selectList').length != 0) {
                var str = $('#selectList').val().trim();
                var arrx = str.split("\n");
                for (var j in arr) {
                    arr[j] = arr[j].trim()
                }
                var z = 0;
                while (z < arrx.length) {
                    if (arrx[z] == "") {
                        arrx.splice(z, 1);
                        z--;
                    }
                    z++;
                }

                ele.list = arrx;
            }

            //映射系统字段
            if ($('#systemFieldMap').length != 0) {
                var one = JSON.parse($('#systemFieldMap option:selected').attr('data-systemfield'));
                ele.systemFieldsMap = new Array();
                ele.systemFieldsMap.push(one);
            }

            ele.text = $(this).closest("form").siblings(".title:eq(0)").text();


            var oldData = elementsTEMP['elements'][id];

            var oldDatax = $.extend(true, {}, oldData, ele); //数组会最大化扩张，属性值是数组的一律要重新赋值

            if ($('#selectList').length != 0) {
                oldDatax.list = ele.list;
            }
            elementsTEMP['elements'][id] = oldDatax;

            //缓存字段
            elementsTEMP['existingCODE'][id] = thiscode;



            jQuery(this).success("ok");

            $('#formSchedule #' + id).data("edit", '1');
            $('#formSchedule #' + id).find('.unedit').text("已编辑").removeClass("unedit").addClass("edited");

        })

        $("body").on("click", '#cancel', function() {
            var id = jQuery('#formEdit').data("id-for");

            $("#formSchedule .onemember#" + id).find(".name:eq(0)").trigger("click")
        })


        //保存
        $('#confirmSaveFormSetting').one("click", function A() {

            var cancel = false;
            $('#formSchedule .onemember').each(function() {
                var edit = $(this).data('edit');
                if (edit == 0) {
                    cancel = true;
                    $(this).warning("")
                }
            })
            if (cancel) {
                $('a#confirmSaveFormSetting').one('click', A)
                return;

            }
            var BeforePost = elementsTEMP['elements'];
            var t = $('#formSchedule').sortable("toArray")
            var obj = new Object();
            for (var i in t) {
                obj[t[i]] = i;
            }
            for (var i in BeforePost) {
                BeforePost[i].orderIndex = obj[i]
            }
            var systemFields = elementsTEMP['systemFieldMapUnique'];

            var ElementContent = new Object();
            ElementContent.systemFieldsMap = systemFields;
            ElementContent.elementList = BeforePost;
            var ItemID = $('#ItemID').text();
            var thisbutton = $(this);

            $.ajax({
                url: "/ServiceItems/SaveFromSetting",
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    ElementContent: JSON.stringify(ElementContent),
                    ItemID: ItemID,
                    TemplteID: (function() {
                        var ServiceItemTemplteID = $('body #ServiceItemTemplteID').text();
                        ServiceItemTemplteID = ServiceItemTemplteID ? ServiceItemTemplteID : '';
                        return ServiceItemTemplteID;
                    })()
                }),
                dataType: 'json',
                beforeSend: function() {
                    thisbutton.buttonposting("正在保存");
                },
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        // thisbutton.success("保存成功");
                        thisbutton.posttedbutton("保存成功", '保存，去设置价格');

                        var ItemID = $('#ItemID').text();
                        window.location.href = "/ServiceItems/PriceSetting?ItemID=" + ItemID;
                    }
                },
                failded: function() {
                    $('a#nowsave').one('click', A);
                },
                complete: function(XHR, TS) {
                    if (TS !== "success") {
                        thisbutton.posttedbutton("保存失败", '保存，去设置价格');
                        $('a#nowsave').one('click', A);
                    }
                }
            })
        })


        $("#upfile").bind("click", function() {
            $("#getfile").trigger("click");
        })

        $("#getfile").bind("change", function(e) {
            upFileWithProgress(e);
        })





        //克隆


        var remote_cities = new Bloodhound({
            datumTokenizer: function(d) {
                return Bloodhound.tokenizers.whitespace(d.name);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 15,
            // 在文本框输入字符时才发起请求
            // 
            // local:dt,
            remote: {
                wildcard: '%QUERY',
                url: '/ServiceItems/GetItemsByStr?Str=%QUERY',
                filter: function(data) {
                    return $.map(data.data, function(one) {
                        return {
                            name: one.cnItemName,
                            serviceItemID: one.ServiceItemID,
                            code: one.ServiceCode,
                        };
                    });
                }
            }
        });
        remote_cities.initialize();
        $('#ServiceItems').typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
        }, {
            name: 'xxx',
            displayKey: 'name',
            limit: 30,
            source: remote_cities,
            templates: {
                empty: [
                    '<div class="empty-message">',
                    '没有找到相关产品',
                    '</div>'
                ].join('\n'),
                header: function(data) {
                    return ([
                        '<div class="empty-message">',
                        '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                        '</div>'
                    ].join('\n'));
                },
                pending: [
                    '<div class="empty-message">',
                    '正在搜索......',
                    '</div>'
                ].join(''),
                suggestion: Handlebars.compile('<div>{{name}}{{code}}</div>')
            }
        }).bind('typeahead:select', function(ev, suggestion) {
            $('#warningError').addClass("hidden");
            $('#selectedItem').data("cloneltemid", suggestion['serviceItemID']);
            $('#selectedTips').removeClass("hidden");
            $('#which').text($('#ServiceItems').val());
        }).on("keypress", function(e) {
            $('#warningError').removeClass("hidden");
            $('#selectedTips').addClass("hidden");
            $('#warningError').text("您未选择产品");
        });

        $('#toCopy').one("click", function A(e) {
            var thisbutton = $(this);
            var CloneItemID = $('#selectedItem').data("cloneltemid");
            var ItemID = $('#ItemID').text();
            $.ajax({
                url: "/ServiceItems/CloneFromSetting",
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    CloneItemID: CloneItemID,
                    ItemID: ItemID,

                }),
                dataType: 'json',
                beforeSend: function() {
                    thisbutton.buttonposting("正在克隆");
                    $('#responsetext').text("");

                },


                success: function(data) {
                    if (data.ErrorCode == 200) {
                        // thisbutton.success("保存成功");
                        thisbutton.posttedbutton("克隆成功", '克隆成功');

                        window.location.reload();
                    } else {
                        thisbutton.posttedbutton("克隆失败", '克隆');
                        $('#responsetext').text(data.ErrorMessage);
                        $('a#toCopy').one('click', A)

                    }
                },
                complete: function(XHR, TS) {
                    if (TS !== "success") {
                        thisbutton.posttedbutton("克隆失败", '克隆');
                    }
                    $('a#toCopy').one('click', A)

                }

            })

        })

    })



    //提交文件
    function upFileWithProgress(e) {
        var downurl = '/ServiceItems/DownFile?';
        var name = $('#itemname').text();

        if (document.getElementById("getfile").value == "") {} else {


            try {
                var xhr = new XMLHttpRequest(); /* ff */
            } catch (e) {
                try {
                    var xhr = new ActiveObject("Msxml2.XMLHTTP"); /* some ie */
                } catch (e) {
                    try {
                        var xhr = new ActiveXObject("Microsoft.XMLHTTP"); /*other ie */
                    } catch (e) {
                        var xhr = false;
                    }
                }
            }
            //创建xhr
            // var xhr = new XMLHttpRequest();



            var fileObj = document.getElementById("getfile").files[0];

            var url = "/ServiceItems/UploadFile";
            //FormData对象
            var fd = new FormData();
            fd.append("file", fileObj);
            fd.append("acttime", new Date().toString()); //本人喜欢在参数中添加时间戳，防止缓存（--、）
            fd.append("ItemID", $('#ItemID').text()); //本人喜欢在参数中添加时间戳，防止缓存（--、）


            xhr.onreadystatechange = function() {
                    if (xhr.readyState == 4 && xhr.status == 200) {
                        var result = JSON.parse(xhr.responseText);
                        var urldown = downurl + 'ServiceItemTemplteID=' + result.ServiceItemTemplteID + "&fileName=" + name.urlSwitch() + "-模板.html";
                        $('#progress').closest("a").attr("href", urldown);
                        document.getElementById("progress").innerHTML = name + "-模板.html";
                        if ($('body #ServiceItemTemplteID').length == 0) {
                            $('body').append('<div class="hidden" id="ServiceItemTemplteID"></div>');
                        }
                        $('body #ServiceItemTemplteID').text(result.ServiceItemTemplteID);


                    }
                }
                //进度条部分
            xhr.upload.onprogress = function(evt) {
                if (evt.lengthComputable) {
                    var percentComplete = Math.round(evt.loaded * 100 / evt.total) + "%";
                    document.getElementById("progress").innerHTML = percentComplete
                        // document.getElementById('progress').value = percentComplete;
                        // document.getElementById('progressNumber').style.width = percentComplete + "%";
                }
            };
            xhr.open("POST", url, true);
            xhr.send(fd);

            var obj = document.getElementById('getfile');
            $("#getfile").replaceWith($("#getfile").clone(true, false));
        }
    }