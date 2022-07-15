using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using System.Runtime;
using System.Collections;
using System.Text;
using Newtonsoft.Json;

namespace Swagger.Clone.Services
{
    public class SwaggerService
    {
        private List<Api> _apiList = new List<Api>();
        private readonly string[] _systemReturnTypes = { "Task", "ActionResult", "IActionResult" };
        private readonly string[] _systemCollections = { "IEnumerable", "List", "IList", "ICollection" };

        #region private all refrence data types values

        private readonly string _stringValue = "string";
        private readonly char _charValue = '"';
        private readonly string _strTOCheckJSParam = "\":";
        private string _curlyBracesStart = "{";
        private string _curlyBracesEnd = "}";
        private string _nullStr = "Nullable";
        #endregion

        /// Method to generate api configurations
        public void GenerateApiConfig(string dir)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var classes = assembly.GetExportedTypes()
                            .Where(type => type.IsClass)
                            .ToList();
            var controllers = classes.Where(cls => cls.Name.EndsWith("Controller"))
                            .ToList();

            foreach (var controller in controllers)
            {
                #region  setting controllers
                string className = controller.Name;
                var attributes = controller.GetCustomAttributes(typeof(Attribute));
                int controllerIndex = controller.Name.IndexOf("Controller");
                string controllerName = controller.Name.Substring(0, controllerIndex);

                var versionAttribute = (ApiVersionAttribute)attributes
                                        .FirstOrDefault(att => att.GetType() == typeof(ApiVersionAttribute));


                ////version settings
                if (versionAttribute != null)
                {
                    var versions = versionAttribute?.Versions
                                .Select(x => x.MajorVersion).FirstOrDefault();

                }

                var routeAttribute = (RouteAttribute)attributes
                                    .FirstOrDefault(att => att
                                    .GetType() == typeof(RouteAttribute));
                if (routeAttribute != null)
                {
                    /*
                    Logic left for generic route attribute
                    separate the sections in path
                    */

                    var routeTemplate = routeAttribute?.Template;
                }

                var methods = controller.GetMethods()
                        .Where(x => x.IsPublic && x.DeclaringType.Name == className)
                        .ToList();

                ///loop for methods
                foreach (var method in methods)
                {
                    Api api = new Api();
                    var methodAttributes = method.GetCustomAttributes(typeof(Attribute));

                    #region setting method type and relative path
                    var httpGetAttr = (HttpGetAttribute)methodAttributes
                            .FirstOrDefault(attr => attr.GetType() == typeof(HttpGetAttribute));
                    if (httpGetAttr != null)
                    {
                        var methodType = httpGetAttr.HttpMethods?.FirstOrDefault().ToString();
                        api.MethodType = methodType;


                        var actionName = httpGetAttr?.Template;
                        api.ControllerName = controllerName;
                        api.ActionName = String.IsNullOrEmpty(actionName) ? method.Name : actionName;
                        api.FunctionName = method.Name;
                        api.RelativeUrl = "api/v1/" + controllerName + "/" + actionName;
                    }

                    #endregion
                    /*
                    parameters logic
                    */

                    var parameters = method.GetParameters().ToList();

                    #region setting path params

                    api.Parameters = GetSerializeBody<FromRouteAttribute>(parameters, true);
                    api.Header = GetSerializeBody<FromHeaderAttribute>(parameters, false);
                    api.Body = GetSerializeBody<FromBodyAttribute>(parameters, false);

                    # endregion


                    #region setting return response
                    Type t = null;
                    string serializedResponse = "";
                    var returnypesmethod = method.ReturnParameter.ParameterType;
                    foreach (var type in _systemReturnTypes)
                    {
                        if (returnypesmethod.Name.Contains(type))
                        {
                            t = GetReturnType(returnypesmethod.GetGenericArguments().FirstOrDefault());
                        }
                    }

                    if (t != null)
                    {
                        object serializeResponse = GenerateReturnObject(t);
                        api.Response = serializeResponse;
                    }
                    #endregion
                    _apiList.Add(api);
                }
                #endregion
            }
            CreateJSFiles(dir);
        }

        public Type GetReturnType(Type returnType)
        {

            if (TypeChecks(_systemReturnTypes, returnType.Name))
            {
                var returnTypeRec = GetReturnType(returnType.GenericTypeArguments.FirstOrDefault());
                return returnTypeRec;
            }
            return returnType;
        }


        private object GetSerializeBody<T>(List<ParameterInfo> parameters, bool isPathParameter)
        {
            Dictionary<string, object> paramsToSerialize = new Dictionary<string, object>();
            foreach (ParameterInfo parameter in parameters)
            {

                if (parameter.CustomAttributes.Any(x => x.AttributeType == typeof(T)))
                {
                    object paramobj = GenerateReturnObject(parameter.ParameterType);
                    paramsToSerialize.Add(parameter.Name, paramobj);
                }

                if (isPathParameter && !parameter.CustomAttributes.Any())
                {
                    object paramobj = GenerateReturnObject(parameter.ParameterType);
                    paramsToSerialize.Add(parameter.Name, paramobj);
                }

            }
            return paramsToSerialize;
        }

