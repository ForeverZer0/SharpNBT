using Xunit;

namespace SharpNBT.Tests
{
    public class VarLongTest
    {
        [Fact]
        public void Encode1()
        {
            long value = -9223372036854775808;
            var expected = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode2()
        {
            long value = -2147483648;
            var expected = new byte[] { 0x80, 0x80, 0x80, 0x80, 0xf8, 0xff, 0xff, 0xff, 0xff, 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode3()
        {
            long value = -1;
            var expected = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode4()
        {
            long value = 9223372036854775807;
            var expected = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode5()
        {
            long value = 2147483647;
            var expected = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode6()
        {
            long value = 255;
            var expected = new byte[] { 0xff, 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode7()
        {
            long value = 128;
            var expected = new byte[] { 0x80, 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode8()
        {
            long value = 127;
            var expected = new byte[] { 0x7f };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode9()
        {
            long value = 2;
            var expected = new byte[] { 0x02 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode10()
        {
            long value = 1;
            var expected = new byte[] { 0x01 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode11()
        {
            long value = 0;
            var expected = new byte[] { 0x00 };
            var actual = VarLong.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        
        
        
        
        
        
        
        
        
        
        
               [Fact]
        public void Decode1()
        {
            long expected = -9223372036854775808;
            var bytes = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode2()
        {
            long expected = -2147483648;
            var bytes = new byte[] { 0x80, 0x80, 0x80, 0x80, 0xf8, 0xff, 0xff, 0xff, 0xff, 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode3()
        {
            long expected = -1;
            var bytes = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode4()
        {
            long expected = 9223372036854775807;
            var bytes = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode5()
        {
            long expected = 2147483647;
            var bytes = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode6()
        {
            long expected = 255;
            var bytes = new byte[] { 0xff, 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode7()
        {
            long expected = 128;
            var bytes = new byte[] { 0x80, 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode8()
        {
            long expected = 127;
            var bytes = new byte[] { 0x7f };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode9()
        {
            long expected = 2;
            var bytes = new byte[] { 0x02 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode10()
        {
            long expected = 1;
            var bytes = new byte[] { 0x01 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode11()
        {
            long expected = 0;
            var bytes = new byte[] { 0x00 };
            var actual = VarLong.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
    }
}