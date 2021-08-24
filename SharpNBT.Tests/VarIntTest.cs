using System;
using Xunit;

namespace SharpNBT.Tests
{
    public class VarIntTest
    {

        [Fact]
        public void Encode1()
        {
            var value = -2147483648;
            var expected = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x08 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode2()
        {
            var value = -1;
            var expected = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x0f };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode3()
        {
            var value = 2147483647;
            var expected = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode4()
        {
            var value = 2097151;
            var expected = new byte[] { 0xff, 0xff, 0x7f };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode5()
        {
            var value = 255;
            var expected = new byte[] { 0xff, 0x01 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode6()
        {
            var value = 128;
            var expected = new byte[] { 0x80, 0x01 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode7()
        {
            var value = 127;
            var expected = new byte[] { 0x7f };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
  
        [Fact]
        public void Encode8()
        {
            var value = 2;
            var expected = new byte[] { 0x02 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode9()
        {
            var value = 1;
            var expected = new byte[] { 0x01 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Encode10()
        {
            var value = 0;
            var expected = new byte[] { 0x00 };
            var actual = VarInt.Encode(value);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode1()
        {
            var expected = -2147483648;
            var bytes = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x08 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode2()
        {
            var expected = -1;
            var bytes = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x0f };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode3()
        {
            var expected = 2147483647;
            var bytes = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x07 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode4()
        {
            var expected = 2097151;
            var bytes = new byte[] { 0xff, 0xff, 0x7f };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode5()
        {
            var expected = 255;
            var bytes = new byte[] { 0xff, 0x01 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode6()
        {
            var expected = 128;
            var bytes = new byte[] { 0x80, 0x01 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode7()
        {
            var expected = 127;
            var bytes = new byte[] { 0x7f };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
  
        [Fact]
        public void Decode8()
        {
            var expected = 2;
            var bytes = new byte[] { 0x02 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode9()
        {
            var expected = 1;
            var bytes = new byte[] { 0x01 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Decode10()
        {
            var expected = 0;
            var bytes = new byte[] { 0x00 };
            var actual = VarInt.Decode(bytes, out var dummy);
            Assert.Equal(expected, actual);
        }
    }
}