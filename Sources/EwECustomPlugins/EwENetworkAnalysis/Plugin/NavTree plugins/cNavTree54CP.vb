#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cNavTree54CP
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav4_output_extend
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.PAGE_CYCLES_PATHWAYS
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.Pathway_cons_tl1
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa54"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot & "|nwa00"
        End Get
    End Property
End Class
