using Modeller.Interfaces;

namespace Modeller.Outputs
{
    public class Snippet : ISnippet
    {
        public Snippet(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                content = string.Empty;
            }
            Content = content;
        }

        public Snippet(string name, string content)
            : this(content)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name=string.Empty;
            }
            Name = name;
        }

        public string Content { get; }

        public string Name { get; }
    }

}
