using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.BotAggregate
{
    public class Benchmark
    {
        public string Target { get; set; } //Target for the benchmark (e.g. Buy Trade)
        public long TimeTaken { get; set; } //Latency in ms to the target
    }
}
