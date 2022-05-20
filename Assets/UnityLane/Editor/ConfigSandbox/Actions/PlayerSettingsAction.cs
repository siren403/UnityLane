using UnityEditor;

namespace UnityLane.Editor.ConfigSandbox.Actions
{
    [Action("player-settings")]
    public class PlayerSettingsAction : IAction
    {
        public TargetPlatform Targets => TargetPlatform.All;

        private readonly string _companyName;
        private readonly string _productName;
        private readonly string _version;

        public PlayerSettingsAction(
            string companyName,
            string productName,
            string version = null
        )
        {
            _companyName = companyName;
            _productName = productName;
            _version = version;
        }


        public void Execute(WorkflowContext context)
        {
            PlayerSettings.companyName = context.Format(_companyName);
            PlayerSettings.productName = context.Format(_productName);
            if (!string.IsNullOrEmpty(_version))
            {
                PlayerSettings.bundleVersion = _version;
            }
        }
    }
}