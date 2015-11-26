function GetVisa() {
    function m(text) {
        return text.split("：")[1];
    }
    function x(text) {
        return text.replace('\\', '\\\\"').replace('"', '\\"').replace("'", "\\'");
    }
    function y(text) {
        return text.replace('<', '&lt;').replace('>', '&gt;');
    }
    var submitinfo = '';
    $('.hd a').each(function (i, n) {
        var $me = $(this);
        var type = $.trim($me.text());
        if (type.indexOf('用户咨询') != -1 || type.indexOf('常见问题') != -1)
            return;
        var data = '';
        $('#tagContent' + (i + 1) + " tr").each(function () {
            var tr = $(this);
            var td1 = tr.children('td:eq(0)');
            if (td1.length < 1) return;
            var td2 = tr.children('td:eq(1)');
            data += (',{ "name":"' + x($.trim(td1.text())) + '", "val": "' + x($.trim(td2.text())) + '"}');
        });
        submitinfo += ',{ "type":"' + x($.trim($me.text())) + '", "data":[' + data.substring(1, data.length) + '] }';
    });

    var mid = $('.mid');
    var visaItem = '<VisaItem>'
        + '<Name>' + y($.trim($('a.f16.b.blue').text())) + '</Name>'
        + '<ImgUrl>' + y($('.pic img').attr('src')) + '</ImgUrl>'
        + '<办理费用>' + y($.trim(mid.find('span:eq(0) i:last').text())) + '</办理费用>'
        + '<最长停留时间>' + y(m($.trim(mid.find('span:eq(1)').text()))) + '</最长停留时间>'
        + '<入境次数>' + y(m($.trim(mid.find('span:eq(2)').text()))) + '</入境次数>'
        + '<办理时长>' + y(m($.trim(mid.find('span:eq(3)').text()))) + '</办理时长>'
        + '<面试>' + y(m($.trim(mid.find('span:eq(4)').text()))) + '</面试>'
        + '<有效期>' + y(m($.trim(mid.find('span:eq(5)').text()))) + '</有效期>'
        + '<友情提示>' + y($.trim($('.visaDesc4.brown2 p').text())) + '</友情提示>'
        + '<所需材料>[' + y(submitinfo.substring(1, submitinfo.length)) + ']</所需材料>'
        + '</VisaItem>';
    //console.info(visaItem.replace(/\s/g, ''));
    window.external.Handle("Cabbage.Crawler.Plugins.Uzai.JsCallback,GetVisaItem"
        , visaItem);
    return true;
}