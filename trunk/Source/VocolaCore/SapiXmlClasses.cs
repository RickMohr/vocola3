using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vocola
{

    public class SapiGrammar
    {
        private List<SapiRule> rules = new List<SapiRule>();

        public void Add(SapiRule rule)
        {
            rules.Add(rule);
        }

        public string GetXml()
        {
            TheStringBuilder = new StringBuilder();
            WriteLine(0, "<grammar LANGID=\"{0:x}\">", Win.GetCurrentLanguageID());
            foreach (SapiRule rule in rules)
                rule.AddXml(this, 1);
            WriteLine(0, "</grammar>");
            return TheStringBuilder.ToString();
        }

        public int SpacesPerIndentLevel = 2;
        private StringBuilder TheStringBuilder;

        public void WriteLine(int indent, string text, params object[] arguments)
        {
            TheStringBuilder.Append(' ', indent * SpacesPerIndentLevel);
            if (arguments.Length == 0)
                TheStringBuilder.AppendLine(text);
            else
                TheStringBuilder.AppendLine(String.Format(text, arguments));
        }

    }

    public interface SapiElement
    {
        void AddXml(SapiGrammar g, int indent);
    }

    public abstract class SapiContainer : SapiElement
    {
        protected List<SapiElement> Elements = new List<SapiElement>();

        public int Count { get { return Elements.Count; } }

        public void Add(SapiElement element)
        {
            Elements.Add(element);
        }

        public virtual void AddXml(SapiGrammar g, int indent)
        {
            foreach (SapiElement element in Elements)
                element.AddXml(g, indent);
        }
    }

    public class SapiRule : SapiContainer
    {
        public string RuleName;
        private bool IsPublic;
        public bool Export = false;
        public string Id = null;

        public SapiRule(string ruleName, bool isPublic, SapiChoices choices)
        {
            RuleName = ruleName;
            IsPublic = isPublic;
            Elements.Add(choices);
        }

        public SapiRule(string ruleName, bool isPublic, List<SapiElement> elements)
        {
            RuleName = ruleName;
            IsPublic = isPublic;
            Elements = elements;
        }

        public SapiRule()
        {
        }

        public override void AddXml(SapiGrammar g, int indent)
        {
            string attributes = "";
            if (IsPublic)   attributes += " toplevel=\"active\"";
            if (Export)     attributes += " export=\"true\"";
            if (Id != null) attributes += " id=\"" + Id + "\"";
            g.WriteLine(indent, "<rule name=\"{0}\"{1}>", RuleName, attributes);
            base.AddXml(g, indent + 1);
            g.WriteLine(indent, "</rule>");
        }
    }

    public class SapiDictationInCommandRule : SapiRule
    {
        public SapiDictationInCommandRule()
        {
        }

        public override void AddXml(SapiGrammar g, int indent)
        {
            g.WriteLine(indent, "<rule name=\"dictationInCommand\">");
            g.WriteLine(indent + 1, "<dictation max=\"inf\"/>");
            g.WriteLine(indent, "</rule>");
        }
    }

    public class SapiSequence : SapiContainer
    {
        public int Id = -1;

        public SapiSequence()
        {
        }

        public SapiSequence(List<SapiElement> elements)
        {
            Elements = elements;
        }

        public override void AddXml(SapiGrammar g, int indent)
        {
            if (Id == -1)
                g.WriteLine(indent, "<p>");
            else
                g.WriteLine(indent, "<p propid=\"{0}\">", Id);
            base.AddXml(g, indent + 1);
            g.WriteLine(indent, "</p>");
        }
    }

    public class SapiText : SapiElement
    {
        private string Text;
        public int Id = -1;
        public bool IsOptional = false;

        public SapiText(string text)
        {
            Text = XmlSafe(text);
        }

        public SapiText(string text, bool isOptional)
        {
            Text = XmlSafe(text);
            IsOptional = isOptional;
        }

        public SapiText(string text, int id)
        {
            Text = XmlSafe(text);
            Id = id;
        }

        private static string XmlSafe(string s)
        {
            if (s == "*")
                return "Asterisk";
            else if (s == "/")
                return "Slash";
            else
                return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        public void AddXml(SapiGrammar g, int indent)
        {
            if (IsOptional)
                g.WriteLine(indent, "<o>{0}</o>", Text);
            else if (Id == -1)
                g.WriteLine(indent, "<p>{0}</p>", Text);
            else
                g.WriteLine(indent, "<p propid=\"{0}\">{1}</p>", Id, Text);
        }
    }

    public class SapiChoices : SapiContainer
    {
        public override void AddXml(SapiGrammar g, int indent)
        {
            g.WriteLine(indent, "<l>");
            base.AddXml(g, indent + 1);
            g.WriteLine(indent, "</l>");
        }
    }

    public class SapiOptional : SapiContainer
    {
        public override void AddXml(SapiGrammar g, int indent)
        {
            g.WriteLine(indent, "<o>");
            base.AddXml(g, indent + 1);
            g.WriteLine(indent, "</o>");
        }
    }

    public class SapiRuleRef : SapiElement
    {
        private string ReferenceText;

        public SapiRuleRef(SapiRule rule)
        {
            ReferenceText = rule.RuleName;
        }

        public SapiRuleRef(string text)
        {
            ReferenceText = text;
        }

        public void AddXml(SapiGrammar g, int indent)
        {
            switch (ReferenceText)
            {
                case "_anything":
                    g.WriteLine(indent, "<ruleref name=\"dictationInCommand\"/>");
                    break;
                case "_textInDocument":
                    g.WriteLine(indent, "<ruleref url=\"sharing:Microsoft.SpeechUX.BuiltIn.DictationCommands\" name=\"Dictation1\"/>");
                    break;
                case "_itemInWindow":
                    g.WriteLine(indent, "<ruleref url=\"sharing:Microsoft.SpeechUX.BuiltIn.MSAACommands\" name=\"CTL_ITEM_TEXTBUFFER\"/>");
                    break;
                case "_startableName":
                    g.WriteLine(indent, "<ruleref url=\"sharing:Microsoft.SpeechUX.BuiltIn.LaunchCommands\" name=\"DYNAMIC_ITEM_TEXTBUFFER\"/>");
                    break;
                case "_vocolaDictation":
                    g.WriteLine(indent, "<textbuffer propid=\"1\"/>");
                    //g.WriteLine(indent, "<ruleref url=\"sharing:VocolaDictation\" name=\"VocolaDictation\"/>");
                    break;
                case "_windowTitle":
                    g.WriteLine(indent, "<ruleref url=\"sharing:Microsoft.SpeechUX.BuiltIn.SwitchCommands\" name=\"SWITCH_ITEM_TBUFFER\"/>");
                    break;
                default:
                    g.WriteLine(indent, "<ruleref name=\"{0}\"/>", ReferenceText);
                    break;
            }
        }
    }

}
