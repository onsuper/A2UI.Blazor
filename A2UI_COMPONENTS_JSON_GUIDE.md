# A2UI 组件 JSON 使用指南

本文档详细说明如何使用 JSON 来定义和渲染各种 A2UI 组件。

## 📋 目录

- [基本概念](#基本概念)
- [消息结构](#消息结构)
- [布局组件](#布局组件)
  - [Card (卡片)](#card-卡片)
  - [Column (列布局)](#column-列布局)
  - [Row (行布局)](#row-行布局)
  - [List (列表)](#list-列表)
  - [Divider (分割线)](#divider-分割线)
- [基础组件](#基础组件)
  - [Text (文本)](#text-文本)
  - [Button (按钮)](#button-按钮)
  - [Image (图片)](#image-图片)
- [输入组件](#输入组件)
  - [TextField (文本输入框)](#textfield-文本输入框)
  - [CheckBox (复选框)](#checkbox-复选框)
  - [Slider (滑动条)](#slider-滑动条)
  - [DateTimeInput (日期时间输入)](#datetimeinput-日期时间输入)
- [高级组件](#高级组件)
  - [MultipleChoice (多选题)](#multiplechoice-多选题)
  - [Tabs (标签页)](#tabs-标签页)
  - [Modal (模态框)](#modal-模态框)
- [媒体组件](#媒体组件)
  - [AudioPlayer (音频播放器)](#audioplayer-音频播放器)
  - [Video (视频)](#video-视频)
  - [Icon (图标)](#icon-图标)
- [数据绑定](#数据绑定)
- [事件处理](#事件处理)
- [完整示例](#完整示例)

---

## 基本概念

A2UI 使用声明式 JSON 来描述用户界面。Agent 返回包含组件定义和数据的消息，客户端渲染器将其转换为实际的 UI。

### 核心原则

1. **组件定义**: 每个组件都有唯一的 ID 和类型
2. **父子关系**: 通过 ID 引用建立组件层次结构
3. **数据绑定**: 组件属性可以绑定到数据模型
4. **事件响应**: 用户交互触发动作，发送回 Agent

---

## 消息结构

一个完整的 UI 渲染流程通常包含以下消息：

### surfaceId 说明

**什么是 surfaceId？**

`surfaceId` 是一个唯一标识符，用于区分不同的 UI 渲染表面（Surface）。每个 Surface 代表一个独立的 UI 渲染区域，包含自己的组件树和数据模型。

**surfaceId 的生成方式：**

1. **客户端生成**：由客户端应用在发起查询前生成，确保唯一性
2. **命名规范**：建议使用描述性名称，如：
   - `"demo-surface"` - 演示表面
   - `"chat-{timestamp}"` - 聊天会话
   - `"surface-{counter}"` - 序号递增
   - `"user-{userId}-dashboard"` - 用户仪表板

**示例代码（C# Blazor）：**

```csharp
// 方式1：使用计数器
private int _surfaceCounter = 0;
var surfaceId = $"surface-{++_surfaceCounter}";

// 方式2：使用 GUID
var surfaceId = $"surface-{Guid.NewGuid():N}";

// 方式3：使用时间戳
var surfaceId = $"chat-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

// 方式4：基于业务场景
var surfaceId = $"user-{userId}-dashboard";
```

**重要提示：**
- surfaceId 必须在同一个应用实例中保持唯一
- Agent 返回的所有消息必须使用相同的 surfaceId
- 同一个 surfaceId 对应的组件树和数据模型是隔离的
- 可以通过清除 surface 来重置 UI 状态

### 1. BeginRendering - 开始渲染

```json
{
  "beginRendering": {
    "surfaceId": "demo-surface",
    "root": "root-component-id",
    "catalogId": "org.a2ui.standard@0.8"
  }
}
```

**参数说明：**
- `surfaceId`: 由客户端生成的唯一标识符
- `root`: 根组件的 ID
- `catalogId`: 组件目录版本（通常为 "org.a2ui.standard@0.8"）

### 2. SurfaceUpdate - 更新界面

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo-surface",
    "components": [
      {
        "id": "component-id",
        "component": {
          "ComponentType": {
            // 组件属性
          }
        }
      }
    ]
  }
}
```

**参数说明：**
- `surfaceId`: 必须与 BeginRendering 中的 surfaceId 一致
- `components`: 组件定义数组，可以包含新组件或更新现有组件

### 3. DataModelUpdate - 更新数据（可选）

```json
{
  "dataModelUpdate": {
    "surfaceId": "demo-surface",
    "path": "/",
    "contents": [
      {
        "key": "dataKey",
        "valueString": "value"
      }
    ]
  }
}
```

**参数说明：**
- `surfaceId`: 必须与 BeginRendering 中的 surfaceId 一致
- `path`: 数据更新的路径，`"/"` 表示根路径
- `contents`: 数据条目数组

---

## 布局组件

### Card (卡片)

Card 是一个容器组件，用于包装其他组件，提供视觉分组。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| child | string | 是 | 子组件的 ID |

#### JSON 示例

```json
{
  "id": "my-card",
  "component": {
    "Card": {
      "child": "card-content"
    }
  }
}
```

#### 完整示例

```json
[
  {
    "beginRendering": {
      "surfaceId": "demo",
      "root": "card1",
      "catalogId": "org.a2ui.standard@0.8"
    }
  },
  {
    "surfaceUpdate": {
      "surfaceId": "demo",
      "components": [
        {
          "id": "card1",
          "component": {
            "Card": {
              "child": "text1"
            }
          }
        },
        {
          "id": "text1",
          "component": {
            "Text": {
              "text": {
                "literalString": "这是卡片内容"
              }
            }
          }
        }
      ]
    }
  }
]
```

---

### Column (列布局)

Column 垂直排列多个子组件。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| children | object | 是 | 子组件列表 |
| children.explicitList | array | 是 | 子组件 ID 数组 |
| distribution | string | 否 | 分布方式 (start, center, end, spaceBetween, spaceAround) |
| alignment | string | 否 | 对齐方式 (start, center, end) |

#### JSON 示例

```json
{
  "id": "my-column",
  "component": {
    "Column": {
      "children": {
        "explicitList": ["child1", "child2", "child3"]
      },
      "alignment": "center"
    }
  }
}
```

#### 完整示例

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "main-column",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["title", "description", "button"]
            }
          }
        }
      },
      {
        "id": "title",
        "component": {
          "Text": {
            "text": { "literalString": "标题" },
            "usageHint": "h1"
          }
        }
      },
      {
        "id": "description",
        "component": {
          "Text": {
            "text": { "literalString": "这是描述文本" }
          }
        }
      },
      {
        "id": "button",
        "component": {
          "Button": {
            "child": "btn-text",
            "primary": true
          }
        }
      },
      {
        "id": "btn-text",
        "component": {
          "Text": {
            "text": { "literalString": "点击我" }
          }
        }
      }
    ]
  }
}
```

---

### Row (行布局)

Row 水平排列多个子组件。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| children | object | 是 | 子组件列表 |
| children.explicitList | array | 是 | 子组件 ID 数组 |
| distribution | string | 否 | 分布方式 (start, center, end, spaceBetween, spaceAround) |
| alignment | string | 否 | 对齐方式 (start, center, end) |

#### JSON 示例

```json
{
  "id": "my-row",
  "component": {
    "Row": {
      "children": {
        "explicitList": ["item1", "item2", "item3"]
      },
      "distribution": "spaceBetween",
      "alignment": "center"
    }
  }
}
```

#### 完整示例 - 按钮组

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "button-row",
        "component": {
          "Row": {
            "children": {
              "explicitList": ["btn-save", "btn-cancel"]
            },
            "distribution": "end"
          }
        }
      },
      {
        "id": "btn-save",
        "component": {
          "Button": {
            "child": "btn-save-text",
            "primary": true,
            "action": {
              "name": "save"
            }
          }
        }
      },
      {
        "id": "btn-save-text",
        "component": {
          "Text": {
            "text": { "literalString": "保存" }
          }
        }
      },
      {
        "id": "btn-cancel",
        "component": {
          "Button": {
            "child": "btn-cancel-text",
            "action": {
              "name": "cancel"
            }
          }
        }
      },
      {
        "id": "btn-cancel-text",
        "component": {
          "Text": {
            "text": { "literalString": "取消" }
          }
        }
      }
    ]
  }
}
```

---

### List (列表)

List 用于渲染重复的数据项，支持模板绑定。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| children | object | 是 | 子组件配置 |
| children.explicitList | array | 否 | 显式子组件 ID 列表 |
| children.template | object | 否 | 模板配置（用于数据绑定） |
| direction | string | 否 | 方向 (vertical/horizontal) |
| alignment | string | 否 | 对齐方式 |

#### JSON 示例 - 显式列表

```json
{
  "id": "my-list",
  "component": {
    "List": {
      "direction": "vertical",
      "children": {
        "explicitList": ["item1", "item2", "item3"]
      }
    }
  }
}
```

#### JSON 示例 - 模板绑定

```json
{
  "id": "contact-list",
  "component": {
    "List": {
      "direction": "vertical",
      "children": {
        "template": {
          "componentId": "contact-card-template",
          "dataBinding": "/contacts"
        }
      }
    }
  }
}
```

#### 完整示例 - 联系人列表

```json
[
  {
    "surfaceUpdate": {
      "surfaceId": "demo",
      "components": [
        {
          "id": "root",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["title", "contact-list"]
              }
            }
          }
        },
        {
          "id": "title",
          "component": {
            "Text": {
              "text": { "literalString": "联系人列表" },
              "usageHint": "h1"
            }
          }
        },
        {
          "id": "contact-list",
          "component": {
            "List": {
              "direction": "vertical",
              "children": {
                "template": {
                  "componentId": "contact-card",
                  "dataBinding": "/contacts"
                }
              }
            }
          }
        },
        {
          "id": "contact-card",
          "component": {
            "Card": {
              "child": "contact-info"
            }
          }
        },
        {
          "id": "contact-info",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["name-text", "title-text"]
              }
            }
          }
        },
        {
          "id": "name-text",
          "component": {
            "Text": {
              "text": { "path": "name" },
              "usageHint": "h3"
            }
          }
        },
        {
          "id": "title-text",
          "component": {
            "Text": {
              "text": { "path": "title" }
            }
          }
        }
      ]
    }
  },
  {
    "dataModelUpdate": {
      "surfaceId": "demo",
      "path": "/",
      "contents": [
        {
          "key": "contacts",
          "valueMap": [
            {
              "key": "contact1",
              "valueMap": [
                { "key": "name", "valueString": "张三" },
                { "key": "title", "valueString": "工程师" }
              ]
            },
            {
              "key": "contact2",
              "valueMap": [
                { "key": "name", "valueString": "李四" },
                { "key": "title", "valueString": "设计师" }
              ]
            }
          ]
        }
      ]
    }
  }
]
```

---

### Divider (分割线)

Divider 在内容之间添加视觉分隔。

#### 属性

无特殊属性，仅渲染一条水平线。

#### JSON 示例

```json
{
  "id": "divider1",
  "component": {
    "Divider": {}
  }
}
```

#### 完整示例

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "content",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["section1", "divider", "section2"]
            }
          }
        }
      },
      {
        "id": "section1",
        "component": {
          "Text": {
            "text": { "literalString": "第一部分内容" }
          }
        }
      },
      {
        "id": "divider",
        "component": {
          "Divider": {}
        }
      },
      {
        "id": "section2",
        "component": {
          "Text": {
            "text": { "literalString": "第二部分内容" }
          }
        }
      }
    ]
  }
}
```

---

## 基础组件

### Text (文本)

Text 用于显示文本内容，支持不同的样式提示。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| text | object | 是 | 文本内容 |
| text.literalString | string | * | 字面字符串 |
| text.path | string | * | 数据绑定路径 |
| usageHint | string | 否 | 样式提示 (h1, h2, h3, h4, h5, caption) |

\* text.literalString 和 text.path 二选一

#### JSON 示例 - 字面文本

```json
{
  "id": "welcome-text",
  "component": {
    "Text": {
      "text": {
        "literalString": "欢迎使用 A2UI！"
      },
      "usageHint": "h1"
    }
  }
}
```

#### JSON 示例 - 数据绑定

```json
{
  "id": "username-text",
  "component": {
    "Text": {
      "text": {
        "path": "user.name"
      },
      "usageHint": "h2"
    }
  }
}
```

#### usageHint 值说明

- `h1`: 一级标题（最大）
- `h2`: 二级标题
- `h3`: 三级标题
- `h4`: 四级标题
- `h5`: 五级标题
- `caption`: 说明文字（小字）
- 默认: 普通段落文本

#### 完整示例

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "content",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["title", "subtitle", "body"]
            }
          }
        }
      },
      {
        "id": "title",
        "component": {
          "Text": {
            "text": { "literalString": "这是主标题" },
            "usageHint": "h1"
          }
        }
      },
      {
        "id": "subtitle",
        "component": {
          "Text": {
            "text": { "literalString": "这是副标题" },
            "usageHint": "h3"
          }
        }
      },
      {
        "id": "body",
        "component": {
          "Text": {
            "text": { "literalString": "这是正文内容，可以包含详细信息。" }
          }
        }
      }
    ]
  }
}
```

---

### Button (按钮)

Button 触发用户交互，可以关联动作。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| child | string | 是 | 按钮文本组件的 ID |
| primary | boolean | 否 | 是否为主要按钮（默认 false） |
| action | object | 否 | 按钮动作配置 |
| action.name | string | 是 | 动作名称 |
| action.context | array | 否 | 动作上下文数据 |

#### JSON 示例 - 基础按钮

```json
{
  "id": "submit-btn",
  "component": {
    "Button": {
      "child": "submit-text",
      "primary": true,
      "action": {
        "name": "submit_form"
      }
    }
  }
}
```

#### JSON 示例 - 带上下文的按钮

```json
{
  "id": "delete-btn",
  "component": {
    "Button": {
      "child": "delete-text",
      "action": {
        "name": "delete_item",
        "context": [
          {
            "key": "itemId",
            "value": {
              "path": "id"
            }
          }
        ]
      }
    }
  }
}
```

#### 完整示例 - 表单提交

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "form",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["form-title", "name-input", "button-row"]
            }
          }
        }
      },
      {
        "id": "form-title",
        "component": {
          "Text": {
            "text": { "literalString": "用户信息表单" },
            "usageHint": "h2"
          }
        }
      },
      {
        "id": "name-input",
        "component": {
          "TextField": {
            "label": { "literalString": "姓名" },
            "text": { "path": "userName" }
          }
        }
      },
      {
        "id": "button-row",
        "component": {
          "Row": {
            "children": {
              "explicitList": ["submit-btn", "cancel-btn"]
            },
            "distribution": "end"
          }
        }
      },
      {
        "id": "submit-btn",
        "component": {
          "Button": {
            "child": "submit-text",
            "primary": true,
            "action": {
              "name": "submit_form",
              "context": [
                {
                  "key": "name",
                  "value": { "path": "userName" }
                }
              ]
            }
          }
        }
      },
      {
        "id": "submit-text",
        "component": {
          "Text": {
            "text": { "literalString": "提交" }
          }
        }
      },
      {
        "id": "cancel-btn",
        "component": {
          "Button": {
            "child": "cancel-text",
            "action": {
              "name": "cancel"
            }
          }
        }
      },
      {
        "id": "cancel-text",
        "component": {
          "Text": {
            "text": { "literalString": "取消" }
          }
        }
      }
    ]
  }
}
```

---

### Image (图片)

Image 显示图片内容。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| url | object | 是 | 图片 URL |
| url.literalString | string | * | 字面 URL |
| url.path | string | * | 数据绑定路径 |
| fit | string | 否 | 适配方式 (contain, cover, fill) |
| usageHint | string | 否 | 使用提示 (icon, avatar, smallFeature, mediumFeature, largeFeature, header) |

#### JSON 示例 - 字面 URL

```json
{
  "id": "logo",
  "component": {
    "Image": {
      "url": {
        "literalString": "https://example.com/logo.png"
      },
      "usageHint": "icon"
    }
  }
}
```

#### JSON 示例 - 数据绑定

```json
{
  "id": "user-avatar",
  "component": {
    "Image": {
      "url": {
        "path": "user.avatarUrl"
      },
      "usageHint": "avatar",
      "fit": "cover"
    }
  }
}
```

#### 完整示例 - 图片画廊

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "gallery",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["gallery-title", "image-row"]
            }
          }
        }
      },
      {
        "id": "gallery-title",
        "component": {
          "Text": {
            "text": { "literalString": "图片画廊" },
            "usageHint": "h2"
          }
        }
      },
      {
        "id": "image-row",
        "component": {
          "Row": {
            "children": {
              "explicitList": ["img1", "img2", "img3"]
            }
          }
        }
      },
      {
        "id": "img1",
        "component": {
          "Image": {
            "url": { "literalString": "https://picsum.photos/200/200" },
            "fit": "cover"
          }
        }
      },
      {
        "id": "img2",
        "component": {
          "Image": {
            "url": { "literalString": "https://picsum.photos/201/201" },
            "fit": "cover"
          }
        }
      },
      {
        "id": "img3",
        "component": {
          "Image": {
            "url": { "literalString": "https://picsum.photos/202/202" },
            "fit": "cover"
          }
        }
      }
    ]
  }
}
```

---

## 输入组件

### TextField (文本输入框)

TextField 允许用户输入文本。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| label | object | 否 | 输入框标签 |
| text | object | 是 | 文本值（支持数据绑定） |
| usageHint | string | 否 | 使用提示 (longText 显示为多行文本框) |
| validationRegexp | string | 否 | 验证正则表达式 |

#### JSON 示例

```json
{
  "id": "email-input",
  "component": {
    "TextField": {
      "label": {
        "literalString": "电子邮件"
      },
      "text": {
        "path": "user.email"
      },
      "validationRegexp": "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"
    }
  }
}
```

#### 完整示例 - 用户信息表单

```json
{
  "surfaceUpdate": {
    "surfaceId": "demo",
    "components": [
      {
        "id": "user-form",
        "component": {
          "Card": {
            "child": "form-column"
          }
        }
      },
      {
        "id": "form-column",
        "component": {
          "Column": {
            "children": {
              "explicitList": ["form-title", "name-field", "email-field", "bio-field", "submit-btn"]
            }
          }
        }
      },
      {
        "id": "form-title",
        "component": {
          "Text": {
            "text": { "literalString": "用户信息" },
            "usageHint": "h2"
          }
        }
      },
      {
        "id": "name-field",
        "component": {
          "TextField": {
            "label": { "literalString": "姓名" },
            "text": { "path": "name" }
          }
        }
      },
      {
        "id": "email-field",
        "component": {
          "TextField": {
            "label": { "literalString": "邮箱" },
            "text": { "path": "email" }
          }
        }
      },
      {
        "id": "bio-field",
        "component": {
          "TextField": {
            "label": { "literalString": "个人简介" },
            "text": { "path": "bio" },
            "usageHint": "longText"
          }
        }
      },
      {
        "id": "submit-btn",
        "component": {
          "Button": {
            "child": "submit-text",
            "primary": true,
            "action": {
              "name": "save_user"
            }
          }
        }
      },
      {
        "id": "submit-text",
        "component": {
          "Text": {
            "text": { "literalString": "保存" }
          }
        }
      }
    ]
  }
}
```

---

### CheckBox (复选框)

CheckBox 允许用户进行布尔选择。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| label | object | 是 | 复选框标签 |
| checked | object | 是 | 选中状态（支持数据绑定） |

#### JSON 示例

```json
{
  "id": "agree-checkbox",
  "component": {
    "CheckBox": {
      "label": {
        "literalString": "我同意服务条款"
      },
      "checked": {
        "path": "agreedToTerms"
      }
    }
  }
}
```

---

### Slider (滑动条)

Slider 允许用户从范围中选择数值。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| label | object | 否 | 滑动条标签 |
| value | object | 是 | 当前值（支持数据绑定） |
| min | number | 否 | 最小值（默认 0） |
| max | number | 否 | 最大值（默认 100） |
| step | number | 否 | 步长（默认 1） |

#### JSON 示例

```json
{
  "id": "volume-slider",
  "component": {
    "Slider": {
      "label": {
        "literalString": "音量"
      },
      "value": {
        "path": "volume"
      },
      "min": 0,
      "max": 100,
      "step": 5
    }
  }
}
```

---

### DateTimeInput (日期时间输入)

DateTimeInput 允许用户选择日期和时间。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| label | object | 否 | 输入框标签 |
| value | object | 是 | 日期时间值（支持数据绑定） |
| usageHint | string | 否 | 类型 (date, time, datetime) |

#### JSON 示例

```json
{
  "id": "birthday-input",
  "component": {
    "DateTimeInput": {
      "label": {
        "literalString": "出生日期"
      },
      "value": {
        "path": "birthday"
      },
      "usageHint": "date"
    }
  }
}
```

---

## 高级组件

### MultipleChoice (多选题)

MultipleChoice 显示多个选项供用户选择。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| question | object | 是 | 问题文本 |
| options | array | 是 | 选项列表 |
| selectedOption | object | 否 | 当前选中的选项 |

#### JSON 示例

```json
{
  "id": "survey-question",
  "component": {
    "MultipleChoice": {
      "question": {
        "literalString": "您最喜欢哪种编程语言？"
      },
      "options": ["Python", "JavaScript", "C#", "Java"],
      "selectedOption": {
        "path": "selectedLanguage"
      }
    }
  }
}
```

---

### Tabs (标签页)

Tabs 在多个内容面板之间切换。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| tabs | array | 是 | 标签页配置数组 |
| selectedTab | object | 否 | 当前选中的标签 |

#### JSON 示例

```json
{
  "id": "content-tabs",
  "component": {
    "Tabs": {
      "tabs": [
        {
          "id": "tab1",
          "label": "概览",
          "content": "overview-content"
        },
        {
          "id": "tab2",
          "label": "详情",
          "content": "details-content"
        }
      ],
      "selectedTab": {
        "path": "currentTab"
      }
    }
  }
}
```

---

### Modal (模态框)

Modal 在当前页面上方显示内容。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| child | string | 是 | 模态框内容组件 ID |
| isOpen | object | 是 | 是否打开（支持数据绑定） |
| title | object | 否 | 模态框标题 |

#### JSON 示例

```json
{
  "id": "confirm-modal",
  "component": {
    "Modal": {
      "title": {
        "literalString": "确认操作"
      },
      "child": "modal-content",
      "isOpen": {
        "path": "showModal"
      }
    }
  }
}
```

---

## 媒体组件

### AudioPlayer (音频播放器)

AudioPlayer 播放音频文件。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| url | object | 是 | 音频文件 URL |
| autoplay | boolean | 否 | 自动播放（默认 false） |

#### JSON 示例

```json
{
  "id": "music-player",
  "component": {
    "AudioPlayer": {
      "url": {
        "literalString": "https://example.com/audio.mp3"
      }
    }
  }
}
```

---

### Video (视频)

Video 播放视频文件。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| url | object | 是 | 视频文件 URL |
| autoplay | boolean | 否 | 自动播放（默认 false） |
| controls | boolean | 否 | 显示控件（默认 true） |

#### JSON 示例

```json
{
  "id": "tutorial-video",
  "component": {
    "Video": {
      "url": {
        "literalString": "https://example.com/video.mp4"
      },
      "controls": true
    }
  }
}
```

---

### Icon (图标)

Icon 显示图标。

#### 属性

| 属性 | 类型 | 必需 | 说明 |
|------|------|------|------|
| name | string | 是 | 图标名称 |
| size | string | 否 | 图标大小 |

#### JSON 示例

```json
{
  "id": "settings-icon",
  "component": {
    "Icon": {
      "name": "settings",
      "size": "medium"
    }
  }
}
```

---

## 数据绑定

A2UI 支持将组件属性绑定到数据模型。

### 字面值 (literalString)

直接提供字符串值：

```json
{
  "text": {
    "literalString": "Hello World"
  }
}
```

### 路径绑定 (path)

绑定到数据模型路径：

```json
{
  "text": {
    "path": "user.name"
  }
}
```

### 数据模型更新

使用 `dataModelUpdate` 消息设置数据：

```json
{
  "dataModelUpdate": {
    "surfaceId": "demo",
    "path": "/",
    "contents": [
      {
        "key": "user",
        "valueMap": [
          { "key": "name", "valueString": "张三" },
          { "key": "age", "valueNumber": 25 }
        ]
      }
    ]
  }
}
```

### 数据类型

- `valueString`: 字符串值
- `valueNumber`: 数值
- `valueBoolean`: 布尔值
- `valueMap`: 对象（嵌套键值对）
- `valueList`: 数组

---

## 事件处理

### Action 配置

按钮等组件可以配置 action 来处理用户交互：

```json
{
  "Button": {
    "child": "btn-text",
    "action": {
      "name": "action_name",
      "context": [
        {
          "key": "param1",
          "value": {
            "path": "data.field"
          }
        }
      ]
    }
  }
}
```

### 事件消息

用户触发事件后，客户端会发送 `userAction` 消息给 Agent：

```json
{
  "userAction": {
    "surfaceId": "demo",
    "componentId": "my-button",
    "action": {
      "name": "action_name",
      "context": {
        "param1": "value"
      }
    客户端-Agent 交互流程

以下是一个完整的客户端与 Agent 交互流程：

```
┌─────────────┐                                    ┌─────────────┐
│   客户端    │                                    │    Agent    │
└──────┬──────┘                                    └──────┬──────┘
       │                                                  │
       │ 1. 生成 surfaceId = "surface-1"                 │
       │                                                  │
       │ 2. 发送查询 + surfaceId                         │
       ├─────────────────────────────────────────────────>│
       │   "显示附近的餐厅"                               │
       │                                                  │
       │                        3. 处理查询，生成 A2UI JSON│
       │                                                  │
       │ 4. 返回消息数组                                  │
       │<─────────────────────────────────────────────────┤
       │   [BeginRendering, SurfaceUpdate, DataModelUpdate]│
       │   (所有消息包含相同的 surfaceId: "surface-1")    │
       │                                                  │
       │ 5. 渲染 UI                                       │
       │   <A2UISurface SurfaceId="surface-1" />          │
       │                                                  │
       │ 6. 用户点击按钮                                  │
       │                                                  │
       │ 7. 发送 UserAction + surfaceId                   │
       ├─────────────────────────────────────────────────>│
       │   { action: "reserve", surfaceId: "surface-1" }  │
       │                                                  │
       │                        8. 处理动作，生成新的 UI  │
       │                                                  │
       │ 9. 返回更新消息                                  │
       │<─────────────────────────────────────────────────┤
       │   [SurfaceUpdate]                                │
       │                                                  │
```

### }
  }
}
```

---

## 完整示例

### 餐厅推荐应用

以下是一个完整的餐厅推荐界面示例：

```json
[
  {
    "beginRendering": {
      "surfaceId": "restaurant-app",
      "root": "main-layout",
      "catalogId": "org.a2ui.standard@0.8"
    }
  },
  {
    "surfaceUpdate": {
      "surfaceId": "restaurant-app",
      "components": [
        {
          "id": "main-layout",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["header", "restaurant-list"]
              }
            }
          }
        },
        {
          "id": "header",
          "component": {
            "Card": {
              "child": "header-content"
            }
          }
        },
        {
          "id": "header-content",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["app-title", "app-subtitle"]
              }
            }
          }
        },
        {
          "id": "app-title",
          "component": {
            "Text": {
              "text": {
                "literalString": "🍽️ 附近餐厅"
              },
              "usageHint": "h1"
            }
          }
        },
        {
          "id": "app-subtitle",
          "component": {
            "Text": {
              "text": {
                "literalString": "为您推荐附近的优质餐厅"
              }
            }
          }
        },
        {
          "id": "restaurant-list",
          "component": {
            "List": {
              "direction": "vertical",
              "children": {
                "template": {
                  "componentId": "restaurant-card",
                  "dataBinding": "/restaurants"
                }
              }
            }
          }
        },
        {
          "id": "restaurant-card",
          "component": {
            "Card": {
              "child": "restaurant-layout"
            }
          }
        },
        {
          "id": "restaurant-layout",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["restaurant-header", "restaurant-info", "restaurant-actions"]
              }
            }
          }
        },
        {
          "id": "restaurant-header",
          "component": {
            "Row": {
              "children": {
                "explicitList": ["restaurant-image", "restaurant-basic"]
              }
            }
          }
        },
        {
          "id": "restaurant-image",
          "component": {
            "Image": {
              "url": {
                "path": "image"
              },
              "usageHint": "mediumFeature",
              "fit": "cover"
            }
          }
        },
        {
          "id": "restaurant-basic",
          "component": {
            "Column": {
              "children": {
                "explicitList": ["restaurant-name", "restaurant-cuisine"]
              }
            }
          }
        },
        {
          "id": "restaurant-name",
          "component": {
            "Text": {
              "text": {
                "path": "name"
              },
              "usageHint": "h2"
            }
          }
        },
        {
          "id": "restaurant-cuisine",
          "component": {
            "Text": {
              "text": {
                "path": "cuisine"
              }
            }
          }
        },
        {
          "id": "restaurant-info",
          "component": {
            "Row": {
              "children": {
                "explicitList": ["rating-text", "distance-text"]
              },
              "distribution": "spaceBetween"
            }
          }
        },
        {
          "id": "rating-text",
          "component": {
            "Text": {
              "text": {
                "path": "rating"
              }
            }
          }
        },
        {
          "id": "distance-text",
          "component": {
            "Text": {
              "text": {
                "path": "distance"
              }
            }
          }
        },
        {
          "id": "restaurant-actions",
          "component": {
            "Row": {
              "children": {
                "explicitList": ["view-btn", "reserve-btn"]
              },
              "distribution": "end"
            }
          }
        },
        {
          "id": "view-btn",
          "component": {
            "Button": {
              "child": "view-btn-text",
              "action": {
                "name": "view_restaurant",
                "context": [
                  {
                    "key": "restaurantId",
                    "value": {
                      "path": "id"
                    }
                  }
                ]
              }
            }
          }
        },
        {
          "id": "view-btn-text",
          "component": {
            "Text": {
              "text": {
                "literalString": "查看详情"
              }
            }
          }
        },
        {
          "id": "reserve-btn",
          "component": {
            "Button": {
              "child": "reserve-btn-text",
              "primary": true,
              "action": {
                "name": "reserve_table",
                "context": [
                  {
                    "key": "restaurantName",
                    "value": {
                      "path": "name"
                    }
                  }
                ]
              }
            }
          }
        },
        {
          "id": "reserve-btn-text",
          "component": {
            "Text": {
              "text": {
                "literalString": "预订"
              }
            }
          }
        }
      ]
    }
  },
  {
    "dataModelUpdate": {
      "surfaceId": "restaurant-app",
      "path": "/",
      "contents": [
        {
          "key": "restaurants",
          "valueMap": [
            {
              "key": "rest1",
              "valueMap": [
                { "key": "id", "valueString": "1" },
                { "key": "name", "valueString": "川味小馆" },
                { "key": "cuisine", "valueString": "川菜" },
                { "key": "rating", "valueString": "⭐ 4.5 分" },
                { "key": "distance", "valueString": "📍 500米" },
                { "key": "image", "valueString": "https://picsum.photos/300/200" }
              ]
            },
            {
              "key": "rest2",
              "valueMap": [
                { "key": "id", "valueString": "2" },
                { "key": "name", "valueString": "日式料理" },
                { "key": "cuisine", "valueString": "日本料理" },
                { "key": "rating", "valueString": "⭐ 4.8 分" },
                { "key": "distance", "valueString": "📍 800米" },
                { "key": "image", "valueString": "https://picsum.photos/301/200" }
              ]
            },
            {
              "key": "rest3",
              "valueMap": [
                { "key": "id", "valueString": "3" },
                { "key": "name", "valueString": "意式披萨" },
                { "key": "cuisine", "valueString": "意大利菜" },
                { "key": "rating", "valueString": "⭐ 4.3 分" },
                { "key": "distance", "valueString": "📍 1.2公里" },
                { "key": "image", "valueString": "https://picsum.photos/302/200" }
              ]
            }
          ]
        }
      ]
    }
  }
]
```

---

## 最佳实践

### 1. 组件命名

使用描述性的组件 ID：
- ✅ `user-profile-card`
- ✅ `submit-button-text`
- ❌ `comp1`
- ❌ `text123`

### 2. 组件层次

保持合理的嵌套深度（建议不超过 5 层）：

```json
root
└── main-card
    └── content-column
        ├── header-row
        └── body-section
