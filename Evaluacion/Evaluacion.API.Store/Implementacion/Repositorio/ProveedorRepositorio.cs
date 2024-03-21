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
    public class ProveedorRepositorio : IProveedorRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProveedorRepositorio(IConfiguration conf)
        {
            configuration = conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Proveedor";
        }

        //Crear Proveedor
        public async Task<bool> Create(Proveedor proveedor)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpsertEntityAsync(proveedor);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        //Eliminar Proveedor
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

        //ListarPorId Proveedor
        public async Task<Proveedor> GetByRowKey(string rowKey)
        {
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Store' and RowKey eq '{rowKey}'";
            await foreach (var entidad in tablaCliente.QueryAsync<Proveedor>(filter: filtro))
            {
                return entidad;
            }
            return null;
        }

        //Listar Proveedor
        public async Task<List<Proveedor>> GetAll()
        {
            List<Proveedor> lista = new List<Proveedor>();
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Store'";

            await foreach (Proveedor proveedor in tablaCliente.QueryAsync<Proveedor>(filter: filtro))
            {
                lista.Add(proveedor);
            }
            return lista;
        }

        //Editar Proveedor
        public async Task<bool> Update(Proveedor proveedor)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpdateEntityAsync(proveedor, proveedor.ETag);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}

