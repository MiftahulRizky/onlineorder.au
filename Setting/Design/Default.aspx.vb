Partial Class Setting_Design_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            txtSearch.Text = Session("designSearch")

            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("designSearch") = txtSearch.Text
        Response.Redirect("~/setting/design/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
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

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("designSearch") = txtSearch.Text
            Session("designDetail") = UCase(row.Cells(1).Text)
            Response.Redirect("~/setting/design/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE DESIGN TYPE. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(5).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE DESIGN. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("designDetail") = "" : Session("designSearch") = ""
        Try
            Dim bySearch As String = String.Empty
            If Not SearchText = "" Then
                bySearch = " WHERE Id LIKE '%" + SearchText + "%' OR Name LIKE '%" + SearchText + "%' OR Company LIKE '%" + SearchText + "%' OR Page LIKE '%" + SearchText + "%'"
            End If
            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT * FROM Designs {0} ORDER BY Name ASC", bySearch))
            gvList.DataBind()
            For i As Integer = 0 To gvList.HeaderRow.Cells.Count - 1
                gvList.HeaderRow.Cells(i).Text = If(gvList.HeaderRow.Cells(i).Text = "", "Empty", gvList.HeaderRow.Cells(i).Text)
            Next
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat fs-3 me-2 opacity-50'></i>Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-rotate'></i>" : End If
        Return result
    End Function

    ' Icon active before text name
    Protected Function IconActive(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-success fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function
End Class
