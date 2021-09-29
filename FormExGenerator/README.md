# Introduction

- Need to find any classes that are decorated with the following attribute:
  * <FormEx>
    This adds several behaviors and consistent extension points for additional functionality/behaviors such as.
    - Prompt for close. 
- Adds:
  * Handles some standard events - do not repeat usage... see new methods below.
    - Load
    - FormClosing
    - etc. 
  * Adds the follwing properties:
    - IsLoading (Boolean)
    - IsNew (Boolean)
    - IsDirty (Boolean)
  * Adds the following methods:
    - UpdateState
- Now that these exists, expects the following methods to exist:
  - LoadAsync
  - InitializeDataAsync
  - ValidateControl(ctrl)
  - CreateAsync
  - UpdateAsync
  - UnloadAsync

## Requirements

- Visual Studio 2019 v16.10.0+
- VB

## Getting Started

Once you have this in your project, you can enjoy common behaviors provided by FormEx!

Given the following interface:

```vb
<FormEx>
Public Class Form1
  '...
End Class
```

Which results in Visual Studio generating the following:

```vb
Public Partial Class Form1

  Private Property IsDirty As Boolean

  Private Sub Form_Loaded(sender As Object, e As EventArgs) Handles Me.Loaded
    '...
    ' The following line is optional, only included if an existing Loaded method exists.
    'Loaded()
  End Sub

End Class
```

## More

- You can find the project [here](https://github/dualbrain/samples); please leave feedback, questions, etc. via the [github project](https://github/dualbrain/samples).
- Feel free to reach out to me directly via [twitter](https://twitter.com/dualbrain).
- Even better, join the VB conversation over on [gitter](https://gitter.im/VB-NET).
