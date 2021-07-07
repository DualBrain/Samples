# Introduction

VB currently provides what is called explicit interface implementations; meaning when you implement an interface you must explicitly specify which methods, properties, etc. are going to handle the interface definition.  Implicit interface implementations will attempt to infer the *wiring* between the interface and the class based on a set of predefined rules.  This package will do this by creating a backing partial class that handles this *wiring* automatically for any methods, properties, etc. that can be implicitly aligned.

## Requirements

- Visual Studio 2019 v16.10.0+
- VB

## Getting Started

Once you have this in your project, you can enjoy explicit (what you have now) and implicit (what this provides) interface implementations!

Given the following interface:

```vb
Public Interface IPerson
  Property Name As String
End Interface
```

You could create an employee class:

```vb
Public Class Employee
  Implements IPerson

  Public Property Name As String

End Class
```

Which results in Visual Studio seeing this as completely legal code because the generator automatically creates:

```vb
Public Partial Class Employee

  Private Property IPerson_Name As String Implements IPerson.Name
    Get
      Return Name
    End Get
    Set(value As String)
      Name = value
    End Set
  End Property

End Class
```

The implicit implementation follows these rules:

- If the interface is explicity defined, skip.
- A method will map to a similarly named method with matching param signature.
- A property will first attempt to map to a similarly named property with matching param signature.
- If a property can't be determined, will attempt to map to a similarly named field; but only if it doesn't have params.

If unable to determine a map, that mapping will not be generated; which results in Visual Studio notifying the user that the interface has not been defined.

## Events

Events are not handled as I haven't figured out a way to not cause double raising of events by "wrapping" an event.  So, for the time being, Events must be interfaced explicitly.

## More

- You can find the project [here](https://github/dualbrain/samples); please leave feedback, questions, etc. via the [github project](https://github/dualbrain/samples).
- Feel free to reach out to me directly via [twitter](https://twitter.com/dualbrain).
- Even better, join the VB conversation over on [gitter](https://gitter.im/VB-NET).
