namespace Swagger.Clone.Services
{
    public class Api
    {
        public string MethodType { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string ActionName { get; set; }
        public string RelativeUrl { get; set; }
        public object Header { get; set; }
        public object Parameters { get; set; }
        public object Body { get; set; }
        public object Response { get; set; }


    }
}
