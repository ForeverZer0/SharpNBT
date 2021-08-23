using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SharpNBT.Tests
{
    public class PrettyPrint
    {
        private readonly ITestOutputHelper output;
        
        public PrettyPrint(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public void PrettyPrintToStdout()
        {
            using var stream = NbtStream.OpenRead("./Data/bigtest.nbt");
            var topLevel = stream.ReadTag<CompoundTag>();
            
            output.WriteLine(topLevel.PrettyPrinted());
        }
    }
}