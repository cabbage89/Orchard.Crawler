## 简介 ##
[Orchard建站视频](http://www.chuanke.com/3027295-124882.html)    
本模块于2014年开发,用于采集某网站签证信息(现已改版,该功能已经失效)    


## 使用方法 ##

1.在Orchard后台管理面板中开启Cabbage.Crawler模块功能

2.启用Orchard命令行,执行


    crawler start /PluginName:Uzai






***可以根据自己的需要写自己的爬虫插件,本例只是提供示例***


本示例回调方法在Cabbage.Crawler\Plugins\Uzai\Uzai.cs中,
需要执行插入数据库操作的请在此类中扩展

## 注意 ##


JS脚本中回调.NET接口,传参只能是字符串(作者本人测试)

字符串表示的数据结构可选JSON和XML,但是JSON在嵌套时会有引号转义问题，

所以需要用escape方法将其转化为unicode！
