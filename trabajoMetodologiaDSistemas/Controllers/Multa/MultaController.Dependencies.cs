using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class MultaController : Controller
    {
        protected readonly AppDbContext _context;
        public MultaController(AppDbContext context)
        {
            _context = context;
        }
    }
}
