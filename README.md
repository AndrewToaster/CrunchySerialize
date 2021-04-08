![AppIcon](Icon.png)
# CrunchySerialize
 A light-weight serialization engine

# How to use
There are 2 ways to serialize / deserialize objects
## Using Intefaces
The best way to ensure proper serialization is to use the `ISerializable` interface
### Example
```csharp
public class StringData : ISerializable
{
    public string Text { get; set; }

    public StringData(string text)
    {
        Text = text;
    }

    public void Serialize(ByteWriter writer)
    {
        writer.WriteString(Text);
    }

    public void Deserialize(ByteBuffer data)
    {
        Text = data.ReadString();
    }
}
```
Then calling
```csharp
ByteBuffer data = Serializator.Serialize(new StringData("Hello, World!"));
```
And deserializing
```csharp
StringData stringData = Serializator.Deserialize<StringData>(data);
```
## Using Reflection-based Automatic Serialization
For those that don't want to write code for reading and writing from binary buffers can use automatic serialization.
## Example
```csharp
public class IntData
{
    public int Value { get; set; }

    public IntData(int val)
    {
        Value = val;
    }
}
```
Then calling
```csharp
ByteBuffer data = Serializator.Automatic.Serialize(new IntData(69420));
```
And deserializing
```csharp
IntData intData = Serializator.Automatic.Deserialize<IntData>(data);
```
## How it works
Reflection-based parsing works by getting all fields and properties and doing 3 things.
### If the type is a Natively Serializable
This means that if the type is either a Primitive, Enum or String then we go ahead and write it into the buffer
### If the type implements `ISerializable`
If the type implements `ISerializable` then we call `ISerializable.Serialize`
### None of the above
If none of these are reached, then we serialize all fields and properites in the type depending on the given `depth` parameter.
This is recursively done.

### Exceptions
If the field / property has the `[IgnoreMember]` attribute, then we just skip it
