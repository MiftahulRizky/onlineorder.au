Partial Class Setting_System_Session
    Inherits Page

    Dim settingCfg As New SettingConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("RoleName") <> "Administrator" Then
            Response.Redirect("~/setting/system", False)
            Exit Sub
        End If

        If Session("LevelName") = "Support" Then
            Response.Redirect("~/setting/system", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("sessionSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
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

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()

            Session("sessionSearch") = txtSearch.Text
            Response.Redirect("~/setting/system/session", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        MessageError(False, String.Empty)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Sessions.Id LIKE '%" + SearchText + "%' OR Sessions.LoginId LIKE '%" + SearchText + "%' OR Customers.Id LIKE '%" + SearchText + "%' OR Customers.Name LIKE '%" + SearchText + "%' OR CustomerLogins.UserName LIKE '%" + SearchText + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT Sessions.*, Customers.Id AS CustId, Customers.Name AS CustomerName, CustomerLogins.UserName AS UserName FROM Sessions LEFT JOIN CustomerLogins ON Sessions.LoginId = CustomerLogins.Id LEFT JOIN Customers ON CustomerLogins.CustomerId = Customers.Id {0}", search)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Protected Function VisibleDelete() As Boolean
        If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
            Return True
        End If
        Return False
    End Function
End Class
