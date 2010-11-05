#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cNavTree46Key_Table
    Inherits cNavTree44Key

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav1_application_put
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.PAGE_KEYSTONENESS_TABLE
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.KeystonenessTable
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa46"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return MyBase.NavigationTreeItemLocation & "|" & MyBase.Name
        End Get
    End Property
End Class
