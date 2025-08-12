Imports System.Runtime.InteropServices
Imports System.Diagnostics
Imports System.Threading

Class MainWindow
    Dim processes() As Process
    <DllImport("kernel32.dll")>
    Private Shared Function OpenThread(dwDesiredAccess As Integer, bInheritHandle As Boolean, dwThreadId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function SuspendThread(hThread As IntPtr) As Integer
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function ResumeThread(hThread As IntPtr) As Integer
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function

    Const THREAD_SUSPEND_RESUME As Integer = &H2


    Function GetSelectedProcess() As Process
        Try
            Dim proccessId As String = ProcessesList.SelectedItem.ToString().Split("-").FirstOrDefault().Trim()
            Return Process.GetProcessById(Convert.ToInt32(proccessId))
        Catch exception As Exception
            MessageBox.Show("Proccess not found.")
        End Try
    End Function

    Sub Search_OnClick()
        Dim filter As String = Filter_ComboBox.SelectedValue

        If String.IsNullOrEmpty(Filter_TextBox.Text) Then
            processes = Process.GetProcesses()
            Return
        End If

        If filter = "Id" Then
            Dim founded = Process.GetProcessById(Convert.ToInt32(Filter_TextBox.Text))

            If founded Is Nothing Then
                MessageBox.Show("Processo não encontrado.")
            Else
                processes = {founded}
            End If
        End If

        ProccessList_Render()
    End Sub

    Sub Pause_Button_OnClick()
        Dim proccess = GetSelectedProcess()

        If proccess Is Nothing Then Return

        Try
            For Each thread As ProcessThread In proccess.Threads
                Dim pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, False, thread.Id)

                If pOpenThread <> IntPtr.Zero Then
                    SuspendThread(pOpenThread)
                    CloseHandle(pOpenThread)
                End If
            Next
            ProcessesList.Items.Clear()
            ProccessList_Render()
            MessageBox.Show($"Process {proccess.ProcessName} suspended.")
        Catch exception As Exception
            MessageBox.Show("Error suspending process: " & exception.Message)
        End Try

    End Sub

    Sub Run_Button_OnClick()
        Dim proccess = GetSelectedProcess()

        If proccess Is Nothing Then Return

        Try
            For Each thread As ProcessThread In proccess.Threads
                Dim pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, False, thread.Id)

                If pOpenThread <> IntPtr.Zero Then
                    ResumeThread(pOpenThread)
                    CloseHandle(pOpenThread)
                End If
            Next
            ProcessesList.Items.Clear()
            ProccessList_Render()
            MessageBox.Show($"Process {proccess.ProcessName} resumed.")
        Catch excpetion As Exception
            MessageBox.Show("Error resuming process: " & excpetion.Message)
        End Try
    End Sub

    Sub ProccessList_Render()
        For Each process As Process In processes
            Dim status As String = If(process.Responding, "Running", "Not Responding")
            ProcessesList.Items.Add($"{process.Id} - {process.ProcessName} - {process} - {status}")
        Next

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        processes = Process.GetProcesses()
        ProccessList_Render()
    End Sub
End Class
