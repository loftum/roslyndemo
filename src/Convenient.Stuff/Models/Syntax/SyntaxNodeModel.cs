using System.Collections.Generic;

namespace Convenient.Stuff.Models.Syntax
{
    public class SyntaxNodeModel
    {
        public string Type { get; set; }
        public string Kind { get; set; }
        public string Text { get; set; }
        public List<SyntaxNodeModel> ChildNodes { get; set; }
        public List<SyntaxTokenModel> ChildTokens { get; set; }

        public SyntaxNodeModel()
        {
            ChildNodes = new List<SyntaxNodeModel>();
            ChildTokens = new List<SyntaxTokenModel>();
        }
    }
}