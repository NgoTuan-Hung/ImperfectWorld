using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TooltipHelper
{
    // Regex to match <link=key>...</link>
    private static readonly Regex LinkRegex = new Regex(@"<link=(.*?)>", RegexOptions.Compiled);

    /// <summary>
    /// Extract unique link keys from a string containing <link=...> tags.
    /// </summary>
    public static HashSet<string> ExtractLinks(string input)
    {
        var links = new HashSet<string>();
        var matches = LinkRegex.Matches(input);

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
                links.Add(match.Groups[1].Value);
        }

        return links;
    }

    /// <summary>
    /// Given a text with <link=...> tags, returns a combined tooltip of unique link descriptions.
    /// </summary>
    public static string GenerateTooltip(string input)
    {
        var links = ExtractLinks(input);
        var sb = new StringBuilder();
        string desc;

        foreach (var link in links)
        {
            if ((desc = GameManager.Instance.GetDescription(link)) != null)
            {
                sb.AppendLine(desc);
                sb.AppendLine();
            }
            else
            {
                Debug.LogWarning($"No description found for link '{link}'");
            }
        }

        return sb.ToString().Trim();
    }
}
