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

        Me.Redim(1, 4)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_FIXEDESCAPE)
        Me(0, 3) = New EwEColumnHeaderCell("Fixed fishing mortality")

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim MSEGrp As cMSEGroupInput = Nothing
        Dim group As cCoreInputOutputBase = Nothing
        Dim cell As ICell = Nothing

        ' For each group
        For iGroup As Integer = 1 To Me.Core.nLivingGroups

            Me.AddRow()

            ' Get the group info
            group = Me.Core.EcoPathGroupInputs(iGroup)
            MSEGrp = Me.Core.MSEManager.GroupInputs(iGroup)

            Me(iGroup, 0) = New EwERowHeaderCell(iGroup)

            'Group name as row header
            Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, MSEGrp, eVarNameFlags.Name)
            Me(iGroup, 2) = New PropertyCell(Me.PropertyManager, MSEGrp, eVarNameFlags.MSEFixedEscapement)
            Me(iGroup, 3) = New PropertyCell(Me.PropertyManager, MSEGrp, eVarNameFlags.MSEFixedF)
        Next

    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.MSE
        End Get
    End Property

End Class
