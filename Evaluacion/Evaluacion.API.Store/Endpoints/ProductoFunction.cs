using Evaluacion.API.Store.Contratos.Repositorio;
using Evaluacion.API.Store.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace Evaluacion.API.Store.Endpoints
{
    public class ProductoFunction
    {
        private readonly ILogger<ProductoFunction> _logger;
        private readonly IProductoRepositorio repos;

        public ProductoFunction(ILogger<ProductoFunction> logger, IProductoRepositorio repos)
        {
            _logger = logger;
            this.repos = repos;
        }

        //INSERTAR

        [Function("InsertarProducto")]
        [OpenApiOperation("Insertarspec", "InsertarProducto", Description = "Sirve para Insertar un Producto")]
        [OpenApiRequestBody("application/json", typeof(Producto), Description = "Producto modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Producto), Description = "Mostrara el Producto Creado")]

        public async Task<HttpResponseData> InsertarProducto([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var registro = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar una Producto con todos sus datos");
                registro.RowKey = Guid.NewGuid().ToString();
                registro.Timestamp = DateTime.UtcNow;
             
                bool sw = await repos.Create(registro);
                if (sw)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                else
                {
                    respuesta = req.CreateResponse(HttpStatusCode.BadRequest);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }

        //LISTAR
        [Function("ListarProducto")]
        [OpenApiOperation("Listarspec", "ListarProducto", Description = "Sirve para listar todas las Productos")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Producto>),
         Description = "Mostrara una lista de Productos")]
        public async Task<HttpResponseData> ListarProducto([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var lista = repos.GetAll();
                respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(lista.Result);
                return respuesta;
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }

        //ELIMINAR
        [Function("EliminarProducto")]
        [OpenApiOperation("Eliminarspec", "EliminarProducto", Description = "Sirve para Eliminar una Producto")]
        [OpenApiParameter(name: "partitionkey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "rowkey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]

        public async Task<HttpResponseData> EliminarProducto(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Productoes/{partitionkey}/{rowkey}")] HttpRequestData req, string partitionkey, string rowkey, ILogger log)
        {
            try
            {
                var exito = await repos.Delete(partitionkey, rowkey);
                if (exito)
                    return req.CreateResponse(HttpStatusCode.OK);
                else
                    return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                log.LogError($"Error al eliminar el Producto: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //EDITAR

        [Function("EditarProducto")]
        [OpenApiOperation("Modificarspec", "ModificarProducto", Description = "Sirve para Modificar una Producto")]
        [OpenApiRequestBody("application/json", typeof(Producto), Description = "Producto modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Producto),
         Description = "Mostrara la Producto modificada")]

        public async Task<HttpResponseData> EditarProducto([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var registro = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar una Producto con todos sus datos");

                // Verifica si la institución existe antes de intentar actualizarla
                if (string.IsNullOrEmpty(registro.RowKey))
                {
                    respuesta = req.CreateResponse(HttpStatusCode.BadRequest);
                    return respuesta;
                }

                bool sw = await repos.Update(registro);
                if (sw)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                else
                {
                    respuesta = req.CreateResponse(HttpStatusCode.BadRequest);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }

        //LISTAR POR ROWKEY
        [Function("ListarProductoPorRowkey")]
        [OpenApiOperation("Obtenerspec", "ObtenerProductoById", Description = "Sirve para obtener un Producto")]
        [OpenApiParameter(name: "RowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Producto), Description = "Mostrara una Producto")]

        public async Task<HttpResponseData> ListarProductoPorRowkey([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Producto/{RowKey}")] HttpRequestData req, string rowKey)
        {
            HttpResponseData respuesta;
            try
            {
                var lista = await repos.GetAll();
                var Producto = lista.FirstOrDefault(inst => inst.RowKey == rowKey);

                if (Producto != null)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(Producto);
                }
                else
                {
                    respuesta = req.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return respuesta;
        }

    }
}
