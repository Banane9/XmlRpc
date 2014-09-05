XmlRpc
======

Implementation of the [XmlRpc Spec](http://xmlrpc.scripting.com/spec.html). Originally part of the ManiaNet project.

--------------------------------------------------------------------------------------------------------------------------------

##Usage##

For a real-world usage example, check [here](https://github.com/Banane9/ManiaNet/tree/master/ManiaNet.DedicatedServer/XmlRpc) and [here](https://github.com/Banane9/ManiaNet/blob/32d4533e0548a52e9024cabd2732363b48a62154/ManiaNet.DedicatedServer.Controller/ServerController.cs#L192).

--------------------------------------------------------------------------------------------------------------------------------

###Method Calls###

#####Definition#####

``` CSharp
using XmlRpc.Methods;
using XmlRpc.Types;

/// <summary>
/// Represents a call to the AddGuest method.
/// Simply derive from XmlRpcMethodCall<> and pass it the generic paramameters corresponding to your method.
/// This works like Func<>, in that the last parameter pair is the return value and before that, you have the arguments.
/// </summary>
public sealed class AddGuest : XmlRpcMethodCall<XmlRpcString, string, XmlRpcBoolean, bool>
{
    /// <summary>
    /// Gets or sets the login that will be added to the guestlist.
    /// For convenience, you should provide properties to change the parameter values (for calls).
    /// For callback methods, a readonly property is better, so that one consumer can't modify the values for others.
    /// </summary>
    public string Login
    {
        get { return param1.Value; }
        set { param1.Value = value; }
    }

    /// <summary>
    /// Gets the name of the method this call is for.
    /// This has to be overridden and will be the name inside the `<methodName>` tag.
    /// </summary>
    public override string MethodName
    {
        get { return "AddGuest"; }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="AddGuest"/> class for the given login.
    /// The MethodCall has to be constructed using the base's constructor.
    /// It will automatically set the parameter fields to the given values.
    /// For callback methods, there should also be a parameterless constructor to simplify usage.
    /// </summary>
    /// <param name="login">The login that will be added to the guestlist.</param>
    public AddGuest(string login)
        : base(login)
    { }
}
```

#####Usage#####

``` CSharp
var addGuest = new AddGuest("banane9");

// Assuming xmlRpcClient implements IXmlRpcClient,
// this will send the Xml representing the MethodCall to the server.
uint handle = xmlRpcClient.SendRequest(addGuest.GenerateXml().ToString());
```

One then has to wait for the MethodResponse event with the matching handle to fire, and that will contain the response to that method.

For an implementation of a CallMethod<> function that does this and automatically has the function parse the response, check [here](https://github.com/Banane9/ManiaNet/blob/32d4533e0548a52e9024cabd2732363b48a62154/ManiaNet.DedicatedServer.Controller/ServerController.cs#L192) on the ManiaNet project again.

--------------------------------------------------------------------------------------------------------------------------------

###Custom Structs###

#####Definition#####

This is the content of the [FaultStruct.cs](https://github.com/Banane9/XmlRpc/blob/master/XmlRpc/Types/Structs/FaultStruct.cs) file with some additional comments.


``` CSharp
/// <summary>
/// Gets the struct returned when a method call has a fault.
/// Simply derive from BaseStruct.
/// </summary>
public sealed class FaultStruct : BaseStruct
{
    // Add some private fields for your content

    /// <summary>
    /// Backing field for the FaultCode property.
    /// </summary>
    private XmlRpcInt faultCode = new XmlRpcInt();

    /// <summary>
    /// Backing field for the FaultString property.
    /// </summary>
    private XmlRpcString faultString = new XmlRpcString();

    // And some properties to get the values for returned structs;
    // or get/set them for call-parameter structs.
    
    /// <summary>
    /// Gets the fault code.
    /// </summary>
    public int FaultCode
    {
        get { return faultCode.Value; }
    }

    /// <summary>
    /// Gets the description of the fault.
    /// </summary>
    public string FaultString
    {
        get { return faultString.Value; }
    }

    /// <summary>
    /// Generates an XElement storing the information in this struct.
    /// The order of the members doesn't matter, but casing in the name does.
    /// </summary>
    /// <returns>The generated XElement.</returns>
    public override XElement GenerateXml()
    {
        return new XElement(XName.Get(XmlRpcElements.StructElement),
            makeMemberElement("faultCode", faultCode),
            makeMemberElement("faultString", faultString));
    }

    /// <summary>
    /// Fills the property of this struct that has the correct name with the information contained in the member-XElement.
    /// This method will be called for every member element when parsing the Xml.
    /// </summary>
    /// <param name="member">The member element storing the information.</param>
    /// <returns>Whether it was successful or not.</returns>
    protected override bool parseXml(XElement member)
    {
        XElement value = getMemberValueElement(member);

        switch (getMemberName(member))
        {
            case "faultCode":
                if (!faultCode.ParseXml(value))
                    return false;
                break;


            case "faultString":
                if (!faultString.ParseXml(value))
                    return false;
                break;

            default:
                return false;
        }

        return true;
    }
}

```

For convenience, there's a [.snippet file](https://github.com/Banane9/XmlRpc/blob/master/XmlRpc/Types/Structs/ParseStruct.snippet) for the scaffolding of the `parseXml` method.

#####Usage#####

For usage, the `BaseStruct`-derived Type has to be wrapped in `XmlRpcStruct<>`.

``` CSharp
// MethodCall definition
public sealed class ReturnFaultStruct : MethodCall<XmlRpcStruct<FaultStruct>, FaultStruct>

// Field (for example in another struct).
private XmlRpcStruct<FaultStruct> fault = new XmlRpcStruct<FaultStruct>();
```

--------------------------------------------------------------------------------------------------------------------------------

###Types###

You shouldn't have to define any of those, as all from the XmlRpc spec are included. But in case your application has to deal with custom ones, simply follow the implementation of the spec ones, [here](https://github.com/Banane9/XmlRpc/tree/master/XmlRpc/Types).

--------------------------------------------------------------------------------------------------------------------------------

##License##

#####[LGPL V2.1](https://github.com/Banane9/XmlRpc/tree/master/LICENSE.md)#####
