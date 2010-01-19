#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Controls.EwEGrid

#End Region ' Imports

<CLSCompliant(False)> _
Public Class gridFixedEscapement
    Inherits EwEGrid

    Public Sub New()
        MyBase.new()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim core As cCore = cCore.GetInstance()
        Dim src As cCoreInputOutputBase = Nothing

        Me.Redim(1, 3)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_FIXEDESCAPE)

        Me.FixedColumns = 2
    End Sub

    Protected Overrides Sub FillData()

        Dim core As cCore = cCore.GetInstance()
        Dim MSEGrp As cMSEGroupInput = Nothing
        Dim group As cCoreInputOutputBase = Nothing
        Dim cell As ICell = Nothing

        ' For each group
        For iGroup As Integer = 1 To core.nLivingGroups

            Me.AddRow()

            ''Get the group info
            group = core.EcoPathGroupInputs(iGroup)
            MSEGrp = core.MSEManager.GroupInputs(iGroup)

            Me(iGroup, 0) = New EwERowHeaderCell(iGroup)

            'Group name as row header
            Me(iGroup, 1) = New PropertyRowHeaderCell(MSEGrp, eVarNameFlags.Name)
            Me(iGroup, 2) = New PropertyCell(MSEGrp, eVarNameFlags.MSEFixedEscapement)
        Next

    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.MSE
        End Get
    End Property

End Class
