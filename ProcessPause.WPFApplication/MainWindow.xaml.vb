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
            Dim selected = ProcessesList.SelectedItem
            If selected Is Nothing Then Return Nothing
            Dim processIdString As String = selected.ToString().Split("-"c).FirstOrDefault().Trim()
            Dim pid As Integer
            If Integer.TryParse(processIdString, pid) Then
                Return Process.GetProcessById(pid)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("Process not found.")
            Return Nothing
        End Try
    End Function

    Sub Search_OnClick()
        Dim filter As String = Filter_ComboBox.SelectedItem.Content
        Dim searchText As String = Filter_TextBox.Text

        If String.IsNullOrEmpty(searchText) Then
            ProcessList_Load()
            ProccessList_Render()
            Return
        End If

        Try
            If filter = "Id" Then
                Dim pid As Integer
                If Integer.TryParse(searchText, pid) Then
                    Dim founded = Process.GetProcessById(pid)
                    processes = {founded}
                Else
                    MessageBox.Show("Por favor, insira um ID válido.")
                    Return
                End If
            ElseIf filter = "Name" Then
                processes = Process.GetProcesses().Where(Function(p) p.ProcessName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray()
            Else
                MessageBox.Show("Selecione um filtro válido.")
                Return
            End If
        Catch ex As Exception
            MessageBox.Show("Processo não encontrado ou erro: " & ex.Message)
            Return
        End Try

        ProccessList_Render()
    End Sub

    Sub Pause_Button_OnClick()
        Dim process = GetSelectedProcess()
        If process Is Nothing Then Return

        Try
            For Each thread As ProcessThread In process.Threads
                Dim pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, False, thread.Id)
                If pOpenThread <> IntPtr.Zero Then
                    SuspendThread(pOpenThread)
                    CloseHandle(pOpenThread)
                End If
            Next
            ProcessList_Load()
            ProccessList_Render()
            MessageBox.Show($"Process {process.ProcessName} suspended.")
        Catch ex As Exception
            MessageBox.Show("Error suspending process: " & ex.Message)
        End Try
    End Sub

    Sub Run_Button_OnClick()
        Dim process = GetSelectedProcess()
        If process Is Nothing Then Return

        Try
            For Each thread As ProcessThread In process.Threads
                Dim pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, False, thread.Id)
                If pOpenThread <> IntPtr.Zero Then
                    ResumeThread(pOpenThread)
                    CloseHandle(pOpenThread)
                End If
            Next
            ProcessList_Load()
            ProccessList_Render()
            MessageBox.Show($"Process {process.ProcessName} resumed.")
        Catch ex As Exception
            MessageBox.Show("Error resuming process: " & ex.Message)
        End Try
    End Sub

    Sub ProccessList_Render()
        Dim processIds = New HashSet(Of Integer)(processes.Select(Function(p) p.Id))

        For i As Integer = ProcessesList.Items.Count - 1 To 0 Step -1
            Dim itemText As String = ProcessesList.Items(i).ToString()
            Dim idString As String = itemText.Split("-"c).First().Trim()
            Dim id As Integer
            If Integer.TryParse(idString, id) Then
                If Not processIds.Contains(id) Then
                    ProcessesList.Items.RemoveAt(i)
                End If
            Else
                ProcessesList.Items.RemoveAt(i)
            End If
        Next

        Dim existingIds = New HashSet(Of Integer)
        For Each item As String In ProcessesList.Items
            Dim idString As String = item.Split("-"c).First().Trim()
            Dim id As Integer
            If Integer.TryParse(idString, id) Then
                existingIds.Add(id)
            End If
        Next

        For Each proc As Process In processes
            If Not existingIds.Contains(proc.Id) Then
                Dim status As String = If(proc.Responding, "Running", "Not Responding")
                ProcessesList.Items.Add($"{proc.Id} - {proc.ProcessName} - {status}")
            End If
        Next
    End Sub


    Sub ProcessList_Load()
        processes = Process.GetProcesses()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ProcessList_Load()
        ProccessList_Render()
    End Sub
End Class
