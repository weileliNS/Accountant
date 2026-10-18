# Accountant CN

国服社区维护分支，基于 [Ottermandias/Accountant](https://github.com/Ottermandias/Accountant) 、[MapleRecall/Accountant](https://github.com/MapleRecall/Accountant) 和历史汉化分支 [decorwdyun/Accountant](https://github.com/decorwdyun/Accountant)。保留 Apache-2.0 协议和原作者署名。

## 安装与更新

自定义插件仓库地址：

```text
https://raw.githubusercontent.com/weileliNS/DalamudPlugins/main/repo.json
```

在 `/xlsettings` 的自定义插件仓库列表中添加上述地址，再于 `/xlplugins` 中选择本仓库的 Accountant CN。

内部名称从 1.8.3.6 起为 `AccountantCN`，与官方 `Accountant` 独立。国服框架会按官方清单中的内部名称屏蔽第三方同名条目，是否已卸载官方版不影响这项过滤；因此旧的 1.8.3.5 自定义条目无法正常显示，重启也不能解决。

保留旧版配置和数据，卸载或停用官方 Accountant，刷新插件列表后安装 **Accountant CN 1.8.3.6**（或本源更新版本）。不要同时启用两个版本，它们使用相同的聊天命令。

首次启动且 CN 配置为空时，插件会把旧 `Accountant.json` 和 `Accountant/` 数据复制到独立的 `AccountantCN.json` 和 `AccountantCN/`，同时迁移 JSON 中的程序集类型引用。原文件保留不动；已有 CN 配置或数据时不会自动覆盖。现有作物的播种时间也会保留。

## 当前修复

- 中文播种确认框、园圃/地垄编号及护理状态解析。
- 当前国服 World 表筛选；排除数据中心编号为零的旧服务器记录。
- 中文角色名没有空格时也能重载雇员、任务和私人作物数据。
- 取消确认框时不创建/删除作物计时，拒绝越界的种植位置。
- 保留 MapleRecall 分支的仙人仙彩开奖时间缺失回退。
- 恢复历史汉化中的中文转轮确认、仙人仙彩号码提取和园圃等基础名称。
- 国服转轮直接读取中文物品表，避免依赖不可用的英文 Item 表。

## 支持边界

构建目标为国服 Dalamud API 15、.NET 10；参考程序集固定为国服 15.0.3.8，对应客户端构建 `2026.10.06.0000.0000`。更新游戏或 API 后须重新审查。

继承原插件的 81 条作物/种子配置。文本解析测试通过不代表每种植物均已实机测试。用户已确认补录的 3 株克拉卡萝卜和 5 株巫茄可正常显示；新播种、护理、收获的完整流程仍需更多实测。

尚未扩展花盆观赏花卉种子，未实现施肥后精确提前倒数，也未完成所有界面中文翻译。中文转轮映射和仙人仙彩号码提取已验证，完整实机流程仍待验证。详见 `CN_COMPATIBILITY.md`。

## 本地构建

安装 .NET SDK 10.0.402，用当前国服 Dalamud 的 Hooks 目录作为引用路径：

```powershell
dotnet run --project tests/Accountant.CnText.Tests -c Release
./scripts/Build-Release.ps1 -DalamudPath 'C:/path/to/XIVLauncherCN/addon/Hooks/15.0.3.8'
```

产物位于 `dist/latest.zip`、`dist/Accountant.json` 和 `dist/SHA256SUMS.txt`。构建依赖国服官方参考程序集；不要把游戏文件、玩家配置或日志提交到源码仓库。

## 发布流程

1. 修改 `Accountant/Accountant.csproj` 中的版本和 `RELEASE_NOTES.txt`。
2. 提交源码，等待 `Build CN plugin` 成功。
3. 手动运行 `Publish CN release`，输入对应的 `v四段版本号`。
4. 到 `weileliNS/DalamudPlugins` 手动运行 `Update plugin index`，输入同一标签。

工作流分别使用各自仓库自带的短期令牌，不需要跨仓库 PAT。保留国际服与国服上游 remote，更新时合并、检查 API 和文本模板，再回归测试。

## AI 使用说明

本维护分支的修复、测试与发布脚本由用户和 Codex 协作完成，包含大量 AI 生成代码。公开收录申请应如实说明，须由维护者审阅、理解并承担维护责任。
