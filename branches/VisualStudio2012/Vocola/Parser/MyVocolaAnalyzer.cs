using PerCederberg.Grammatica.Parser;

namespace Vocola
{

    // Used only to distinguish different "unexpected end of file" conditions

    internal class MyVocolaAnalyzer : VocolaAnalyzer
    {
        public bool NothingParsed = true;
        public Node DanglingQuote = null;
        public Node DanglingIf = null;

        public override void Enter(Node node)
        {
            switch (node.GetId())
            {
            case (int) VocolaConstants.CONTEXT:
                DanglingIf = node;
                break;
            }
        }

        public override Node Exit(Node node)
        {
            NothingParsed = false;
            switch (node.GetId())
            {
            case (int) VocolaConstants.CONTEXT:
                DanglingIf = null;
                break;
            }
            return node;
        }

    }

}
