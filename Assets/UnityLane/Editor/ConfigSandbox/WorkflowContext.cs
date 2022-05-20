namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowContext
    {
        private readonly WorkflowArgumentView _argumentView;

        public TargetPlatform Target { get; }
        public WorkflowContext(WorkflowArgumentView argumentView, TargetPlatform target)
        {
            _argumentView = argumentView;
            Target = target;
        }

        public string Format(string format) => _argumentView.Format(format);
    }
}