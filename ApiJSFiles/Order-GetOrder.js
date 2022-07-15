import axios from 'http-request.js'
let GetOrder = 
{
  MethodType: "GET",
  ControllerName: "Order",
  FunctionName: "GetOrder",
  ActionName: "GetOrder",
  RelativeUrl: "api/v1/Order/",
  Header: {},
  Parameters: {},
  Body: {},
  Response: [
    [
      {
        Id: "00000000-0000-0000-0000-000000000000",
        OrderId: 0,
        OrderFloat: 0.0,
        OrderLong: 0,
        OrderDouble: 0.0,
        OrderBool: false,
        OrderChar: "\u0000",
        ProductName: "string",
        ProductCategory: "string",
        Address: {
          Id: "00000000-0000-0000-0000-000000000000",
          City: "string",
          RegionId: 0,
          PostalCode: "string",
          Country: "string",
          Phone: "string"
        },
        ProductDateTime: "0001-01-01T00:00:00",
        ProductNames: [
          "string"
        ],
        Users: [
          {
            Id: 0,
            Name: "string",
            Email: "string",
            Alias: "string"
          }
        ]
      }
    ]
  ]
}
function callGetOrder() {
    return axios
        .get(GetOrder.RelativeUrl)
        .then((response) => {
            GetOrder.Response = response.data;
            return GetOrder.Response;
        })
        .catch(error => console.lof(error));
}
                    
export {
    callGetOrder
}
                    
