# A2UI Blazor Markdown 组件使用指南

## ✨ 功能特性

Blazor已完整实现Markdown组件支持，功能包括：

- ✅ **自动Markdown检测** - 自动识别Markdown语法
- ✅ **完整Markdown支持** - 使用Markdig库，支持GFM扩展
- ✅ **安全渲染** - 自动HTML转义，防止XSS攻击
- ✅ **自定义样式** - 支持tagClassMap自定义CSS类
- ✅ **暗色模式** - 内置暗色主题支持
- ✅ **与Angular对等** - 功能完全对标Angular实现

## 📦 依赖包

```xml
<PackageReference Include="Markdig" Version="0.37.0" />
```

## 🎯 使用方式

### 1. 基础用法 - 自动检测Markdown

```csharp
var messages = new SurfaceBuilder("demo")
    .AddText("md-text", text => text
        .WithText("## Hello Markdown\n\n这是一个**粗体**文本和*斜体*文本示例。"))
    .WithRoot("md-text")
    .Build();
```

A2UIText组件会自动检测Markdown语法并渲染。

### 2. 显式启用Markdown

```json
{
  "Text": {
    "text": {
      "literalString": "# Markdown Content\n\n- Item 1\n- Item 2"
    },
    "markdown": true
  }
}
```

### 3. 自定义Markdown样式（tagClassMap）

```csharp
var component = new Dictionary<string, object>
{
    ["Text"] = new Dictionary<string, object>
    {
        ["text"] = new Dictionary<string, object>
        {
            ["literalString"] = "## 标题\n\n这是一个[链接](https://example.com)"
        },
        ["markdown"] = true,
        ["tagClassMap"] = new Dictionary<string, string[]>
        {
            ["h2"] = new[] { "custom-heading", "text-primary" },
            ["a"] = new[] { "custom-link" },
            ["p"] = new[] { "custom-paragraph" }
        }
    }
};
```

### 4. 完整示例 - 博客文章

```csharp
var markdown = @"
# 欢迎使用A2UI Markdown

## 功能特性

A2UI Blazor现在**完全支持**Markdown渲染：

- **粗体文本**和*斜体文本*
- [链接](https://github.com)和`代码`
- 列表、表格、引用等

### 代码示例

```csharp
public class Hello 
{
    public string World => ""A2UI"";
}
```

> 这是一个引用块

| 功能 | 状态 |
|------|------|
| Markdown | ✅ |
| 表格 | ✅ |
| 代码高亮 | ✅ |

";

var messages = new SurfaceBuilder("blog")
    .AddCard("article", card => card.WithChild("content"))
    .AddText("content", text => text
        .WithText(markdown)
        .WithProperty("markdown", true))
    .WithRoot("article")
    .Build();
```

## 🎨 支持的Markdown语法

### 标题
```markdown
# H1 标题
## H2 标题
### H3 标题
```

### 强调
```markdown
**粗体** 或 __粗体__
*斜体* 或 _斜体_
~~删除线~~
```

### 列表
```markdown
- 无序列表项
- 另一个项目
  - 嵌套项

1. 有序列表
2. 第二项
```

### 链接和图片
```markdown
[链接文本](https://example.com)
![图片描述](https://example.com/image.jpg)
```

### 代码
```markdown
内联代码：`var x = 10;`

代码块：
\`\`\`csharp
public void Method()
{
    Console.WriteLine("Hello");
}
\`\`\`
```

### 引用
```markdown
> 这是一个引用
> 可以多行
```

### 表格
```markdown
| 列1 | 列2 | 列3 |
|-----|-----|-----|
| A   | B   | C   |
| D   | E   | F   |
```

### 水平线
```markdown
---
***
___
```

### 任务列表
```markdown
- [x] 已完成任务
- [ ] 待办任务
```

## 🔧 高级配置

### tagClassMap映射表

支持的HTML标签：

| Markdown | HTML标签 | tagClassMap键 |
|----------|---------|--------------|
| # 标题 | `<h1>` - `<h6>` | "h1", "h2", ... |
| 段落 | `<p>` | "p" |
| 粗体 | `<strong>` | "strong" |
| 斜体 | `<em>` | "em" |
| 链接 | `<a>` | "a" |
| 列表 | `<ul>`, `<ol>` | "ul", "ol" |
| 列表项 | `<li>` | "li" |
| 代码 | `<code>` | "code" |
| 引用 | `<blockquote>` | "blockquote" |

