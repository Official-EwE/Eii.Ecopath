'==============================================================================
'
' $Log: cLayerEditorAdvection.vb,v $
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
    ''' Layer editor that supports manual modification of Ecospace advection data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorAdvection
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorAdvection))
        End Sub

#End Region ' Construction

    End Class

End Namespace