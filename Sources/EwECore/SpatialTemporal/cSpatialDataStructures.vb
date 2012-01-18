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

            Me.m_data.Clear()

            For Each vn As eVarNameFlags In Me.DataAdapters.Keys
                Dim adt As cSpatialDataAdapter = Me.DataAdapters(vn)
                adt.SetDefaults()
                Dim iLen As Integer = adt.Length
                Dim arr(iLen) As cData
                For i As Integer = 0 To iLen : arr(iLen) = New cData() : Next
                Me.m_data(vn) = arr
            Next vn

        End Sub

        Public ReadOnly Property NumItems(varname As eVarNameFlags) As Integer
            Get
                Return Me.m_data(varname).Length
            End Get
        End Property

        Public Property DatasetGUID(varname As eVarNameFlags, index As Integer) As String
            Get
                Return Me.m_data(varname)(index).DatasetGUID
            End Get
            Set(value As String)
                Me.m_data(varname)(index).DatasetGUID = value
            End Set
        End Property

        Public Property ConverterName(varname As eVarNameFlags, index As Integer) As String
            Get
                Return Me.m_data(varname)(index).ConverterName
            End Get
            Set(value As String)
                Me.m_data(varname)(index).ConverterName = value
            End Set
        End Property

        Public Property ConverterConfiguration(varname As eVarNameFlags, index As Integer) As String
            Get
                Return Me.m_data(varname)(index).ConverterConfig
            End Get
            Set(value As String)
                Me.m_data(varname)(index).ConverterConfig = value
            End Set
        End Property

#Region " Internals "

        Private Class cData
            Public DatasetGUID As String
            Public ConverterName As String
            Public ConverterConfig As String
        End Class

        Private m_data As New Dictionary(Of eVarNameFlags, cData())

#End Region ' Internals

    End Class

End Namespace
