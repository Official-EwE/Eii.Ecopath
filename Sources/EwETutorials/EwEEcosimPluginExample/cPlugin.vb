Imports EwEPlugin
Imports EwECore

Public Class cPlugin
    Implements IEcosimEndTimestepPlugin
    Implements IEcospaceEndTimestepPlugin


    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize

        MsgBox(Me.Name & " loaded")

    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        If iTime = 42 Then
            MsgBox("Group 1 has biomass " & BiomassAtTimestep(1) & " at time step " & iTime)
        End If

    End Sub

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Bill Jobs"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "Nobody, please"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.Name
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwE25 years: Ecosim plug-in example"
        End Get
    End Property

    Public Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        If iTime = 13 Then

            Dim data As cEcospaceDataStructures = CType(EcospaceDatastructures, cEcospaceDataStructures)
            MsgBox("Group 1 has a biomass of " & data.Bcell(1, 1, 1) & " in cell (1, 1)")

        End If

    End Sub
End Class
