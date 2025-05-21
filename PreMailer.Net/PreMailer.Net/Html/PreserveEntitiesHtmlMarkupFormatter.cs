using AngleSharp.Html;
using AngleSharp.Dom;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PreMailer.Net.Html
{
    public class PreserveEntitiesHtmlMarkupFormatter : HtmlMarkupFormatter
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
        
        public static new readonly PreserveEntitiesHtmlMarkupFormatter Instance = new PreserveEntitiesHtmlMarkupFormatter();

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
