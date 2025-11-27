
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Group_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" And Not Session("RoleName") = "PPIC & DE" And Not Session("RoleName") = "Data Entry" Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("customerGroupSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Customer Group"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Process(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnAdd_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "gvList_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    txtId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Customer Group"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerGroups WHERE Id = '" + txtId.Text + "'")
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Process(True, "Please contact IT at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkEdit_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "CustomerList" Then
                MessageErrorCustomer(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showCustomer(); };"
                Try
                    gvListCustomer.DataSource = settingCfg.GetListData("SELECT * FROM Customers WHERE [Group] = '" + dataId + "'")
                    gvListCustomer.DataBind()

                    thisScript = "window.onload = function() { showCustomer(); };"
                    ClientScript.RegisterStartupScript(Me.GetType(), "showCustomer", thisScript, True)
                Catch ex As Exception
                    MessageErrorCustomer(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorCustomer(True, "Please contact IT at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkCustomer_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showCustomer", thisScript, True)
                End Try
            ElseIf e.CommandName = "CustomerDiscount" Then
                MessageErrorDiscount(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDiscount(); };"
                Try
                    lblId.Text = dataId
                    gvListDiscount.DataSource = settingCfg.GetListData("SELECT * FROM CustomerDiscounts WHERE CustomerData = '" + dataId + "' AND Active = 1")
                    gvListDiscount.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDiscount", thisScript, True)
                Catch ex As Exception
                    MessageErrorDiscount(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorDiscount(True, "Please contact IT at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkDiscount_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDiscount", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE DataId = '" + dataId + "' AND Type='CustomerGroups' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnManageDiscount_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Session("customerGroupDiscount") = lblId.Text
            Response.Redirect("~/setting/customer/group/discount/", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnManageDiscount_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerGroups WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("customerGroupSearch") = txtSearch.Text
            Response.Redirect("~/setting/customer/group", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtId.Text = "" Then
                MessageError_Process(True, "ID IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lblAction.Text = "Add" Then
                Dim checkId As Integer = settingCfg.GetItemData_Integer("SELECT COUNT(*) FROM CustomerGroups WHERE Id='" + txtId.Text.Trim() + "'")
                If checkId > 0 Then
                    MessageError_Process(True, "ID ALREADY EXISTS !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                MessageError_Process(True, "NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerGroups VALUES (@Id, @Name, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerGroups", txtId.Text, Session("LoginId").ToString(), "Customer Group Created"}
                    settingCfg.Log_Customer(dataLog)

                    Session("customerGroupSearch") = txtSearch.Text
                    Response.Redirect("~/setting/customer/group", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerGroups SET Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerGroups", txtId.Text, Session("LoginId").ToString(), "Customer Group Updated"}
                    settingCfg.Log_Customer(dataLog)

                    Session("customerGroupSearch") = txtSearch.Text
                    Response.Redirect("~/setting/customer/group", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Process(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitProccess_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("customerGroupSearch") = txtSearch.Text
        Response.Redirect("~/setting/customer/group", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Id LIKE '%" + SearchText + "%' OR Name LIKE '%" + SearchText + "%'"
            End If

            Dim myQuery As String = String.Format("SELECT *, CASE WHEN Active = 0 THEN 'No' WHEN Active = 1 THEN 'Yes' ELSE 'Error' END AS DataActive FROM CustomerGroups {0} ORDER BY Id ASC", search)

            gvList.DataSource = settingCfg.GetListData(myQuery)
            gvList.DataBind()

            gvList.Columns(1).Visible = False
            If Session("RoleName") = "Administrator" Then
                gvList.Columns(1).Visible = True
            End If

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                btnAdd.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function TextType(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerDiscounts WHERE Id = '" + Id + "'")

            Dim type As String = thisData.Tables(0).Rows(0).Item("DiscountType").ToString()
            If type = "Product" Then
                Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
                Dim designName As String = settingCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                result = "Product : " & designName
            End If

            If type = "Fabric" Then
                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
                Dim fabricCustom As String = Convert.ToInt32(thisData.Tables(0).Rows(0).Item("FabricCustom"))
                Dim fabricName As String = settingCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")

                result = "Fabric : " & fabricName

                If fabricCustom = 1 Then
                    If Not String.IsNullOrEmpty(fabricColourId) Then
                        Dim ids As String() = fabricColourId.Split(","c).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).ToArray()

                        Dim names As New List(Of String)

                        For Each thisId As String In ids
                            Dim name As String = settingCfg.GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" & thisId.Trim() & "'")
                            names.Add(name)
                        Next
                        Dim colourName As String = String.Join(",", names)
                        result = "Fabric : " & fabricName & " (" & colourName & ")"
                    End If
                End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function ValueDiscount(Data As Decimal) As String
        Dim result As String = String.Empty
        Try
            If Data > 0 Then
                result = Data.ToString("G29", enUS) & "%"
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function FinalDiscount(Type As String, Data As Boolean) As String
        Dim result As String = String.Empty
        Try
            If Type = "Fabric" Then
                result = "No"
                If Data = True Then : result = "Yes" : End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Process(Show As Boolean, Msg As String)
        divErrorProcess.Visible = Show : msgErrorProcess.InnerText = Msg
    End Sub

    Private Sub MessageErrorCustomer(Show As Boolean, Msg As String)
        divErrorCustomer.Visible = Show : msgErrorCustomer.InnerText = Msg
    End Sub

    Private Sub MessageErrorDiscount(Show As Boolean, Msg As String)
        divErrorDiscount.Visible = Show : msgErrorDiscount.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Id = '" + Id + "'")
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
        Dim result As Boolean = False
        If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
            result = True
        End If
        Return result
    End Function
End Class
