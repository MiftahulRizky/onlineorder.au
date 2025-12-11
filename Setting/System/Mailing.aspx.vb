Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_System_Mailing
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/system", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("mailingSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        BindApp()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Mailing"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                BindApp()
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Mailing"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Mailings WHERE Id = '" + lblId.Text + "'")

                    ddlAppId.SelectedValue = myData.Tables(0).Rows(0).Item("ApplicationId").ToString()
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtServer.Text = myData.Tables(0).Rows(0).Item("Server").ToString()
                    txtHost.Text = myData.Tables(0).Rows(0).Item("Host").ToString()
                    txtPort.Text = myData.Tables(0).Rows(0).Item("Port").ToString()

                    ddlNetworkCredentials.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("NetworkCredentials"))
                    ddlDefaultCredentials.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("DefaultCredentials"))
                    ddlEnableSSL.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("EnableSSL"))

                    txtAccount.Text = myData.Tables(0).Rows(0).Item("Account").ToString()
                    txtPassword.Text = myData.Tables(0).Rows(0).Item("Password").ToString()
                    txtAlias.Text = myData.Tables(0).Rows(0).Item("Alias").ToString()
                    txtSubject.Text = myData.Tables(0).Rows(0).Item("Subject").ToString()
                    txtTo.Text = myData.Tables(0).Rows(0).Item("To").ToString()
                    txtCc.Text = myData.Tables(0).Rows(0).Item("Cc").ToString()
                    txtBcc.Text = myData.Tables(0).Rows(0).Item("Bcc").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    btnSubmitProcess.Visible = False
                    If Session("LevelName") = "Leader" OR Session("levelName") = "Super Admin" Then btnSubmitProcess.Visible = True

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Systems WHERE DataId = '" + dataId + "' AND Type='Mailings' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Mailings WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("mailingSearch") = txtSearch.Text
            Response.Redirect("~/setting/system/mailing", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdCopy.Text

            Dim thisId As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mailings OUTPUT INSERTED.Id SELECT NEWID(), ApplicationId, Name + ' - Copy', Server, Host, Port, NetworkCredentials, DefaultCredentials, EnableSSL, Account, Password, Alias, Subject, [To], Cc, Bcc, Description, Active FROM Mailings WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    thisId = myCmd.ExecuteScalar().ToString()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"Mailings", thisId, Session("LoginId").ToString(), "Mailing Created | Duplicated of " & lblId.Text}
            settingCfg.Log_System(dataLog)

            Session("mailingSearch") = txtSearch.Text
            Response.Redirect("~/setting/system/mailing", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlAppId.SelectedValue = "" Then
                MessageError_Process(True, "APPLICATION NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError_Process(True, "MAILING NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtServer.Text = "" Then
                MessageError_Process(True, "SERVER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtHost.Text = "" Then
                MessageError_Process(True, "HOST IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtPort.Text = "" Then
                MessageError_Process(True, "PORT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtAlias.Text = "" Then
                MessageError_Process(True, "EMAIL ALIAS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtSubject.Text = "" Then
                MessageError_Process(True, "EMAIL SUBJECT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(txtPassword.Text) And txtAccount.Text = "" Then
                MessageError_Process(True, "EMAIL ACCOUNT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim appId As String = UCase(ddlAppId.SelectedValue).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mailings OUTPUT INSERTED.Id VALUES (NEWID(), @ApplicationId, @Name, @Server, @Host, @Port, @NetworkCredentials, @DefaultCredentials, @EnableSsl, @Account, @Password, @Alias, @Subject, @To, @Cc, @Bcc, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@ApplicationId", appId)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Server", txtServer.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Host", txtHost.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Port", txtPort.Text.Trim())
                            myCmd.Parameters.AddWithValue("@NetworkCredentials", ddlNetworkCredentials.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DefaultCredentials", ddlDefaultCredentials.SelectedValue)
                            myCmd.Parameters.AddWithValue("@EnableSsl", ddlEnableSSL.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Account", txtAccount.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                            myCmd.Parameters.AddWithValue("@To", txtTo.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Cc", txtCc.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Bcc", txtBcc.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Mailings", thisId, Session("LoginId").ToString(), "Mailing Created"}
                    settingCfg.Log_System(dataLog)

                    Session("mailingSearch") = txtSearch.Text
                    Response.Redirect("~/setting/system/mailing", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Mailings SET ApplicationId=@ApplicationId, Name=@Name, Server=@Server, Host=@Host, Port=@Port, NetworkCredentials=@NetworkCredentials, DefaultCredentials=@DefaultCredentials, EnableSSL=@EnableSSL, Account=@Account, Password=@Password, Alias=@Alias, Subject=@Subject, [To]=@To, Cc=@Cc, Bcc=@Bcc, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@ApplicationId", appId)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Server", txtServer.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Host", txtHost.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Port", txtPort.Text.Trim())
                            myCmd.Parameters.AddWithValue("@NetworkCredentials", ddlNetworkCredentials.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DefaultCredentials", ddlDefaultCredentials.SelectedValue)
                            myCmd.Parameters.AddWithValue("@EnableSsl", ddlEnableSSL.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Account", txtAccount.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                            myCmd.Parameters.AddWithValue("@To", txtTo.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Cc", txtCc.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Bcc", txtBcc.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Mailings", lblId.Text, Session("LoginId").ToString(), "Mailing Updated"}
                    settingCfg.Log_System(dataLog)

                    Session("mailingSearch") = txtSearch.Text
                    Response.Redirect("~/setting/system/mailing", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("mailingSearch") = txtSearch.Text
        Response.Redirect("~/setting/system/mailing", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Mailings.Name LIKE '%" + SearchText.Trim() + "%' OR Applications.Name LIKE '%" + SearchText.Trim() + "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT Mailings.*, Applications.Name AS AppName, CASE WHEN Mailings.Active=1 THEN 'Yes' WHEN Mailings.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Mailings LEFT JOIN Applications ON Mailings.ApplicationId = Applications.Id {0} ORDER BY Applications.Name, Mailings.Name ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin") Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindApp()
        ddlAppId.Items.Clear()
        Try
            ddlAppId.DataSource = settingCfg.GetListData("SELECT * FROM Applications ORDER BY Name ASC")
            ddlAppId.DataTextField = "Name"
            ddlAppId.DataValueField = "Id"
            ddlAppId.DataBind()

            If ddlAppId.Items.Count > 1 Then
                ddlAppId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Process(Show As Boolean, Msg As String)
        divErrorProcess.Visible = Show : msgErrorProcess.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Systems WHERE Id = '" + Id + "'")
            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function

    Protected Function VisibleAction() As Boolean
        If Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function
End Class
