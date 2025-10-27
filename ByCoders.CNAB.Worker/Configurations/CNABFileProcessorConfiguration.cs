using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Worker.Configurations;

public record CNABFileProcessorConfiguration
{
    public int PoolingIntervalSeconds { get; set; }
}
