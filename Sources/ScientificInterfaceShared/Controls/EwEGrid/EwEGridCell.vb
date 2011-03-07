#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports SourceGrid2.DataModels

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A standard EwE grid cell for static values.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwECell
        : Inherits EwECellBase

#Region " Construction "

        Public Sub New(ByVal objVal As Object, ByVal t As Type, _
                       Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK)
            MyBase.New(objVal, t)
            ' Set value
            If objVal IsNot Nothing Then Me.Value = objVal
            ' Set style
            Me.Style = style
        End Sub

        Public Sub New(ByVal objVal As Object, ByVal ed As EditorControlBase, _
                       Optional ByVal style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK)
            MyBase.New(objVal, ed)
            ' Set value
            If objVal IsNot Nothing Then Me.Value = objVal
            ' Set style
            Me.Style = style
        End Sub

        Public Overrides Sub Dispose()
            ' JS 13Dec10: Memory leaks were discovered on tooltips. Perhaps explicitly 
            '             clearing the grid tooltip text wil fix this.
            Me.ToolTipText = ""
            MyBase.Dispose()
        End Sub

#End Region ' Construction 

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <summary>Locally maintained value.</summary>
        ''' -------------------------------------------------------------------
        Private m_objValue As Object = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commonly called in response to end edit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub SetValue(ByVal p_Position As SourceGrid2.Position, ByVal p_Value As Object)
            Me.Value = p_Value
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the locally maintained value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                Return Me.m_objValue
            End Get
            Set(ByVal objValue As Object)
                Me.m_objValue = objValue
            End Set
        End Property

#End Region ' Data

    End Class

End Namespace
