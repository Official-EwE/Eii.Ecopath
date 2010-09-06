#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorVector
        Inherits cLayerEditor

#Region " Private vars "

        Private m_ptfDelta As PointF = Nothing
        Private m_szfCell As SizeF = Nothing
        Private m_sScaleFactor As Single = 25

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal t As Type)
            MyBase.New(t)
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.StartEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub StartEdit(ByVal ptClick As Point, ByVal buttons As MouseEventArgs)
            MyBase.StartEdit(ptClick, buttons)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Edit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Edit(ByVal ptFrom As Point, _
                                  ByVal ptTo As Point, _
                                  ByVal ptDeltaMouse As Point, _
                                  ByVal szfCell As SizeF, _
                                  ByVal args As MouseEventArgs, _
                                  ByRef ptUpdateMin As Point, _
                                  ByRef ptUpdateMax As Point)

            Me.m_ptfDelta = New PointF(ptDeltaMouse.X, ptDeltaMouse.Y)
            Me.m_szfCell = New SizeF(szfCell.Width, szfCell.Height)

            MyBase.Edit(ptFrom, ptTo, ptDeltaMouse, szfCell, args, ptUpdateMin, ptUpdateMax)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.EndEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndEdit()
            MyBase.EndEdit()
        End Sub

        Protected Overrides Sub SetCellValue(ByVal ptSet As Point, _
                                             ByVal e As MouseEventArgs, _
                                             ByVal ptClick As Point)

            ' Distance mouse has travelled
            Dim dx As Single = CSng(Math.Sqrt(Me.m_ptfDelta.X * Me.m_ptfDelta.X + Me.m_ptfDelta.Y * Me.m_ptfDelta.Y))

            If dx < 4 Then Return

            Me.Layer.Value(ptSet.Y, ptSet.X) = New Single() {Me.m_ptfDelta.X * Me.m_sScaleFactor / Me.m_szfCell.Width, _
                                                             Me.m_ptfDelta.Y * Me.m_sScaleFactor / Me.m_szfCell.Height}

        End Sub

        Public Overrides Property CellValue() As Object
            Get
                Debug.Assert(False, "Code should be bypassed for vector layers")
                Return New Single() {1, -1}
            End Get
            Set(ByVal value As Object)
                Debug.Assert(False, "Code should be bypassed for vector layers")
                'MyBase.CellValue = value
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace