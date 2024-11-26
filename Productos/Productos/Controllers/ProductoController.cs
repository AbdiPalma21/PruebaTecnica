using Azure.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Productos.Models;
using Productos.Shared;

namespace Productos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductosDbContext _dbcontext;

        public ProductoController(ProductosDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("productos")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<ProductoDTO>>();
            var listaProductoDTO = new List<ProductoDTO>();

            try
            {
                foreach (var item in await _dbcontext.Productos.ToListAsync())
                {
                    listaProductoDTO.Add(new ProductoDTO
                    {
                        Id = item.Id,
                        Nombre = item.Nombre,
                        Precio = item.Precio, 
                        Cantidad = item.Cantidad
                    });

                    responseApi.EsCorrecto = true;
                    responseApi.Valor = listaProductoDTO;
                }
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
            }
            return Ok(responseApi);
        }

        [HttpGet]
        [Route("productos/{id}")]
        public async Task<IActionResult> Buscar (int id)
        {
            var responseApi = new ResponseAPI<ProductoDTO>();
            var ProductoDTO = new ProductoDTO();
            try
            {
                var dbProducto = await _dbcontext.Productos.FirstOrDefaultAsync(x => x.Id == id);
                if (dbProducto != null)
                {
                    ProductoDTO.Id = dbProducto.Id;
                    ProductoDTO.Nombre = dbProducto.Nombre; 
                    ProductoDTO.Precio = dbProducto.Precio;
                    ProductoDTO.Cantidad = dbProducto.Cantidad;

                    responseApi.EsCorrecto = true;
                    responseApi.Valor = ProductoDTO;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No encontrado";
                }

            }
            catch(Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
            }
            return Ok(responseApi);
        }

        [HttpPost]
        [Route("productos")]
        public async Task<IActionResult> Guardar(ProductoCreateUpdateDTO producto)
        {
            var responseApi = new ResponseAPI<ProductoDTO>();

            try
            {
                if (!string.IsNullOrWhiteSpace(producto.Nombre) && producto.Precio > 0 && producto.Cantidad > 0)
                {
                    var dbProducto = new Producto
                    {
                        Nombre = producto.Nombre,
                        Precio = producto.Precio,
                        Cantidad = producto.Cantidad
                    };
                    _dbcontext.Productos.Add(dbProducto);
                    await _dbcontext.SaveChangesAsync();

                    if (dbProducto.Id != 0)
                    {
                        var ProductoDTO = new ProductoDTO
                        {
                            Id = dbProducto.Id,
                            Nombre = dbProducto.Nombre,
                            Precio = dbProducto.Precio,
                            Cantidad = dbProducto.Cantidad
                        };

                        responseApi.EsCorrecto = true;
                        responseApi.Valor = ProductoDTO;
                    }
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Los valores de los productos no son correctos";
                }
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
            }
            return Ok(responseApi);
        }


        [HttpPut]
        [Route("productos/{id}")]
        public async Task<IActionResult> Editar(ProductoCreateUpdateDTO producto, int id)
        {
            var responseApi = new ResponseAPI<ProductoDTO>();

            try
            {
                if (!string.IsNullOrWhiteSpace(producto.Nombre) && producto.Precio > 0 && producto.Cantidad > 0)
                {
                    var dbProducto = await _dbcontext.Productos.FirstOrDefaultAsync(x => x.Id == id);

                    if (dbProducto != null)
                    {
                        dbProducto.Nombre = producto.Nombre;
                        dbProducto.Precio = producto.Precio;
                        dbProducto.Cantidad = producto.Cantidad;

                        _dbcontext.Productos.Update(dbProducto);
                        await _dbcontext.SaveChangesAsync();

                        var ProductoDTO = new ProductoDTO
                        {
                            Id = dbProducto.Id,
                            Nombre = dbProducto.Nombre,
                            Precio = dbProducto.Precio,
                            Cantidad = dbProducto.Cantidad
                        };

                        responseApi.EsCorrecto = true;
                        responseApi.Valor = ProductoDTO;
                    }
                    else
                    {
                        responseApi.EsCorrecto = false;
                        responseApi.Mensaje = "Producto no encontrado";
                    }
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Los valores de los productos no son correctos";
                }
                    
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
            }
            return Ok(responseApi);
        }


        [HttpDelete]
        [Route("productos/{Id}")]
        public async Task<IActionResult> Eliminar (int Id)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var dbProducto = await _dbcontext.Productos.FirstOrDefaultAsync(e => e.Id == Id);
                if(dbProducto != null)
                {
                    _dbcontext.Productos.Remove(dbProducto);
                    await _dbcontext.SaveChangesAsync();

                    responseApi.EsCorrecto = true;
                    responseApi.Mensaje = "Producto eliminado";
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Producto no encontrado";
                }
            }catch(Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
            }
            return Ok(responseApi); 
        }
    }
}
