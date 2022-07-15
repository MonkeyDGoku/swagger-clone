import axios from 'http-request.js'
let Get = 
{
  MethodType: "GET",
  ControllerName: "WeatherForecast",
  FunctionName: "Get",
  ActionName: "Get",
  RelativeUrl: "api/v1/WeatherForecast/",
  Header: {},
  Parameters: {},
  Body: {},
  Response: null
}
function callGet() {
    return axios
        .get(Get.RelativeUrl)
        .then((response) => {
            Get.Response = response.data;
            return Get.Response;
        })
        .catch(error => console.lof(error));
}
                    
export {
    callGet
}
                    
