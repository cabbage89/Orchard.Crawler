function getCountries() {
    var dd = $('dd.d1');
    var countriesUrl = '';
    dd.each(function () {
        var $me = $(this);
        $me.children('a').each(function () {
            var $a = $(this);
            countriesUrl += ',{ url: "' + $a.attr('href') + '", text: "' + $a.text().replace('\'', '\\\'').replace('"', '\\"') + '"}';
        });
    });
    countriesUrl = '[' + countriesUrl.substr(1, countriesUrl.length) + ']';
    window.external.Handle("Cabbage.Crawler.Plugins.Uzai.JsCallback,GetCountries"
        , countriesUrl);
    return true;
}