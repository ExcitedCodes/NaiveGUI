[中文](#中文)

[由社区提供的视频教程, 感谢视频作者](https://youtu.be/p9MZMAvVLAA?t=510)

# NaiveGUI
NaiveGUI is a Windows GUI wrapper of [NaïveProxy](https://github.com/klzgrad/naiveproxy).

The original purpose of this project is to configure your NaïveProxy and switch between remotes easily.

It also provides useful features like subscription, auto start, log highlighting and so on.

## Quick Start
1. Grab the latest _NaiveGUI.zip_ from [Releases](https://github.com/ExcitedCodes/NaiveGUI/releases) and extract everything to wherever you want
2. Get the latest _naive.exe_ from  [NaïveProxy Releases](https://github.com/klzgrad/naiveproxy/releases)
3. Move the _naive.exe_ to the same folder of _NaiveGUI.exe_
4. Start _NaiveGUI.exe_, click the plus button to create listeners
5. Double-click(or right click and select "Add") the __Default__ group in the __Remote__ section, create a new remote
6. Select the listener, when the listener is selected(Notice the deep shadow), left-click any remote to associate it with your listener
7. Click the `Disabled` located at the right buttom of listener card, if the card become blue, it means your listener is working!

## Subscription
NaiveGUI current support only one format of subscription. Each subscription url can contain multiple groups.
You can always find the latest subscription format [Here](https://github.com/ExcitedCodes/NaiveGUI/blob/master/NaiveWPF/Data/Subscription.cs)
The following example may not represent the latest subscription format.
```jsonc
 {
    "GroupName1": [
        {
            "name": "Name here!",
            "host": "xxx.xxx",
            "port": 2333,
            "scheme": "https", // optional
            "username": "UserXD", // optional
            "password": "Password0", // optional
            "padding": true // optional
        },
        ...
    ],
    "GroupName2": [
        ...
    ]
}
```


# 中文
NaiveGUI 是一个 [NaïveProxy](https://github.com/klzgrad/naiveproxy) 的  Windows GUI.

这个项目的初衷是方便的配置 NaïveProxy 并且在服务器之间简单的切换

它还提供有用的特性, 如订阅、自动启动、高亮显示日志等

## 快速开始
1. 从 [Releases](https://github.com/ExcitedCodes/NaiveGUI/releases) 获取最新的 _NaiveGUI.zip_ 并将所有内容解压到您想要的任何地方
2. 从 [NaïveProxy Releases](https://github.com/klzgrad/naiveproxy/releases) 获取最新的 _naive.exe_
3. 移动 _naive.exe_ 到 _NaiveGUI.exe_ 的相同目录下
4. 启动 _NaiveGUI.exe_, 点击加号按钮来创建代理监听器
5. 双击(或者右击并选择 "添加") 在 __服务器__ 一节中的 __Default__ 组, 创建一个新的服务器
6. 选择一个代理监听器, 当代理监听器被选中(注意更深的阴影), 左键点击任意服务器并将它关联到监听器
7. 点击代理监听器卡片右下角的 `Disabled` 字样, 如果卡片变蓝, 它代表你的代理监听器开始工作了!

## 订阅
NaiveGUI 当前只支持一种订阅格式. 每个订阅 URL 可以包含多个组.
你总能在 [这里](https://github.com/ExcitedCodes/NaiveGUI/blob/master/NaiveWPF/Data/Subscription.cs) 找到最新的订阅格式
下面的例子不一定能代表最新的订阅格式
```jsonc
 {
    "GroupName1": [
        {
            "name": "Name here!",
            "host": "xxx.xxx",
            "port": 2333,
            "scheme": "https", // 可选
            "username": "UserXD", // 可选
            "password": "Password0", // 可选
            "padding": true // 可选
        },
        ...
    ],
    "GroupName2": [
        ...
    ]}
```
