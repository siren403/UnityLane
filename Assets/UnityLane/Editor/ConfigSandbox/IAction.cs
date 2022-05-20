using YamlDotNet.Serialization;

namespace UnityLane.Editor.ConfigSandbox
{
    public interface IAction
    {
        TargetPlatform Targets { get; }
        void Execute(WorkflowContext context);
    }

    public interface IRegistration
    {
        void Register(DeserializerBuilder builder);
    }
}