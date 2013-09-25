
Public Class GOTMPythonCallback
    Public Counter As Integer
    Public EwERuntime As Integer
    Public EwETimestep As Double
    Public Shared sizeret As Single
    Public Shared GOTMPT As GOTMplugin
    Dim PyResult As Python.Runtime.PyObject
    Public Sub SimulationStarted(ByRef timestep As Single, ByRef duration As Integer, ByRef stpsz As Double)
        sizeret = GOTMPT.setstep(duration, stpsz)
    End Sub

    Public Sub writeGOTMPT(ByRef plug As GOTMplugin)

        GOTMPT = plug
    End Sub

    Public Sub TimeStepOver()
        Dim hasended As Boolean
        hasended = GOTMPT.runstep()
    End Sub
    Public Sub SimIsOver()
        GOTMPT.isover()
    End Sub
    Public Sub RepStepSize(ByRef startin As Single, ByRef endin As Single, ByRef stepszin As Single)
        Dim si, ei, szi As Double
        si = startin
        ei = endin
        szi = stepszin
    End Sub

    Dim PyArgs As Python.Runtime.PyTuple
    Dim PyNumarg As Python.Runtime.PyNumber

    REM Public Sub CreateArgs()
    REM PyArgs.SetItem(0,Py
    REM End Sub





End Class
