Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceRegion
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Try

            Me.m_dataType = eDataTypes.EcospaceRegion
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceHabitat, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceRegion.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceRegion. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variables by dot '.' operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of cells in a region.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumCells() As Integer 
        Get
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim l As cEcospaceLayerRegion = bm.LayerRegion
            Dim iIndex As Integer = Me.Index
            Dim iNumCells As Integer = 0

            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol
                    If CInt(l.Cell(iRow, iCol)) = iIndex Then
                        iNumCells += 1
                    End If
                Next
            Next
            Return iNumCells

        End Get
    End Property

#End Region ' Variables by dot '.' operator

End Class