```

### 3. 数据组织

将相关数据组织在一起：

```json
{surfaceId 如何选择？

**建议：**
- 每次新的对话或查询使用新的 surfaceId
- 避免 surfaceId 冲突，使用递增计数器或 GUID
- 在多用户环境中，可以在 surfaceId 中包含用户标识
- 开发调试时，使用有意义的名称便于追踪

### Q: 组件不显示？

**检查清单：**
1. 确认 `beginRendering` 的 `root` ID 正确
2. 确认所有引用的子组件 ID 都已定义
3. 检查所有消息的 `surfaceId` 是否一致
4. 确认 surfaceId 在客户端已正确生成和传递
5   "settings": {
      "theme": "...",
      "language": "..."
    }
  }
}
```

### 4. 复用模板

使用 List 模板复用组件：

```json
{
  "List": {
    "children": {
      "template": {
        "componentId": "item-template",
        "dataBinding": "/items"
      }
    }
  }
}
```

### 5. 错误处理

始终提供有意义的默认值和错误提示。

---

## 常见问题

### Q: 组件不显示？

**检查清单：**
1. 确认 `beginRendering` 的 `root` ID 正确
2. 确认所有引用的子组件 ID 都已定义
3. 检查 `surfaceId` 是否一致
4. 查看浏览器控制台的错误信息

### Q: 数据绑定不工作？

**检查清单：**
1. 确认数据路径正确（区分大小写）
2. 确认 `dataModelUpdate` 已发送
3. 检查数据类型是否匹配（valueString, valueNumber 等）

### Q: 按钮点击无响应？

**检查清单：**
1. 确认 `action.name` 已设置
2. 确认客户端已正确处理 `userAction` 事件
3. 检查 Agent 是否收到事件消息

---

## 总结

A2UI 提供了一套强大而灵活的 JSON 格式来描述用户界面。通过组合这些组件，可以创建丰富的交互式应用。

**关键要点：**
- 使用清晰的组件 ID 和层次结构
- 善用数据绑定减少重复
- 通过 List 模板复用组件
- 合理设计 action 来处理用户交互
- 遵循最佳实践提高可维护性

---

## 相关资源

- [A2UI 规范文档](../A2UI/specification/)
- [示例项目](../samples/)
- [API 参考](./API_REFERENCE.md)

---

**文档版本：** 1.0  
**更新日期：** 2025-12-26  
**适用于：** A2UI.Blazor v0.8+
