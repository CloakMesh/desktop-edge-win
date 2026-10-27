/*
	Copyright NetFoundry Inc.

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	https://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace ZitiDesktopEdge.Utility {
    /// <summary>
    /// 简体中文运行时翻译层（WPF）。
    ///
    /// 设计目标：不做 XAML 级 i18n 改造（那要改动全部视图并引入 .resx 资源），
    /// 只在界面装载/刷新时按“英文整串精确匹配”替换可见文本，源码改动仅限本文件 + App.OnStartup 一行。
    ///
    /// 行为约定：
    /// 1. 只翻译未被数据绑定的文本属性，绝不打断既有的 Binding（避免状态不刷新）
    /// 2. 未命中词表的文本原样保留，身份名/URL/ID 等动态数据天然不受影响
    /// 3. 每 1 秒对已打开窗口做一次轻量复检，用于覆盖代码里 SetText 的后续改动
    /// </summary>
    public static class Localizer {
        private static readonly Dictionary<string, string> Zh = new Dictionary<string, string>(StringComparer.Ordinal) {
            { "0 Services", "0 个服务" },
            { "3rd Party Certificate", "第三方证书" },
            { "<Choose Certificate File>", "<选择证书文件>" },
            { "<Choose Key File>", "<选择密钥文件>" },
            { "A previous feedback collection is still running.", "上一次反馈收集仍在进行中。" },
            { "ADD", "添加" },
            { "Add DNS", "添加 DNS" },
            { "Add Fingerprint and click to enable mfa", "添加指纹后点击以启用 MFA" },
            { "Add Identity by JWT...", "通过 JWT 添加身份..." },
            { "Add Identity by URL...", "通过 URL 添加身份..." },
            { "Add by JWT", "通过 JWT 添加" },
            { "Add by URL", "通过 URL 添加" },
            { "Adding Identity", "正在添加身份" },
            { "Addresses", "地址" },
            { "Advanced Settings", "高级设置" },
            { "Alias", "别名" },
            { "All of the services with a timeout set for the identity", "该身份所有设置了超时的服务" },
            { "Already have a network?", "已有网络？" },
            { "An Error Occurred", "发生错误" },
            { "An Unknown Error Occurred, you could try restarting the service and the interface to continue", "发生未知错误，可尝试重启服务与界面后继续" },
            { "An Update is available!", "有可用更新！" },
            { "An error occurred while trying check for update.", "检查更新时发生错误。" },
            { "An error occurred while trying to disable the data service. Is the monitor service running?", "禁用数据服务时发生错误。监控服务是否在运行？" },
            { "An error occurred while trying to gather feedback. Is the monitor service running?", "收集反馈时发生错误。监控服务是否在运行？" },
            { "An error occurred while trying to trigger the update.", "触发更新时发生错误。" },
            { "An unexpected error has occured while removing the identity. Please verify the service is still running and try again.", "移除身份时发生异常错误。请确认服务仍在运行后重试。" },
            { "Any time", "任意时间" },
            { "Api Page Size (10-500)", "API 分页大小（10-500）" },
            { "Auth needed", "需要认证" },
            { "Authenticate", "认证" },
            { "Authentication Code", "认证码" },
            { "Authentication Failed", "认证失败" },
            { "Authentication Successful", "认证成功" },
            { "Authorization Required", "需要授权" },
            { "Authorize MFA", "授权 MFA" },
            { "Automatic Upgrades", "自动升级" },
            { "Automatic upgrade settings are controlled by your organization and cannot be changed.", "自动升级设置由你的组织管理，无法修改。" },
            { "Automatic upgrades", "自动升级" },
            { "Back", "返回" },
            { "Browse", "浏览" },
            { "Busy collecting feedback -- try again shortly", "正在收集反馈——请稍后再试" },
            { "By NetFoundry", "由 NetFoundry 提供" },
            { "By date", "按日期" },
            { "By weekday", "按星期" },
            { "Capture Feedback...", "收集反馈..." },
            { "Certificate File", "证书文件" },
            { "Check for updates now", "立即检查更新" },
            { "Check the Log for Details", "查看日志了解详情" },
            { "Checking for updates |", "正在检查更新 |" },
            { "Checking for updates...", "正在检查更新..." },
            { "Choose External Provider", "选择外部提供方" },
            { "Choose how this identity connects to the network", "选择此身份接入网络的方式" },
            { "Click here to collect logs", "点击此处收集日志" },
            { "Click to Authorize", "点击授权" },
            { "Click to configure external auth providers", "点击配置外部认证提供方" },
            { "Click to generate codes", "点击生成验证码" },
            { "Click to remove mfa", "点击移除 MFA" },
            { "Click to return codes", "点击返回验证码" },
            { "Click to show service details", "点击查看服务详情" },
            { "Click to verify enrollment", "点击验证登记" },
            { "Close", "关闭" },
            { "Close Error", "关闭错误" },
            { "Collecting Information", "正在收集信息" },
            { "Collecting feedback...", "正在收集反馈..." },
            { "Commercial support for NetFoundry customers:", "NetFoundry 客户商业支持：" },
            { "Communication Error with monitor?", "与监控服务通信出错？" },
            { "Community support for open source users:", "开源用户社区支持：" },
            { "Config Save, Please Restart Ziti to Update", "配置已保存，请重启 Ziti 以生效" },
            { "Configure Enrollment", "配置登记" },
            { "Configured Providers", "已配置的提供方" },
            { "Confirm", "确认" },
            { "Confirm Forget", "确认忘记" },
            { "Confirmation Title", "确认标题" },
            { "Controller URL", "控制器 URL" },
            { "Could Not Collect Feedback", "无法收集反馈" },
            { "Could Not Save Settings", "无法保存设置" },
            { "Could Not Set Automatic Update", "无法设置自动更新" },
            { "Could not save maintenance window settings.", "无法保存维护窗口设置。" },
            { "Could not set update URL.", "无法设置更新 URL。" },
            { "Current status is:", "当前状态为：" },
            { "DISABLED", "已禁用" },
            { "Default provider?", "设为默认提供方？" },
            { "Delete Item?", "删除该项？" },
            { "Deploy a Network", "部署网络" },
            { "Device certificate", "设备证书" },
            { "Disabling Service", "正在禁用服务" },
            { "Disabled", "已禁用" },
            { "Download Speed", "下载速度" },
            { "Downloading update for next restart...", "正在下载更新，将在下次重启时安装..." },
            { "Drag to detach/move, right click to reattach", "拖动可分离/移动，右键可重新附着" },
            { "Enabled", "已启用" },
            { "Enrolled", "已登记" },
            { "Enter Alias", "输入别名" },
            { "Enter Authorization or Recovery Code", "输入授权码或恢复码" },
            { "Enter the code below to finish setup.", "输入下方验证码以完成设置。" },
            { "Enter the controller URL", "输入控制器 URL" },
            { "Error -- see logs", "错误——请查看日志" },
            { "Error Collecting Feedback", "收集反馈出错" },
            { "Error Disabling Service", "禁用服务出错" },
            { "Error Saving Settings", "保存设置出错" },
            { "Error Starting Service", "启动服务出错" },
            { "Error When Checking for Update", "检查更新出错" },
            { "Error When Triggering Update", "触发更新出错" },
            { "Error checking -- see logs", "检查出错——请查看日志" },
            { "Error checking for updates. See logs.", "检查更新出错，请查看日志。" },
            { "Error requesting update", "请求更新出错" },
            { "Error requesting update.", "请求更新出错。" },
            { "External Auth Failed", "外部认证失败" },
            { "External authentication could not start. No URL was returned to login. Inform your network administrator.", "外部认证无法启动，未返回登录 URL。请联系网络管理员。" },
            { "Failed to Authenticate", "认证失败" },
            { "Feedback collection started", "已开始收集反馈" },
            { "Feedback in progress", "反馈收集进行中" },
            { "Filter", "过滤" },
            { "Force Quit", "强制退出" },
            { "Frequency", "频率" },
            { "Frequency Saved", "频率已保存" },
            { "From", "从" },
            { "Generating MFA", "正在生成 MFA" },
            { "Go To Link", "前往链接" },
            { "IDENTITY", "身份" },
            { "IP Address", "IP 地址" },
            { "Identity Alias", "身份别名" },
            { "Identity Details", "身份详情" },
            { "Identity Error", "身份错误" },
            { "Identity Name", "身份名称" },
            { "Identity Provider", "身份提供方" },
            { "Identity connected. Click to see service details.", "身份已连接，点击查看服务详情。" },
            { "Identity disabled, MFA cannot continue.", "身份已禁用，无法继续 MFA。" },
            { "Identity is disabled. MFA cannot continue.", "身份已禁用，无法继续 MFA。" },
            { "If a network administrator has already sent you an enrollment file or URL join that network now:", "如果网络管理员已发送登记文件或 URL，请立即加入该网络：" },
            { "If this keeps happening please collect logs and report the issue.", "如果问题持续出现，请收集日志并反馈该问题。" },
            { "If you delete this identity you will no longer have access to the resources that it grants you.", "删除此身份后，你将无法再访问它授权的资源。" },
            { "Important Notice", "重要提示" },
            { "Install on next restart", "下次重启时安装" },
            { "Invalid MFA Url", "MFA URL 无效" },
            { "Invalid or unreachable host name", "主机名无效或不可达" },
            { "KBps", "KBps" },
            { "Key File", "密钥文件" },
            { "L2 Enabled", "已启用二层" },
            { "Last day", "最近一天" },
            { "Learn how to deploy a network", "了解如何部署网络" },
            { "Loading", "加载中" },
            { "MAIN", "主界面" },
            { "MENU", "菜单" },
            { "MFA", "MFA" },
            { "MFA Recovery Codes", "MFA 恢复码" },
            { "Main Menu", "主菜单" },
            { "Managed by your organization", "由你的组织管理" },
            { "Min/Max For", "最小/最大值" },
            { "Mode", "模式" },
            { "Monitor service is offline", "监控服务离线" },
            { "Monitor service offline", "监控服务离线" },
            { "Name", "名称" },
            { "Need Help?", "需要帮助？" },
            { "NetFoundry Support", "NetFoundry 支持" },
            { "Never", "从不" },
            { "New to OpenZiti? Get started by deploying your own zero trust network overlay.", "刚接触 OpenZiti？先部署你自己的零信任覆盖网络。" },
            { "New version available", "有新版本可用" },
            { "No Recovery Codes Available", "无可用恢复码" },
            { "No UI log folder found", "未找到界面日志目录" },
            { "No external providers found! This is a configuration error. Inform your network administrator.", "未找到外部提供方！这是配置错误，请联系网络管理员。" },
            { "No updates available", "没有可用更新" },
            { "Notification Frequency", "通知频率" },
            { "On", "开" },
            { "On day", "在某天" },
            { "On the", "在" },
            { "One Moment Please...", "请稍候..." },
            { "Open log folder", "打开日志目录" },
            { "OpenZiti Discourse Community", "OpenZiti 社区论坛" },
            { "Operation Timed Out", "操作超时" },
            { "Option 1", "选项 1" },
            { "Option 2", "选项 2" },
            { "Option 3", "选项 3" },
            { "Option 4", "选项 4" },
            { "Override Keychain?", "覆盖钥匙串？" },
            { "Pcap Interface", "Pcap 接口" },
            { "Permissions", "权限" },
            { "Please Wait.", "请稍候。" },
            { "Please report this issue:", "请反馈此问题：" },
            { "Please wait for the service to stop.", "请等待服务停止。" },
            { "Please wait while the identity is added", "正在添加身份，请稍候" },
            { "Please wait while the service stops", "正在停止服务，请稍候" },
            { "Please wait...", "请稍候..." },
            { "Ports", "端口" },
            { "Protocols", "协议" },
            { "Provided code could not be verified", "提供的验证码无法验证" },
            { "Quit App", "退出应用" },
            { "Quit Ziti Desktop Edge", "退出 CloakMesh Desktop Edge" },
            { "Regenerate", "重新生成" },
            { "Request Update Again", "再次请求更新" },
            { "Requesting automatic update...", "正在请求自动更新..." },
            { "Reset", "重置" },
            { "STOP", "停止" },
            { "Save Recovery Codes", "保存恢复码" },
            { "Select Certificate File", "选择证书文件" },
            { "Select Key File", "选择密钥文件" },
            { "Select a Pcap Interface", "选择 Pcap 接口" },
            { "Service", "服务" },
            { "Service Added:", "已添加服务：" },
            { "Service Not Started", "服务未启动" },
            { "Service is Stopping", "服务正在停止" },
            { "Service is started", "服务已启动" },
            { "Service is starting...", "服务正在启动..." },
            { "Service is stopped", "服务已停止" },
            { "Service is stopping...", "服务正在停止..." },
            { "Service started!", "服务已启动！" },
            { "Service stopped!", "服务已停止！" },
            { "Services", "服务" },
            { "Set Log Level", "设置日志级别" },
            { "Set Up MFA", "设置 MFA" },
            { "Settings Saved.", "设置已保存。" },
            { "Setup MFA", "设置 MFA" },
            { "Show QR Code", "显示二维码" },
            { "Show Secret", "显示密钥" },
            { "Show welcome screen", "显示欢迎界面" },
            { "Showing Notification from identity", "显示来自身份的通知" },
            { "Some or all of the services will be timing out in", "部分或全部服务将在以下时间后超时：" },
            { "Start the Ziti Tunnel Service to get started", "启动 Ziti 隧道服务以开始使用" },
            { "Starting the data service", "正在启动数据服务" },
            { "Status", "状态" },
            { "Subnet Mask", "子网掩码" },
            { "Switch Tunneler", "切换隧道器" },
            { "Switch failed", "切换失败" },
            { "Switch tunneler instance (dev)", "切换隧道器实例（开发用）" },
            { "Switching tunneler", "正在切换隧道器" },
            { "Tap to Connect", "点击连接" },
            { "Tap to Disconnect", "点击断开" },
            { "The Service Appears Stuck", "服务似乎卡住" },
            { "The Service Is Still Running", "服务仍在运行" },
            { "The data channel has stopped unexpectedly", "数据通道意外停止" },
            { "The file selected is not a valid JWT", "所选文件不是有效的 JWT" },
            { "The file was not found.", "文件未找到。" },
            { "The monitor has indicated an upgrade is in progress. Shutting down the UI", "监控服务指示正在升级，正在关闭界面" },
            { "The monitor has indicated the application should shut down.", "监控服务指示应用应关闭。" },
            { "The monitor service is offline", "监控服务离线" },
            { "The services for", "服务（" },
            { "The version of the Service is not compatible", "服务版本不兼容" },
            { "These settings are controlled by your organization and cannot be changed.", "这些设置由你的组织管理，无法修改。" },
            { "This is the description it should and could be long it might contain newlines and should wrap.", "这是描述，可能较长并可包含换行，应自动换行。" },
            { "This is the description.", "这是描述。" },
            { "This version is newer than the latest:", "该版本比最新版更新：" },
            { "Timed Out", "已超时" },
            { "Timeout", "超时" },
            { "Timing out", "正在超时" },
            { "Tunnel Config", "隧道配置" },
            { "Unexpected Error", "意外错误" },
            { "Unexpected Error!", "意外错误！" },
            { "Unexpected error when adding identity!", "添加身份时发生意外错误！" },
            { "Unexpected error when processing MFA", "处理 MFA 时发生意外错误" },
            { "Untrusted certificate or TLS error", "证书不受信任或 TLS 错误" },
            { "Update Failed", "更新失败" },
            { "Update In Progress", "更新进行中" },
            { "Update Now", "立即更新" },
            { "Update Requested", "已请求更新" },
            { "Update Requested at", "请求更新时间" },
            { "Update available", "有可用更新" },
            { "Update on Restart", "重启时更新" },
            { "Update scheduled for maintenance window", "已安排在维护窗口更新" },
            { "Update staged for next restart", "更新已准备就绪，下次重启生效" },
            { "Upgrade available:", "可升级版本：" },
            { "Upload Speed", "上传速度" },
            { "Url", "URL" },
            { "Use Pcap", "使用 Pcap" },
            { "User session", "用户会话" },
            { "Version mismatch!", "版本不匹配！" },
            { "Visit our support forum", "访问我们的支持论坛" },
            { "Waiting for service to stop... Service did not reach stopped state in the expected amount of time.", "正在等待服务停止... 服务未在预期时间内进入停止状态。" },
            { "Welcome to Ziti Desktop Edge", "欢迎使用 Ziti Desktop Edge" },
            { "With JWT", "使用 JWT" },
            { "With URL", "使用 URL" },
            { "Would you like to try to force close the service?", "是否尝试强制关闭服务？" },
            { "You can always add more later via the \"Add Identity\" button at the top right of the main screen.", "你随时可以通过主界面右上角的“添加身份”按钮继续添加。" },
            { "You must authenticate to access services", "必须完成认证才能访问服务" },
            { "You must enable MFA to access services", "必须启用 MFA 才能访问服务" },
            { "Ziti Desktop Edge", "CloakMesh Desktop Edge" },
            { "Ziti Desktop Edge will initiate auto installation in the next minute!", "CloakMesh Desktop Edge 将在下一分钟内开始自动安装！" },
            { "and sort by", "并排序依据" },
            { "any text", "任意文本" },
            { "checking version information", "正在检查版本信息" },
            { "enumerating pipes…", "正在枚举管道…" },
            { "no re-auth, tied to this machine", "无需重复认证，绑定本机" },
            { "requires periodic login, tied to user account", "需要定期登录，绑定用户账号" },
            { "Available", "可用" },
        };

        private static bool _installed;
        private static bool _applying;
        private static DispatcherTimer _timer;

        /// <summary>翻译单个英文串；未命中词表时原样返回。</summary>
        public static string T(string english) {
            if (string.IsNullOrEmpty(english)) return english;
            string translated;
            return Zh.TryGetValue(english.Trim(), out translated) ? translated : english;
        }

        /// <summary>
        /// 安装翻译层。应在 App.OnStartup 中调用一次。
        /// </summary>
        public static void Install() {
            if (_installed) return;
            _installed = true;

            // 任何 FrameworkElement 进入可视树时立即翻译（覆盖 UserControl 切换、菜单、弹窗）
            EventManager.RegisterClassHandler(
                typeof(FrameworkElement),
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler(OnElementLoaded),
                true);

            if (Application.Current != null) {
                if (Application.Current.MainWindow != null) {
                    Apply(Application.Current.MainWindow);
                }
                _timer = new DispatcherTimer(DispatcherPriority.Background) {
                    Interval = TimeSpan.FromSeconds(1),
                };
                _timer.Tick += (sender, args) => {
                    if (Application.Current == null) return;
                    foreach (Window window in Application.Current.Windows) {
                        Apply(window);
                    }
                };
                _timer.Start();
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e) {
            if (sender is DependencyObject d) {
                ApplyNode(d);
            }
        }

        /// <summary>翻译一个窗口/控件子树（视觉树 + 逻辑树）。</summary>
        public static void Apply(DependencyObject root) {
            if (root == null || _applying) return;
            _applying = true;
            try {
                ApplyTree(root, new HashSet<DependencyObject>(ReferenceComparer.Instance));
            } finally {
                _applying = false;
            }
        }

        private static void ApplyTree(DependencyObject node, HashSet<DependencyObject> visited) {
            if (node == null || !visited.Add(node)) return;
            ApplyNode(node);

            foreach (object child in LogicalTreeHelper.GetChildren(node)) {
                if (child is DependencyObject childObject) {
                    ApplyTree(childObject, visited);
                }
            }

            if (node is Visual || node is Visual3D) {
                int count = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < count; i++) {
                    ApplyTree(VisualTreeHelper.GetChild(node, i), visited);
                }
            }
        }

        private static void ApplyNode(DependencyObject node) {
            if (node is Window window) {
                SetIfFree(window, Window.TitleProperty, window.Title);
            }

            if (node is TextBlock textBlock) {
                SetIfFree(textBlock, TextBlock.TextProperty, textBlock.Text);
            } else if (node is AccessText accessText) {
                SetIfFree(accessText, AccessText.TextProperty, StripMnemonic(accessText.Text));
            } else if (node is Run run) {
                SetIfFree(run, Run.TextProperty, run.Text);
            }

            if (node is ContentControl contentControl && contentControl.Content is string content) {
                SetIfFree(contentControl, ContentControl.ContentProperty, content);
            }

            if (node is HeaderedContentControl headered && headered.Header is string header) {
                SetIfFree(headered, HeaderedContentControl.HeaderProperty, header);
            }

            if (node is HeaderedItemsControl headeredItems && headeredItems.Header is string itemsHeader) {
                SetIfFree(headeredItems, HeaderedItemsControl.HeaderProperty, itemsHeader);
            }

            if (node is FrameworkElement element && element.ToolTip is string toolTip) {
                SetIfFree(element, FrameworkElement.ToolTipProperty, toolTip);
            }
        }

        /// <summary>
        /// 仅在属性未被数据绑定时写入译文，避免打断 MVVM 绑定导致界面不再刷新。
        /// </summary>
        private static void SetIfFree(DependencyObject target, DependencyProperty property, string current) {
            if (current == null) return;
            if (BindingOperations.IsDataBound(target, property)) return;
            string translated = T(current);
            if (string.Equals(translated, current, StringComparison.Ordinal)) return;
            target.SetValue(property, translated);
        }

        private static string StripMnemonic(string text) {
            return string.IsNullOrEmpty(text) ? text : text.Replace("&", string.Empty);
        }

        private sealed class ReferenceComparer : IEqualityComparer<DependencyObject> {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();

            public bool Equals(DependencyObject x, DependencyObject y) {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(DependencyObject obj) {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}