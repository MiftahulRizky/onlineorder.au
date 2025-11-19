Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Design
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/product/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("designSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Design Type"
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
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Design Type"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Designs WHERE Id = '" + lblId.Text + "'")
                    lblNameOld.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    ddlType.SelectedValue = myData.Tables(0).Rows(0).Item("Type").ToString()
                    txtPage.Text = myData.Tables(0).Rows(0).Item("Page").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Designs' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "DESIGN NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lblAction.Text = "Edit" Then
                If txtName.Text <> lblNameOld.Text Then
                    If settingCfg.GetItemData_Integer("SELECT COUNT(*) FROM Designs WHERE Name = '" + txtName.Text.Trim() + "'") > 0 Then
                        MessageError_Process(True, "DESIGN NAME ALREADY EXISTS !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                        Exit Sub
                    End If
                End If
            End If

            If ddlType.SelectedValue = "" Then
                MessageError_Process(True, "TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtPage.Text = "" Then
                MessageError_Process(True, "PAGE URL IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Designs OUTPUT INSERTED.Id VALUES (NEWID(), @Name, @Type, @Description, @Page, @Active)")
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=CONCAT(DesignId, @Id)")
                            myCmd.Parameters.AddWithValue("@Id", thisId)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Designs", thisId, Session("LoginId").ToString(), "Design Type Created"}
                    settingCfg.Log_Product(dataLog)

                    Session("designSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/design", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Designs SET Name=@Name, Type=@Type, Description=@Description, Page=@Page, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Designs", lblId.Text, Session("LoginId").ToString(), "Design Type Updated"}
                    settingCfg.Log_Product(dataLog)

                    Session("designSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/design", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            'BOTTOM RAIL CHAINS FABRICS
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Designs WHERE Id=@Id UPDATE Blinds SET DesignId=NULL WHERE DesignId=@Id UPDATE Products SET DesignId=NULL WHERE DesignId=@Id UPDATE PriceProductGroups SET DesignId=NULL WHERE DesignId=@Id UPDATE PriceSurcharges SET DesignId=NULL WHERE DesignId=@Id DELETE FROM CustomerDiscounts WHERE DesignId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("designSearch") = txtSearch.Text
            Response.Redirect("~/setting/product/design", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("designSearch") = txtSearch.Text
        Response.Redirect("~/setting/product/design", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Id LIKE '%" + SearchText.Trim() + "%' OR Name LIKE '%" + SearchText.Trim() + "%' OR Description LIKE '%" + SearchText.Trim() + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Designs {0} ORDER BY Name ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Products WHERE Id = '" + Id + "'")
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
        If Session("LevelName") = "Leader" Then
            Return True
        End If
        Return False
    End Function
End Class
