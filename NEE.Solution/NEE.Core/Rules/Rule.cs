namespace NEE.Core.Rules
{
    public abstract class Rule
    {
        public string Name { get; set; }

        abstract public bool? CheckHasFailed();

        public bool? HasFailed { get; set; }

        public Rule(string name = null)
        {
            Name = name;
        }
    }
}
