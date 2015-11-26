function GetConsulate() {
    $('.visaTab').css({ border: 'solid 10px red' });
    var list = '';
    $('.hd a').each(function (i, n) {
        //领区
        var $me = $(this);
        var name = $me.text();
        var consulate = $('.bd .item:eq(' + i + ')');
        var desc = consulate.children('.d0').text();
        var visaList = '';
        consulate.find('li').each(function () {
            //领区下签证
            var _$me = $(this);
            var a = _$me.find('a.f14.b.blue');
            visaList += '<VisaItem><url>' + a.attr('href') + '</url><text>' + $.trim(a.text()) + '</text></VisaItem>';
        });
        list += '<Consulate><name>' + $.trim($me.text()) + '</name><desc>' + $.trim(consulate.children('.d0').text())
            + '</desc><visaList>' + visaList + '</visaList></Consulate>';
    });
    var visa = '<Visa><Name>' + $.trim($('h1.f14.b.f333').text()) + '</Name><ImgUrl>'
        + $('.visaDesc.f666 img').attr('src')
        + '</ImgUrl><Desc>'
        + $.trim($('.visaDesc2.f666 p').text()) + '</Desc><ConsulateList>' + list + '</ConsulateList></Visa>';
    window.external.Handle("Cabbage.Crawler.Plugins.Uzai.JsCallback,GetConsulate", visa);
    return true;
}