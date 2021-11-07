## 介绍

`XmlConfigInitialization` 是一个使用 `C#` 语言编写的方便使用 `xml` 格式数据进行存储键值的一个工具。

## 产生的文件

产生的配置内容格式如下

```xml
<?xml version="1.0" encoding="UTF-8"?>
<root>
  <default>
    <item key="name" value="cglang" />
    <item key="age" value="22" />
  </default>
  <node1>
    <item key="test1" value="value1" />
    <item key="test2" value="value2" />
  </node1>
  <node2>
    <item key="test1" value="value3" />
    <item key="test2" value="value4" />
  </node2>
</root>
```

## 使用方式

### 通过 `XmlConfigFactory` 方式获取 `XmlConfig` 对象.

```csharp
var _xmlConfig = XmlConfigFactory.GetSingletonInstance();

或者

var option = new XmlOptions()
{
    ConfigName = "config.xml",
    Directory = "config",
    AutoSave = true
};
var _xmlConfig = XmlConfigFactory.GetSingletonInstance(option);
```


### 在 `ASP.NET Core` 中以注入的方式获取 `XmlConfig` 对象.

```csharp
// 自定义配置文件的配置信息
services.Configure<XmlOptions>(option =>
{
    option.ConfigName = "config.xml";
    option.Directory = "config";
    option.AutoSave = true;
}).AddSingleton<XmlConfig>();

// 使用默认的配置信息
services.AddSingleton<XmlConfig>();
...

// 然后在构造函数中注入即可
private readonly XmlConfig _xmlConfig;

public HomeController(XmlConfig xmlConfig)
{
    this._xmlConfig = xmlConfig;
}
```

- option.ConfigName 配置文件的名字
- option.Directory 配置文件要存放的相对目录
- option.AutoSave 在进行添加/更新/删除操作之后是否自动保存


## 案例

以上面展示的 `xml` 配置文件为例.

```csharp
_xmlConfig.GetValue("age"); // 值为 22

_xmlConfig.GetValue("test1","node1"); // 值为: value1
_xmlConfig.GetValue("test1","node2"); // 值为: value3

_xmlConfig.SetValue("age","23"); // 将 default 节点下的 key = age 的 value 设置为 23

_xmlConfig.Delete("age"); // 删除 default 节点下的 key = age 记录

_xmlConfig.DeleteNode("node1"); // node1 节点将会被全部删除
```

- 在使用 `SetValue` 方法时如果有对应的项将会被修改没有则新添加。
