using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthors.Utilities
{
    public class GroupByVersionSwagger : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace; // Controller v1
            var apiVersion = namespaceController.Split(".").Last().ToLower(); // v1
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}
