'==============================================================================
'
' $Log: cLayerEditorRange.vb,v $
' Revision 1.1  2008/11/04 04:40:16  jeroens
' Split into separate files, moved
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of layers where cells
    ''' can have a range of values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorRange
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorRange))
        End Sub

#End Region ' Construction

    End Class

End Namespace
