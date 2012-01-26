#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    Public Class cSpatialDataStructures

        ''' <summary>Availalable data adapters</summary>
        Public DataAdapters As New List(Of cSpatialDataAdapter)
       
        Public Sub SetDefaults()

            Me.m_data.Clear()

            For Each adt As cSpatialDataAdapter In Me.DataAdapters
                adt.SetDefaults()
                Dim iLen As Integer = adt.Length
                Dim arr(iLen) As cData
                For i As Integer = 0 To iLen - 1 : arr(i) = New cData() : Next
                Me.m_data(adt.VarName) = arr
            Next adt

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

        Public Property ConverterType(varname As eVarNameFlags, index As Integer) As String
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
            Public DatasetGUID As String = ""
            Public ConverterName As String = ""
            Public ConverterConfig As String = ""
        End Class

        Private m_data As New Dictionary(Of eVarNameFlags, cData())

#End Region ' Internals

    End Class

End Namespace
