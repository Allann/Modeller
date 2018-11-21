using Hy.Modeller.Interfaces;

namespace Hy.Modeller.Generator
{
    internal abstract class ValidatorBase : IValidator
    {
        protected ValidatorBase(Context context)
        {
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        protected void AddIssue(string issue) => Context.AddIssue(issue);

        public abstract void Validate();
    }

}
