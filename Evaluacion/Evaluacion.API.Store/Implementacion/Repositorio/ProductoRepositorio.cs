using Azure.Data.Tables;
using Evaluacion.API.Store.Contratos.Repositorio;
using Evaluacion.API.Store.Modelo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.API.Store.Implementacion.Repositorio
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProductoRepositorio(IConfiguration conf)
        {
            configuration = conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Producto";
        }

        //Crear Producto
        public async Task<bool> Create(Producto producto)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpsertEntityAsync(producto);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        //Eliminar Producto
        public async Task<bool> Delete(string partitionkey, string rowkey)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.DeleteEntityAsync(partitionkey, rowkey);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        //ListarPorId Producto
        public async Task<Producto> GetByRowKey(string rowKey)
        {
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Store' and RowKey eq '{rowKey}'";
            await foreach (var entidad in tablaCliente.QueryAsync<Producto>(filter: filtro))
            {
                return entidad;
            }
            return null;
        }

        //Listar Producto
        public async Task<List<Producto>> GetAll()
        {
            List<Producto> lista = new List<Producto>();
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Store'";

            await foreach (Producto producto in tablaCliente.QueryAsync<Producto>(filter: filtro))
            {
                lista.Add(producto);
            }
            return lista;
        }

        //Editar Producto
        public async Task<bool> Update(Producto producto)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpdateEntityAsync(producto, producto.ETag);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
