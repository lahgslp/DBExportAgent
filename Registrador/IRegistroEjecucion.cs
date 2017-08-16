using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrador
{
    public interface IRegistroEjecucion
    {
        void Registrar(string evento);
        void RegistrarError(string error);
        void RegistrarAdvertencia(string advertencia);
        List<string> Errores();
        List<string> Advertencias();
    }
}
