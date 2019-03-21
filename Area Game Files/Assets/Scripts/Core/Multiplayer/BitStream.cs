using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

public interface INetworkObject
{
    void Serialize(BitStream stream);
    void Deserialize(BitStream stream);
}

[StructLayout(LayoutKind.Explicit)]
public struct ByteStruct
{
    [FieldOffset(0)]
    public byte b1;
    [FieldOffset(1)]
    public byte b2;
    [FieldOffset(2)]
    public byte b3;
    [FieldOffset(3)]
    public byte b4;
    [FieldOffset(4)]
    public byte b5;
    [FieldOffset(5)]
    public byte b6;
    [FieldOffset(6)]
    public byte b7;
    [FieldOffset(7)]
    public byte b8;

    [FieldOffset(0)]
    public short s;
    [FieldOffset(0)]
    public int i;
    [FieldOffset(0)]
    public long l;
    [FieldOffset(0)]
    public ushort us;
    [FieldOffset(0)]
    public uint ui;
    [FieldOffset(0)]
    public ulong ul;
    [FieldOffset(0)]
    public float f;
    [FieldOffset(0)]
    public double d;
}

public partial class BitStream
{
    private bool empty;
    private int bit;
    private byte current;
    private MemoryStream stream;
    private ByteStruct bs = new ByteStruct();

    public long BytesLeft => stream.Length - stream.Position;
    public long BitsLeft => BytesLeft * 8 + (empty ? 0 : (8 - bit));

    public bool CheckBits(int bits)
    {
        return BytesLeft > 0 || (!empty && bit < 8);
    }

    public BitStream()
    {
        bit = 0;
        current = 0;
        stream = new MemoryStream();
    }

    public BitStream(byte[] bytes)
    {
        bit = 0;
        stream = new MemoryStream(bytes);
        empty = stream.Length == 0;
        current = (byte)(empty ? 0 : stream.ReadByte());
    }

    public byte[] GetBytes()
    {
        bit = 0;

        if (current != 0)
        {
            stream.WriteByte(current);
            current = 0;
        }

        return stream.ToArray();
    }

    public BitStream Reset()
    {
        if (current != 0)
        {
            stream.WriteByte(current);
            current = 0;
        }

        bit = 0;
        stream.Seek(0, SeekOrigin.Begin);
        empty = stream.Length == 0;
        current = (byte)(empty ? 0 : stream.ReadByte());
        return this;
    }

    public BitStream Write(byte o, int bits = 8)
    {
        if (bits <= 0) return this;
        if (bits > 8) throw new Exception("Byte has only 8 bits.");

        current = (byte)((current & (1 << bit) - 1) + (o << bit));

        if (bit + bits >= 8)
        {
            stream.WriteByte(current);
            current = (byte)(o >> (8 - bit));
        }

        bit = (bit + bits) % 8;
        return this;
    }

    public BitStream Write(byte[] o, int bitsPerByte = 8)
    {
        if (bitsPerByte <= 0) return this;
        if (bitsPerByte > 8) throw new Exception("Byte has only 8 bits.");

        Write((ushort)o.Length);

        for (int i = 0; i < o.Length; i++)
        {
            Write(o[i], bitsPerByte);
        }

        return this;
    }

