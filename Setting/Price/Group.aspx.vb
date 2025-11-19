Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Group
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesignSearch()

            ddlDesignSearch.SelectedValue = Session("priceProductGroupDesign")
            txtSearch.Text = Session("priceProductGroupSearch")
            BindData(ddlDesignSearch.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        BindDesignType()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Product Price Group"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub ddlDesignSearch_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignSearch.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignSearch.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlDesignSearch.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                BindDesignType()
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Product Price Group"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM PriceProductGroups WHERE Id = '" + lblId.Text + "'")
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    ddlDesignType.SelectedValue = myData.Tables(0).Rows(0).Item("DesignId").ToString()
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
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Prices WHERE DataId = '" + dataId + "' AND Type='PriceProductGroups' ORDER BY ActionDate DESC")
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
                MessageError_Process(True, "PRODUCT PRICE GROUP NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlDesignType.SelectedValue = "" Then
                MessageError_Process(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim designId As String = UCase(ddlDesignType.SelectedValue).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceProductGroups OUTPUT INSERTED.Id VALUES (NEWID(), @DesignId, @Name, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", designId)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PriceProductGroups", thisId, Session("LoginId").ToString(), "Price Product Group Created"}
                    settingCfg.Log_Price(dataLog)

                    Session("priceProductGroupDesign") = ddlDesignSearch.SelectedValue
                    Session("priceProductGroupSearch") = txtSearch.Text

                    Response.Redirect("~/setting/price/group", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE PriceProductGroups SET DesignId=@DesignId, Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", designId)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PriceProductGroups", lblId.Text, Session("LoginId").ToString(), "Price Product Group Updated"}
                    settingCfg.Log_Price(dataLog)

                    Session("priceProductGroupDesign") = ddlDesignSearch.SelectedValue
                    Session("priceProductGroupSearch") = txtSearch.Text
                    Response.Redirect("~/setting/price/group", False)
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
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM PriceProductGroups WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("priceProductGroupDesign") = ddlDesignSearch.SelectedValue
            Session("priceProductGroupSearch") = txtSearch.Text

            Response.Redirect("~/setting/price/group", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("priceProductGroupDesign") = ddlDesignSearch.SelectedValue
        Session("priceProductGroupSearch") = txtSearch.Text

        Response.Redirect("~/setting/price/group", False)
    End Sub

    Private Sub BindData(SearchDesign As String, SearchText As String)
        Try
            Dim stringSearch As String = String.Empty
            Dim stringDesign As String = " WHERE PriceProductGroups.DesignId = '" + UCase(SearchDesign).ToString() + "'"

            If SearchDesign = "" Then
                stringDesign = " WHERE PriceProductGroups.DesignId IS NOT NULL"
            End If
            If Not SearchText = "" Then
                stringSearch = " AND (PriceProductGroups.Id LIKE '%" + SearchText.Trim() + "%' OR PriceProductGroups.Name LIKE '%" + SearchText.Trim() + "%')"
            End If
            Dim myQuery As String = String.Format("SELECT PriceProductGroups.*, CASE WHEN PriceProductGroups.Active = 1 THEN 'Yes' WHEN PriceProductGroups.Active = 0 THEN 'No' ELSE 'Error' END AS DataActive, Designs.Name AS DesignName FROM PriceProductGroups LEFT JOIN Designs ON PriceProductGroups.DesignId = Designs.Id {0} {1} ORDER BY Designs.Name, PriceProductGroups.Name ASC", stringDesign, stringSearch)

            gvList.DataSource = settingCfg.GetListData(myQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesignSearch()
        ddlDesignSearch.Items.Clear()
        Try
            ddlDesignSearch.DataSource = settingCfg.GetListData("SELECT Id, UPPER(Name) NameText FROM Designs WHERE Active = 1 ORDER BY Name ASC")
            ddlDesignSearch.DataTextField = "NameText"
            ddlDesignSearch.DataValueField = "Id"
            ddlDesignSearch.DataBind()

            If ddlDesignSearch.Items.Count > 0 Then
                ddlDesignSearch.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            ddlDesignType.DataSource = settingCfg.GetListData("SELECT *, UPPER(Id) AS IdText FROM Designs WHERE Active = 1 ORDER BY Name ASC")
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
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
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Prices WHERE Id = '" + Id + "'")
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