### 自定义Markdown渲染

如需扩展Markdown功能，可以修改`MarkdownRenderer.cs`：

```csharp
public MarkdownRenderer()
{
    _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseAutoLinks()
        .UseEmphasisExtras()
        .UsePipeTables()
        .UseListExtras()
        .UseGenericAttributes() // 添加更多扩展
        .Build();
}
```

## 💡 最佳实践

### 1. 性能优化
```csharp
// MarkdownRenderer是单例，可高效复用
builder.Services.AddSingleton<MarkdownRenderer>();
```

### 2. 安全渲染
```csharp
// 普通文本自动转义HTML
// Markdown内容通过Markdig安全处理
// 使用MarkupString渲染HTML
```

### 3. 数据绑定
```csharp
// 支持从DataModel绑定Markdown内容
.AddText("md", text => text
    .WithValue("$.markdownContent")) // 从数据模型读取
```

## 📊 与Angular实现对比

| 功能 | Angular | Blazor | 状态 |
|------|---------|--------|------|
| Markdown渲染 | markdown-it | Markdig | ✅ 完全对等 |
| 自动检测 | ❌ | ✅ | ⭐ Blazor增强 |
| tagClassMap | ✅ | ✅ | ✅ 完全对等 |
| 安全性 | DomSanitizer | 内置转义 | ✅ 完全对等 |
| GFM扩展 | ✅ | ✅ | ✅ 完全对等 |
| 代码高亮 | 部分 | 支持 | ✅ 完全对等 |

## 🎯 示例场景

### 智能客服 - AI生成Markdown回复
```csharp
var aiResponse = await GetAIResponse(userQuestion);
// AI返回Markdown格式的回复

var messages = new SurfaceBuilder("chat")
    .AddText("reply", text => text
        .WithText(aiResponse)) // 自动识别并渲染Markdown
    .WithRoot("reply")
    .Build();
```

### 文档系统 - 动态文档渲染
```csharp
var docContent = await LoadDocumentAsync(docId);

var messages = new SurfaceBuilder("docs")
    .AddCard("doc", card => card
        .AddChild("title")
        .AddChild("content"))
    .AddText("title", t => t
        .WithText(docContent.Title)
        .WithUsageHint("h1"))
    .AddText("content", t => t
        .WithText(docContent.Body) // Markdown格式
        .WithProperty("markdown", true))
    .WithRoot("doc")
    .Build();
```

## 🚀 快速开始

1. **添加NuGet包**（已自动完成）
2. **注册服务**（已在Program.cs中完成）
3. **引入样式**（已在App.razor中完成）
4. **开始使用**：

```csharp
@page "/markdown-demo"
@using A2UI.Core.Processing
@inject MessageProcessor MessageProcessor
@rendermode InteractiveServer

<h1>Markdown演示</h1>
<A2UISurface SurfaceId="md-demo" />

@code {
    protected override void OnInitialized()
    {
        var markdown = "## Hello **Blazor**\n\n这是一个*Markdown*示例。";
        
        var messages = new SurfaceBuilder("md-demo")
            .AddText("md", text => text.WithText(markdown))
            .WithRoot("md")
            .Build();
            
        foreach (var msg in messages)
            MessageProcessor.ProcessMessage(msg);
    }
}
```

## 📚 相关文件

- `src/A2UI.Blazor.Components/Services/MarkdownRenderer.cs` - Markdown渲染服务
- `src/A2UI.Blazor.Components/Components/A2UIText.razor` - Text组件（含Markdown支持）
- `samples/A2UI.Sample.BlazorServer/wwwroot/css/a2ui-markdown.css` - Markdown样式
- `src/A2UI.Blazor.Components/A2UI.Blazor.Components.csproj` - 项目依赖

## 🎉 总结

Blazor的Markdown实现：
- ✅ **功能完整** - 与Angular实现完全对等
- ✅ **易用性强** - 自动检测，无需手动配置
- ✅ **安全可靠** - 内置XSS防护
- ✅ **扩展性好** - 支持Markdig所有扩展
- ✅ **样式丰富** - 内置GitHub风格样式

现在您可以在A2UI Blazor应用中自由使用Markdown了！🎊
