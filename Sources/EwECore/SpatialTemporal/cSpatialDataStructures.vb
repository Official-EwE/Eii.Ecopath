#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    Public Class cSpatialDataStructures

        ''' <summary>Avalailable data adapters</summary>
        Public DataAdapters As New Dictionary(Of eVarNameFlags, cSpatialDataAdapter)
        ''' <summary>Flag stating how Ecospace time steps are interpreted when accessing remote data. If true, 
        ''' an Ecospace time step is interpreted as an offset to the start time of a remote dataset. If false,
        ''' an Ecospace time step is translated to an absolute time value for matching remote dataset data.
        ''' </summary>
        Public AdapterUseRelativeTime As Boolean = True

        Public Sub SetDefaults()
            For Each vn As eVarNameFlags In Me.DataAdapters.Keys
                Dim adt As cSpatialDataAdapter = Me.DataAdapters(vn)
                adt.SetDefaults()
            Next
        End Sub

        Public Property DatasetGUID(varname As eVarNameFlags, index As Integer) As String
            Get

            End Get
            Set(value As String)

            End Set
        End Property

        Public Property ConverterName(varname As eVarNameFlags, index As Integer) As String
            Get

            End Get
            Set(value As String)

            End Set
        End Property

        Public Property ConverterConfiguration(varname As eVarNameFlags, index As Integer) As String
            Get

            End Get
            Set(value As String)

            End Set
        End Property

    End Class

End Namespace
