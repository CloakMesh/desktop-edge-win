# CloakMesh Windows 客户端

CloakMesh 的 Windows 桌面客户端，fork 自 [OpenZiti Ziti Desktop Edge for Windows](https://github.com/openziti/desktop-edge-win)（Apache License 2.0）。

> **上游说明**：拦截逻辑、Windows 服务、wintun 数据面与升级机制均来自上游。CloakMesh 的差异是**简体中文界面**与**品牌化**。

## 与上游的差异

| 项目 | 上游 | CloakMesh | 状态 |
|---|---|---|---|
| 界面语言 | 英文 | 简体中文（运行时翻译层，约 250 条词表） | ✅ 已完成 |
| 产品名（标题栏/关于/提示） | Ziti Desktop Edge | CloakMesh Desktop Edge | ✅ 已完成 |
| 程序集版权/公司 | NetFoundry Inc. | aiuc-labs（保留上游署名） | ✅ 已完成 |
| 可执行文件名（`ZitiDesktopEdge.exe`） | 不变 | 保留 | ⚠️ 见下 |

**为什么 exe/服务名暂不改**：`ZitiDesktopEdge.exe`、Windows 服务名与安装包脚本、升级通道、签名流程深度耦合，本机无 Windows/.NET Framework 4.8 构建链，无法回归验证。改名需要一次专门的 Windows 侧改动 + 全新安装/升级回归，建议作为独立阶段进行（涉及 `Installer/`、`ZitiUpdateService/`、服务名常量与签名证书）。

## 汉化

实现细节见 [`I18N-zh-CN.md`](I18N-zh-CN.md)。要点：

- 新增 `DesktopEdge/Utils/Localizer.cs`（词表 + 可视树翻译），`App.OnStartup` 一行安装；
- 只翻译**未被数据绑定**的属性，不打断 MVVM 绑定；
- XAML 视图与 ViewModel 零改动。

## 构建（必须 Windows）

```powershell
git clone https://github.com/aiuc-labs/cloakmesh-desktop-edge-win.git
cd cloakmesh-desktop-edge-win
msbuild ZitiDesktopEdge.sln /p:Configuration=Release
```

打包 msi 见仓库 `releasing.md`。

## 许可

Apache License 2.0，与上游一致。