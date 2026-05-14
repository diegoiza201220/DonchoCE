using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class CelSecuenciaSri
{
    public int Id { get; set; }

    public int TipoDocumento { get; set; }

    public string Establecimiento { get; set; } = null!;

    public string PuntoDeEmision { get; set; } = null!;

    public int SecuenciaActual { get; set; }

    public bool Estado { get; set; }
}
