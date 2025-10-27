using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Core.BackgroundServices;

public interface IScoopedService
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}