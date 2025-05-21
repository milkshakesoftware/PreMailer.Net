using AngleSharp.Dom;
using AngleSharp.Html;
using System;
using System.Collections.Generic;

namespace PreMailer.Net.Html
{
    public class EmailHtmlMarkupFormatter : HtmlMarkupFormatter
    {
        private static readonly Dictionary<string, string> EntityReplacements = new Dictionary<string, string>
        {
            { "©", "&copy;" },
            { "®", "&reg;" },
            { "™", "&trade;" },
            { "£", "&pound;" },
            { "€", "&euro;" },
            { "¥", "&yen;" },
            { "§", "&sect;" },
            { "±", "&plusmn;" },
            { "¼", "&frac14;" },
            { "½", "&frac12;" },
            { "¾", "&frac34;" }
        };
        
        public static new readonly EmailHtmlMarkupFormatter Instance = new EmailHtmlMarkupFormatter();

        public override string CloseTag(IElement element, Boolean selfClosing)
        {
            var prefix = element.Prefix;
            var name = element.LocalName;
            var tag = !String.IsNullOrEmpty(prefix) ? String.Concat(prefix, ":", name) : name;
            
            return String.Concat("</", tag, ">");
        }

        public override string Text(ICharacterData text)
        {
            var result = base.Text(text);
            
            foreach (var entity in EntityReplacements)
            {
                result = result.Replace(entity.Key, entity.Value);
            }
            
            return result;
        }
    }
}
