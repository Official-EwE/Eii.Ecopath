#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPFile
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports System.IO

#End Region ' Imports

Namespace Ecospace.Basemap

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for importing and exporting data from external spatial
    ''' data sources.
    ''' </summary>
    ''' <remarks>
    ''' <para>This is a very, VERY temporary solution right from the moment of
    ''' conception. This class has several shortcomings, such as improper use
    ''' of spatial extents etc.</para>
    ''' <para>Eventually, data will live in EwE as real spatial data
    ''' and will only be rasterized when used in models.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cImportExportData

        ' ToDo_JS: globalize this
        Public Shared cMAPPING_IMPLICIT As String = "(file content)"

        Private m_nRows As Integer = 0
        Private m_nCols As Integer = 0
        Private m_data As New Dictionary(Of String, Single())
        Private m_astrAttributes As String() = Nothing
        Private m_bRowColImplicit As Boolean = False

        Public Sub New(ByVal nRows As Integer, ByVal nCols As Integer, _
                       Optional ByVal astrAttributes() As String = Nothing)
            Me.m_nRows = nRows
            Me.m_nCols = nCols
            Me.Attributes = astrAttributes
        End Sub

        Public Property Attributes() As String()
            Get
                Return Me.m_astrAttributes
            End Get
            Set(ByVal value As String())

                If value Is Nothing Then
                    Me.m_bRowColImplicit = True
                Else
                    Me.m_bRowColImplicit = (value.Count = 0)
                End If

                If (Me.m_bRowColImplicit) Then
                    Me.m_astrAttributes = New String() {cImportExportData.cMAPPING_IMPLICIT}
                Else
                    Me.m_astrAttributes = value
                End If

                ' Clear
                Me.m_data.Clear()

                ' Create storage
                For Each strAttribute As String In Me.Attributes
                    Dim asCells(Me.NumCells) As Single
                    Me.m_data.Add(strAttribute, asCells)
                Next

            End Set
        End Property

        Public Property Value(ByVal iRow As Integer, ByVal iCol As Integer, _
                              Optional ByVal strAttribute As String = "") As Single
            Get
                Return Me.Value(Me.Cell(iRow, iCol), strAttribute)
            End Get
            Set(ByVal value As Single)
                Me.Value(Me.Cell(iRow, iCol), strAttribute) = value
            End Set
        End Property

        Public Property Value(ByVal iCell As Integer, _
                              Optional ByVal strAttribute As String = "") As Single
            Get
                If String.IsNullOrEmpty(strAttribute) Then
                    strAttribute = cImportExportData.cMAPPING_IMPLICIT
                End If
                Return Me.m_data(strAttribute)(iCell)
            End Get
            Set(ByVal value As Single)
                If String.IsNullOrEmpty(strAttribute) Then
                    strAttribute = cImportExportData.cMAPPING_IMPLICIT
                End If
                Me.m_data(strAttribute)(iCell) = value
            End Set
        End Property

        Public Function Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Integer
            Return iRow * Me.m_nCols + iCol
        End Function

        Public Function NumCells() As Integer
            Return Me.m_nCols * Me.m_nRows
        End Function

        Public Function IsRowColImplicit() As Boolean
            Return Me.m_bRowColImplicit
        End Function

    End Class

End Namespace
