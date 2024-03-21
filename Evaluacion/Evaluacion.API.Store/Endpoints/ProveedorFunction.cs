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
    public class ProveedorFunction
    {
        private readonly ILogger<ProveedorFunction> _logger;
        private readonly IProveedorRepositorio repos;

        public ProveedorFunction(ILogger<ProveedorFunction> logger, IProveedorRepositorio repos)
        {
            _logger = logger;
            this.repos = repos;
        }
        //INSERTAR

        [Function("InsertarProveedor")]


        [OpenApiOperation("Insertarspec", "InsertarProveedor", Description = "Sirve para Insertar un Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor), Description = "Proveedor modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Proveedor), Description = "Mostrara el Proveedor Creado")]
        public async Task<HttpResponseData> InsertarProveedor([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var registro = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar una Proveedor con todos sus datos");
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
        [Function("ListarProveedor")]
        [OpenApiOperation("Listarspec", "ListarProveedor", Description = "Sirve para listar todas las Proveedors")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Proveedor>),
         Description = "Mostrara una lista de Proveedors")]
        public async Task<HttpResponseData> ListarProveedor([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
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
        [Function("EliminarProveedor")]
        [OpenApiOperation("Eliminarspec", "EliminarProveedor", Description = "Sirve para Eliminar una Proveedor")]
        [OpenApiParameter(name: "partitionkey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "rowkey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        public async Task<HttpResponseData> EliminarProveedor(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Proveedores/{partitionkey}/{rowkey}")] HttpRequestData req, string partitionkey, string rowkey, ILogger log)
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
                log.LogError($"Error al eliminar el Proveedor: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //EDITAR

        [Function("EditarProveedor")]
        [OpenApiOperation("Modificarspec", "ModificarProveedor", Description = "Sirve para Modificar una Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor), Description = "Proveedor modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Proveedor),
         Description = "Mostrara la Proveedor modificada")]
        public async Task<HttpResponseData> EditarProveedor([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var registro = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar una Proveedor con todos sus datos");

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
        [Function("ListarProveedorPorRowkey")]
        [OpenApiOperation("Obtenerspec", "ObtenerProveedorById", Description = "Sirve para obtener un Proveedor")]
        [OpenApiParameter(name: "RowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Proveedor), Description = "Mostrara una Proveedor")]
        public async Task<HttpResponseData> ListarProveedorPorRowkey([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Proveedor/{RowKey}")] HttpRequestData req, string rowKey)
        {
            HttpResponseData respuesta;
            try
            {
                var lista = await repos.GetAll();
                var Proveedor = lista.FirstOrDefault(inst => inst.RowKey == rowKey);

                if (Proveedor != null)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(Proveedor);
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
