using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Infrastructure.Correlation;

public interface ICorrelation
{
    void SetCorrelationId(Guid correlationId);
    Guid GetCorrelationId();
}