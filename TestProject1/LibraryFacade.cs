
namespace TestProject1
{
    internal class LibraryFacade
    {
        public LibraryFacade()
        {
        }

        internal async Task<(object regOk, object, object numero)> RegisterMemberAsync(Socio socio)
        {
            throw new NotImplementedException();
        }

        internal async Task<(bool ok, object? err, object? due)> RequestLoanAsync(object numero, string v)
        {
            throw new NotImplementedException();
        }
    }
}