
Imports System.Data

Partial Class Setting_Customer_Login
    Inherits Page

    Dim settingCfg As New SettingConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Session("LevelName") = "Support" Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            txtSearch.Text = Session("customerLoginSearch")
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        BackColor()
        BindApplicaton()
        BindCustomer()
        BindRole()
        BindLevel()

        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            lblAction.Text = "Add"
            titleLogin.InnerText = "Add Customer Login"

            divApplication.Visible = False
            divPassword.Visible = True
            If Session("LevelName") = "Leader" Then
                divAccess.Visible = True
            End If
            txtPassword.Text = settingCfg.GenerateNewPassword(15)
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)
            lblId.Text = UCase(row.Cells(1).Text).ToString()

            lblAction.Text = "Edit"
            titleLogin.InnerText = "Edit Customer Price Group"

            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerLogins WHERE Id = '" + lblId.Text + "'")
            'myData.Tables(0).Rows(0).Item("Name").ToString()'

            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        Catch ex As Exception
            Call MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitProccess_Click(sender As Object, e As EventArgs)
        MessageError_Proccess(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProccess(); };"
        Try
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                If ddlApplication.SelectedValue = "" Then
                    Call MessageError_Proccess(True, "APPLICATION ID IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                    Exit Sub
                End If
            End If

            If ddlCustomer.SelectedValue = "" Then
                Call MessageError_Proccess(True, "CUSTOMER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If ddlRole.SelectedValue = "" Then
                Call MessageError_Proccess(True, "ROLE MEMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If ddlLevel.SelectedValue = "" Then
                Call MessageError_Proccess(True, "LEVEL MEMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If txtFullName.Text = "" Then
                Call MessageError_Proccess(True, "FULL NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If txtUserName.Text = "" Then
                Call MessageError_Proccess(True, "USERNAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            If Not Regex.IsMatch(txtUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                Call MessageError_Proccess(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                Exit Sub
            End If

            Dim checkUsername As String = settingCfg.GetItemData("SELECT UserName FROM CustomerLogins WHERE UserName = '" + txtUserName.Text + "'")

            If lblAction.Text = "Add" Then
                If txtUserName.Text = checkUsername Then
                    Call MessageError_Proccess(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                    Exit Sub
                End If
            End If

            If lblAction.Text = "Edit" And txtUserName.Text <> lblLoginUserNameOld.Text Then
                If txtUserName.Text = checkUsername Then
                    Call MessageError_Proccess(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorProccess.InnerText = "" Then
                lblAppId.Text = UCase(ddlApplication.SelectedValue).ToString()
                lblRoleId.Text = UCase(ddlRole.SelectedValue).ToString()
                lblLevelId.Text = UCase(ddlLevel.SelectedValue).ToString()
                If txtPassword.Text = "" Then
                    txtPassword.Text = txtUserName.Text
                End If
                lblPasswordHash.Text = settingCfg.Encrypt(txtPassword.Text)

                If Session("RoleName") = "Administrator" And (Session("LevelName") = "Member" Or Session("LevelName") = "Support") Then
                    lblAppId.Text = UCase(Session("ApplicationId").ToString())
                End If
                If lblAction.Text = "Add" Then sdsPage.Insert()
                If lblAction.Text = "Edit" Then sdsPage.Update()

                Response.Redirect("~/setting/customer/login", False)
            End If
        Catch ex As Exception
            MessageError_Proccess(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProccess", thisScript, True)
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Call MessageError(False, String.Empty)
        Session("customerLoginSearch") = ""
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE CustomerLogins.Id LIKE '%" + SearchText + "%' OR CustomerLogins.CustomerId LIKE '%" + SearchText + "%' OR CustomerLogins.UserName LIKE '%" + SearchText + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT CustomerLogins.*, Applications.Name AS AppName, Customers.Name AS CustomerName, CustomerLoginRoles.Name AS RoleName, CustomerLoginLevels.Name AS LevelName FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId = Customers.Id LEFT JOIN Applications ON CustomerLogins.ApplicationId = Applications.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id LEFT JOIN CustomerLoginLevels ON CustomerLogins.LevelId = CustomerLoginLevels.Id {0} ORDER BY Customers.Id, CustomerLogins.UserName ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        Call MessageError_Proccess(False, String.Empty)
        Session("customerLoginSearch") = ""

        ddlApplication.BackColor = Drawing.Color.Empty
        ddlRole.BackColor = Drawing.Color.Empty
        ddlLevel.BackColor = Drawing.Color.Empty
        ddlCustomer.BackColor = Drawing.Color.Empty
        txtFullName.BackColor = Drawing.Color.Empty
        txtUserName.BackColor = Drawing.Color.Empty
        txtPassword.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub BindApplicaton()
        ddlApplication.Items.Clear()
        Try
            ddlApplication.DataSource = settingCfg.GetListData("SELECT * FROM Applications ORDER BY Name ASC")
            ddlApplication.DataTextField = "Name"
            ddlApplication.DataValueField = "Id"
            ddlApplication.DataBind()
            If ddlApplication.Items.Count > 1 Then
                ddlApplication.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlApplication.Items.Clear()
        End Try
    End Sub

    Private Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingCfg.GetListData("SELECT * FROM Customers ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()
            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
        End Try
    End Sub

    Private Sub BindRole()
        ddlRole.Items.Clear()
        Try
            ddlRole.DataSource = settingCfg.GetListData("SELECT * FROM CustomerLoginRoles ORDER BY Name ASC")
            ddlRole.DataTextField = "Name"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()
            ddlRole.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlRole.Items.Clear()
        End Try
    End Sub

    Private Sub BindLevel()
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = settingCfg.GetListData("SELECT * FROM CustomerLoginLevels ORDER BY Name ASC")
            ddlLevel.DataTextField = "Name"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()
            ddlLevel.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLevel.Items.Clear()
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Proccess(Show As Boolean, Msg As String)
        divErrorProccess.Visible = Show : msgErrorProccess.InnerText = Msg
    End Sub

    Protected Function VisiblePrimaryYes(Primary As Boolean) As Boolean
        Dim result As Boolean = False
        If Primary = True Then : result = True : End If
        Return result
    End Function

    Protected Function VisiblePrimaryNo(Primary As Boolean) As Boolean
        Dim result As Boolean = False
        If Primary = False Then : result = True : End If
        Return result
    End Function

    Protected Function TextActive_Login(Active As Boolean) As String
        Dim result As String = "Enable"
        If Active = True Then : Return "Disable" : End If
        Return result
    End Function

    Protected Function DencryptPassword(Password As String) As String
        Dim result As String = settingCfg.Decrypt(Password)
        Return result
    End Function
End Class
