# 国服官方收录/本地化提交路径

来源：[ottercorp/DalamudPluginsD17 README](https://github.com/ottercorp/DalamudPluginsD17#readme)，核对日期 2026-10-19。

这里的“汉化申请”通过清单 PR 进行。官方仓库没有单独的汉化认领 Issue 模板；PR 模板包含“汉化”“适配国服”“修 Bug”等勾选项。

## 新插件

1. 源码放在公开可克隆的仓库，Release 构建成功，提交项目和锁文件。
2. Fork `ottercorp/DalamudPluginsD17`。
3. 通常在 `testing/live/内部名称/manifest.toml` 添加源码地址、准确 Git commit、owners 和 project_path。
4. 放置规定尺寸的插件图标，创建 PR，说明汉化/适配内容和验证情况。

```toml
[plugin]
repository = "https://github.com/weileliNS/Accountant.git"
commit = "填写通过构建和测试的实际源码commit"
owners = ["Ottermandias", "weileliNS"]
project_path = "Accountant"
changelog = "修复国服中文园艺、服务器筛选及中文角色名重载。"
```

## Accountant 已有条目

当前条目在 `stable/Accountant/manifest.toml`，指向 MapleRecall 源码。在准备提交时应先与该条目的维护者沟通，说明替换源码地址、追加维护者的理由和现有分支关系，由审核者确认维护权和采用的发布轨道。不要同时创建第二份同名稳定条目。

当前 PR 模板有“同步上游 / 汉化 / 适配国服 / 修 Bug / 新插件”。本维护版应说明是基于已有国服 fork 的修复，并如实披露 Codex 参与和未完成的实机测试。国际服官网另有 AI 使用政策；国服审核是否额外采用相同条款，应向国服审核者确认，不能把国际服规则直接当成国服的现行规定。

官方构建和审查通过后才会进入官方分发。更新通常只需修改清单的实际源码 commit；需要重建时，国服文档给出的评论是 `@aonyxbot rebuild`。

自定义插件仓的发布独立于官方收录，不需要先获官方批准；自定义仓也不代表已通过官方代码审核。本次仅建立个人维护和分发仓，尚未代表维护者向官方发起接管申请。