        private object GenerateReturnObject(Type t)
        {
            if (TypeChecks(_systemCollections, t.Name))
            {
                var obj = GenerateReturnObject(t.GetGenericArguments().FirstOrDefault());
                return GenerateListObject(obj);
            }

            if (t.Name == "String")
                return _stringValue;
            object inst = Activator.CreateInstance(t);
            object genrobj = GenerateInstance(inst.GetType());
            return genrobj;
        }

        /// <summary>
        /// Checks the types provided against a source array
        /// </summary>
        ///
        public bool TypeChecks(string[] sourceArr, string toCheck)
        {
            foreach (var source in sourceArr)
            {
                if (toCheck.Contains(source))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Generate instance of any type of nested object
        /// </summary>
        public object GenerateInstance(Type t)
        {
            object obj;
            var props = t.GetProperties().ToList();

            if (t.Name == "String")
            {
                return _stringValue;
            }

            obj = Activator.CreateInstance(t);
            var non_generic_props = props.Where(x => x.PropertyType.IsGenericType
                            && !x.PropertyType.Name.Contains(_nullStr)
                            ).ToList();

            foreach (var prop in non_generic_props)
            {
                var genobj = GenerateInstance(prop.PropertyType.GetGenericArguments().FirstOrDefault());
                var list = GenerateListObject(genobj);
                obj.GetType().GetProperty(prop.Name).SetValue(obj, list);
            }

            var stringProps = props.Where(x => x.PropertyType.Name == "String").ToList();
            foreach (var prop in stringProps)
            {
                obj.GetType().GetProperty(prop.Name).SetValue(obj, _stringValue);
            }

            var classProps = props
                    .Where(x => x.PropertyType.Name != "String"
                    && x.PropertyType.IsClass
                    && !x.PropertyType.IsGenericType
                    ).ToList();

            foreach (var prop in classProps)
            {
                var genobj = GenerateInstance(prop.PropertyType);
                obj.GetType().GetProperty(prop.Name).SetValue(obj, genobj);
            }

            return obj;

        }

        public object GenerateListObject(object obj)
        {
            Type getType = obj.GetType();
            Type listType = typeof(List<>).MakeGenericType(new[] { getType });
            IList list = (IList)Activator.CreateInstance(listType);
            list.Add(obj);
            return list;

        }


        public void CreateJSFiles(string dir)
        {
            foreach (var api in _apiList)
            {
                string currentdir = dir;
                string folderForFiles = "ApiJSFiles";
                if (!Directory.Exists(currentdir + "\\" + folderForFiles))
                {
                    Directory.CreateDirectory(currentdir + "\\" + folderForFiles);
                }

                string path = $"{currentdir}\\{folderForFiles}\\{api.ControllerName}-{api.ActionName}.js";
                string serializetest = JsonConvert.SerializeObject(api, Formatting.Indented);

                try
                {
                    using (FileStream fs = File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(serializetest);
                        fs.Write(info, 0, info.Length);
                    }

                    StringBuilder wholestr = new StringBuilder();
                    string importstr = @"import axios from 'http-request.js'";
                    wholestr.AppendLine(importstr);
                    wholestr.AppendLine($"let {api.FunctionName} = ");

                    foreach (string line in File.ReadLines(path))
                    {
                        string linestr = line;
                        int firstIndex = 0;
                        int secondIndex = 0;

                        if (linestr.IndexOf(_strTOCheckJSParam) != -1)
                        {
                            firstIndex = linestr.IndexOf(_charValue);
                            if (firstIndex != -1)
                            {
                                linestr = linestr.Substring(0, firstIndex) + "" + linestr.Substring(firstIndex + 1);
                                secondIndex = linestr.IndexOf(_charValue);

                                if (secondIndex != -1)
                                {
                                    linestr = linestr.Substring(0, secondIndex) + "" + linestr.Substring(secondIndex + 1);
                                }
                            }
                        }

                        wholestr.AppendLine(linestr);
                    }


                    string functionStr = $@"function call{api.FunctionName}() {_curlyBracesStart}
    return axios
        .get({api.FunctionName}.RelativeUrl)
        .then((response) => {_curlyBracesStart}
            {api.FunctionName}.Response = response.data;
            return {api.FunctionName}.Response;
        {_curlyBracesEnd})
        .catch(error => console.lof(error));
{_curlyBracesEnd}
                    ";


                    wholestr.AppendLine(functionStr);

                    string exportStr = $@"export {_curlyBracesStart}
    call{api.FunctionName}
{_curlyBracesEnd}
                    ";

                    wholestr.AppendLine(exportStr);

                    using (FileStream fs = File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(wholestr.ToString());
                        fs.Write(info, 0, info.Length);
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}
