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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the scale factor for rendering this layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ScaleFactor() As Single
            Get
                Return Me.m_sScaleFactor
            End Get
            Set(ByVal value As Single)
                Me.m_sScaleFactor = value
            End Set
        End Property

#End Region ' Public interfaces

#Region " Internal overrides "

        Protected Overrides Sub SetCellValue(ByVal ptSet As Point, _
                                             ByVal value As Object, _
                                             ByVal e As MouseEventArgs, _
                                             ByVal ptClick As Point)

            ' Calc the distance the mouse has travelled
            Dim dx As Single = CSng(Math.Sqrt(Me.m_ptfDelta.X * Me.m_ptfDelta.X + Me.m_ptfDelta.Y * Me.m_ptfDelta.Y))
            ' Only process significant changes
            If dx <= 2 Then Return
            ' Ignore value
            Me.Layer.Value(ptSet.Y, ptSet.X) = New Single() {Me.m_ptfDelta.X * Me.m_sScaleFactor / dx, _
                                                             Me.m_ptfDelta.Y * Me.m_sScaleFactor / dx}

        End Sub

        Public Overrides Property CellValue() As Object
            Get
                ' Bypass
                Return Nothing
            End Get
            Set(ByVal value As Object)
                ' Bypass
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick up the cell value at a given point, and store this value in the
        ''' layer editor as the next value that will be set.
        ''' Overridden to pick up the scale factor at a given location.
        ''' </summary>
        ''' <param name="pt">The cell location to pick up a value from.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Pickup(ByVal pt As System.Drawing.Point)

            Try
                ' JS: pt(X,Y) translated to value(row, col); it never fails to confuse me. Even if I wrote this code...
                Dim asValue As Single() = DirectCast(Me.Layer.Value(pt.Y, pt.X), Single())
                Me.m_sScaleFactor = CSng(Math.Sqrt(asValue(0) * asValue(0) + asValue(1) * asValue(1)))

                ' Notify the editor GUI, if any
                If Me.GUI IsNot Nothing Then
                    Me.GUI.UpdateContent(Me)
                End If

            Catch ex As Exception
            End Try

        End Sub

#End Region ' Internal overrides

    End Class

End Namespace