    public BitStream Write(short o, int bits = 16)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits > 16) throw new Exception("Short has only 16 bits.");
        bs.s = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b2, bits - 8);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2, bits - 8);
        }

        return this;
    }

    public BitStream Write(ushort o, int bits = 16)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits > 16) throw new Exception("Short has only 16 bits.");
        bs.us = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b2, bits - 8);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2, bits - 8);
        }

        return this;
    }

    public BitStream Write(int o, int bits = 32)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits <= 16) return Write((short)o, bits);
        if (bits > 32) throw new Exception("Int has only 32 bits.");
        bs.i = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b4, bits - 24);
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b2);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2);
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b4, bits - 24);
        }

        return this;
    }

    public BitStream Write(uint o, int bits = 32)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits <= 16) return Write((ushort)o, bits);
        if (bits > 32) throw new Exception("UInt has only 32 bits.");
        bs.ui = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b4, bits - 24);
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b2);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2);
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b4, bits - 24);
        }

        return this;
    }

    public BitStream Write(float o, int bits = 32)
    {
        if (bits > 32) throw new Exception("Float has only 32 bits.");
        bs.f = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b4, bits - 24);
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b2, Math.Min(bits - 8, 8));
            Write(bs.b1, Math.Min(bits, 8));
        }

        else
        {
            Write(bs.b1, Math.Min(bits, 8));
            Write(bs.b2, Math.Min(bits - 8, 8));
            Write(bs.b3, Math.Min(bits - 16, 8));
            Write(bs.b4, bits - 24);
        }

        return this;
    }

    public BitStream Write(long o, int bits = 64)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits <= 16) return Write((short)o, bits);
        if (bits <= 32) return Write((int)o, bits);
        if (bits > 64) throw new Exception("Long has only 64 bits.");
        bs.l = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b8, bits - 56);
            Write(bs.b7, Math.Min(bits - 48, 8));
            Write(bs.b6, Math.Min(bits - 40, 8));
            Write(bs.b5, Math.Min(bits - 32, 8));
            Write(bs.b4);
            Write(bs.b3);
            Write(bs.b2);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2);
            Write(bs.b3);
            Write(bs.b4);
            Write(bs.b5, Math.Min(bits - 32, 8));
            Write(bs.b6, Math.Min(bits - 40, 8));
            Write(bs.b7, Math.Min(bits - 48, 8));
            Write(bs.b8, bits - 56);
        }

        return this;
    }

    public BitStream Write(ulong o, int bits = 64)
    {
        if (bits <= 8) return Write((byte)o, bits);
        if (bits <= 16) return Write((ushort)o, bits);
        if (bits <= 32) return Write((uint)o, bits);
        if (bits > 64) throw new Exception("ULong has only 64 bits.");
        bs.ul = o;

        if (BitConverter.IsLittleEndian)
        {
            Write(bs.b8, bits - 56);
            Write(bs.b7, Math.Min(bits - 48, 8));
            Write(bs.b6, Math.Min(bits - 40, 8));
            Write(bs.b5, Math.Min(bits - 32, 8));
            Write(bs.b4);
            Write(bs.b3);
            Write(bs.b2);
            Write(bs.b1);
        }

        else
        {
            Write(bs.b1);
            Write(bs.b2);
            Write(bs.b3);
            Write(bs.b4);
            Write(bs.b5, Math.Min(bits - 32, 8));
            Write(bs.b6, Math.Min(bits - 40, 8));
            Write(bs.b7, Math.Min(bits - 48, 8));
            Write(bs.b8, bits - 56);
        }

        return this;
    }

    public BitStream Write(bool o)
    {
        return Write((byte)(o ? 1 : 0), 1);
    }

    public BitStream Write(string o)
    {
        return Write(Encoding.UTF8.GetBytes(o));
    }

    public BitStream Write(Vector3 o)
    {
        Write(o.x);
        Write(o.y);
        return Write(o.z);
    }

    public BitStream Write(Quaternion o)
    {
        Vector3 e = o.eulerAngles;
        Write(e.x);
        Write(e.y);
        return Write(e.z);
    }

    public byte ReadByte(int bits = 8)
    {
        if (bits <= 0) return 0;
        if (bits > 8) throw new Exception("Byte has only 8 bits.");

        int nextbit = (bit + bits) % 8;
        byte result = (byte)((current & ~((byte)(1 << bit) - 1)) >> bit);

        if (bit + bits >= 8)
        {
            empty = BytesLeft == 0;
            current = (byte)(empty ? 0 : stream.ReadByte());
            result += (byte)((current & ((byte)(1 << nextbit) - 1)) << (bits - nextbit));
        }

        else
        {
            result -= (byte)((current & ~((byte)(1 << nextbit) - 1)) >> bit);
        }

        bit = nextbit;
        return result;
    }

    public byte[] ReadBytes(int bitsPerByte = 8)
    {
        if (bitsPerByte <= 0) return new byte[0];
        if (bitsPerByte > 8) throw new Exception("Byte has only 8 bits.");

        ushort length = ReadUShort();
        byte[] result = new byte[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = ReadByte(bitsPerByte);
        }

        return result;
    }

    public short ReadShort(int bits = 16)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits > 16) throw new Exception("Short has only 16 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b2 = ReadByte(bits - 8);
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte(bits - 8);
        }

        return bs.s;
    }

    public ushort ReadUShort(int bits = 16)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits > 16) throw new Exception("UShort has only 16 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b2 = ReadByte(bits - 8);
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte(bits - 8);
        }

        return bs.us;
    }

    public int ReadInt(int bits = 32)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits <= 16) return ReadShort(bits);
        if (bits > 32) throw new Exception("Int has only 32 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b4 = ReadByte(bits - 24);
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b2 = ReadByte();
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte();
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b4 = ReadByte(bits - 24);
        }

        return bs.i;
    }

    public uint ReadUInt(int bits = 32)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits <= 16) return ReadUShort(bits);
        if (bits > 32) throw new Exception("UInt has only 32 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b4 = ReadByte(bits - 24);
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b2 = ReadByte();
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte();
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b4 = ReadByte(bits - 24);
        }

        return bs.ui;
    }

    public float ReadFloat(int bits = 32)
    {
        if (bits > 32) throw new Exception("Float has only 32 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b4 = ReadByte(bits - 24);
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b2 = ReadByte(Math.Min(bits - 8, 8));
            bs.b1 = ReadByte(Math.Min(bits, 8));
        }

        else
        {
            bs.b1 = ReadByte(Math.Min(bits, 8));
            bs.b2 = ReadByte(Math.Min(bits - 8, 8));
            bs.b3 = ReadByte(Math.Min(bits - 16, 8));
            bs.b4 = ReadByte(bits - 24);
        }

        return bs.f;
    }

    public long ReadLong(int bits = 64)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits <= 16) return ReadShort(bits);
        if (bits <= 32) return ReadInt(bits);
        if (bits > 64) throw new Exception("Long has only 64 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b8 = ReadByte(bits - 56);
            bs.b7 = ReadByte(Math.Min(bits - 48, 8));
            bs.b6 = ReadByte(Math.Min(bits - 40, 8));
            bs.b5 = ReadByte(Math.Min(bits - 32, 8));
            bs.b4 = ReadByte();
            bs.b3 = ReadByte();
            bs.b2 = ReadByte();
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte();
            bs.b3 = ReadByte();
            bs.b4 = ReadByte();
            bs.b5 = ReadByte(Math.Min(bits - 32, 8));
            bs.b6 = ReadByte(Math.Min(bits - 40, 8));
            bs.b7 = ReadByte(Math.Min(bits - 48, 8));
            bs.b8 = ReadByte(bits - 56);
        }

        return bs.l;
    }

    public ulong ReadULong(int bits = 64)
    {
        if (bits <= 8) return ReadByte(bits);
        if (bits <= 16) return ReadUShort(bits);
        if (bits <= 32) return ReadUInt(bits);
        if (bits > 64) throw new Exception("ULong has only 64 bits.");

        if (BitConverter.IsLittleEndian)
        {
            bs.b8 = ReadByte(bits - 56);
            bs.b7 = ReadByte(Math.Min(bits - 48, 8));
            bs.b6 = ReadByte(Math.Min(bits - 40, 8));
            bs.b5 = ReadByte(Math.Min(bits - 32, 8));
            bs.b4 = ReadByte();
            bs.b3 = ReadByte();
            bs.b2 = ReadByte();
            bs.b1 = ReadByte();
        }

        else
        {
            bs.b1 = ReadByte();
            bs.b2 = ReadByte();
            bs.b3 = ReadByte();
            bs.b4 = ReadByte();
            bs.b5 = ReadByte(Math.Min(bits - 32, 8));
            bs.b6 = ReadByte(Math.Min(bits - 40, 8));
            bs.b7 = ReadByte(Math.Min(bits - 48, 8));
            bs.b8 = ReadByte(bits - 56);
        }

        return bs.ul;
    }

    public bool ReadBool()
    {
        return ReadByte(1) == 1;
    }

    public string ReadString()
    {
        return Encoding.UTF8.GetString(ReadBytes());
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Quaternion ReadQuaternion()
    {
        return Quaternion.Euler(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public BitStream Write(INetworkObject o)
    {
        o.Serialize(this);
        return this;
    }

    public T ReadNetworkObject<T>() where T : INetworkObject
    {
        return (T)ReadNetworkObject(typeof(T));
    }

    private INetworkObject ReadNetworkObject(Type type)
    {
        INetworkObject o = (INetworkObject)Activator.CreateInstance(type);
        o.Deserialize(this);
        return o;
    }
}