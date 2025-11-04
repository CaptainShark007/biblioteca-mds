using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
        protected readonly AppDbContext _context;
        public PrestamoController(AppDbContext context)
        {
            _context = context;
        }
    }
}
