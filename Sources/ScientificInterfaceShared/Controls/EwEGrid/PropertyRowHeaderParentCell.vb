#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderParentCell implements a PropertyRowHeaderCell rendered 
    ''' as a parent cell in a hierarchy.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyRowHeaderParentCell
        : Inherits PropertyRowHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwEParentRowHeader()
        ''' <summary>Optional linked hierarchy cell</summary>
        Private m_hcell As EwEHierarchyGridCell = Nothing

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal Source As cCoreInputOutputBase, _
                       ByVal VarName As eVarNameFlags, _
                       Optional ByVal SourceSec As cCoreInputOutputBase = Nothing, _
                       Optional ByVal hcell As EwEHierarchyGridCell = Nothing)
            Me.New(pm.GetProperty(Source, VarName, SourceSec), hcell)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, _
                       Optional ByVal hcell As EwEHierarchyGridCell = Nothing)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
            Me.m_hcell = hcell
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strText As String = MyBase.DisplayText
                If (Me.m_hcell IsNot Nothing) Then
                    Dim iNumChildren As Integer = Me.m_hcell.NumChildRows
                    If (iNumChildren > 0) And (Not Me.m_hcell.Expanded) Then
                        strText = String.Format(My.Resources.GENERIC_LABEL_DETAILED, strText, iNumChildren)
                    End If
                End If
                Return strText
            End Get
        End Property

#End Region ' Construction 

    End Class

End Namespace
