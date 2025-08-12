Class MainWindow
    Dim processes() As Process

    Function GetSelectedProcess() As String
        Try
            Dim proccessId As String = ProcessesList.SelectedItem.ToString().Split("-").FirstOrDefault().Trim()
            Return proccessId
        Catch exception As Exception

        End Try
    End Function

    Sub Pause_Button_OnClick()
        Dim id = GetSelectedProcess()
    End Sub

    Sub Run_Button_OnClick()
        Dim id = GetSelectedProcess()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        processes = Process.GetProcesses()

        For Each process As Process In processes
            ProcessesList.Items.Add($"{process.Id} - {process.ProcessName}")
        Next
    End Sub
End Class
