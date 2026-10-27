# 简体中文汉化说明（Ziti Desktop Edge for Windows）

本仓库在官方 ZDEW 之上增加了一层**运行时中文翻译层**，不改动任何既有 XAML 视图与 ViewModel。

## 改动范围

| 文件 | 类型 | 说明 |
|---|---|---|
| `DesktopEdge/Utils/Localizer.cs` | 新增 | 词表（约 250 条）+ WPF 运行时翻译逻辑 |
| `DesktopEdge/App.xaml.cs` | 修改 1 行 | `OnStartup` 首行调用 `Localizer.Install()` |
| `DesktopEdge/ZitiDesktopEdge.csproj` | 修改 1 行 | 把新文件加入编译项（老式 csproj 需显式 Include） |

28 个 XAML 视图、全部 ViewModel **零改动**。

## 工作机制

1. `App.OnStartup` 注册 `FrameworkElement.LoadedEvent` 类处理器：任何控件（窗口、UserControl、菜单、弹窗、列表项）进入可视树时立即翻译其文本属性。
2. 翻译覆盖面：`TextBlock.Text`、`AccessText.Text`（去掉 `&` 助记符）、`Run.Text`、`ContentControl.Content`、`HeaderedContentControl.Header`、`HeaderedItemsControl.Header`、`Window.Title`、`ToolTip`。
3. 每 1 秒对已打开窗口做一次轻量复检，用于覆盖代码里动态设置文本的场景（例如身份状态从 Connected 变为 Disconnected）。
4. **只翻译未被数据绑定的属性**（`BindingOperations.IsDataBound` 判空后跳过），绝不打断 MVVM 绑定，否则状态将不再刷新。
5. 未命中词表的文本原样保留，身份名、控制器 URL、端口等动态数据不会被误改。

## 如何停用

删除 `App.xaml.cs` 中 `Localizer.Install();` 一行并重新编译即可（或其内部 `_installed` 判定后不再注册）。

## 如何扩展词表

在 `Localizer.cs` 的 `Zh` 字典中按 `{ "英文原文", "中文译文" }` 添加，键必须与界面上的实际英文**逐字一致**。

## 构建与验证

ZDEW 为 .NET Framework 4.8 WPF 应用，必须在 **Windows** 上构建（macOS/Linux 无法编译 WPF）：

```powershell
msbuild ZitiDesktopEdge.sln /p:Configuration=Release
```

本次改动在 macOS 上的验证方式与结论：

- `Localizer.cs` 使用 Roslyn + `Microsoft.NETFramework.ReferenceAssemblies.net48` 参考程序集、以 C# 7.3（项目实际语言版本）编译通过，**0 error 0 warning**；
- 类名采用 `Localizer` 而非 `Localization`，规避与 `System.Windows.Localization` 的 CS0104 二义性；
- 完整解决方案构建与界面实测需在 Windows 上完成。

## 已知限制

- 代码中以 `MessageBox.Show()` 直接弹出的英文（异常信息、错误码）不在此层覆盖范围。
- 中文字形依赖 Windows 字体回退（Segoe UI → 微软雅黑/宋体），未修改字体设置。
- 界面中由后端返回的英文错误详情保持原文。