/*
 Copyright 2025 xuzeyu

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

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text;

namespace A2UI.Blazor.Components.Services;

/// <summary>
/// Markdown renderer for A2UI text components.
/// Provides safe HTML rendering with custom tag class mapping.
/// </summary>
public class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownRenderer()
    {
        // Configure Markdig pipeline with common extensions
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Tables, task lists, etc.
            .UseAutoLinks() // Auto-detect URLs
            .UseEmphasisExtras() // Strikethrough, superscript, etc.
            .UsePipeTables() // GitHub-style tables
            .UseListExtras() // Extra list features
            .Build();
    }

    /// <summary>
    /// Render markdown to safe HTML string.
    /// </summary>
    /// <param name="markdown">The markdown text to render</param>
    /// <param name="tagClassMap">Optional CSS classes to apply to specific HTML tags</param>
    /// <returns>Rendered HTML string (safe for MarkupString)</returns>
    public string Render(string markdown, Dictionary<string, string[]>? tagClassMap = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Parse markdown to document
        var document = Markdown.Parse(markdown, _pipeline);

        // Apply tag class mapping if provided
        if (tagClassMap != null && tagClassMap.Count > 0)
        {
            ApplyTagClassMap(document, tagClassMap);
        }

        // Render to HTML
        using var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(document);
        writer.Flush();

        return writer.ToString();
    }

    /// <summary>
    /// Apply CSS classes to specific HTML tags in the markdown document.
    /// </summary>
    private void ApplyTagClassMap(MarkdownDocument document, Dictionary<string, string[]> tagClassMap)
    {
        foreach (var block in document.Descendants())
        {
            // Handle block-level elements
            switch (block)
            {
                case ParagraphBlock p when tagClassMap.ContainsKey("p"):
                    ApplyClasses(p, tagClassMap["p"]);
                    break;
                case HeadingBlock h when tagClassMap.ContainsKey($"h{h.Level}"):
                    ApplyClasses(h, tagClassMap[$"h{h.Level}"]);
                    break;
                case ListBlock list:
                    if (list.IsOrdered && tagClassMap.ContainsKey("ol"))
                        ApplyClasses(list, tagClassMap["ol"]);
                    else if (!list.IsOrdered && tagClassMap.ContainsKey("ul"))
                        ApplyClasses(list, tagClassMap["ul"]);
                    break;
                case ListItemBlock li when tagClassMap.ContainsKey("li"):
                    ApplyClasses(li, tagClassMap["li"]);
                    break;
                case CodeBlock code when tagClassMap.ContainsKey("code"):
                    ApplyClasses(code, tagClassMap["code"]);
                    break;
                case QuoteBlock quote when tagClassMap.ContainsKey("blockquote"):
                    ApplyClasses(quote, tagClassMap["blockquote"]);
                    break;
            }

            // Handle inline elements
            if (block is ContainerBlock container)
            {
                foreach (var inline in container.Descendants<Inline>())
                {
                    switch (inline)
                    {
                        case LinkInline link when tagClassMap.ContainsKey("a"):
                            ApplyInlineClasses(link, tagClassMap["a"]);
                            break;
                        case EmphasisInline em when em.DelimiterChar == '*' && em.DelimiterCount == 2 && tagClassMap.ContainsKey("strong"):
                            ApplyInlineClasses(em, tagClassMap["strong"]);
                            break;
                        case EmphasisInline em when em.DelimiterChar == '*' && em.DelimiterCount == 1 && tagClassMap.ContainsKey("em"):
                            ApplyInlineClasses(em, tagClassMap["em"]);
                            break;
                        case CodeInline code when tagClassMap.ContainsKey("code"):
                            ApplyInlineClasses(code, tagClassMap["code"]);
                            break;
                    }
                }
            }
        }
    }

    private void ApplyClasses(MarkdownObject obj, string[] classes)
    {
        // Get or create HtmlAttributes for the object
        var attributes = obj.TryGetAttributes() ?? new HtmlAttributes();
        foreach (var className in classes)
        {
            attributes.AddClass(className);
        }
        obj.SetAttributes(attributes);
    }

    private void ApplyInlineClasses(Inline inline, string[] classes)
    {
        // Get or create HtmlAttributes for the inline element
        var attributes = inline.TryGetAttributes() ?? new HtmlAttributes();
        foreach (var className in classes)
        {
            attributes.AddClass(className);
        }
        inline.SetAttributes(attributes);
    }

    /// <summary>
    /// Check if a string contains markdown syntax.
    /// </summary>
    public static bool IsMarkdown(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Common markdown indicators
        return text.Contains("##") || // Headers
               text.Contains("**") || // Bold
               text.Contains("__") || // Bold alternative
               (text.Contains('*') && text.IndexOf('*') != text.LastIndexOf('*')) || // Italic
               (text.Contains('[') && text.Contains(']') && text.Contains('(')) || // Links
               text.Contains("```") || // Code blocks
               text.Contains('`') || // Inline code
               text.Contains("- ") || // Lists
               text.Contains("* ") || // Lists
               text.Contains("1. ") || // Numbered lists
               text.Contains("> "); // Blockquotes
    }

    /// <summary>
    /// Convert plain text to safe HTML (escaping HTML entities).
    /// </summary>
    public static string EscapeHtml(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return System.Net.WebUtility.HtmlEncode(text);
    }
}
