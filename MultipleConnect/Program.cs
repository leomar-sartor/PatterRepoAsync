using MultipleConnect;
using MultipleConnect.Services;

Console.WriteLine("Iniciando!");

var conexaoAsync = new ConexaoAsync();

long PedidoId = 291;
long EmpresaId = 2;
long UserId = 2;

var service = new ProdutorPedidoService(conexaoAsync);
var estornado = await service.EstornarPedido(PedidoId, UserId, EmpresaId);

Console.ReadKey();