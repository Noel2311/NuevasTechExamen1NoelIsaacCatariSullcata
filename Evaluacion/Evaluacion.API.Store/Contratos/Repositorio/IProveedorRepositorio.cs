using Evaluacion.API.Store.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.API.Store.Contratos.Repositorio
{
    public interface IProveedorRepositorio
    {
        public Task<bool> Create(Proveedor proveedor);
        public Task<bool> Update(Proveedor proveedor);
        public Task<bool> Delete(string partitionkey, string rowkey);
        public Task<List<Proveedor>> GetAll();
        public Task<Proveedor> GetByRowKey(string id);
    }
}

