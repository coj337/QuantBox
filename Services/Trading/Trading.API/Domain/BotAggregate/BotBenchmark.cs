using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.BotAggregate
{
    public class BotBenchmark
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Benchmark> ExchangeBenchmarks { get; set; } //Benchmarks for exchange (e.g. latency to exchange)
        public List<Benchmark> IndicatorBenchmarks { get; set; } //Benchmarks for indicators (e.g. how long RSI takes to execute)
        public List<Benchmark> SafetyBenchmarks { get; set; } //Benchmarks for safeties (e.g. how long RSI takes to execute)
        public List<Benchmark> InsuranceBenchmarks { get; set; } //Benchmarks for insurances (e.g. how long Overcome Fee Cost takes to execute)
        public List<Benchmark> ActionBenchmarks { get; set; } //Benchmarks for actions (e.g. trades)
    }
}
