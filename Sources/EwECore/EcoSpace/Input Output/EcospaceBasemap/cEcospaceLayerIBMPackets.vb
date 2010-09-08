#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports


''' <summary>
''' Layer providing access to Ecospace IBM packets data.
''' </summary>
Public Class cEcospaceLayerIBMPackets
    Inherits cEcospaceLayerInteger

    ''' <summary>Layer data cache (row, col) = iStage</summary>
    Private m_aiPositions As Integer(,)
    ''' <summary>iStanza to find packets for</summary>
    Private m_iStanza As Integer = 1

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iStanza As Integer)
        MyBase.New(theCore, manager, eVarNameFlags.LayerIBMPackets)
        Me.m_dataType = eDataTypes.EcospaceLayerIBMPackets
        Me.m_iStanza = iStanza
    End Sub

#Region " Cell interaction "

    Public ReadOnly Property iStanza() As Integer
        Get
            Return Me.m_iStanza
        End Get
    End Property

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            If Me.m_aiPositions Is Nothing Then
                Me.Refresh()
            End If
            Return Me.m_aiPositions(iRow, iCol)
        End Get
        Set(ByVal value As Object)
            ' Cannot set
        End Set
    End Property

    Public Overrides Sub Invalidate()
        Me.m_aiPositions = Nothing
    End Sub

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return CSng(Me.m_core.nMaxStanza)
        End Get
    End Property

#End Region ' Cell interaction

#Region " Private bits "

    Private Function StanzaData() As cStanzaDatastructures
        Return DirectCast(Me.Data, cStanzaDatastructures)
    End Function

    Private Sub Refresh()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim data As cStanzaDatastructures = Me.StanzaData

        ReDim m_aiPositions(bm.InRow, bm.InCol)

        ' Clear
        For iRowTest As Integer = 1 To bm.InRow
            For iColTest As Integer = 1 To bm.InCol
                Me.m_aiPositions(iRowTest, iColTest) = cCore.NULL_VALUE
            Next
        Next

        For iLifeStage As Integer = 1 To CInt(Me.MaxValue)
            For iPacket As Integer = 1 To data.Npackets
                Dim iRow As Integer = CInt(data.iPacket(Me.m_iStanza, iLifeStage, iPacket))
                Dim iCol As Integer = CInt(data.jPacket(Me.m_iStanza, iLifeStage, iPacket))
                If Me.ValidateCellPosition(iRow, iCol) Then
                    Me.m_aiPositions(iRow, iCol) = iLifeStage
                End If
            Next
        Next
    End Sub

#End Region ' Private bits

End Class
